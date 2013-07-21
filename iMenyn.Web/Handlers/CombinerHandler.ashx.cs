using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Caching;
using iMenyn.Web.Helpers;
using Yahoo.Yui.Compressor;

namespace iMenyn.Web.Handlers
{
    /// <summary>
    /// Summary description for CombinerHandler
    /// </summary>
    public class CombinerHandler : IHttpHandler
    {
        //private ILogger Logger { get { return DependencyManager.Logger; } }
        private const bool DoGzip = true;
        private readonly static TimeSpan CacheDuration = TimeSpan.FromDays(30);
        private string _path;

        private List<string> _allowedFileExtension = new List<string>();

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            // Read fileString, contentType and version. All are required. They are
            // used as cache key
            string fileString = request["f"] ?? string.Empty;
            string contentType = request["t"] ?? string.Empty;
            string version = request["v"] ?? string.Empty;
            _path = request["p"] ?? string.Empty;

            // Decide if browser supports compressed response
            bool isCompressed = DoGzip && this.CanGZip(context.Request);

            // Response is written as UTF8 encoding. If you are using languages like
            // Arabic, you should change this to proper encoding 
            var encoding = new UTF8Encoding(false);

            // If the set has already been cached, write the response directly from
            // cache. Otherwise generate the response and cache it
            if (!this.WriteFromCache(context, fileString, version, isCompressed, contentType))
            {
                using (var memoryStream = new MemoryStream(5000))
                {
                    // Decide regular stream or GZipStream based on whether the response
                    // can be cached or not
                    using (Stream writer = isCompressed ? (Stream)(new GZipStream(memoryStream, CompressionMode.Compress)) : memoryStream)
                    {
                        // Add default allowed file extensions
                        _allowedFileExtension.Add(".js");
                        _allowedFileExtension.Add(".css");

                        // Make sure we only got unique extensions
                        _allowedFileExtension = _allowedFileExtension.Distinct().ToList();

                        // Load the files defined in the querystring and process each file
                        string[] fileNames = fileString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string fileName in fileNames)
                        {
                            // Write commented filename to stream
                            //string separator = "/* " + fileName + " */\n";
                            //writer.Write(encoding.GetBytes(separator), 0, separator.Length);

                            byte[] fileBytes = this.GetFileBytes(context, fileName.Trim(), encoding);

                            // Write file data to stream
                            writer.Write(fileBytes, 0, fileBytes.Length);

                            // Write linebreak to stream
                            writer.Write(encoding.GetBytes("\n\n"), 0, 2);
                        }

                        writer.Close();
                    }

                    // Cache the combined response so that it can be directly written in subsequent calls
                    byte[] responseBytes = memoryStream.ToArray();
                    context.Cache.Insert(GetCacheKey(fileString, version, isCompressed),
                        responseBytes, GetCacheDependency(context), Cache.NoAbsoluteExpiration,
                        CacheDuration);

                    // Generate the response
                    this.OutputFilesAndHeaders(responseBytes, context, isCompressed, contentType);
                }
            }
        }

        private CacheDependency GetCacheDependency(HttpContext context)
        {
            string fullFolderPath = context.Server.MapPath(context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/")));
            return new CacheDependency(fullFolderPath);
        }

        private byte[] GetFileBytes(HttpContext context, string virtualPath, Encoding encoding)
        {
            if (virtualPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || virtualPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var client = new WebClient())
                {
                    return client.DownloadData(virtualPath);
                }
            }
            else
            {
                virtualPath = CombinerHelper.GetUrlFromFilename(_path, virtualPath);
                string physicalPath = HttpContext.Current.Server.MapPath(virtualPath);

                string extension = Path.GetExtension(virtualPath);

                if (!_allowedFileExtension.Contains(extension))
                    throw new SecurityException("Combiner couldn't find a valid parser for file " + virtualPath);

                byte[] bytes = new byte[1024];

                // Don't compress if it already is compressed
                if (!(physicalPath.Contains(".min.") || physicalPath.Contains("-min.")))
                {
                    // Use minified file if one exists
                    string newPhysicalPath = physicalPath.Replace(extension, "");
                    newPhysicalPath = newPhysicalPath + ".min" + extension;
                    if (File.Exists(newPhysicalPath))
                    {
                        physicalPath = newPhysicalPath;
                        bytes = File.ReadAllBytes(physicalPath);
                    }
                    else
                    {
                        if (File.Exists(physicalPath))
                        {
                            // Compress on-the-fly using YUI
                            string uncompresed = File.ReadAllText(physicalPath);
                            string compressed = "";

                            try
                            {
                                if (extension == ".css")
                                {
                                    compressed = new CssCompressor().Compress(uncompresed);
                                    // Fix for media-queries, please remove once its no longer needed
                                    compressed = compressed.Replace(" screen and(", " screen and (");
                                }
                                else if (extension == ".js")
                                {
                                    compressed = new JavaScriptCompressor().Compress(uncompresed);
                                }

                            }
                            catch (Exception ex)
                            {
                                // Something went wrong... let's use the uncompressed version
                                //Logger.Fatal("Error compressing " + virtualPath, ex);
                                compressed = "/* Error compressing */\n" + uncompresed;
                            }
                            bytes = encoding.GetBytes(compressed);
                        }
                        else
                        {
                            // File does not exist
                            throw new Exception(physicalPath + " does not exist");
                        }
                    }
                }
                else
                {
                    // File already compressed. Lets let it
                    string alreadycompresed = File.ReadAllText(physicalPath);
                    bytes = encoding.GetBytes(alreadycompresed);
                }

                // TODO: Convert unicode files to specified encoding. For now, assuming
                // files are either ASCII or UTF8
                return bytes;
            }
        }

        private bool WriteFromCache(HttpContext context, string fileString, string version,
            bool isCompressed, string contentType)
        {
            byte[] responseBytes = context.Cache[GetCacheKey(fileString, version, isCompressed)] as byte[];

            if (null == responseBytes || 0 == responseBytes.Length) return false;

            this.OutputFilesAndHeaders(responseBytes, context, isCompressed, contentType);
            return true;
        }

        private void OutputFilesAndHeaders(byte[] bytes, HttpContext context, bool isCompressed, string contentType)
        {
            HttpResponse response = context.Response;

            response.AppendHeader("Content-Length", bytes.Length.ToString());
            response.ContentType = contentType;
            if (isCompressed)
                response.AppendHeader("Content-Encoding", "gzip");

            DateTime lastModified = new DateTime(DateTime.Now.Year - 1, 1, 1);
            context.Response.Cache.SetLastModified(lastModified);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.Add(CacheDuration));
            context.Response.Cache.SetMaxAge(CacheDuration);
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            context.Response.Cache.SetOmitVaryStar(true);
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.Cache.SetETag("");

            //context.Response.AddFileDependencies(files.Select(f => f.FullName).ToArray());

            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.Flush();
        }

        private bool CanGZip(HttpRequest request)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding) &&
                 (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate")))
                return true;
            return false;
        }

        private string GetCacheKey(string fileString, string version, bool isCompressed)
        {
            return "HttpCombiner." + fileString + "." + version + "." + isCompressed;
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
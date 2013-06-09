using System;
using System.Collections.Generic;

namespace Rantup.Web.Helpers
{
    public class CombinerHelper
    {
        public static string GetUrlFromFilename(string path, string fileName)
        {
            string url = "";
            if (path != String.Empty)
                url = CombineUrls(path, fileName);
            return url;
        }

        public static List<string> GetFilenames(string path, string files, string type, int version, bool enabled)
        {
            var retFiles = new List<string>();

            string url;
            if (enabled)
            {
                url = String.Format("{1}/Combiner?f={0}&p={1}&t={2}&v={3}", files, path, type, version);
                retFiles.Add(url);
            }
            else
            {
                var arrFiles = files.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                /* Write files as seperate tags to simulate "normal" use for debug */
                foreach (string fileName in arrFiles)
                {
                    url = fileName;
                    if (!IsExternal(fileName))
                        url = GetUrlFromFilename(path, fileName);

                    retFiles.Add(url);
                }
            }
            return retFiles;
        }

        private static bool IsExternal(string fileName)
        {
            return fileName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                   fileName.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
        }

        private static string CombineUrls(string uri1, string uri2)
        {
            if (!uri1.Equals("/"))
                uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            var sep = (uri1 != String.Empty && uri2 != String.Empty && !uri1.Equals("/")) ? "/" : "";
            return uri1 + sep + uri2;
        }
    }
}
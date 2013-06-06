﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rantup.Yelp.Data;
using Rantup.Yelp.Data.Options;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace Rantup.Yelp
{
    /// <summary>
    /// 
    /// </summary>
    public class Yelp
    {
        //--------------------------------------------------------------------------
        //
        //	Variables
        //
        //--------------------------------------------------------------------------

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected const string rootUri = "http://api.yelp.com/v2/";

        /// <summary>
        /// 
        /// </summary>
        protected Options options { get; set; }

        #endregion

        //--------------------------------------------------------------------------
        //
        //	Constructors
        //
        //--------------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Driver for the Yelp API
        /// </summary>
        /// <param name="options">OAuth options for using the Yelp API</param>
        public Yelp(Options options)
        {
            this.options = options;
        }

        #endregion

        //--------------------------------------------------------------------------
        //
        //	Public Methods
        //
        //--------------------------------------------------------------------------

        #region Search

        /// <summary>
        /// Simple search method to look for a term in a given plain text address
        /// </summary>
        /// <param name="term">what to look for (ex: coffee)</param>
        /// <param name="location">where to look for it (ex: seattle)</param>
        /// <returns>a strongly typed result</returns>
        public Task<SearchResults> Search(string term, string location)
        {
            var result = makeRequest<SearchResults>("search", null, new Dictionary<string, string>
                {
                    { "term", term },
                    {"category_filter", "food,restaurants"},
                    { "location", location + ",sweden" }
                });
            
            return result;
        }

        /// <summary>
        /// advanced search based on search options object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<SearchResults> Search(SearchOptions options)
        {
            var result = makeRequest<SearchResults>("search", null, options.GetParameters());
            return result;
        }

        #endregion

        #region GetBusiness
        /// <summary>
        /// search the list of business based on name
        /// </summary>
        /// <param name="name">name of the business you want to get information on</param>
        /// <returns>Business details</returns>
        public Task<Business> GetBusiness(string name)
        {
            var result =  makeRequest<Business>("business", name, null);
            return result;
        }

        #endregion


        //--------------------------------------------------------------------------
        //
        //	Internal Methods
        //
        //--------------------------------------------------------------------------

        #region makeRequest
        /// <summary>
        /// contains all of the oauth magic, makes the http request and returns raw json
        /// </summary>
        /// <param name="parameters">hash array of qs parameters</param>
        /// <returns>plain text json response from the api</returns>
        protected Task<T> makeRequest<T>(string area, string id, Dictionary<string, string> parameters)
        {
            // build the url with parameters
            var url = area;
            if (!String.IsNullOrEmpty(id)) url += "/" + Uri.EscapeDataString(id);
        
            // restsharp FTW!
            var client = new RestClient(rootUri);
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(options.ConsumerKey, options.ConsumerSecret, options.AccessToken, options.AccessTokenSecret);
            var request = new RestRequest(url, Method.GET);

            if (parameters != null)
            {
                string[] keys = parameters.Keys.ToArray();
                foreach (string k in keys)
                {
                    request.AddParameter(k, parameters[k].Replace("&", "%26"));
                }
            }

            var tcs = new TaskCompletionSource<T>();
            var handle = client.ExecuteAsync(request, response =>
            {
                var results = JsonConvert.DeserializeObject<T>(response.Content);
                tcs.SetResult(results);
            });
            
            return tcs.Task;
        }
        #endregion


    }
}

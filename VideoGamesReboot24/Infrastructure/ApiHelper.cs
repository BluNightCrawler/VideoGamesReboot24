using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.Types;
using VideoGamesReboot24.Models;

namespace VideoGamesReboot24.Infrastructure
{
    public class ApiHelper
    {
        GameStoreDbContext context;
        string apiName;
        /// <summary>
        /// Constructor for ApiHelper
        /// </summary>
        /// <param name="context">The GameStoreDbContext (Used to store Api Access Tokens)</param>
        /// <param name="apiName">The Name of the Api to be used</param>
        public ApiHelper(GameStoreDbContext context, string apiName)
        {
            this.context = context;
            this.apiName = apiName;
        }
        /// <summary>
        /// Updates the access token, Check if the Access Token needs to be updated first.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns>Returns true if succesfully updated the access token</returns>
        public bool updateAccessToken(string clientId, string clientSecret)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://id.twitch.tv/");
                client.DefaultRequestHeaders.Clear();
                var res = client.PostAsync(
                    $"oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials",null).Result;
                
                if (!res.IsSuccessStatusCode) return false;

                string resString = res.Content.ReadAsStringAsync().Result;
                var resJObject = JObject.Parse(resString);
                string accessToken = (string)resJObject["access_token"];
                int secondsUntilExpire = (int)resJObject["expires_in"];

                if (accessTokenExists())
                {
                    ApiAccessToken apiAccessToken = context.ApiAccessTokens.First(a => a.ApiName == apiName);
                    //apiAccessToken.ApiName = apiName;
                    apiAccessToken.AccessToken = accessToken;
                    apiAccessToken.CreationTime = DateTime.Now;
                    apiAccessToken.ExpirationTime = DateTime.Now.AddSeconds(secondsUntilExpire - 1);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    ApiAccessToken apiAccessToken = new ApiAccessToken();
                    apiAccessToken.ApiName = apiName;
                    apiAccessToken.AccessToken = accessToken;
                    apiAccessToken.CreationTime = DateTime.Now;
                    apiAccessToken.ExpirationTime = DateTime.Now.AddSeconds(secondsUntilExpire - 1);
                    context.Add(apiAccessToken);
                    context.SaveChanges();
                    return true;
                }
            }
        }
        /// <summary>
        /// Checks the validity of the current access token (if any)
        /// </summary>
        /// <returns>Returns true if apiName access token exist and does not need to be refreshed</returns>
        public bool accessTokenValid()
        {
            var existingToken = context.ApiAccessTokens.Where(item => item.ApiName == apiName);
            if (existingToken.Any())
            {
                if (existingToken.First().ExpirationTime > DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        private bool accessTokenExists()
        {
            var existingToken = context.ApiAccessTokens.Where(item => item.ApiName == apiName);
            if (existingToken.Any()) { return true; }
            else { return false; }
        }

        public ApiAccessToken getApiAccessToken()
        {
            return context.ApiAccessTokens.First(item => item.ApiName == apiName);
        }
    }
}

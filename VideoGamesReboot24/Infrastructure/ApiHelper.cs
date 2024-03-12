using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Build.Execution;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
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
                string accessToken = (string)resJObject["access_token"]!;
                int secondsUntilExpire = (int)resJObject["expires_in"]!;

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
        public VideoGameFull? GetVideoGame(string game)
        {
            if (!accessTokenValid())
                return null;

            JArray res = SendApiRequest($"fields name, age_ratings.category, age_ratings.rating, cover.url, genres.name, " +
                    $"platforms.name, rating, rating_count, release_dates.date, summary; where version_parent = null & category = 0 & name=\"{game}\"; limit 100;");

            return ParseVideoGame((JObject)res[0]);
        }

        public List<VideoGameFull> SearchVideoGame(string game)
        {
            if (!accessTokenValid())
                return new List<VideoGameFull>();

            JArray res = SendApiRequest($"fields name, age_ratings.category, age_ratings.rating, cover.url, genres.name, " +
                    $"platforms.name, rating, rating_count, release_dates.date, summary; search \"{game}\"; where version_parent = null & category = 0; limit 100;");

            return ParseVideoGameList(res);
        }

        private JArray SendApiRequest(string reqBody)
        {
            var jObject = JObject.Parse(File.ReadAllText("credentials.json"));
            JObject twitchCredentials = (JObject)jObject["twitchCredentials"]!;
            string clientID = (string)twitchCredentials["clientID"]!;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.igdb.com/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {getApiAccessToken().AccessToken}");
                client.DefaultRequestHeaders.Add("Client-ID", clientID);
                var res = client.PostAsync("v4/games",
                    new StringContent(reqBody)).Result;

                if (!res.IsSuccessStatusCode) return new JArray();

                string resString = res.Content.ReadAsStringAsync().Result;
                return JArray.Parse(resString);
            }
        }

        private List<VideoGameFull> ParseVideoGameList(JArray responceList)
        {
            List <VideoGameFull> parsedList = new List <VideoGameFull>();
            foreach (JObject resGame in responceList)
            {
                VideoGameFull newGame = ParseVideoGame(resGame);
                if (newGame.ImagePath != null)
                {
                    parsedList.Add(newGame);
                }
            }
            return parsedList;
        }

        private VideoGameFull ParseVideoGame(JObject responceItem)
        {
            string? agerating;
            string? coverUrl;
            string? longImageUrl;
            DateTime? releaseDate;
            List<string> categories = new List<string>();
            List<string> systems = new List<string>();
            double price;
            if (responceItem.ContainsKey("age_ratings"))
            {
                JArray ratingJObject = (JArray)responceItem["age_ratings"]!;
                JArray rating_esrb = JArray.FromObject(ratingJObject.Where(x => (int)x["category"]! == 1));

                agerating = rating_esrb.Any() ? ((Ratings)(int)rating_esrb.First()["rating"]!).ToString() : "None";
            }
            else
            {
                agerating = null;
            }
            if (responceItem.ContainsKey("cover"))
            {
                string tempStr = (string)((JObject)responceItem["cover"]!)["url"]!;
                coverUrl = "https:" + tempStr.Replace("t_thumb", "t_720p");
                longImageUrl = "https:" + tempStr.Replace("t_thumb", "t_screenshot_big");
            }
            else
            {
                coverUrl = null;
                longImageUrl = null;
            }
            if (responceItem.ContainsKey("release_dates"))
            {
                JArray datesArray = (JArray)responceItem["release_dates"]!;
                JObject firstDate = (JObject)datesArray.First();

                releaseDate = firstDate.ContainsKey("date") ? new DateTime(1965, 1, 1, 0, 0, 0, 0).AddSeconds((int)firstDate["date"]!) : null;
            }
            else
            {
                releaseDate = null;
            }
            if (responceItem.ContainsKey("genres"))
            {
                JArray genresArray = (JArray)responceItem["genres"]!;
                foreach (JObject genre in genresArray)
                {
                    categories.Add((string)genre["name"]!);
                }
            }
            if (responceItem.ContainsKey("platforms"))
            {
                JArray systemsArray = (JArray)responceItem["platforms"]!;
                foreach (JObject system in systemsArray)
                {
                    systems.Add((string)system["name"]!);
                }
            }
            Random r = new Random();
            price = r.Next(70) + .99;

            return new VideoGameFull
            {
                Name = responceItem.ContainsKey("name") ? (string)responceItem["name"]! : "MISSINGNAME_ERROR",
                Description = responceItem.ContainsKey("summary") ? (string)responceItem["summary"]! : "NO DESCRIPTION",
                Price = price,
                Categories = context.Categories.Where(c => categories.Contains(c.Name)).ToList(),
                Systems = context.Systems.Where(s => systems.Contains(s.Name)).ToList(),
                AgeRating = agerating,
                ImagePath = coverUrl,
                LongImagePath = longImageUrl,
                Rating = responceItem.ContainsKey("rating") ? (double)responceItem["rating"]! : null,
                RatingCount = responceItem.ContainsKey("rating_count") ? (int)responceItem["rating_count"]! : null,
                ReleaseDate = releaseDate
            };

        }
    }
}

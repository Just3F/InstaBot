using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InstaBot.Service.Models;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;

namespace InstaBot.Service
{
    public static class InstagramApiFactory
    {
        private static Dictionary<InstagramUser, IInstaApi> apiCollection { get; set; }

        static InstagramApiFactory()
        {
            apiCollection = new Dictionary<InstagramUser, IInstaApi>();
        }

        public static async Task<IInstaApi> GetInstaApiAsync(InstagramUser user)
        {
            IInstaApi instaApi = apiCollection.GetValueOrDefault(user, null);

            if (instaApi == null)
            {
                instaApi = await TryToLoginAsync(user);
                apiCollection.Add(user, instaApi);
            }
            else
            {
                if (!instaApi.IsUserAuthenticated)
                {
                    apiCollection.Remove(user);
                    //await GetInstaApiAsync(user);
                }
            }

            return instaApi;
        }

        private static async Task<IInstaApi> TryToLoginAsync(InstagramUser user)
        {
            var address = "86.57.159.118";
            var port = "33620";
            var httpHandler = new HttpClientHandler
            {
                Proxy = new WebProxy($"{address}:{port}", false),
                UseProxy = true
                //PreAuthenticate = true,
                //UseDefaultCredentials = false,
                //Credentials = new System.Net.NetworkCredential(proxyServerSettings.UserName,
                //    proxyServerSettings.Password),
            };


            var api = InstaApiBuilder.CreateBuilder()
                //.UseHttpClientHandler(httpHandler)
                .SetUser(new UserSessionData { UserName = user.UserName, Password = user.Password })
                .UseLogger(new DebugLogger(LogLevel.Exceptions)).Build();
            var loginRequest = await api.LoginAsync();

            if (loginRequest.Succeeded)
                Console.WriteLine("Logged in!");
            else
            {
                var errorMsg = loginRequest.Info.Message;
                Console.WriteLine("Error Logging in!" + Environment.NewLine + errorMsg);

                if (errorMsg == "Please wait a few minutes before you try again.")
                    Thread.Sleep(TimeSpan.FromMinutes(5));
            }

            return api;
        }
    }
}

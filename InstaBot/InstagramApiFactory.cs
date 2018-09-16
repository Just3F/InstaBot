﻿using System;
using System.Collections.Generic;
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
            var test = user.GetHashCode();
            IInstaApi instaApi = apiCollection.GetValueOrDefault(user, null);

            if (instaApi == null)
            {
                instaApi = await TryToLoginAsync(user);
                apiCollection.Add(user, instaApi);
            }

            return instaApi;
        }

        private static async Task<IInstaApi> TryToLoginAsync(InstagramUser user)
        {
            var api = InstaApiBuilder.CreateBuilder()
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

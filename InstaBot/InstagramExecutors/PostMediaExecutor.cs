using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Models;
using InstaSharper.API;
using InstaSharper.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaBot.Service.InstagramExecutors
{
    public class PostMediaExecutor : IInstagramExecutor
    {
        public async Task Execute(Queue queue, InstaBotContext db, IInstaApi api)
        {
            var allGroups = queue.LoadId.Split(' ');
            foreach (var group in allGroups)
            {
                var instaMediaList = await api.GetUserMediaAsync(group, PaginationParameters.MaxPagesToLoad(0));
            }
        }
    }
}

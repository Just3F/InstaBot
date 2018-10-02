using System;
using System.Linq;
using System.Threading.Tasks;
using InstaBot.Common;
using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Models;
using InstaSharper.API;
using InstaSharper.Classes;

namespace InstaBot.Service.InstagramExecutors
{
    public class LikeExecutor : BaseExecutor, IInstagramExecutor
    {
        public LikeExecutor(IInstaApi instaApi)
            : base(instaApi) { }

        public async Task Execute(Queue queue, InstaBotContext db)
        {
            string tag = queue.LoadId;
            var instaTagFeed = await _instaApi.GetTagFeedAsync(tag, PaginationParameters.MaxPagesToLoad(0));
            var lastPost = instaTagFeed.Value?.Medias?.FirstOrDefault();

            if (instaTagFeed.Succeeded && lastPost != null)
            {
                await _instaApi.LikeMediaAsync(lastPost.InstaIdentifier);
                await UpdateQueueLastActivityAsync(queue, db);
                Console.WriteLine("LikeExecutor for " + queue.LoginData.Name);
            }
        }
    }
}

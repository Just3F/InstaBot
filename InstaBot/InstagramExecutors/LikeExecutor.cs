using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task Execute(QueueEntity queueEntity, InstaBotContext db)
        {
            string tag = queueEntity.LoadId;
            var instaTagFeed = await _instaApi.GetTagFeedAsync(tag, PaginationParameters.MaxPagesToLoad(0));
            var lastPost = instaTagFeed.Value?.Medias?.FirstOrDefault();

            if (instaTagFeed.Succeeded && lastPost != null)
            {
                await _instaApi.LikeMediaAsync(lastPost.InstaIdentifier);
                await UpdateQueueLastActivityAsync(queueEntity, db);
                Console.WriteLine("LikeExecutor for " + queueEntity.LoginData.Name);
            }
        }
    }
}

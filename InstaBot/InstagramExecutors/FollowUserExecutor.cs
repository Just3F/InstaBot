using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Models;
using InstaSharper.API;
using InstaSharper.Classes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InstaBot.Service.InstagramExecutors
{
    public class FollowUserExecutor : BaseExecutor, IInstagramExecutor
    {
        public FollowUserExecutor(IInstaApi instaApi)
            : base(instaApi) { }

        public async Task Execute(Queue queue, InstaBotContext db)
        {
            var instaExploreFeed = await _instaApi.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(0));
            var firstInstaMedia = instaExploreFeed?.Value?.Medias?.FirstOrDefault();

            if (firstInstaMedia != null)
            {
                await _instaApi.FollowUserAsync(firstInstaMedia.User.Pk);
                await UpdateQueueLastActivityAsync(queue, db);
                Console.WriteLine("FollowUserExecutor for " + queue.User.Name);
            }
        }
    }
}

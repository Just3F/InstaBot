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
            long userId = 0;
            if (queue.LoadId != null)
            {
                userId = await GetUserIdByTag(queue, userId);
            }
            else
            {
                userId = await GetUserIdFromFeed(userId);
            }

            if (userId != 0)
            {
                await _instaApi.FollowUserAsync(userId);
                await UpdateQueueLastActivityAsync(queue, db);
                Console.WriteLine("FollowUserExecutor for " + queue.User.Name);
            }
        }

        private async Task<long> GetUserIdFromFeed(long userId)
        {
            var instaExploreFeed = await _instaApi.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(0));
            var firstInstaMedia = instaExploreFeed?.Value?.Medias?.FirstOrDefault();
            if (firstInstaMedia != null)
                userId = firstInstaMedia.User.Pk;
            return userId;
        }

        private async Task<long> GetUserIdByTag(Queue queue, long userId)
        {
            var mediasByTag = await _instaApi.GetTagFeedAsync(queue.LoadId, PaginationParameters.MaxPagesToLoad(0));
            var user = mediasByTag.Value.Medias.Where(x => !x.User.FriendshipStatus.Following && !x.User.IsPrivate).Select(x => x.User).FirstOrDefault();
            if (user != null)
                userId = user.Pk;
            return userId;
        }
    }
}

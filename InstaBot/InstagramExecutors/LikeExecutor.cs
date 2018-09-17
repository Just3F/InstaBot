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
        public async Task Execute(Queue queue, InstaBotContext db, IInstaApi api)
        {
            string tag = queue.LoadId;
            var instaTagFeed = await api.GetTagFeedAsync(tag, PaginationParameters.MaxPagesToLoad(0));
            var lastPost = instaTagFeed.Value?.Medias?.FirstOrDefault();

            if (instaTagFeed.Succeeded && lastPost != null)
            {
                await api.LikeMediaAsync(lastPost.InstaIdentifier);
                await UpdateQueueLastActivityAsync(queue, db);
            }
        }
    }
}

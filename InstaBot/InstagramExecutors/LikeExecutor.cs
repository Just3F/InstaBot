using System;
using System.Linq;
using InstaBot.Service.DataBaseModels;
using System.Threading.Tasks;
using InstaSharper.API;
using InstaSharper.Classes;
using Microsoft.EntityFrameworkCore.Internal;

namespace InstaBot.Service.Models
{
    public class LikeExecutor : IInstagramExecutor
    {
        public async Task Execute(Queue queue, InstaBotContext db, IInstaApi api)
        {
            string tag = queue.LoadId;
            var instaTagFeed = await api.GetTagFeedAsync(tag, PaginationParameters.MaxPagesToLoad(0));
            var lastPost = instaTagFeed.Value?.Medias?.FirstOrDefault();

            if (instaTagFeed.Succeeded && lastPost != null)
            {
                await api.LikeMediaAsync(lastPost.InstaIdentifier);
                await UpdateDBAsync(queue, db);
            }
        }

        private async Task UpdateDBAsync(Queue queue, InstaBotContext db)
        {
            queue.LastActivity = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}

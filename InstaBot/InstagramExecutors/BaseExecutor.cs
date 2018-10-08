using System;
using System.Threading.Tasks;
using InstaBot.Service.DataBaseModels;
using InstaSharper.API;

namespace InstaBot.Service.InstagramExecutors
{
    public abstract class BaseExecutor
    {
        public IInstaApi _instaApi;
        public BaseExecutor(IInstaApi instaApi)
        {
            _instaApi = instaApi;
        }

        protected async Task UpdateQueueLastActivityAsync(QueueEntity queueEntity, InstaBotContext db)
        {
            queueEntity.LastActivity = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}

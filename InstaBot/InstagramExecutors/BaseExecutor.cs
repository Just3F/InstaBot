using System;
using System.Threading.Tasks;
using InstaBot.Service.DataBaseModels;

namespace InstaBot.Service.InstagramExecutors
{
    public abstract class BaseExecutor
    {
        protected async Task UpdateQueueLastActivityAsync(Queue queue, InstaBotContext db)
        {
            queue.LastActivity = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}

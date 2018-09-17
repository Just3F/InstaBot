using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Enums;
using InstaBot.Service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using InstaSharper.API;

namespace InstaBot.Service
{
    public class InstagramBotService
    {
        public bool IsEnabled { get; set; }
        public int DelayInMilliseconds { get; set; }

        public async Task Process()
        {
            while (IsEnabled)
            {
                using (var db = new InstaBotContext())
                {
                    var activeQueues = await db.Queues
                    .Where(x => x.LastActivity < DateTime.UtcNow - TimeSpan.FromSeconds(x.DelayInSeconds) &&
                                x.QueueState == QueueState.InProgress)
                    .Include(x => x.User)
                    .ToListAsync();

                    foreach (var queue in activeQueues)
                    {
                        try
                        {
                            IInstaApi api = await InstagramApiFactory.GetInstaApiAsync(new InstagramUser(queue.User.Name, queue.User.Password));
                            IInstagramExecutor instagramExecutor = InstagramServiceFactory.CreateExecutor(queue.QueueType);
                            await instagramExecutor.Execute(queue, db, api);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            //throw;
                        }
                    }
                }
            }
        }
    }
}

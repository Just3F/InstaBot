using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using InstaBot.Common.Enums;
using InstaBot.Service.InstagramExecutors;
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
                                x.QueueState == QueueState.InProgress && x.IsActive)
                    .Include(x => x.LoginData)
                    .ToListAsync();

                    foreach (var queue in activeQueues)
                    {
                        try
                        {
                            IInstaApi instaApi = await InstagramApiFactory.GetInstaApiAsync(new InstagramUser(queue.LoginData.Name, queue.LoginData.Password));
                            IInstagramExecutor instagramExecutor = InstagramServiceFactory.CreateExecutor(queue.QueueType, instaApi);
                            await instagramExecutor.Execute(queue, db);
                            Console.WriteLine(DateTime.Now);
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

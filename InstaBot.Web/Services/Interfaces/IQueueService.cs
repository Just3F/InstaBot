using System.Collections.Generic;
using System.Threading.Tasks;
using InstaBot.Web.EntityModels;

namespace InstaBot.Web.Services.Interfaces
{
    public interface IQueueService
    {
        Task<IEnumerable<QueueEntity>> GetQueuesForUser(string userId);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;
using InstaBot.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InstaBot.Web.Services
{
    public class QueueService : BaseService, IQueueService
    {
        public QueueService(ApplicationDbContext db): base(db)
        {
        }

        public async Task<IEnumerable<QueueEntity>> GetQueuesForUser(string userId)
        {
            return await _db.Queues.Where(x => x.LoginData.ApplicationUserId == userId).ToListAsync();
        }
    }
}

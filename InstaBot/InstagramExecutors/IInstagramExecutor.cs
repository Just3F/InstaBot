using System.Threading.Tasks;
using InstaBot.Service.DataBaseModels;

namespace InstaBot.Service.InstagramExecutors
{
    public interface IInstagramExecutor
    {
        Task Execute(QueueEntity queueEntity, InstaBotContext db);
    }
}

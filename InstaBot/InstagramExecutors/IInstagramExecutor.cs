using System.Threading.Tasks;
using InstaBot.Common;
using InstaBot.Service.DataBaseModels;

namespace InstaBot.Service.InstagramExecutors
{
    public interface IInstagramExecutor
    {
        Task Execute(Queue queue, InstaBotContext db);
    }
}

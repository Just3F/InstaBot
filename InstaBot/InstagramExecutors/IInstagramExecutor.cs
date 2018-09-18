using InstaBot.Service.DataBaseModels;
using System.Threading.Tasks;
using InstaSharper.API;

namespace InstaBot.Service.Models
{
    public interface IInstagramExecutor
    {
        Task Execute(Queue queue, InstaBotContext db);
    }
}

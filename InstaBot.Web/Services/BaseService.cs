using InstaBot.Web.Data;

namespace InstaBot.Web.Services
{
    public abstract class BaseService
    {
        protected ApplicationDbContext _db { get; }

        protected BaseService(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}

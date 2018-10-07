using InstaBot.Web.EntityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstaBot.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<LoginDataEntity> LoginData { get; set; }
        public DbSet<QueueEntity> Queues { get; set; }
        public DbSet<UserActivityHistoryEntity> UserActivityHistories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}

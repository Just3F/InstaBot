using Microsoft.EntityFrameworkCore;

namespace InstaBot.Service.DataBaseModels
{
    public class InstaBotContext : DbContext
    {
        public DbSet<LoginDataEntity> LoginData { get; set; }
        public DbSet<QueueEntity> Queues { get; set; }
        public DbSet<UserActivityHistoryEntity> UserActivityHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=DB_instabot;");
        }
    }
}

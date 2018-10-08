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
            optionsBuilder.UseSqlServer(@"Data Source=SQL6006.site4now.net;Initial Catalog=DB_A418ED_instabot1;User Id=DB_A418ED_instabot1_admin;Password=Gfhjkm63934710;");
        }
    }
}

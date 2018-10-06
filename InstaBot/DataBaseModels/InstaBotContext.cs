using InstaBot.Common;
using Microsoft.EntityFrameworkCore;

namespace InstaBot.Service.DataBaseModels
{
    public class InstaBotContext : DbContext
    {
        public DbSet<LoginData> Users { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<UserActivityHistory> UserActivityHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=InstaDB2;Trusted_Connection=True;");
        }
    }
}

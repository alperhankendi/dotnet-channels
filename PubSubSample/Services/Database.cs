using Microsoft.EntityFrameworkCore;
using PubSubSample.Services.Data;

namespace PubSubSample.Services
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}

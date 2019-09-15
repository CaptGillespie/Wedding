using Microsoft.EntityFrameworkCore;

    namespace Wedding.Models
    {
        public class WeddingContext : DbContext
        {
            public WeddingContext(DbContextOptions options) : base(options) { }
            public DbSet<RegisterUser> Logged_In_User {get;set;}
            public DbSet<WeddingModel> MyWeddings {get;set;}
            public DbSet<Association> Associations {get;set;}

        }
    }

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ):base(options)
        {
            
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Auther> Authers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<UserOTP> userOTPs { get; set; }     
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            //use this 
            //modelBuilder.Entity<Book>().Property(e => e.Price).HasPrecision(18,2);
            //or use this also
            modelBuilder.Entity<Book>(e =>
            {
                e.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Auther>()
                .HasMany(e=>e.Books)
                .WithOne(e=>e.Auther).
                OnDelete(DeleteBehavior.Cascade);

        }

      
        
     
    }
}

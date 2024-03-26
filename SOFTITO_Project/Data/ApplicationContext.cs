using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SOFTITO_Project.Models;

namespace SOFTITO_Project.Data
{
    public class ApplicationContext : IdentityDbContext<User, IdentityRole, string>
    {
   
        public DbSet<Company>? Companies { get; set; }
        public DbSet<State>? States { get; set; }
        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<SOFTITO_Project.Models.Food>? Food { get; set; }
        public DbSet<SOFTITO_Project.Models.RestaurantBranch>? RestaurantBranch { get; set; }


        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Restaurant>().HasOne(r => r.Company).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<RestaurantBranch>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<RestaurantBranch>().HasOne(r => r.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Food>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Food>().HasOne(r => r.Branch).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Company>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasOne(r => r.Company).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id);

            base.OnModelCreating(modelBuilder);
        }

      
    }
}

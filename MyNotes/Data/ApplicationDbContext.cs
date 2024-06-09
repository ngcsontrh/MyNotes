using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNotes.Models.Entities;

namespace MyNotes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserInformation> UserInformations { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(au => au.UserInformation)
                .WithOne(ui => ui.ApplicationUser)
                .HasForeignKey<UserInformation>(ui => ui.ApplicationUserId);

            builder.Entity<UserInformation>()
                .Property(au => au.ApplicationUserId).IsRequired();

            builder.Entity<Note>()
                .Property(au => au.ApplicationUserId).IsRequired();

            builder.Entity<ApplicationUser>()
                .HasMany(au => au.Notes)
                .WithOne(n => n.ApplicationUser)
                .HasForeignKey(n => n.ApplicationUserId);

        }
    }
}

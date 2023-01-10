using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenDiscussion.Models;
using System.Runtime.CompilerServices;

namespace OpenDiscussion.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers{ get; set; }
        public DbSet <Comment> Comments { get; set; }
        public DbSet <Category> Categories { get; set; }
        public DbSet <Discussion> Discussions { get; set; }
        public DbSet <Topic> Topics { get; set; }
        public DbSet <Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Profile);

            builder.Entity<ApplicationUser>()
                .Property(u => u.DisplayName);

            builder.Entity<ApplicationUser>()
                .Property(u => u.DateOfCreation);

            builder.Entity<ApplicationUser>()
                .Property(u => u.CommentCount);

            builder.Entity<ApplicationUser>()
                .Property(u => u.DiscussionCount);
        }
    }
}
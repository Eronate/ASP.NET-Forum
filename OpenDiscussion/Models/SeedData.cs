using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenDiscussion.Data;
using static System.Net.WebRequestMethods;

namespace OpenDiscussion.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Roles.Any())
                {
                    return;
                }
                context.Roles.AddRange(
                new IdentityRole { Id = "0f0190ec-276c-41d7-9398-25c4e1714481", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole { Id = "0f0190ec-276c-41d7-9398-25c4e1714482", Name = "Moderator", NormalizedName = "Moderator".ToUpper() },
                new IdentityRole { Id = "0f0190ec-276c-41d7-9398-25c4e1714483", Name = "User", NormalizedName = "User".ToUpper() }
                );
            var hasher = new PasswordHasher<ApplicationUser>();

                context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "88744e5d-7de5-482f-b3e8-e818a5605e15",
                    DateOfCreation = DateTime.Now,
                    CommentCount = 0,
                    DiscussionCount = 0,
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    DisplayName = "admin@test.com",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },
                new ApplicationUser
                {
                    Id = "88744e5d-7de5-482f-b3e8-e818a5605e16",
                    DateOfCreation = DateTime.Now,
                    CommentCount = 0,
                    DiscussionCount = 0,
                    UserName = "moderator@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "MODERATOR@TEST.COM",
                    Email = "moderator@test.com",
                    DisplayName = "moderator@test.com",
                    NormalizedUserName = "MODERATOR@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Moderator1!")
                },
                 new ApplicationUser
                 {
                     Id = "88744e5d-7de5-482f-b3e8-e818a5605e17",
                     DateOfCreation = DateTime.Now,
                     CommentCount = 0,
                     DiscussionCount = 0,
                     UserName = "user@test.com",
                     EmailConfirmed = true,
                     NormalizedEmail = "USER@TEST.COM",
                     DisplayName = "user@test.com",
                     Email = "user@test.com",
                     NormalizedUserName = "USER@TEST.COM",
                     PasswordHash = hasher.HashPassword(null, "User1!")
                 }
                 ); ;
                context.UserRoles.AddRange(
                 new IdentityUserRole<string>
                 {
                     RoleId = "0f0190ec-276c-41d7-9398-25c4e1714481",
                     UserId = "88744e5d-7de5-482f-b3e8-e818a5605e15"
                 },
                new IdentityUserRole<string>
                {
                    RoleId = "0f0190ec-276c-41d7-9398-25c4e1714482",
                    UserId = "88744e5d-7de5-482f-b3e8-e818a5605e16"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "0f0190ec-276c-41d7-9398-25c4e1714483",
                    UserId = "88744e5d-7de5-482f-b3e8-e818a5605e17"
                });
                Profile profileAdmin = new Profile();
                profileAdmin.ApplicationUserId = "88744e5d-7de5-482f-b3e8-e818a5605e17";
                profileAdmin.Avatar = "https://st3.depositphotos.com/1767687/16607/v/450/depositphotos_166074422-stock-illustration-default-avatar-profile-icon-grey.jpg";
                Profile profileMod = new Profile();
                profileMod.ApplicationUserId = "88744e5d-7de5-482f-b3e8-e818a5605e16";
                profileMod.Avatar = "https://st3.depositphotos.com/1767687/16607/v/450/depositphotos_166074422-stock-illustration-default-avatar-profile-icon-grey.jpg";
                Profile profileUser = new Profile();
                profileUser.ApplicationUserId = "88744e5d-7de5-482f-b3e8-e818a5605e15";
                profileUser.Avatar = "https://st3.depositphotos.com/1767687/16607/v/450/depositphotos_166074422-stock-illustration-default-avatar-profile-icon-grey.jpg";
                context.Profiles.Add(profileAdmin);
                context.Profiles.Add(profileMod);
                context.Profiles.Add(profileUser);
                context.SaveChanges();
            }
        }
    }
}

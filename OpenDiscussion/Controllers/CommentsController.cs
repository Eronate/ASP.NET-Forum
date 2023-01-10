using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenDiscussion.Data;
using OpenDiscussion.Models;
using System.Data;

namespace OpenDiscussion.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            db= context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {

            Comment comment = db.Comments.Find(id);
            if (_userManager.GetUserId(User) == comment.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                return View(comment);
            else
            {
                TempData["Message"] = "Nu aveti voie sa editati comentariul altcuiva";
                return Redirect("/Discussions/Show/" + comment.DiscussionId);
            }

        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Find(id);
            requestComment.CommentId = id;
            if (ModelState.IsValid)
            {
                if (_userManager.GetUserId(User) == comment.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                {
                    comment.Content = requestComment.Content;
                    db.SaveChanges();
                    return Redirect("/Discussions/Show/" + comment.DiscussionId);
                }
                else
                {
                    TempData["Message"] = "Nu aveti voie sa editati comentariul altcuiva";
                    return Redirect("/Discussions/Show/" + comment.DiscussionId);
                }

            }
            else
            {
                return View(requestComment);
            }
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Delete(int id) 
        {
            string userId = _userManager.GetUserId(User);
            Comment comment = db.Comments.Find(id);
            if (userId == comment.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
            {
                ApplicationUser user = db.Users.Find(userId);
                user.CommentCount -= 1;
                db.Comments.Remove(comment);
                db.SaveChanges();
                return Redirect("/Discussions/Show/" + comment.DiscussionId);
            }
            else
            {
                TempData["Message"] = "Nu aveti voie sa stergeti comentariul altcuiva";
                return Redirect("/Discussions/Show/" + comment.DiscussionId);
            }

        }
    }
}

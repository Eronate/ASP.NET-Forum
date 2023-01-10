using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDiscussion.Data;
using OpenDiscussion.Models;

namespace OpenDiscussion.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public ProfilesController(ApplicationDbContext context, 
                                UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult GetId(string id)
        {
            int profileId = db.Users.Include("Profile").Where(u => u.Id == id).First().Profile.ProfileId;
            return RedirectToAction("Show", new {id = profileId});

        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id) //id va fi id-ul profilului
        {
            Profile prof = db.Profiles.Find(id); //profilul userului
            var currentUser = db.Users.Include("Profile").Where(u => u.Id == prof.ApplicationUserId).First();
            if (prof.ApplicationUserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                ViewBag.AfisareButoane = true;
            var DiscComments = db.Discussions.Include("Comments").Where(c => c.Comments.Where(com => com.UserId == prof.ApplicationUserId).Count() > 0 || c.UserId == prof.ApplicationUserId);
            ViewBag.DiscComments = DiscComments;
            return View(currentUser);
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Show(int id, [FromForm] ApplicationUser requestUser) //id va fi id-ul profilului
        {
            Profile prof = db.Profiles.Find(id); //profilul userului
            var currentUser = db.Users.Include("Profile").Where(u => u.Id == prof.ApplicationUserId).First();
            
            if (ModelState.IsValid)
            {
                if (prof.ApplicationUserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                {
                    prof.Description = requestUser.Profile.Description;
                    prof.Avatar = requestUser.Profile.Avatar;
                    db.SaveChanges();
                }
                else
                {
                    TempData["Message"] = "Nu poti edita profilul altcuiva.";
                    return Redirect("/Categories/Index");
                }
            }
            if (prof.ApplicationUserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                ViewBag.AfisareButoane = true;
            var DiscComments = db.Discussions.Include("Comments").Where(c => c.Comments.Where(com => com.UserId == prof.ApplicationUserId).Count() > 0 || c.UserId == prof.ApplicationUserId);
            ViewBag.DiscComments = DiscComments;
            return View(currentUser);
        }
    }
}

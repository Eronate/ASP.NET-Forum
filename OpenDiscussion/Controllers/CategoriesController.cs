using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDiscussion.Data;
using OpenDiscussion.Models;

namespace OpenDiscussion.Controllers
{

    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CategoriesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            // var categories = db.Categories.Include("categories");
            var categories = from category in db.Categories select category;
            ViewBag.Categories = categories;
            SetAccessRights();
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            if (User.IsInRole("Admin"))
            {
                Category category = db.Categories.Find(id);
                return View(category);
            }
            else
            {
                TempData["Message"] = "Nu aveti voie sa editati o categorie";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);
            requestCategory.CategoryId = id;
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    category.Name = requestCategory.Name;
                    category.Description = requestCategory.Description;
                    db.SaveChanges();
                }
                else
                    TempData["Message"] = "Nu aveti voie sa editati o categorie";
                return RedirectToAction("Index");
            }
            else
                return View(requestCategory);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            if(User.IsInRole("Admin"))
                return View();
            else
            {
                TempData["Message"] = "Nu aveti voie sa adaugati o categorie";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost] 
        public IActionResult New(Category requestCategory)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    db.Categories.Add(requestCategory);
                    db.SaveChanges();
                }
                else
                    TempData["Message"] = "Nu aveti voie sa adaugati o categorie";
                return RedirectToAction("Index");
            }
            else
                return View(requestCategory);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Category category = db.Categories.Include("Topics").Where(cat => cat.CategoryId == id).First();
            if (User.IsInRole("Admin"))
            {
                int categoryId = category.CategoryId;
                var topics = db.Topics.Where(topic => topic.CategoryId== categoryId);
                var topicIds = topics.Select(topic => topic.TopicId).ToList();
                var discussions = db.Discussions.Where(discussion => topicIds.Contains((int)discussion.TopicId));
                var discussionIds = discussions.Select(discussion => discussion.DiscussionId).ToList();
                var comments = db.Comments.Where(comment => discussionIds.Contains((int)comment.DiscussionId));
                foreach (var comm in comments)
                    db.Comments.Remove(comm);
                foreach (var disc in discussions)
                    db.Discussions.Remove(disc);
                foreach(var topic in topics)
                    db.Topics.Remove(topic);
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            else
                TempData["Message"] = "Nu aveti voie sa stergeti o categorie";
            return RedirectToAction("Index");
        }
        private void SetAccessRights()
        {
            ViewBag.ShowButtons = false;
            if(User.IsInRole("Admin"))
                ViewBag.ShowButtons = true;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDiscussion.Data;
using OpenDiscussion.Models;
using System.Data;

namespace OpenDiscussion.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TopicsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index(int id)
        {
            ViewBag.TopicCategoryId = id;
            var topic = db.Topics.Where(top => top.CategoryId == id);
            ViewBag.Topics = topic;
            SetAccessRights();
            return View();
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult New(int id)
        {
            if(User.IsInRole("Moderator") || User.IsInRole("Admin"))
            {
                ViewBag.TopicCategoryId = id;
                return View();
            }
            else
            {
                TempData["Message"] = "Nu aveti voie sa adaugati un topic";
                return RedirectToAction("Index", new {id = id});
            }
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult New(Topic topic)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Moderator") || User.IsInRole("Admin"))
                {
                    db.Topics.Add(topic);
                    db.SaveChanges();
                }
                else
                    TempData["Message"] = "Nu aveti voie sa adaugati un topic";
                return RedirectToAction("Index", new { id = topic.CategoryId });
            }
            else
                return View(topic);
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Edit(int id)
        {
            Topic topic = db.Topics.Find(id);
            if(User.IsInRole("Moderator") || User.IsInRole("Admin"))
                return View(topic);
            else
            {
                TempData["Message"] = "Nu aveti voie sa editati un topic";
                return RedirectToAction("Index", new {id = topic.CategoryId});
            }
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Topic requestTopic)
        {
            Topic topic = db.Topics.Find(id);
            requestTopic.TopicId = id;
            if (ModelState.IsValid)
            {
                if(User.IsInRole("Moderator") || User.IsInRole("Admin"))
                {
                    topic.Name = requestTopic.Name;
                    topic.Description = requestTopic.Description;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = topic.CategoryId });
                }
                else
                {
                    TempData["Messagee"] = "Nu aveti voie sa editati un topic";
                    return RedirectToAction("Index", new { id = topic.CategoryId });
                }
            }
            else
                return View(requestTopic);
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Topic topic = db.Topics.Include("Discussions").Where(top => top.TopicId == id).First();
            if (User.IsInRole("Moderator") || User.IsInRole("Admin"))
            {
                int topicId = topic.TopicId;
                var discussions = db.Discussions.Where(discussion => discussion.TopicId == topicId);
                var discussionIds = discussions.Select(discussion => discussion.DiscussionId).ToList();
                var comments = db.Comments.Where(comment => discussionIds.Contains((int)comment.DiscussionId));
                foreach(var comm in comments)
                    db.Comments.Remove(comm);
                foreach (var disc in discussions)
                    db.Discussions.Remove(disc);
                //db.Discussions.Remove(discussions.AsEnumerable());
                //db.Comments.Remove(comments);
                db.Topics.Remove(topic);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = topic.CategoryId });
        }

        private void SetAccessRights()
        {
            ViewBag.ShowButtons = false;
            if(User.IsInRole("Moderator"))
                ViewBag.ShowButtons = true;
            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.CurrentUser = _userManager.GetUserId(User);
        }
    }
}

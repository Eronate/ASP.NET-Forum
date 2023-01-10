using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenDiscussion.Data;
using OpenDiscussion.Models;
using System.Data;
using System.Drawing.Printing;
using System.Linq;

namespace OpenDiscussion.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public const string SessionKeySort = "";
        public DiscussionsController(ApplicationDbContext context,
                                     UserManager<ApplicationUser> userManager,
                                     RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Sorted(int id)
        {
            string radioButtonValue = Request.Form["Sort"].ToString();
            HttpContext.Session.SetString(SessionKeySort, radioButtonValue);
            return RedirectToAction("Index", new {id = id });
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index(int id)
        {
            var discussions1 = db.Discussions.Include("User").Include("User.Profile")
                    .Where(disc => disc.TopicId == id);

            var search = "";
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List <int> initialIds = discussions1.Select(discussion => discussion.DiscussionId).ToList();
                List <int> discussionIds = discussions1.Where(
                                            discussion => discussion.Title.Contains(search)
                                            || discussion.Content.Contains(search)).Select(discussion => discussion.DiscussionId).ToList();
                List<int> discussionIdsOfComments = db.Comments.Where(
                                                        comment => comment.Content.Contains(search) && initialIds.Contains((int)comment.DiscussionId)).Select(comment => comment.CommentId).ToList();
                List<int> mergedDiscussionIds = discussionIds.Union(discussionIdsOfComments).ToList();
                discussions1 = db.Discussions.Where(discussion => mergedDiscussionIds.Contains(discussion.DiscussionId)).Include("User").Include("User.Profile");
            }

            ViewBag.SearchString = search;

            foreach (var disc in discussions1.AsEnumerable())
                disc.CommentsCount = db.Comments.Where(d => d.DiscussionId == disc.DiscussionId).Count();

            IEnumerable<Discussion> discussions;

            if (HttpContext.Session.GetString(SessionKeySort) == "Comments") 
                discussions = discussions1.AsEnumerable().OrderByDescending(d => d.CommentsCount);
            else if (HttpContext.Session.GetString(SessionKeySort) == "Date")
                discussions = discussions1.AsEnumerable().OrderByDescending(d => d.Date);
            else
                discussions = discussions1.AsEnumerable().OrderByDescending(d => d.Date);

            var topicCount = db.Topics.Where(top => top.TopicId == id).Count();
            int _perpage = 5, offset = 0;
            int countItems = discussions.Count();
            if (search != "")
            {
                if (countItems == 0)
                    ViewBag.NotFound = "Nu au fost gasite rezultate pentru cautarea dumneavoastra.";
                else
                    ViewBag.NotFound = "";
            }
            int currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            int lastpage = countItems / _perpage + Convert.ToInt32(countItems % _perpage != 0);
            ViewBag.lastpage = lastpage;
            if (currentPage != 0)
            {
                offset = (currentPage - 1) * _perpage;
            }

            var discussionsPaginated = discussions.Skip(offset).Take(_perpage);

            if (topicCount > 0)
            { 
                var topic = db.Topics.Where(top => top.TopicId == id).First();
                ViewBag.DiscussionCategoryId = topic.CategoryId;
            }
            ViewBag.DiscussionTopicId = id;
            ViewBag.Discussions = discussionsPaginated;
            SetAccessRights();
            return View();
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {
            Discussion discussion = db.Discussions.Find(id);
            if (_userManager.GetUserId(User) == discussion.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                return View(discussion);
            else
            {
                TempData["Message"] = "Nu aveti voie sa editati o discutie care nu va apartine";
                return RedirectToAction("Index", new { id = discussion.TopicId });
            }
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Discussion requestDiscussion)
        {
            Discussion discussion = db.Discussions.Find(id);
            requestDiscussion.DiscussionId = id;
            if (ModelState.IsValid)
            {
                if (_userManager.GetUserId(User) == discussion.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                {
                    discussion.Title = requestDiscussion.Title;
                    discussion.Content = requestDiscussion.Content;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = discussion.TopicId });
                }
                else
                {
                    TempData["Message"] = "Nu aveti voie sa editati o discutie care nu va apartine";
                    return RedirectToAction("Index", new { id = discussion.TopicId }); 
                }
            }
            else
                return View(requestDiscussion);
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(int id)
        {
            Discussion discussion = new Discussion();
            discussion.TopicId = id;
            return View(discussion);
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult New(int id, [FromForm] Discussion discussion)
        {
            string userId = _userManager.GetUserId(User);
            discussion.UserId = userId;
            discussion.Date = DateTime.Now;
            discussion.TopicId = id;

            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(userId);
                user.DiscussionCount += 1;
                db.Discussions.Add(discussion);
                db.SaveChanges();

                return RedirectToAction("Index", new { id = discussion.TopicId });
            }
            else
            {
                Console.WriteLine("Aici e: \n");
                Console.WriteLine(discussion.DiscussionId);
                Console.WriteLine(discussion.Content);
                Console.WriteLine(discussion.Title);
                Console.WriteLine(discussion.TopicId);
                return View(discussion);
            }
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Discussion discussion = db.Discussions.Include("Comments").Where(diss => diss.DiscussionId == id).First();
            string userId = _userManager.GetUserId(User);
            if (userId == discussion.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
            {
                ApplicationUser user = db.Users.Find(userId);
                user.DiscussionCount -= 1;
                var comments = db.Comments.Where(comm => comm.DiscussionId == discussion.DiscussionId);
                user.CommentCount -= comments.Count();
                foreach (var comm in comments)
                    db.Comments.Remove(comm);
                db.Discussions.Remove(discussion);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = discussion.TopicId });
            }
            else
            {
                TempData["Message"] = "Nu aveti voie sa stergeti o discutie care nu va apartine";
                return RedirectToAction("Index", new { id = discussion.TopicId });
            }

        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            Discussion discussion = db.Discussions
                                    .Include("Comments")
                                    .Include("User")
                                    .Include("User.Profile")
                                    .Include("Comments.User")
                                    .Include("Comments.User.Profile")
                                    .Where(disc => disc.DiscussionId == id)
                                    .First();

            int _perpage = 5, offset = 0;
            int countComms = db.Comments.Where(com => com.DiscussionId == id).Count();
            int currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            int lastpage = countComms / _perpage + Convert.ToInt32(countComms % _perpage != 0);
            ViewBag.lastpage = lastpage;
            if (currentPage != 0)
            {
                offset = (currentPage - 1) * _perpage;
            }
            ViewBag.discussionPaginated = discussion.Comments.Skip(offset).Take(_perpage);

            SetAccessRights();
            return View(discussion);
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Show(int id, [FromForm] Comment comment)
        {
            string userId = _userManager.GetUserId(User);
            comment.UserId = userId;
            comment.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(userId);
                user.CommentCount += 1;
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Show", new { id = comment.DiscussionId });
            }
            else
            {
                Discussion discussion = db.Discussions.Include("Comments")
                                                      .Include("User")
                                                      .Include("User.Profile")
                                                      .Include("Comments.User")
                                                      .Include("Comments.User.Profile")
                                                      .Where(disc => disc.DiscussionId == comment.DiscussionId)
                                                      .First();
                int _perpage = 5, offset = 0;
                int countComms = db.Comments.Where(com => com.DiscussionId == id).Count();
                int currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
                int lastpage = countComms / _perpage + Convert.ToInt32(countComms % _perpage != 0);
                ViewBag.lastpage = lastpage;
                if (currentPage != 0)
                {
                    offset = (currentPage - 1) * _perpage;
                }
                ViewBag.discussionPaginated = discussion.Comments.Skip(offset).Take(_perpage);
                SetAccessRights();
                return View(discussion);
            }
        }
        [Authorize(Roles = "Moderator,Admin")]
        public IEnumerable<SelectListItem> GetTopics()
        {
        var selectList = new List<SelectListItem>();
        var topics = from top in db.Topics select top;
        foreach(var topic in topics)
            {
                selectList.Add(new SelectListItem
                {
                    Value = topic.TopicId.ToString(),
                    Text = topic.Name
                }); 
            }
            return selectList;
        }
        [Authorize(Roles ="Moderator,Admin")]
        public IActionResult Move(int id)
        {
            Discussion disc = db.Discussions.Find(id);
            disc.AllTopics = GetTopics();
            return View(disc);
        }
        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public IActionResult Move(int id, [FromForm] Discussion requestDiscussion )
        {
            var topicId = Convert.ToInt32(requestDiscussion.TopicId);
            var disc = db.Discussions.Find(id);
            var TopicIdOld = disc.TopicId;
            try
            {
                disc.TopicId = topicId;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = TopicIdOld });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Move", new { id = id });
            }
        }
        private void SetAccessRights()
        {
            ViewBag.ShowButtons = false;
            if(User.IsInRole("Moderator") || User.IsInRole("Admin"))
                ViewBag.ShowButtons = true;
            ViewBag.CurrentUser = _userManager.GetUserId(User);
        }
    }
}

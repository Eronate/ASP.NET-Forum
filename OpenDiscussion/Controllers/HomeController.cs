using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenDiscussion.Data;
using OpenDiscussion.Models;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace OpenDiscussion.Controllers
{
    public struct discussionDetails
    {
        public string discussionTitle, discussionContent, topicName, categoryName;
        public DateTime discussionDate;
        public int nrComments, discussionId;
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext db;

        private readonly UserManager <ApplicationUser> _userManager;

        private readonly RoleManager < IdentityRole > _roleManager;
        public HomeController(
            ApplicationDbContext context,
            UserManager <ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<HomeController> logger)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Categories");
            var Discussions = (from discussion in db.Discussions
                               join topic in db.Topics
                                    on discussion.TopicId equals topic.TopicId into discussionDet
                               from disc in discussionDet
                               join category in db.Categories
                                    on disc.CategoryId equals category.CategoryId
                               select new {discussionId = discussion.DiscussionId, discussionTitle = discussion.Title, discussionContent = discussion.Content, discussionDate = discussion.Date, topicName = disc.Name, categoryName = category.Name}).ToList();
            //var Discussions = (from discussion in db.Discussions select discussion);
            int N = Discussions.Count();
            Random random = new Random();
            List<discussionDetails> discutii = new List<discussionDetails>();
            for(int k = 1; k <= 3; k++)
            {
                int randomNumber = random.Next(0, N);
                discutii.Add(new discussionDetails
                {
                    discussionTitle = Discussions[randomNumber].discussionTitle,
                    discussionContent = Discussions[randomNumber].discussionContent,
                    discussionDate = Discussions[randomNumber].discussionDate,
                    topicName = Discussions[randomNumber].topicName,
                    categoryName = Discussions[randomNumber].categoryName,
                    nrComments = db.Comments.Where(comment => comment.DiscussionId == Discussions[randomNumber].discussionId).Count()
                });
            }
            ViewBag.DisplayArticles = discutii;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
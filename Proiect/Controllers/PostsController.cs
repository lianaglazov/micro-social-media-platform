using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
   
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private object article;

        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //[Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            SetAccessRights();
            var posts = db.Posts.Include("User")
                        .OrderByDescending(p => p.Date)
                        .ToList();
            var friends = db.Friends.Where(f =>f.UserUrmaritorId == _userManager.GetUserId(User))
                                    .Select(f => f.UserUrmaritId)
                                    .ToList();
            ViewBag.Friends = friends;
            ViewBag.Posts = posts;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        //[Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("User").Include("Comments").Include("Comments.User")
                               .Where(p => p.Id == id)
                               .First();
            SetAccessRights();
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Like(int postId)
        {
            var post = db.Posts.Find(postId);

            if (post == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            // verifica daca userul a dat deja like
            var hasLiked = db.Likes.Any(l => l.PostId == postId && l.UserId == userId);

            if (!hasLiked)
            {
                post.Likes++;
                db.Likes.Add(new Like { PostId = postId, UserId = userId });
                db.SaveChanges();
            }
            else
            {
                post.Likes--;
                db.Likes.Remove(db.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId));
                db.SaveChanges();
            }

            var likeAdded = !hasLiked;

            return Json(new { likesCount = post.Likes, likeAdded = likeAdded, userId = userId });
        }
        
        public IActionResult GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return Json(userId);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }
            else
            {
                Post p = db.Posts.Include("User")
                                 .Include("Comments")
                                 .Include("Comments.User")
                                 .Where(p => p.Id == comment.PostId)
                                 .First();

                SetAccessRights();

                return View(p);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Post post = new Post();

            return View(post);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Post post)
        {
            //var sanitizer = new HtmlSanitizer();
            post.Date = DateTime.Now;
            post.Likes = 0;

            post.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                //post.Content = sanitizer.Sanitize(post.Content);
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost trimisă";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Post post = db.Posts.Where(p => p.Id == id)
                                         .First();
            if (post.UserId == _userManager.GetUserId(User))
            {
                return View(post);
            }

            else
            {
                TempData["message"] = "Nu aveți dreptul să editați o postare care nu vă aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

  
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {

            Post post  = db.Posts.Find(id);

            if (ModelState.IsValid)
            {
                if (post.UserId == _userManager.GetUserId(User))
                {
                    post.Content = requestPost.Content;
                    TempData["message"] = "Postarea a fost editată!";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveți dreptul să editați o postare care nu vă aparține!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts .Include("Comments")
                          .Where(p => p.Id == id)
                          .First();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost stearsa";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul să ștergeți o postare care nu vă aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteUser = User.IsInRole("User");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}

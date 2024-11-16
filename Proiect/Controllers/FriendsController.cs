using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private object article;

        public FriendsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var friendsIds = db.Friends
                             .Where(f => f.UserUrmaritId == userId)
                             .Select(f => f.UserUrmaritorId)
                             .ToList();
            var friends = db.ApplicationUsers.Where(u => friendsIds.Contains(u.Id)).ToList();
            ViewBag.Friends = friends;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(string UserUrmaritId, string UserUrmaritorId)
        {
            Friend friend = new Friend();
            friend.UserUrmaritorId = UserUrmaritorId;
            friend.UserUrmaritId = UserUrmaritId;
            db.Friends.Add(friend);

            Request request = db.Requests.Where(r => r.UserId == UserUrmaritId && r.RequestUserId == UserUrmaritorId)
                                .First();
            db.Requests.Remove(request);                   
            db.SaveChanges();
            TempData["message"] = "Prieten adaugat cu succes!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(string UserUrmaritorId)
        {
            var userId = _userManager.GetUserId(User);
            Friend friend = db.Friends
                          .Where(f => f.UserUrmaritorId == UserUrmaritorId && f.UserUrmaritId == userId)
                          .First();

             db.Friends.Remove(friend);
             db.SaveChanges();
             TempData["message"] = "Prietenia a fost inlaturata";
             TempData["messageType"] = "alert-success";
             return RedirectToAction("Index");
           
        }

    }
}

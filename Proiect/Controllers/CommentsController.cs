using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class CommentsController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
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
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User))
                return View(comm);
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un comentariu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User))
            {
                if (ModelState.IsValid)
                {
                    comm.Text = requestComment.Text;

                    db.SaveChanges();

                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un comentariu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }

        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un comentariu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }
    }
}

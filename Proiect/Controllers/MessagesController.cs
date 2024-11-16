using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class MessagesController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public MessagesController(
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
            Message msg = db.Messages.Find(id);
            if (msg.UserId == _userManager.GetUserId(User))
                return View(msg);
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un mesaj care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Conversations");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Message requestMessage)
        {
            Message msg = db.Messages.Find(id);
            if (msg.UserId == _userManager.GetUserId(User))
            {
                if (ModelState.IsValid)
                {
                    msg.Content = requestMessage.Content;

                    db.SaveChanges();

                    return Redirect("/Conversations/Show/" + msg.ConversationId);
                }
                else
                {
                    return View(requestMessage);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un mesaj care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Conversations");
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Message msg = db.Messages.Find(id);
            if (msg.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Messages.Remove(msg);
                db.SaveChanges();
                return Redirect("/Conversations/Show/" + msg.ConversationId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un mesaj care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Conversations");
            }
        }
    }
}

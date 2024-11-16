using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Proiect.Data;
using Proiect.Data.Migrations;
using Proiect.Models;
using System.Linq.Expressions;
namespace Proiect.Controllers
{
    public class ConversationsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ConversationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        //userii pot vedea conversatiile proprii
        //adminul poate vedea toate conversatiile
        [Authorize(Roles = "Admin,User")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            SetAccessRights();

            if (User.IsInRole("User"))
            {

                var conversations = from conversation in db.Conversations.Include("AppUserConversations.ApplicationUser")
                                    where conversation.AppUserConversations.Any(uc => uc.UserId == _userManager.GetUserId(User))
                                    select conversation;

                ViewBag.Conversations = conversations;

                return View();
            }
            else
            if (User.IsInRole("Admin"))
            {

                var conversations = from conversation in db.Conversations.Include("AppUserConversations.ApplicationUser")
                                    select conversation;

                ViewBag.Conversations = conversations;

                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi asupra conversatiei";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }

        [Authorize(Roles="Admin,User")]
        public IActionResult Show(int id)
        {
            SetAccessRights();

            var conversation = db.Conversations.Include("AppUserConversations.ApplicationUser")
                                               .Include("Messages").Include("Messages.User")
                                               .Where(c => c.Id == id).FirstOrDefault(); 
            if(User.IsInRole("Admin") && conversation.AppUserConversations.Any(uc => uc.UserId == _userManager.GetUserId(User)))
            {
                ViewBag.AdminInConv = true;
            }
            else
                ViewBag.AdminInConv = false;
            return View(conversation);
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Message message)
        {
            message.MessageTime = DateTime.Now;
            message.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Conversations/Show/" + message.ConversationId);
            }
            else
            {
                Conversation c = db.Conversations.Include("Messages")
                                                 .Include("Messages.User")
                                                 .Where(c => c.Id == message.ConversationId)
                                                 .First();
                var userConversations = db.AppUserConversations
                        .Where(uc => uc.ConversationId == c.Id)
                        .ToList();

               
                c.AppUserConversations = userConversations;
                SetAccessRights();
                if (User.IsInRole("Admin") && c.AppUserConversations.Any(uc => uc.UserId == _userManager.GetUserId(User)))
                {
                    ViewBag.AdminInConv = true;
                }
                else
                    ViewBag.AdminInConv = false;

                return View(c);
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            var currentUser = _userManager.GetUserId(User);
            ViewBag.UserCurent= currentUser;
            ViewBag.AllUsers = _userManager.Users.ToList();
            Conversation conv = new Conversation();

            return View();
        }

        
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult New(Conversation conv, List<string> selectedUsers)
        {
            
            if (selectedUsers == null || !selectedUsers.Any())
            {
                // eroare daca nu au fost selectati alti useri participanti la conversatie
                TempData["message"] = "Selectati cel putin un utilizator pentru conversatie.";
                TempData["messageType"] = "alert-danger";

                return RedirectToAction("New");
            }
            var uId = _userManager.GetUserId(User);
            var currentUser = db.ApplicationUsers.Where(user => user.Id == uId).First();

            //se adauga userul care a creat conversatia in baza de date
            var userConv = new AppUserConversation();
            userConv.Conversation = conv;
            userConv.ApplicationUser = currentUser;
            db.AppUserConversations.Add(userConv);

            //se adauga userii selectati de userul curent in conversatie
            foreach (var user in selectedUsers)
            {
                var userToAdd = db.ApplicationUsers.Where(u => u.Id == user).First();
                if (userToAdd != null)
                {
                    var userConversation = new AppUserConversation();
                    userConversation.ApplicationUser = userToAdd;
                    userConversation.Conversation = conv;
                    db.AppUserConversations.Add(userConversation);
                }

            }
            db.Conversations.Add(conv);
            db.SaveChanges();

            TempData["message"] = "Conversatia a fost creata";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
            
        }
        //edit - membrii unei conversatii pot adauga persoane noi
        [Authorize(Roles="User,Admin")]
        public IActionResult Edit(int id)
        {

            Conversation conv = db.Conversations.Include("AppUserConversations.ApplicationUser")
                                    .Where(c => c.Id == id)
                                    .First();
            
            if (conv.AppUserConversations.Any(uc => uc.UserId == _userManager.GetUserId(User)))
            {
                var currentUser = _userManager.GetUserId(User);
                ViewBag.UserCurent = currentUser;
                var userConversations = db.AppUserConversations
                                .Where(uc => uc.ConversationId == conv.Id)
                                .ToList();
                var allUsers = _userManager.Users.ToList();
                var usersNotIn = new List<ApplicationUser>();
                foreach (var user in allUsers)
                {
                    if (conv.AppUserConversations.Any(u => u.UserId == user.Id))
                        continue;
                    usersNotIn.Add(user);
                }
                ViewBag.UsersNotIn = usersNotIn;
                return View(conv);
            }

            else
            {
                TempData["message"] = "Nu aveți dreptul să adăugați membrii noi într-o conversație care nu vă aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, List<string> selectedUsers)
        {
            Conversation conv = db.Conversations.Find(id);
            foreach (var user in selectedUsers)
            {
                var userToAdd = db.ApplicationUsers.Where(u => u.Id == user).First();
                var userConversation = new AppUserConversation();
                userConversation.ApplicationUser = userToAdd;
                userConversation.Conversation = conv;
                db.AppUserConversations.Add(userConversation);
                

            }
            db.SaveChanges();
            TempData["message"] = "Ati adaugat membrii noi in conversatie!";
            TempData["messageType"] = "alert-success";
            return Redirect("/Conversations/Show/" + id);

        }

        //cand un utilizator iese din grup, se sterge instanta sa din UserConversation
        [Authorize(Roles ="User,Admin")]
        [HttpPost]
        public ActionResult Leave(int convId, string userId)
        {
            AppUserConversation uc = db.AppUserConversations.FirstOrDefault(uc => uc.UserId == userId && uc.ConversationId == convId);
            db.AppUserConversations.Remove(uc);
            db.SaveChanges();
            var participanti = db.AppUserConversations.Count(uc => uc.ConversationId == convId);

            if (participanti == 0)
            {
                // cand nu mai exista useri intr-o conversatie aceasta va fi stearsa
                Conversation conversation = db.Conversations.Include("Messages")
                                                            .Where(c => c.Id == convId)
                                                            .First();

                if (conversation != null)
                {
                    db.Conversations.Remove(conversation);
                    db.SaveChanges();
                }
            }
            
            TempData["message"] = "Ati parasit conversatia";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }
        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

    }
}

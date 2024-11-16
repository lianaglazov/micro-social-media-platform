using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;
        private IWebHostEnvironment _env;

        public ApplicationUsersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _env = env;
        }
        //[Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            int _perPage = 3;

            var users = db.ApplicationUsers.OrderBy(a => a.UserName); ;
            ViewBag.Users = users;
            var search = "";
            var usercurent = _userManager.GetUserId(User);
            ViewBag.UserCurent = usercurent;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }



            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                // eliminam spatiile libere
                search =
                Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare in user (UserName)
                List<string> searchuserIds = users.Where
                (
                at => at.UserName.Contains(search)
                ).Select(a => a.Id).ToList();


                // Lista userilor care contin cuvantul cautat

                // users = (DbSet<ApplicationUser>)db.ApplicationUsers.Where(user =>
                //searchuserIds.Contains(user.Id));
                users = db.ApplicationUsers.Where(user => searchuserIds.Contains(user.Id)).OrderBy(a => a.UserName); ;


            }
            ViewBag.SearchString = search;

            // Fiind un numar variabil de useri, verificam de
            // fiecare data utilizand
            // metoda Count()
            int totalItems = users.Count();

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page
            // din ruta
            // /ApplicationUsers/Index?page=valoare
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3
            // Asadar offsetul este egal cu numarul de useri
            //care au fost deja afisati pe paginile anterioare
            var offset = 0;
            // Se calculeaza offsetul in functie de numarul
            //paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }



            // Se preiau userii corespunzatoari pentru
            //fiecare pagina la care ne aflam
            // in functie de offset
            var paginatedArticles =
            users.Skip(offset).Take(_perPage);

            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems /
            (float)_perPage);
            // Trimitem userii cu ajutorul unui ViewBag
            //catre View-ul corespunzator
            ViewBag.Users = paginatedArticles;


            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/ApplicationUsers/Index/?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/ApplicationUsers/Index/?page";
            }

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {

            return View();
        }



        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(ApplicationUser user)
        {

            return View(user);

        }


        //AFISARE a profilului


       // [Authorize(Roles = "User,Admin")]
        public IActionResult Show(string id)
        {
            ApplicationUser user = db.ApplicationUsers.Include("Posts")
                                         .Include("Comments")
                                         .Where(user => user.Id == id)
                                         .First();

            SetAccessRights();
            var UserCurent = _userManager.GetUserId(User);

            Friend friend = db.Friends.Where(r => r.UserUrmaritId == id && r.UserUrmaritorId == UserCurent)
                                .FirstOrDefault();
            if(friend != null)
            {
                ViewBag.Urmareste = 1;
            }

            return View(user);
        }

        //EDITARE 


        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(string id)
        {

            ApplicationUser user = db.ApplicationUsers.Include("Posts")
                                        .Where(user => user.Id == id)
                                        .First();

            user.PrivOption = PublicPrivateList();

            if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(user);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        //EDIT cu POST

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser requestUser)
        {
            ApplicationUser user = db.ApplicationUsers.Find(id);


            //if (ModelState.IsValid)
            //{
                if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    user.UserName = requestUser.UserName;
                    user.Description = requestUser.Description;
                    user.Privacy = requestUser.Privacy;
                    user.ProfileImage = requestUser.ProfileImage;

                    TempData["message"] = "Profilul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Show", new { id = user.Id });
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            //}
            //else
            //{

               // return View(requestUser);
            //}
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult DeleteProfilePic(string id)
        {
            ApplicationUser user = db.ApplicationUsers.Find(id);

            user.ProfileImage = null;
            db.SaveChanges();

            return RedirectToAction("Show", new { id = user.Id });
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult SendRequest(string id)
        {
            var idUsercurent = _userManager.GetUserId(User);
            ApplicationUser user = db.ApplicationUsers.Find(id);
            Request req = new Request();
            req.UserId = user.Id;
            req.RequestUserId = idUsercurent;
            db.Requests.Add(req);
            db.SaveChanges();
            TempData["message"] = "Cererea a fost trimisa";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");

        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Requests(string id)
        {
            var requests = db.Requests.Where(r => r.UserId == id)
                                      .Select(f => f.RequestUserId)
                                      .ToList();

            var users = new List<ApplicationUser>();

            foreach (var requestId in requests)
            {
                var requestedUser = db.ApplicationUsers.FirstOrDefault(u => u.Id == requestId);
                if (requestedUser != null)
                {
                    users.Add(requestedUser);
                }
            }
            SetAccessRights();

            ViewBag.RequestedUsers = users;

            ViewBag.Requests = requests;

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult DeleteRequest(string UserUrmaritId, string UserUrmaritorId)
        {
            Request request = db.Requests.Where(r => r.UserId == UserUrmaritId && r.RequestUserId == UserUrmaritorId)
                                .FirstOrDefault();
            db.Requests.Remove(request);
            db.SaveChanges();
            TempData["message"] = "Cererea a fost stearsa cu succes!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index", "Friends");
        }


        private void SetAccessRights()
        {
            //ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteUser = User.IsInRole("User");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        //Lista cu optiunea profil privat/public
        [NonAction]
        public IEnumerable<SelectListItem> PublicPrivateList()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();
            var publicacc = "Profil public";
            var privateacc = "Profil privat";

            // adaugam in lista elementele necesare pentru dropdown
            selectList.Add(new SelectListItem
            {
                Value = '0'.ToString(),
                Text = publicacc.ToString()
            });
            selectList.Add(new SelectListItem
            {
                Value = '1'.ToString(),
                Text = privateacc.ToString()
            });

            return selectList;

        }
    }
}

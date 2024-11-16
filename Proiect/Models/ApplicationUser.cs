using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Description { get; set; }
        public int? PostsNumber { get; set; }
        public int? Followers { get; set; }

        //var in care vom stoca daca profilul este privat sau nu
        //null sau 0 => public (null pentru ca initial profilul se creeaza ca si public)
        // 1=> privat
        public int Privacy { get; set; }
        public string? ProfileImage { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<AppUserConversation>? AppUserConversations { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? PrivOption { get; set; }

        public static implicit operator ApplicationUser(Task<ApplicationUser> v)
        {
            throw new NotImplementedException();
        }
    }
}

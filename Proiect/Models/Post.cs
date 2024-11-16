using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int Likes { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? LikesList { get; set; }
    }
}

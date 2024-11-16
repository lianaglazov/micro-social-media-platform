using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Textul este obligatoriu")]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? PostId { get; set; }
        public virtual Post? Post { get; set; }
    }
}

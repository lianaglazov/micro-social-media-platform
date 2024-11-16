using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }
        //un mesaj este trimis de un user
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        //un mesaj se afla intr-o conversatie
        public int? ConversationId { get; set; }
        public virtual Conversation? Conversation { get; set; }

        public DateTime MessageTime { get; set; }
    }
}

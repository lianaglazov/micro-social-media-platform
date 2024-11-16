using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class AppUserConversation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public int? ConversationId { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }
}

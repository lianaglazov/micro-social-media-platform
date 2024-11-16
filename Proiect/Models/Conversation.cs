using NuGet.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        //o conversatie are mai multe mesaje

        public virtual ICollection<Message>? Messages { get; set; }

        //conversatiile au mai multi useri
        public virtual ICollection<AppUserConversation>? AppUserConversations{ get; set; }
    }
}

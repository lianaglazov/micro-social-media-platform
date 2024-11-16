using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? RequestUserId { get; set; }
    }
}

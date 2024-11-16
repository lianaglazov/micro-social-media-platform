using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }
        public string? UserUrmaritId { get; set; }
        public string? UserUrmaritorId { get; set; }

    }
}

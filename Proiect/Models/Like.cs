namespace Proiect.Models
{
    public class Like
    {
        public int Id { get; set; }

        // fk carte post
        public int PostId { get; set; }
        public Post Post { get; set; }

        // fk catre user (userul care a dat like postarii)
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

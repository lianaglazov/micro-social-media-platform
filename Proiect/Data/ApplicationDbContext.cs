using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proiect.Models;

namespace Proiect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<AppUserConversation> AppUserConversations { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUserConversation>()
                .HasKey(uc => new
                {
                    uc.Id,
                    uc.UserId,
                    uc.ConversationId
                });

            modelBuilder.Entity<AppUserConversation>()
                .HasOne(uc => uc.ApplicationUser)
                .WithMany(uc => uc.AppUserConversations)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<AppUserConversation>()
                .HasOne(uc => uc.Conversation)
                .WithMany(uc => uc.AppUserConversations)
                .HasForeignKey(uc => uc.ConversationId);

        }
    }
}

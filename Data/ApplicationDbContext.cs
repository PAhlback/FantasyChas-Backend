using FantasyChas_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ActiveStory> ActiveStories { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<SavedStory> SavedStories { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Species> Species { get; set; }
    }
}

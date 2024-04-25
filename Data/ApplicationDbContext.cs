using FantasyChas_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        DbSet<ActiveStory> ActiveStories { get; set; }
        public DbSet<Character> Characters { get; set; }
        DbSet<ChatHistory> ChatHistories { get; set; }
        DbSet<SavedStory> SavedStories { get; set; }
        DbSet<Profession> Professions { get; set; }
        DbSet<Species> Species { get; set; }
    }
}

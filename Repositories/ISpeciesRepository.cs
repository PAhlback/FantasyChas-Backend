using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;

namespace FantasyChas_Backend.Repositories
{
    public interface ISpeciesRepository
    {
        public Species GetSpeciesById(int speciesId);
    }

    public class SpeciesRepository : ISpeciesRepository
    {
        private static ApplicationDbContext _context;

        public SpeciesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Species GetSpeciesById(int speciesId)
        {
            try
            {
                Species? species = _context.Species
                    .Where(p => p.Id == speciesId)
                    .SingleOrDefault();

                if (species == null)
                {
                    throw new Exception("No species found");
                }

                return species;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

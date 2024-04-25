using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;

namespace FantasyChas_Backend.Repositories
{
    public interface IProfessionRepository
    {
        public Profession GetProfessionById(int professionId);
    }

    public class ProfessionRepository : IProfessionRepository
    {
        private static ApplicationDbContext _context;

        public ProfessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Profession GetProfessionById(int professionId)
        {
            try
            {
                Profession? profession = _context.Professions
                    .Where(p => p.Id == professionId)
                    .SingleOrDefault();

                if(profession == null)
                {
                    throw new Exception("No profession found");
                }

                return profession;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Reflection.Metadata.Ecma335;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _context;
        public VillaRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }
        public async Task<Villa> UpdateAsync(Villa villa)
        {
            villa.UpdatedDate = DateTime.Now;

            _context.Update(villa);
            await _context.SaveChangesAsync();

            return villa; 
        }
    }
}

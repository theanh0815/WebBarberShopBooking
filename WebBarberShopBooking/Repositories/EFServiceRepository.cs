using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;

namespace WebBarberShopBooking.Repositories
{
    public class EFServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;
        public EFServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services.ToListAsync();
        }
        public async Task<Service> GetServiceByIdAsync(int id)
        {
            return await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task AddServiceAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateServiceAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteServiceAsync(int id)
        {
            var service = await GetServiceByIdAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }
}

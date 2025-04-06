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
        public IEnumerable<Service> GetAllServices()
        {
            return _context.Services.ToList();
        }
        public Service GetServiceById(int id)
        {
            return _context.Services.Find(id);
        }
        public void AddService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
        }
        public void UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
        }
        public void DeleteService(int id)
        {
            var service = GetServiceById(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }
        }
    }
}

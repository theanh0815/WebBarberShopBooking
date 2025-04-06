using System.Collections.Generic;
using WebBarberShopBooking.Models;

namespace WebBarberShopBooking.Repositories
{
    public interface IServiceRepository
    {
        IEnumerable<Service> GetAllServices();
        Service GetServiceById(int id);
        void AddService(Service service);
        void UpdateService(Service service);
        void DeleteService(int id);
    }
}

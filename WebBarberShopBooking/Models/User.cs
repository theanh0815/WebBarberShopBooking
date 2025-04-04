using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebBarberShopBooking.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

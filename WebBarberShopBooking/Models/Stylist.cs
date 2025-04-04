using System.ComponentModel.DataAnnotations;

namespace WebBarberShopBooking.Models
{
    public class Stylist
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thợ làm tóc là bắt buộc.")]
        [MaxLength(255)]
        public string Name { get; set; }

        public string? Bio { get; set; } // Tiểu sử/giới thiệu về thợ

        public string? ImageUrl { get; set; } // Ảnh đại diện

        // Navigation property
        public ICollection<Appointment> Appointments { get; set; }
    }
}

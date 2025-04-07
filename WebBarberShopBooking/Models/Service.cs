using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBarberShopBooking.Models
{
    public class Service // Dịch vụ
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc.")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả dịch vụ là bắt buộc.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá dịch vụ là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là một số dương.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Hình ảnh là bắt buộc.")]
        public string ?ImageUrl { get; set; }

        // Navigation property
        public ICollection<Appointment> ?Appointments { get; set; }
        public ICollection<OrderDetail> ?OrderDetails { get; set; }
    }
}

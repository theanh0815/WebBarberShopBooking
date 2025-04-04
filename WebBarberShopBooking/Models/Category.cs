using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBarberShopBooking.Models
{
    public class Category // Danh mục sản phẩm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}

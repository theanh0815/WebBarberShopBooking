using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBarberShopBooking.Models
{
    public class Product // Sản phẩm
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là một số dương.")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // Đường dẫn đến hình ảnh sản phẩm

        [Required(ErrorMessage = "Danh mục sản phẩm là bắt buộc.")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        // Navigation property
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

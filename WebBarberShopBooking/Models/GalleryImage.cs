using System.ComponentModel.DataAnnotations;

namespace WebBarberShopBooking.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn hình ảnh là bắt buộc.")]
        public string ImageUrl { get; set; }

        public string? Caption { get; set; } // Mô tả cho hình ảnh
    }
}

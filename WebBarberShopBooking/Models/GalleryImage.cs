using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Needed for IFormFile

namespace WebBarberShopBooking.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn hình ảnh là bắt buộc.")]
        public string ImageUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Chú thích không được vượt quá 255 ký tự.")]
        public string? Caption { get; set; }

        [NotMapped] // Not mapped to the database
        [Display(Name = "Chọn ảnh")]
        public IFormFile? ImageUrlFile { get; set; }
    }
}
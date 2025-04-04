using System;
using System.ComponentModel.DataAnnotations;

namespace WebBarberShopBooking.Models
{
    public class News // Tin tức
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề tin tức là bắt buộc.")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung tin tức là bắt buộc.")]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.Now;
        public string? ImageUrl { get; set; } // Đường dẫn đến hình ảnh tin tức
    }
}

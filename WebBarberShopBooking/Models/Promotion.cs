using System;
using System.ComponentModel.DataAnnotations;

namespace WebBarberShopBooking.Models
{
    public class Promotion // Khuyến mãi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề khuyến mãi là bắt buộc.")]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }

        public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
    }
}

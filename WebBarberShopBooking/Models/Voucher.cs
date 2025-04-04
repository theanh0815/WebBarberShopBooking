using System;
using System.ComponentModel.DataAnnotations;

namespace WebBarberShopBooking.Models
{
    public class Voucher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá là bắt buộc.")]
        [MaxLength(50)]
        public string Code { get; set; }

        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive => !ExpiryDate.HasValue || ExpiryDate >= DateTime.Now;

        // Có thể thêm thuộc tính để giới hạn số lần sử dụng
    }
}

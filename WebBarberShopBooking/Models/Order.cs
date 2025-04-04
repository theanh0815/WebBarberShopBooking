using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBarberShopBooking.Models
{
    public class Order // Đơn hàng
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentMethod PaymentMethod { get; set; }

        public string? ShippingAddress { get; set; }

        
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Completed,
        Cancelled
    }

    public enum PaymentMethod
    {
        COD,
        EmailSent
    }
}

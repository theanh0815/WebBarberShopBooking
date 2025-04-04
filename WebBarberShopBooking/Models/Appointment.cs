using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBarberShopBooking.Models
{
    public class Appointment // Lịch hẹn
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Thời gian hẹn là bắt buộc.")]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "Dịch vụ là bắt buộc.")]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        public int? StylistId { get; set; }

        [ForeignKey("StylistId")]
        public Stylist? Stylist { get; set; }

        [Required(ErrorMessage = "Khách hàng là bắt buộc.")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string? Notes { get; set; } // Ghi chú của khách hàng

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}

using System.ComponentModel.DataAnnotations;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Users;

namespace VehicleInsuranceSystem.Models.Notifications
{
    public class Notification
    {
        public int NotificationId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int UserId { get; set; }

        // Navigation
        public Users.User User { get; set; } = null!;
    }
}
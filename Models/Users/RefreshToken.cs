using System.ComponentModel.DataAnnotations;

namespace VehicleInsuranceSystem.Models.Users
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int UserId { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}
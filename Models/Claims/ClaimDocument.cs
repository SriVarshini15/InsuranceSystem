using System.ComponentModel.DataAnnotations;

namespace VehicleInsuranceSystem.Models.Claims
{
    public class ClaimDocument
    {
        public int ClaimDocumentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string DocumentPath { get; set; } = string.Empty;

        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int ClaimId { get; set; }

        // Navigation Property
        public Claim Claim { get; set; } = null!;
    }
}
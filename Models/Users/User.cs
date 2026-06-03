using System.ComponentModel.DataAnnotations;
using VehicleInsuranceSystem.Models.Vehicles;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Models.Notifications;

namespace VehicleInsuranceSystem.Models.Users;

public class User
{
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DOB { get; set; }

    public int Age { get; set; }

    [Required]
    public string AadhaarNumber { get; set; } = string.Empty;

    [Required]
    public string PANNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public ICollection<Vehicle> Vehicles { get; set; }
        = new List<Vehicle>();

    public ICollection<Proposal> Proposals { get; set; }
        = new List<Proposal>();

    public ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();

   
}
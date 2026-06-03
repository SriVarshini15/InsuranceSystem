using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.UserPolicies;

namespace VehicleInsuranceSystem.Models.Claims
{
    public class Claim
    {
        public int ClaimId { get; set; }

        public string ClaimNumber { get; set; } = Guid.NewGuid().ToString();

        public decimal ClaimAmount { get; set; }

        public DateTime IncidentDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public ClaimStatus Status { get; set; }

        public int UserPolicyId { get; set; }

        public UserPolicies.UserPolicy UserPolicy { get; set; } = null!;
        public ICollection<ClaimDocument> ClaimDocuments { get; set; }
         = new List<ClaimDocument>();
    }
}
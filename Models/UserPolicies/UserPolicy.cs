using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Models.Claims; 

namespace VehicleInsuranceSystem.Models.UserPolicies
{
    public class UserPolicy

    {
        public int UserPolicyId { get; set; }

        public string PolicyNumber { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string PdfPath { get; set; } = string.Empty;

        public int ProposalId { get; set; }

        public Proposals.Proposal Proposal { get; set; } = null!;
        public ICollection<Claim> Claims { get; set; }
           = new List<Claim>();
    }
}
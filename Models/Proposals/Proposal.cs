using VehicleInsuranceSystem.Models.Policies;
using VehicleInsuranceSystem.Models.Users;
using VehicleInsuranceSystem.Models.Vehicles;
using VehicleInsuranceSystem.Models.Enums;

namespace VehicleInsuranceSystem.Models.Proposals
{
    public class Proposal
    {


        public int ProposalId { get; set; }

        public string ProposalNumber { get; set; } = Guid.NewGuid().ToString();

        public decimal PremiumAmount { get; set; }

        public ProposalStatus Status { get; set; }

        public DateTime SubmittedDate { get; set; }

        public DateTime? ReviewedDate { get; set; }

        public string? Remarks { get; set; }

        public int UserId { get; set; }

        public Users.User User { get; set; } = null!;

        public int VehicleId { get; set; }

        public Vehicles.Vehicle Vehicle { get; set; } = null!;

        public int PolicyId { get; set; }

        public Policies.Policy Policy { get; set; } = null!;
        public ICollection<ProposalDocument> ProposalDocuments { get; set; }
            = new List<ProposalDocument>();
    }
}
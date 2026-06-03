using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Proposals;

namespace VehicleInsuranceSystem.Models.Payments
{

    public class Payment
    {
        public int PaymentId { get; set; }

        public decimal Amount { get; set; }

        public string TransactionNumber { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public int ProposalId { get; set; }

        public Proposals.Proposal Proposal { get; set; } = null!;
    }
}
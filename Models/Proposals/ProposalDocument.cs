namespace VehicleInsuranceSystem.Models.Proposals
{
    public class ProposalDocument
    {
        public int ProposalDocumentId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public string DocumentPath { get; set; } = string.Empty;

        public int ProposalId { get; set; }

        public Proposal Proposal { get; set; } = null!;
    }
}

namespace VehicleInsuranceSystem.Models.Proposals;

public class Quote
{
    public int QuoteId { get; set; }

    public string QuoteNumber { get; set; } = Guid.NewGuid().ToString();

    public decimal BasePremium { get; set; }

    public decimal AddonPremium { get; set; }

    public decimal TotalPremium { get; set; }

    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiryDate { get; set; }

    public bool IsEmailSent { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public int ProposalId { get; set; }

    public Proposal Proposal { get; set; } = null!;
}

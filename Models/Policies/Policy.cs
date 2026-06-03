namespace VehicleInsuranceSystem.Models.Policies
{
    public class Policy
    {
        public int PolicyId { get; set; }

        public string PolicyName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal BasePremium { get; set; }

        public decimal CoverageAmount { get; set; }

        public int DurationMonths { get; set; }

        public bool IsActive { get; set; }

        public int PolicyCategoryId { get; set; }

        public PolicyCategory PolicyCategory { get; set; } = null!;

        public ICollection<PolicyFeature> Features { get; set; }
            = new List<PolicyFeature>();

        public ICollection<PolicyAddon> Addons { get; set; }
            = new List<PolicyAddon>();
    }
}
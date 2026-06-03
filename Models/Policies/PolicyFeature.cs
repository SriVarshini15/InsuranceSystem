namespace VehicleInsuranceSystem.Models.Policies
{
    public class PolicyFeature
    {
        public int PolicyFeatureId { get; set; }

        public string FeatureName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int PolicyId { get; set; }

        public Policy Policy { get; set; } = null!;
    }
}

namespace VehicleInsuranceSystem.Models.Policies

{
    public class PolicyAddon
    {
        public int PolicyAddonId { get; set; }

        public string AddonName { get; set; } = string.Empty;

        public decimal AddonCost { get; set; }

        public string Description { get; set; } = string.Empty;

        public int PolicyId { get; set; }

        public Policy Policy { get; set; } = null!;
    }
}

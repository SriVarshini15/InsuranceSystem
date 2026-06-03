namespace VehicleInsuranceSystem.Models.Policies
{
    public class PolicyCategory
    {
        public int PolicyCategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Policy> Policies { get; set; }
            = new List<Policy>();
    }
}

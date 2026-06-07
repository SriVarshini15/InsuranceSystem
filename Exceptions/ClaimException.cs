namespace VehicleInsuranceSystem.Exceptions;

public class ClaimException : BusinessRuleException
{
    public ClaimException(string message)
        : base(message)
    {
    }
}

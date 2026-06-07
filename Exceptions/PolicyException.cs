namespace VehicleInsuranceSystem.Exceptions;

public class PolicyException : BusinessRuleException
{
    public PolicyException(string message)
        : base(message)
    {
    }
}

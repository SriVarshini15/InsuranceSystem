namespace VehicleInsuranceSystem.Exceptions;

public class BusinessRuleException : ApplicationExceptionBase
{
    public BusinessRuleException(string message)
        : base(message)
    {
    }
}

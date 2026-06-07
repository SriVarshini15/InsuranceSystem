namespace VehicleInsuranceSystem.Exceptions;

public class ValidationException : ApplicationExceptionBase
{
    public ValidationException(string message)
        : base(message)
    {
    }
}

namespace VehicleInsuranceSystem.Exceptions;

public class DuplicateResourceException : ApplicationExceptionBase
{
    public DuplicateResourceException(string message)
        : base(message)
    {
    }
}

namespace VehicleInsuranceSystem.Exceptions;

public class ResourceNotFoundException : ApplicationExceptionBase
{
    public ResourceNotFoundException(string message)
        : base(message)
    {
    }
}

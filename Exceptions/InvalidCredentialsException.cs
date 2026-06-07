namespace VehicleInsuranceSystem.Exceptions;

public class InvalidCredentialsException : ApplicationExceptionBase
{
    public InvalidCredentialsException(string message = "Invalid email or password.")
        : base(message)
    {
    }
}

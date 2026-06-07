namespace VehicleInsuranceSystem.Exceptions;

public class ProposalException : BusinessRuleException
{
    public ProposalException(string message)
        : base(message)
    {
    }
}

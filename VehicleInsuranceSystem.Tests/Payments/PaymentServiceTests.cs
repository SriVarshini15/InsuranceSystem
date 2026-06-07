using VehicleInsuranceSystem.DTOs.PaymentDTOs;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Services.Payments;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Payments;

public class PaymentServiceTests : TestBase
{
    [Test]
    public async Task CreatePaymentAsync_ProcessesPayment()
    {
        var proposal = await AddProposalAsync();
        var service = new PaymentService(Context, Mapper, Notifications);

        var dto = await service.CreatePaymentAsync(new CreatePaymentDto(proposal.ProposalId, 1250, "UPI"));

        Assert.Multiple(() =>
        {
            Assert.That(dto.PaymentId, Is.GreaterThan(0));
            Assert.That(dto.Status, Is.EqualTo("Success"));
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyPaymentSuccessAsync)));
        });
    }

    [Test]
    public async Task ConfirmPaymentAsync_ActivatesProposal()
    {
        var proposal = await AddProposalAsync();
        var service = new PaymentService(Context, Mapper, Notifications);
        var payment = await service.CreatePaymentAsync(new CreatePaymentDto(proposal.ProposalId, 1250, "UPI"));

        await service.ConfirmPaymentAsync(payment.PaymentId);

        Assert.That(Context.Proposals.Find(proposal.ProposalId)!.Status, Is.EqualTo(ProposalStatus.Active));
    }

    private async Task<Proposal> AddProposalAsync()
    {
        var user = await AddUserAsync();
        var proposal = new Proposal
        {
            UserId = user.UserId,
            VehicleId = 1,
            PolicyId = 1,
            Status = ProposalStatus.QuoteGenerated,
            SubmittedDate = DateTime.UtcNow,
            PremiumAmount = 1250
        };
        Context.Proposals.Add(proposal);
        await Context.SaveChangesAsync();
        return proposal;
    }
}

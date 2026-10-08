using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;

public class BillingValidator : AbstractValidator<RequestBillingJson>
{
    public BillingValidator()
    {
        RuleFor(billing => billing.BarberName).NotEmpty().WithMessage(ResourceErrorMessages.BARBER_NAME_REQUIRED);
        RuleFor(billing => billing.BarberName).Length(2, 80);
        RuleFor(billing => billing.ClientName).NotEmpty().WithMessage(ResourceErrorMessages.CLIENT_NAME_REQUIRED);
        RuleFor(billing => billing.ClientName).Length(2, 120);
        RuleFor(billing => billing.ServiceName).NotEmpty().WithMessage(ResourceErrorMessages.SERVICE_NAME_REQUIRED);
        RuleFor(billing => billing.ClientName).Length(2, 120);
        RuleFor(billing => billing.Amount).GreaterThanOrEqualTo(0).WithMessage(ResourceErrorMessages.AMOUNT_MUST_BE_GRATHER_OR_EQUAL_ZERO);
        RuleFor(billing => billing.PaymentMethod).IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_INVALID);
        RuleFor(billing => billing.Status).IsInEnum().WithMessage(ResourceErrorMessages.STATUS_INVALID);
        RuleFor(billing => billing.Notes).Length(1, 500);
        RuleFor(billing => billing.Date).LessThanOrEqualTo(DateTime.UtcNow).WithMessage(ResourceErrorMessages.EXPENSES_CANNOT_FOR_THE_FUTERE);
    }
}

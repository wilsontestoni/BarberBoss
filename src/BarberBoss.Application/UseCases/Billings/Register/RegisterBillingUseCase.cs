using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase : IRegisterBillingUseCase
{
    public ResponseRegisteredBillingJson Execute(RequestBillingJson request)
    {
        Validate(request);

        return new ResponseRegisteredBillingJson
        {
            BarberName = "Test",
            ClientName = "JoseTest",
            ServiceName = "Corte de cabelo teste"
        };
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new RegiterBillingValidator();

        var result = validator.Validate(request);

        if(result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

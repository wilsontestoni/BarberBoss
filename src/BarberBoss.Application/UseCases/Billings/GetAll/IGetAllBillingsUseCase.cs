using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Billings.GetAll;

public interface IGetAllBillingsUseCase
{
    Task<ResponseBillingsJson> Execute(RequestGetAllBillingsJson request);
}

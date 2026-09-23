using AutoMapper;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionBase;

namespace BarberBoss.Application.UseCases.Billings.GetById;

public class GetBillingByIdUseCase : IGetBillingByIdUseCase
{
    private readonly IBillingRepositoriesReadOnly _repository;
    private readonly IMapper _mapper;
    public GetBillingByIdUseCase(IBillingRepositoriesReadOnly repository, IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ResponseBillingJson> Execute(Guid id)
    {
        var billing = await _repository.GetById(id) ?? throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);

        return _mapper.Map<ResponseBillingJson>(billing);
    }
}

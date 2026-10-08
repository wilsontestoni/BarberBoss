using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Repositories.Billings.Filters;
using System.Runtime.CompilerServices;

namespace BarberBoss.Application.UseCases.Billings.GetAll;

public class GetAllBillingsUseCase : IGetAllBillingsUseCase
{
    private readonly IBillingRepositoriesReadOnly _repository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly IMapper _mapper;

    public GetAllBillingsUseCase(IBillingRepositoriesReadOnly repository, IUnityOfWork unityOfWork, IMapper mapper)
    {
        _repository = repository;
        _unityOfWork = unityOfWork;
        _mapper = mapper;   
    }

    public async Task<ResponseBillingsJson> Execute(RequestGetAllBillingsJson request)
    {

        var billingFilter = _mapper.Map<BillingFilter>(request);

        var result = await _repository.GetAll(billingFilter);

        return new ResponseBillingsJson
        {
            Billings = _mapper.Map<List<ResponseShortBillingJson>>(result.Items),
            HasNextPage = billingFilter.Page * billingFilter.PageSize < result.TotalCount,
            Page = billingFilter.Page,
            PageSize = billingFilter.PageSize,
            TotalItems = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)billingFilter.PageSize),
        };
    }
}

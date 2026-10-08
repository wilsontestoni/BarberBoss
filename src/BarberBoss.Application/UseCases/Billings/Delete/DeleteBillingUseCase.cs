using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionBase;

namespace BarberBoss.Application.UseCases.Billings.Delete;

public class DeleteBillingUseCase : IDeleteBillingUseCase
{
    private readonly IBillingRepositoriesWriteOnly _repository;
    private readonly IUnityOfWork _unityOfWork;
   
    public DeleteBillingUseCase(IBillingRepositoriesWriteOnly repository, IUnityOfWork unityOfWork)
    {
        _repository = repository;
        _unityOfWork = unityOfWork;
    }
    public async Task Execute(Guid id)
    {
        var result = await _repository.Delete(id);


        if(result == false)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        await _unityOfWork.Commit();
    }
}

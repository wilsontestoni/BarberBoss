namespace BarberBoss.Domain.Repositories;

public interface IUnityOfWork
{
    Task Commit();
}

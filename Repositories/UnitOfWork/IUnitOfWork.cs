namespace Market.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}
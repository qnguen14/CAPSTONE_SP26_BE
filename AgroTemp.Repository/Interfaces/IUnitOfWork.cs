using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Repository.Interfaces;

public interface IUnitOfWork : IGenericRepositoryFactory, IDisposable
{
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
    Task ExecuteInTransactionAsync(Func<Task> operation);
    Task<int> SaveChangesAsync();
}

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    TContext Context { get; }
}
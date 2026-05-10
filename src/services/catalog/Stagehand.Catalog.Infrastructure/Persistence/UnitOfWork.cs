using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Catalog.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly CatalogDbContext _dbContext;

    public UnitOfWork(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}

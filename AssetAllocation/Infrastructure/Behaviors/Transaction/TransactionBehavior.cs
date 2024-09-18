using System.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly AssetAllocationDbContext _assetAllocationDbContext;

    public TransactionBehavior(AssetAllocationDbContext assetAllocationDbContext)
    {
        _assetAllocationDbContext = assetAllocationDbContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using TransactionScope transactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            var response = await next();
            transactionScope.Complete();
            
            return response;
        }
        catch
        {
            transactionScope.Dispose();
            
            throw;
        }
    }
}
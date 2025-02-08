using Auth.Domain.Interfaces.Repositories;
using Auth.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Auth.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(AuthDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            throw new TransactionAlreadyStartedException(_currentTransaction.TransactionId);

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        catch(Exception e)
        {
            await RollbackAsync();
            throw new TransactionFailedException(e);
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VFX.Application.Common.Exceptions;
using VFX.Application.Interface;
using VFX.Infrastructure.Data.Repositories;

namespace VFX.Infrastructure.Data;

// UnitOfWork class responsible for managing database transactions and providing access to repositories
public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private readonly ApplicationDbContext _context;

    // repositories

    public ICurrencyRepository CurrencyRepository { get; }
    public IForeignExchangeRateRepository ExchangeRateRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _context = dbContext;
        // repositories
        CurrencyRepository = new CurrencyRepository(_context);
        ExchangeRateRepository = new ForeignExchangeRateRepository(_context);
    }

    // save changes
    public int SaveChanges() => _context.SaveChanges();

    public async Task<int> SaveChangesAsync(CancellationToken token) => await _context.SaveChangesAsync(token);

    // transaction
    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    // commit
    public void Commit()
    {
        if (_transaction == null)
        {
            throw TransactionException.TransactionNotCommitException();
        }
        try
        {
            _context.SaveChanges();
            _transaction.Commit();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task CommitAsync(CancellationToken token)
    {
        if (_transaction == null)
        {
            throw TransactionException.TransactionNotCommitException();
        }

        try
        {
            await _context.SaveChangesAsync(token);
            _transaction.Commit();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    // rollback
    public void Rollback()
    {
        if (_transaction == null)
        {
            throw TransactionException.TransactionNotCommitException();
        }

        _transaction.Rollback();
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            throw TransactionException.TransactionNotCommitException();
        }

        await _transaction.RollbackAsync();
        _transaction.Dispose();
        _transaction = null;
    }

    // dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // execute transaction
    public async Task ExecuteTransactionAsync(Action action, CancellationToken token)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            action();
            await _context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            throw TransactionException.TransactionNotExecuteException(ex);
        }
    }

    public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken token)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await _context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            throw TransactionException.TransactionNotExecuteException(ex);
        }
    }
}

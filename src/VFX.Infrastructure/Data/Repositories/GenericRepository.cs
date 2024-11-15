﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VFX.Application.Common.Models;
using VFX.Application.Interface;

namespace VFX.Infrastructure.Data.Repositories;

// Generic repository implementation for all entities of type T
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<T> entities)
        => await _dbSet.AddRangeAsync(entities);

    #region Read

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        => await _dbSet.AnyAsync(filter);

    public async Task<bool> AnyAsync()
        => await _dbSet.AnyAsync();

    public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        => await _dbSet.CountAsync(filter);

    public async Task<int> CountAsync()
        => await _dbSet.CountAsync();

    public async Task<T> GetByIdAsync(object id)
        => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<Pagination<T>> ToPagination(
         int pageIndex,
         int pageSize,
         Expression<Func<T, bool>>? filter = null,
         Func<IQueryable<T>, IQueryable<T>>? include = null,
         Expression<Func<T, object>>? orderBy = null,
         bool ascending = true)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        orderBy ??= x => EF.Property<object>(x, "Id");

        query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var result = await Pagination<T>.ToPagedList(query, pageIndex, pageSize);

        return result;
    }

    public async Task<T?> FirstOrDefaultAsync(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet.IgnoreQueryFilters().AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(filter);
    }

    #endregion
    #region Update & delete

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void UpdateRange(IEnumerable<T> entities)
        => _dbSet.UpdateRange(entities);

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities)
        => _dbSet.RemoveRange(entities);

    public async Task Delete(object id)
    {
        T entity = await GetByIdAsync(id);
        Delete(entity);
    }
    #endregion
}

﻿using System.Linq.Expressions;
using VFX.Application.Common.Models;

namespace VFX.Application.Interface;

public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    Task<bool> AnyAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> filter);
    Task<int> CountAsync();
    Task<T> GetByIdAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<Pagination<T>> ToPagination(
       int pageIndex,
       int pageSize,
       Expression<Func<T, bool>>? filter = null,
       Func<IQueryable<T>, IQueryable<T>>? include = null,
       Expression<Func<T, object>>? orderBy = null,
       bool ascending = true);
    Task<T?> FirstOrDefaultAsync(
       Expression<Func<T, bool>> filter,
       Func<IQueryable<T>, IQueryable<T>>? include = null);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task Delete(object id);
}
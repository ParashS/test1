using Contracts;
using EFCore.BulkExtensions;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ShipWatchDataContext RepositoryContext { get; set; }
        public RepositoryBase(ShipWatchDataContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
        public IQueryable<T> GetAll() => RepositoryContext.Set<T>().AsNoTracking();
        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression) =>
            RepositoryContext.Set<T>().Where(expression).AsNoTracking();

        public IQueryable<T> GetByConditionWithTracking(Expression<Func<T, bool>> expression) =>
            RepositoryContext.Set<T>().Where(expression);

        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
        public T CreateAndGetEnity(T entity)
        {
            return RepositoryContext.Set<T>().Add(entity).Entity;
        }
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);

        public async Task CreateAsync(T entity)
        {
            await RepositoryContext.Set<T>().AddAsync(entity);
        }

        public async Task<T> CreateAndGetEnityAsync(T entity)
        {
            return (await RepositoryContext.Set<T>().AddAsync(entity)).Entity;
        }

        public void CreateRange(List<T> entity)
        {
            RepositoryContext.Set<T>().AddRange(entity);
        }

        public async Task CreateRangeAsync(List<T> entity)
        {
            await RepositoryContext.Set<T>().AddRangeAsync(entity);
        }

        public void DeleteByCondition(Expression<Func<T, bool>> expression)
        {
            T? entity = GetByConditionWithTracking(expression).FirstOrDefault();
            if(entity != null)
            {
                RepositoryContext.Set<T>().Remove(entity);
            }
        }
    }
}

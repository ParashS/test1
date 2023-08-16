using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        IQueryable<T> GetByConditionWithTracking(Expression<Func<T, bool>> expression);
        void DeleteByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        T CreateAndGetEnity(T entity);
        Task<T> CreateAndGetEnityAsync(T entity);
        Task CreateAsync(T entity);
        void CreateRange(List<T> entity);
        Task CreateRangeAsync(List<T> entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

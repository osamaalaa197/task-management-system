using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Consts;

namespace TaskManagement.Core.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(string id);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip = null, int? take = null,
            Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending);
        T? Find(Expression<Func<T, bool>> criteria);
        T Add(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void DeleteBulk(Expression<Func<T, bool>> criteria);
        bool IsExists(Expression<Func<T, bool>> criteria);
        IQueryable<T> FindAllWithInclude(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
        T FindWithInclude(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

    }
}

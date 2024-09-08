using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Consts;
using TaskManagement.Core.Repositories;
using TaskManagement.Data;

namespace TaskManagement.EF.Repositories
{
    public class BaseRepository<T> :IBaseRepository <T> where T:class
    {
        protected TaskManagementDbContext _context;
        public BaseRepository(TaskManagementDbContext context) => _context = context;

        public IEnumerable<T> GetAll() => _context.Set<T>().ToList();

        public T? GetById(string id) => _context.Set<T>().Find(id);

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip = null, int? take = null,
            Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return query.ToList();
        }

        public T? Find(Expression<Func<T, bool>> criteria) => _context.Set<T>().SingleOrDefault(criteria);

        public T Add(T entity)
        {
            _context.Add(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
            return entities;
        }

        public void Update(T entity) => _context.Update(entity);

        public void Remove(T entity) => _context.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _context.RemoveRange(entities);

        public void DeleteBulk(Expression<Func<T, bool>> criteria) => _context.Set<T>().Where(criteria).ExecuteDelete();

        public bool IsExists(Expression<Func<T, bool>> criteria) => _context.Set<T>().Any(criteria);

        public IQueryable<T> FindAllWithInclude(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.Where(criteria);
        }
        public T FindWithInclude(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.Where(criteria).FirstOrDefault();
        }

    }
}

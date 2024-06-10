using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreMusfer.DateAccsse.Date;
using StoreMusfer.DateAccsse.Repository.IRepository;

namespace StoreMusfer.DateAccsse.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> _dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this._dbset = _db.Set<T>();
           // _db.Products.Include()
        }
        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> Filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> Query;
            if (tracked)
            {
                Query = _dbset;
            }
            else
            {
                Query = _dbset.AsNoTracking();
            }
            
            Query = Query.Where(Filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)){
                    Query = Query.Include(includeProp);
                }
            }
            return Query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter, string? includeProperties=null)
        {
            IQueryable<T> Query = _dbset;
            if (Filter != null)
            {
                Query = Query.Where(Filter);
            }
            if (!string.IsNullOrEmpty(includeProperties) )
            {
                foreach(var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)){
                    Query = Query.Include(includeProp);
                }
            }
           
            return Query.ToList();
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _dbset.RemoveRange(entity);
        }
    }
}

using FrameworkCore.Bases.BaseUnitOfWork;
using FrameworkCore.Enums;
using FrameworkCore.FrameworkCore.DataProperties;
using FrameworkCore.FrameworkCore.Repository;
using FrameworkCore.FrameworkCore.UnitOfWorkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkCore.Bases.BaseRepository
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly UnitOfWork _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(IUnitOfWork dbContext)
        {
            _dbContext = dbContext == null ? throw new ArgumentNullException(nameof(dbContext)) : dbContext as UnitOfWork;
            _dbSet = _dbContext.Set<TEntity>();
        }

        #region[CRUD_OPERATIONS]

        public virtual TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll)
        {
            return _dbSet.Add(entity).Entity;
        }

        public void Insert(params TEntity[] entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await _dbSet.AddAsync(entity, cancellationToken);
            return res.Entity;
        }

        public async Task InsertAsync(params TEntity[] entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(TEntity entity, UpdateStrategy updateStrategy = UpdateStrategy.UpdateAll)
        {
            _dbSet.Update(entity);
        }

        public void Update(params TEntity[] entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public abstract void Delete(object id);
        public abstract void DeleteWithState(object id);

        public abstract void DeleteWithStateRange(IEnumerable<TEntity> entities);

        public abstract void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties);
        public abstract void UpdateWithProperties(TEntity[] entity, Expression<Func<TEntity, object>>[] properties);
        public abstract void UpdateWithPropertiesForProperty(TEntity entity, Expression<Func<TEntity, object>>[] properties);
        public abstract void UpdateWithPropertiesForProperty(TEntity[] entities, Expression<Func<TEntity, object>>[] properties);

        public virtual void Delete(TEntity entity, DeleteStrategy deleteStrategy = DeleteStrategy.MainIfRequiredAddChilds)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(params TEntity[] entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        protected void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> action)
        {
            _dbContext.ChangeTracker.TrackGraph(rootEntity, action);
        }

        #endregion[END_CRUD_OPERATIONS]

        #region[QUERY_OPERATIONS]

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }
        public abstract IQueryable<TEntity> GetAllNotDeleted();

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
        {
            IQueryable<TEntity> query = _dbSet;

            query = SetTracking(query, tracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? orderBy(query) : query;
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
        {
            IQueryable<TEntity> query = _dbSet;

            query = SetTracking(query, tracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
        }

        public TEntity Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public ValueTask<TEntity> FindAsync(params object[] keyValues)
        {
            return _dbSet.FindAsync(keyValues);
        }

        public ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            return _dbSet.FindAsync(keyValues, cancellationToken);
        }

        public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
        {
            IQueryable<TEntity> query = _dbSet;

            query = SetTracking(query, tracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? orderBy(query).FirstOrDefault() : query.FirstOrDefault();
        }

        public TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
        {
            IQueryable<TEntity> query = _dbSet;

            query = SetTracking(query, tracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? orderBy(query).Select(selector).FirstOrDefault() : query.Select(selector).FirstOrDefault();
        }

        public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
        {
            IQueryable<TEntity> query = _dbSet;

            query = SetTracking(query, tracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? await orderBy(query).Select(selector).FirstOrDefaultAsync() : await query.Select(selector).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
        {
            IQueryable<TEntity> query = _dbSet;

            query = SetTracking(query, tracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? await orderBy(query).FirstOrDefaultAsync() : await query.FirstOrDefaultAsync();
        }

        public async Task<object[]> GetPagedAsync(
           Func<IQueryable<TEntity>,
           IOrderedQueryable<TEntity>> orderBy = null,
             Expression<Func<TEntity, object>>[] include = null,
            Expression<IGrouping<object, TEntity>>[] groupBy = null,
           Expression<Func<TEntity, bool>> predicate = null,
           int? page = 0,
           int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null && include.Any())
            {
                query = include.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            if (orderBy != null)
                query = orderBy(query);
            else
                throw new ArgumentNullException("The order by is necessary in Pagining");

            var dataCount = query.Count();

            if (page != null && page > 0)
            {
                if (pageSize == null) throw new ArgumentException("The take paremeter supplied is null, It should be included when skip is used");
                query = query.Skip(((int)page - 1) * (int)pageSize);
            }

            if (pageSize != null)
            {
                query = query.Take((int)pageSize);
            }

            var data = await query.ToListAsync();

            return new object[] { data, dataCount };
        }

        public bool Exists(Expression<Func<TEntity, bool>> selector = null)
        {
            return selector == null ? _dbSet.Any() : _dbSet.Any(selector);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null)
        {
            return selector == null ? await _dbSet.AnyAsync() : await _dbSet.AnyAsync(selector);
        }

        #endregion[END_QUERY_OPERATIONS]

        #region[AGGRIGATIONS]

        public int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet.LongCount() : _dbSet.LongCount(predicate);
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? await _dbSet.LongCountAsync() : await _dbSet.LongCountAsync(predicate);
        }

        public T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            return predicate == null ? _dbSet.Max(selector) : _dbSet.Where(predicate).Max(selector);
        }

        public async Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            return predicate == null
                ? await _dbSet.MaxAsync(selector)
                : await _dbSet.Where(predicate).MaxAsync(selector);
        }

        public T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            return predicate == null ? _dbSet.Min(selector) : _dbSet.Where(predicate).Min(selector);
        }

        public async Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            return predicate == null
                ? await _dbSet.MinAsync(selector)
                : await _dbSet.Where(predicate).MinAsync(selector);
        }

        public decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            return predicate == null ? _dbSet.Average(selector) : _dbSet.Where(predicate).Average(selector);
        }

        public async Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            if (predicate == null)
            {
                return await _dbSet.AverageAsync(selector);
            }

            return await _dbSet.Where(predicate).AverageAsync(selector);
        }

        public decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            return predicate == null ? _dbSet.Sum(selector) : _dbSet.Where(predicate).Sum(selector);
        }

        public async Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            if (predicate == null)
            {
                return await _dbSet.SumAsync(selector);
            }

            return await _dbSet.Where(predicate).SumAsync(selector);
        }

        #endregion[END_AGGRIGATIONS]

        #region[CONTEXT_OPERATIONS]

        public void Detach(TEntity entity)
        {
            var entry = GetEntityEntry(entity);
            if (entry != null)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
        }

        #endregion[END_CONTEXT_OPERATIONS]

        #region [UNITOFWORK_OPERATIONS]
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        #endregion [UNITOFWORK_OPERATIONS]

        protected abstract IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking);

        protected EntityEntry<TEntity> GetEntityEntry(TEntity entity)
        {
            return _dbContext.Entry(entity);
        }

        #region[IQUERYABLE_IMPLEMENTATION]
        public Type ElementType => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).ElementType;

        public Expression Expression => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).Expression;

        public IQueryProvider Provider => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).Provider;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return SetTracking(_dbSet, TrackingBehaviour.ContextDefault).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SetTracking(_dbSet, TrackingBehaviour.ContextDefault).GetEnumerator();
        }

        #endregion[END_IQUERYABLE_IMPLEMENTATION]

        public abstract IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities);
    }
}

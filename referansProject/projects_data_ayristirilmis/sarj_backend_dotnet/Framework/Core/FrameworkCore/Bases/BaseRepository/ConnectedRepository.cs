using EFCore.BulkExtensions;
using FrameworkCore.Enums;
using FrameworkCore.FrameworkCore.DataProperties;
using FrameworkCore.FrameworkCore.Repository;
using FrameworkCore.FrameworkCore.UnitOfWorkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.Bases.BaseRepository
{
    public class ConnectedRepository<TEntity> : BaseRepository<TEntity>, IRepository<TEntity> where TEntity : class, IEntity
    {
        public ConnectedRepository(IUnitOfWork dbContext) : base(dbContext)
        {
        }

        #region[CRUD_OPERATIONS]

        public override void Delete(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public override void DeleteWithState(object id)
        {
            var entity = _dbSet.Find(id);
            entity.Deleted = true;
            if (entity != null)
            {
                Update(entity);
            }
        }

        public override void DeleteWithStateRange(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(item =>
            {
                item.Deleted = true;
            });
            Update(entities);
        }

        public override void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties)
        {
            _dbContext.Entry(entity).State = EntityState.Unchanged;
            foreach (var property in properties)
            {
                try
                {
                    _dbContext.Entry(entity).Property(property).IsModified = true;
                }
                catch (Exception ee)
                {
                    _dbContext.Entry(entity).Reference(property).IsModified = true;
                }
            }
        }

        public override void UpdateWithProperties(TEntity[] entities, Expression<Func<TEntity, object>>[] properties)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Unchanged;
                foreach (var property in properties)
                {
                    _dbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
        }

        public override void UpdateWithPropertiesForProperty(TEntity entity, Expression<Func<TEntity, object>>[] properties)
        {
            _dbContext.Entry(entity).Properties.ToList().ForEach(item => {
                item.IsModified = false;
            });

            foreach (var property in properties)
            {
                _dbContext.Entry(entity).Property(property).IsModified = true;
            }
        }

        public override void UpdateWithPropertiesForProperty(TEntity[] entities, Expression<Func<TEntity, object>>[] properties)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).Properties.ToList().ForEach(item => {
                    item.IsModified = false;
                });

                foreach (var property in properties)
                {
                    _dbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
        }

        public async Task AddBulkAsync(TEntity[] entities)
        {
            await _dbContext.BulkInsertAsync(entities);
        }

        public void AddBulk(TEntity[] entities)
        {
            _dbContext.BulkInsert(entities);
        }

        public async Task UpdateBulkAsync(TEntity[] entities)
        {
            await _dbContext.BulkUpdateAsync(entities);
        }

        public void UpdateBulk(TEntity[] entities)
        {
            _dbContext.BulkUpdate(entities);
        }

        public async Task DeleteBulkAsync(TEntity[] entities)
        {
            await _dbContext.BulkDeleteAsync(entities);
        }

        public void DeleteBulk(TEntity[] entities)
        {
            _dbContext.BulkDelete(entities);
        }

        #endregion[END_CRUD_OPERATIONS]

        #region[QUERY_OPERATIONS]

        public override IQueryable<TEntity> GetAllNotDeleted()
        {
            return _dbSet.Where(t => t.Deleted == false);
        }

        #endregion[END_QUERY_OPERATIONS]

        protected override IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking)
        {
            if (tracking == TrackingBehaviour.AsNoTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }

        public override IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities)
        {
            if (!string.IsNullOrEmpty(directive) && !string.IsNullOrEmpty(property))
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var body = property.Split('.').Aggregate((Expression)parameter, Expression.Property);
                if (body.Type.IsValueType) body = Expression.Convert(body, typeof(object));
                var selector = Expression.Lambda<Func<TEntity, object>>(body, parameter);

                if (directive == "asc")
                {
                    return entities.OrderBy(selector);
                }
                else
                {
                    return entities.OrderByDescending(selector);
                }
            }
            else
            {
                return entities.OrderBy(i => i.Id);
            }
        }
    }
}

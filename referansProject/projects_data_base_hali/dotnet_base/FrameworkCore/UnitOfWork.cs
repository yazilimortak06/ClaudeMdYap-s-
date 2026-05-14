// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\BaseUnitOfWork\UnitOfWork.cs

using FrameworkCore.Bases.BaseRepository;
using FrameworkCore.FrameworkCore.DataProperties;
using FrameworkCore.FrameworkCore.Repository;
using FrameworkCore.FrameworkCore.UnitOfWorkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.Bases.BaseUnitOfWork
{
    public abstract class UnitOfWork : DbContext, IUnitOfWork
    {
        private Dictionary<Type, object> repositories;

        public UnitOfWork([NotNull] DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
            ConfigureContext();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new ConnectedRepository<TEntity>(this);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        public TRepo GetOwnRepository<TRepo, TEntity>()
        {
            TRepo repo = (TRepo)Activator.CreateInstance(typeof(TRepo), new object[] { this });
            return repo;
        }

        public override int SaveChanges()
        {
            var timestamp = DateTime.Now;
            BeforeSave(timestamp);

            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            var timestamp = DateTime.Now;
            BeforeSave(timestamp);

            return await base.SaveChangesAsync();
        }

        private void ConfigureContext()
        {
        }

        private void BeforeSave(DateTime timestamp)
        {
            ChangeTracker.DetectChanges();
        }
    }
}

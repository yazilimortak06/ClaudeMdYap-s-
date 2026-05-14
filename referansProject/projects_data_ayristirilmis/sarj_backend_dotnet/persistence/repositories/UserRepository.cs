// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Persistences\Api.Persistence\Repositories\UserRepositories\UserRepository.cs
using Api.Persistence.DbContext;
using FrameworkCore.Bases.BaseRepository;
using FrameworkCore.FrameworkCore.UnitOfWorkCore;
using FrameworkCore.Utils.EntityUtils;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.UserDtos;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.UserModule;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserRepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Persistence.Repositories.UserRepositories
{
    public class UserRepository : ConnectedRepository<User>, IUserRepository
    {
        private RotaWattDbContext _appDbContext { get => _dbContext as RotaWattDbContext; }

        private readonly IUtilService _utilService;
        public UserRepository(IUnitOfWork dbContext,
            IUtilService utilService) : base(dbContext)
        {
            _utilService = utilService;
        }
        /// <summary>
        /// Kullanıcı çekiliyor
        /// </summary>
        public IQueryable<User> GetUser(UserFilterDto userFilter,
          Func<IQueryable<User>, IIncludableQueryable<User, object>> include,
          bool disableTracking = true)
        {
            Expression<Func<User, bool>> predicate = PredicateUser(userFilter);
            IQueryable<User> query = _appDbContext.User;
            if (include != null)
            {
                query = include(query);
            }
            query = query.AsSplitQuery();
            query = query.Where(predicate);
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }
        /// <summary>
        /// Kullanıcı çekiliyor (sadece temel alanlar)
        /// </summary>
        public IQueryable<User> GetOnlyUser(UserFilterDto userFilter)
        {
            Expression<Func<User, bool>> predicate = PredicateUser(userFilter);
            var query = _appDbContext.User.Where(predicate);
            return query;
        }

        private Expression<Func<User, bool>> PredicateUser(UserFilterDto userFilter)
        {
            Expression<Func<User, bool>> predicate = p => 1 == 1;
            if (userFilter.Deleted != null)
            {
                predicate = predicate.And(p => p.Deleted == userFilter.Deleted);
            }
            else
            {
                predicate = predicate.And(p => !p.Deleted);
            }
            if (userFilter.Id != null)
                predicate = predicate.And(p => p.Id == userFilter.Id);
            if (userFilter.Phone != null)
                predicate = predicate.And(p => p.Phone == userFilter.Phone);
            if (userFilter.Name != null)
                predicate = predicate.And(p => p.Name.Contains(userFilter.Name));
            if (userFilter.Surname != null)
                predicate = predicate.And(p => p.Surname.Contains(userFilter.Surname));
            if (userFilter.IsActive != null)
                predicate = predicate.And(p => p.IsActive == userFilter.IsActive);
            if (userFilter.PhoneContain != null)
                predicate = predicate.And(p => p.Phone.Contains(userFilter.PhoneContain));
            return predicate;
        }

        public IQueryable<User> GetOnlyUser(UserMobilFilterDto userFilter)
        {
            Expression<Func<User, bool>> predicate = p => 1 == 1;
            if (userFilter.Id != null)
                predicate = predicate.And(p => p.Id == userFilter.Id);
            if (userFilter.Phone != null)
                predicate = predicate.And(p => p.Phone == userFilter.Phone);
            if (userFilter.PhoneContain != null)
                predicate = predicate.And(p => p.Phone.Contains(userFilter.PhoneContain));
            var query = _appDbContext.User.Where(predicate);
            return query;
        }
        /// <summary>
        /// id'ye göre kullanıcı çekiliyor
        /// </summary>
        public IQueryable<User> GetUserById(long id)
        {
            Expression<Func<User, bool>> predicate = p => !p.Deleted && p.Id == id;
            var query = _appDbContext.User.Where(predicate);
            return query;
        }
        public IQueryable<User> GetUser(UserMobilFilterDto userFilter, long? userId)
        {
            Expression<Func<User, bool>> predicate = p => !p.Deleted;
            if (userId != null && userId != 0)
                predicate = predicate.And(p => p.Id == userId);
            if (userFilter.Phone != null)
                predicate = predicate.And(p => p.Phone == userFilter.Phone);
            if (userFilter.TcNumber != null)
                predicate = predicate.And(p => p.TcNumber == userFilter.TcNumber);
            if (userFilter.Id != null)
                predicate = predicate.And(p => p.Id == userFilter.Id);
            if (userFilter.PhoneContain != null)
                predicate = predicate.And(p => p.Phone.Contains(userFilter.PhoneContain));
            var query = _appDbContext.User.Where(predicate);
            return query;
        }
        public IQueryable<User> GetUser(UserMobilFilterDto userFilter)
        {
            Expression<Func<User, bool>> predicate = p => !p.Deleted;
            if (userFilter.Id != null)
                predicate = predicate.And(p => p.Id == userFilter.Id);
            if (userFilter.Phone != null)
                predicate = predicate.And(p => p.Phone == userFilter.Phone);
            if (userFilter.PhoneCountryCode != null)
                predicate = predicate.And(p => p.PhoneCountryCode == userFilter.PhoneCountryCode);
            if (userFilter.TcNumber != null)
                predicate = predicate.And(p => p.TcNumber == userFilter.TcNumber);
            if (userFilter.MobilUserGuiId != null)
                predicate = predicate.And(p => p.MobilUserGuiId == userFilter.MobilUserGuiId);
            if (userFilter.PhoneContain != null)
                predicate = predicate.And(p => p.Phone.Contains(userFilter.PhoneContain));
            var query = _appDbContext.User.Where(predicate);
            return query;
        }
        public IQueryable<User> GetUserAsNoTracking(UserMobilFilterDto userFilter)
        {
            return GetUser(userFilter).AsNoTracking();
        }
        public IQueryable<User> GetOnlyUserAsNoTracking(UserFilterDto userFilter)
        {
            return GetOnlyUser(userFilter).AsNoTracking();
        }
        public IQueryable<User> GetOnlyUserAsNoTracking(UserMobilFilterDto userFilter)
        {
            return GetOnlyUser(userFilter).AsNoTracking();
        }
    }
}

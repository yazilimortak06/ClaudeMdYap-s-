using Bank.Persistence.DbContext;
using FrameworkCore.Bases.BaseRepository;
using FrameworkCore.FrameworkCore.UnitOfWorkCore;
using FrameworkCore.Utils.EntityUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shared.Domain.Dto.ApiDto.GibDtos;
using Shared.Domain.Dto.BankDto.PaymentDtos;
using Shared.Domain.Entities.BankEntities.PaymentModule;
using Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.PaymentRepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Persistence.Repositories.PaymentRepositories
{
    public class PaymentRepository : ConnectedRepository<Payment>, IPaymentRepository
    {
        private PaymentDbContext _appDbContext { get => _dbContext as PaymentDbContext; }

        public PaymentRepository(IUnitOfWork dbContext) : base(dbContext)
        {
        }
        /// <summary>
        /// payment çekiliyor
        /// </summary>
        /// <param name="paymentFilter"></param>
        /// <returns></returns>
        public IQueryable<Payment> GetPayment(PaymentFilterDto paymentFilter,
        Func<IQueryable<Payment>, IIncludableQueryable<Payment, object>> include,
         bool disableTracking = true)
        {
            Expression<Func<Payment, bool>> predicate = GetPaymentPredicate(paymentFilter);
            IQueryable<Payment> query = _appDbContext.Payment;
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
        public IQueryable<Payment> GetPayment(PaymentFilterDto paymentFilter)
        {
            var predicate = this.GetPaymentPredicate(paymentFilter);
            var query = _appDbContext.Payment.Include(p => p.PaymentCallbackData)
                                             .Include(p => p.WalletSpendMoney)
                                             .Where(predicate).AsSplitQuery();
            return query;
        }


        public IQueryable<Payment> GetPaymentAsNoTracking(PaymentFilterDto paymentFilter)
        {
            return GetPayment(paymentFilter).AsNoTracking();
        }
        public IQueryable<Payment> GetOnlyPayment(PaymentFilterDto paymentFilter)
        {
            var predicate = this.GetPaymentPredicate(paymentFilter);
            var query = _appDbContext.Payment.Where(predicate).AsSplitQuery();
            return query;
        }
        public IQueryable<Payment> GetOnlyPaymentAsNoTracking(PaymentFilterDto paymentFilter)
        {
            return GetOnlyPayment(paymentFilter).AsNoTracking();
        }
        private Expression<Func<Payment, bool>> GetPaymentPredicate(PaymentFilterDto paymentFilter)
        {
            Expression<Func<Payment, bool>> predicate = p => !p.Deleted;
            if (paymentFilter.Id != null)
            {
                predicate = predicate.And(p => p.Id == paymentFilter.Id);
            }
            if (paymentFilter.PaymentGuiId != null)
            {
                predicate = predicate.And(p => p.GuiId == paymentFilter.PaymentGuiId);
            }
            if (paymentFilter.ChargeGuiId != null)
            {
                predicate = predicate.And(p => p.ChargeGuiId == paymentFilter.ChargeGuiId);
            }
            if (paymentFilter.WalletProcessGuiId != null)
            {
                predicate = predicate.And(p => p.WalletProcessGuiId == paymentFilter.WalletProcessGuiId);
            }
            if (paymentFilter.PaymentReason != null)
            {
                predicate = predicate.And(p => p.PaymentReason == paymentFilter.PaymentReason);
            }
            if (paymentFilter.SecurityKey != null)
            {
                predicate = predicate.And(p => p.SecurityKey == paymentFilter.SecurityKey);
            }
            if (paymentFilter.PaymentMethod != null)
            {
                predicate = predicate.And(p => p.PaymentMethod == paymentFilter.PaymentMethod);
            }
            if (paymentFilter.PaymentStatus != null)
            {
                predicate = predicate.And(p => p.PaymentStatus == paymentFilter.PaymentStatus);
            }
            if (paymentFilter.PaymentStatusList != null && paymentFilter.PaymentStatusList.Count > 0)
            {
                predicate = predicate.And(p => paymentFilter.PaymentStatusList.Contains(p.PaymentStatus));
            }
            return predicate;
        }
    }
}

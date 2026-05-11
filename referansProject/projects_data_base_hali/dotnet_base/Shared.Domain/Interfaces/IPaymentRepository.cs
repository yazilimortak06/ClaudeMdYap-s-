using FrameworkCore.FrameworkCore.Repository;
using Microsoft.EntityFrameworkCore.Query;
using Shared.Domain.Dto.BankDto.PaymentDtos;
using Shared.Domain.Entities.BankEntities.PaymentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.PaymentRepositoryInterfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        IQueryable<Payment> GetPayment(PaymentFilterDto paymentFilter,
        Func<IQueryable<Payment>, IIncludableQueryable<Payment, object>> include,
         bool disableTracking = true);
        IQueryable<Payment> GetPayment(PaymentFilterDto paymentFilter);
        IQueryable<Payment> GetPaymentAsNoTracking(PaymentFilterDto paymentFilter);
        IQueryable<Payment> GetOnlyPayment(PaymentFilterDto paymentFilter);
        IQueryable<Payment> GetOnlyPaymentAsNoTracking(PaymentFilterDto paymentFilter);
    }
}

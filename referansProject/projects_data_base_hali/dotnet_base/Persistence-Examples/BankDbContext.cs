using FrameworkCore.Bases.BaseUnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.BankEntities.PaymentDebitCardVerificationModule;
using Shared.Domain.Entities.BankEntities.PaymentModule;
using Shared.Domain.Entities.BankEntities.WalletModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Persistence.DbContext
{
    public class PaymentDbContext : UnitOfWork
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> dbContextOptions)
           : base(dbContextOptions)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            // base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region Fluent 
   
            #endregion
            #region data seeding 

            #endregion
        }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<PaymentCallbackData> PaymentCallbackData { get; set; }
        public DbSet<PaymentDebitCardVerification> PaymentDebitCardVerification { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<WalletPushMoney> WalletPushMoney { get; set; }
        public DbSet<WalletSpendMoney> WalletSpendMoney { get; set; }
        public DbSet<WalletReductionMoney> WalletReductionMoney { get; set; }
    }
}

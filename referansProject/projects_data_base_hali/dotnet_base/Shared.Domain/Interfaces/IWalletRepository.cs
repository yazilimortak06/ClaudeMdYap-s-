using FrameworkCore.FrameworkCore.Repository;
using Shared.Domain.Dto.BankDto.WalletDtos;
using Shared.Domain.Entities.BankEntities.WalletModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.WalletRepositoryInterfaces
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        IQueryable<Wallet> GetWallet(WalletFilterDto walletFilter);
        IQueryable<Wallet> GetWalletAsNoTracking(WalletFilterDto walletFilter);
    }
}

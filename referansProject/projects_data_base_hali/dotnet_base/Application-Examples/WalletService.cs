using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.BankDto.WalletDtos;
using Shared.Domain.Entities.BankEntities.WalletModule;
using Shared.Domain.Errors.BankErrors;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.WalletRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.BankServiceInterfaces.WalletServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.WalletService
{
    public class WalletService : BaseService, IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletPushMoneyRepository _walletPushMoneyRepository;
        private readonly IWalletReductionMoneyRepository _walletReductionMoneyRepository;
        private readonly IUtilService _utilService;
        private readonly IConfiguration _configuration;

        public WalletService(IMapper mapper,
                           IWalletRepository walletRepository,
                           IUtilService utilService,
                           IConfiguration configuration,
                           IWalletPushMoneyRepository walletPushMoneyRepository,
                           IWalletReductionMoneyRepository walletReductionMoneyRepository) : base(
                           mapper)
        {
            _walletRepository = walletRepository;
            _utilService = utilService;
            _configuration = configuration;
            _walletPushMoneyRepository = walletPushMoneyRepository;
            _walletReductionMoneyRepository = walletReductionMoneyRepository;
        }
        /// <summary>
        /// cüzdan oluşturuluyor
        /// </summary>
        /// <param name="createWalletRequest"></param>
        /// <returns></returns>
        public async Task<Result<CreateWalletResponseDto>> CreateWallet(CreateWalletRequestDto createWalletRequest)
        {
            #region response dto oluşturuluyor
            CreateWalletResponseDto createWalletResponse = new CreateWalletResponseDto();
            #endregion
            #region wallet entity oluşturuluyor
            Wallet wallet = new Wallet();
            wallet.GuiId = Guid.NewGuid() + "";
            wallet.CreatedDate = DateTime.Now;
            wallet.WalletAmount = 0;
            wallet.WalletTockenGuiId = _utilService.Base64Encode(wallet.GuiId + "/" + createWalletRequest.MobilUserGuiId + "/" + _configuration.GetSection("Wallet:TockenKey").Value);
            wallet.AmountTockenGuiId = _utilService.Base64Encode(wallet.GuiId + "/" + wallet.WalletAmount + "/" + wallet.WalletTockenGuiId);
            wallet.ProcessKey = _utilService.GetRandomString(32);
            #endregion
            #region veritabanına kayıt ediliyor
            await _walletRepository.InsertAsync(wallet);
            await _walletRepository.SaveChangesAsync();
            #endregion
            #region response dto setleniyor
            createWalletResponse.WalletGuiId = wallet.GuiId;
            #endregion
            return new SuccessResult<CreateWalletResponseDto>(createWalletResponse);
        }
        /// <summary>
        /// cüzdan bakiye arttırılıyor
        /// </summary>
        /// <param name="increaseAmountofWalletRequest"></param>
        /// <returns></returns>
        public async Task<Result<IncreaseAmountofWalletResponseDto>> IncreaseAmountofWallet(IncreaseAmountofWalletRequestDto increaseAmountofWalletRequest)
        {
            #region response dto oluşturuluyor
            IncreaseAmountofWalletResponseDto increaseAmountofWalletResponse = new IncreaseAmountofWalletResponseDto();
            #endregion
            #region wallet getiriliyor
            #region filter dto oluşturuluyor
            WalletFilterDto walletFilter = _mapper.Map<WalletFilterDto>(increaseAmountofWalletRequest);
            #endregion
            var wallet = await _walletRepository.GetWallet(walletFilter).FirstOrDefaultAsync();
            #endregion
            if (wallet != null)
            {
                #region arttırılacak miktar alınıyor
                var amountTockenBase64Decoded = _utilService.Base64Decode(increaseAmountofWalletRequest.AmountTockenGuiId);
                var amountStringArray = amountTockenBase64Decoded.Split('/');
                var amount = _utilService.ParseDecimal(amountStringArray[1]);
                #endregion
                if (amountStringArray[0] == wallet.GuiId && amountStringArray[2] == wallet.WalletTockenGuiId)
                {
                    #region cüzdan güncelleniyor
                    _walletRepository.UpdateWithProperties(wallet, new Expression<Func<Wallet, object>>[] {
                        s => s.ProcessKey,
                        s => s.WalletAmount,
                        s => s.AmountTockenGuiId,
                    });
                    wallet.ProcessKey = _utilService.GetRandomString(32);
                    wallet.WalletAmount = wallet.WalletAmount + amount;
                    wallet.AmountTockenGuiId = _utilService.Base64Encode(wallet.GuiId + "/" + wallet.WalletAmount + "/" + wallet.WalletTockenGuiId);
                    #endregion
                    #region wallet push money entity oluşturuluyor
                    WalletPushMoney walletPushMoney = new WalletPushMoney();
                    walletPushMoney.PushAmount = amount;
                    walletPushMoney.WalletId = wallet.Id;
                    walletPushMoney.GuiId = Guid.NewGuid() + "";
                    walletPushMoney.WalletProcessGuiId = increaseAmountofWalletRequest.WalletProcessGuiId;
                    #endregion
                    #region veritabanına kayıt ediliyor
                    await _walletPushMoneyRepository.InsertAsync(walletPushMoney);
                    await _walletPushMoneyRepository.SaveChangesAsync();
                    #endregion
                    #region response dto setleniyor
                    increaseAmountofWalletResponse.WalletPushMoneyGuiId = walletPushMoney.GuiId;
                    #endregion
                    return new SuccessResult<IncreaseAmountofWalletResponseDto>(increaseAmountofWalletResponse);
                }
                #region geçersiz tutar
                else
                {
                    return new ErrorResult<IncreaseAmountofWalletResponseDto>(increaseAmountofWalletResponse, WalletErrorEnum.INVALID_AMOUNT);
                }
                #endregion
            }
            #region cüzdan bulunamadı
            else
            {
                return new ErrorResult<IncreaseAmountofWalletResponseDto>(increaseAmountofWalletResponse, WalletErrorEnum.WALLET_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// cüzdan bakiye azaltılıyor
        /// </summary>
        /// <param name="decreaseAmountofWalletRequest"></param>
        /// <returns></returns>
        public async Task<Result<DecreaseAmountofWalletResponseDto>> DecreaseAmountofWallet(DecreaseAmountofWalletRequestDto decreaseAmountofWalletRequest)
        {
            #region response dto oluşturuluyor
            DecreaseAmountofWalletResponseDto decreaseAmountofWalletResponse = new DecreaseAmountofWalletResponseDto();
            #endregion
            #region wallet getiriliyor
            #region filter dto oluşturuluyor
            WalletFilterDto walletFilter = _mapper.Map<WalletFilterDto>(decreaseAmountofWalletRequest);
            #endregion
            var wallet = await _walletRepository.GetWallet(walletFilter).FirstOrDefaultAsync();
            #endregion
            if (wallet != null)
            {
                #region azaltılacak miktar alınıyor
                var amountTockenBase64Decoded = _utilService.Base64Decode(decreaseAmountofWalletRequest.AmountTockenGuiId);
                var amountStringArray = amountTockenBase64Decoded.Split('/');
                var amount = _utilService.ParseDecimal(amountStringArray[1]);
                #endregion
                if (amountStringArray[0] == wallet.GuiId && amountStringArray[2] == wallet.WalletTockenGuiId)
                {
                    if (wallet.WalletAmount >= amount)
                    {
                        #region cüzdan güncelleniyor
                        _walletRepository.UpdateWithProperties(wallet, new Expression<Func<Wallet, object>>[] {
                            s => s.ProcessKey,
                            s => s.WalletAmount,
                            s => s.AmountTockenGuiId,
                        });
                        wallet.ProcessKey = _utilService.GetRandomString(32);
                        wallet.WalletAmount = wallet.WalletAmount - amount;
                        wallet.AmountTockenGuiId = _utilService.Base64Encode(wallet.GuiId + "/" + wallet.WalletAmount + "/" + wallet.WalletTockenGuiId);
                        #endregion
                        #region wallet reduction money entity oluşturuluyor
                        WalletReductionMoney walletReductionMoney = new WalletReductionMoney();
                        walletReductionMoney.ReductionAmount = amount;
                        walletReductionMoney.WalletId = wallet.Id;
                        walletReductionMoney.GuiId = Guid.NewGuid() + "";
                        walletReductionMoney.WalletProcessGuiId = decreaseAmountofWalletRequest.WalletProcessGuiId;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _walletReductionMoneyRepository.InsertAsync(walletReductionMoney);
                        await _walletReductionMoneyRepository.SaveChangesAsync();
                        #endregion
                        #region response dto setleniyor
                        decreaseAmountofWalletResponse.WalletReductionMoneyGuiId = walletReductionMoney.GuiId;
                        #endregion
                        return new SuccessResult<DecreaseAmountofWalletResponseDto>(decreaseAmountofWalletResponse);
                    }
                    #region cüzdan bakiyesi yetersizdir
                    else
                    {
                        return new ErrorResult<DecreaseAmountofWalletResponseDto>(decreaseAmountofWalletResponse, WalletErrorEnum.AMOUNT_MISSING);
                    }
                    #endregion
                }
                #region geçersiz tutar
                else
                {
                    return new ErrorResult<DecreaseAmountofWalletResponseDto>(decreaseAmountofWalletResponse, WalletErrorEnum.INVALID_AMOUNT);
                }
                #endregion
            }
            #region cüzdan bulunamadı
            else
            {
                return new ErrorResult<DecreaseAmountofWalletResponseDto>(decreaseAmountofWalletResponse, WalletErrorEnum.WALLET_NOT_FOUND);
            }
            #endregion
        }


    }

}

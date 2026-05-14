using AutoMapper;
using Bank.Persistence.Repositories.WalletRepositories;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Domain.Dto.ApiDto.MobilPaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.PaymentInfoDtos;
using Shared.Domain.Dto.BankDto.PaymentCallbackDataDtos;
using Shared.Domain.Dto.BankDto.PaymentDtos;
using Shared.Domain.Dto.BankDto.WalletDtos;
using Shared.Domain.Dto.IntegrationDto.FirmPaymentIntegrationSettingDtos;
using Shared.Domain.Dto.IntegrationDto.PaymentIntegrationDtos;
using Shared.Domain.Dto.IntegrationDto.PaymentMokaIntegrationDtos;
using Shared.Domain.Dto.NotificationDto.PaymentNotificationDtos;
using Shared.Domain.Entities.ApiEntities.WalletInfoModule;
using Shared.Domain.Entities.BankEntities.PaymentModule;
using Shared.Domain.Entities.BankEntities.WalletModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.BankEnums;
using Shared.Domain.Enums.IntegrationEnums;
using Shared.Domain.Errors.BankErrors;
using Shared.Domain.Errors.IntegrationErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.BankApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.IntegrationApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.MobilApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.NotificationApiInterfaces;
using Shared.Domain.RabbitMq.Command.ArchiveCreateRequest;
using Shared.Domain.RabbitMq.ProducersInterfaces;
using Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.PaymentCallbackDataRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.PaymentRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.BankApiRepositoryInterfaces.WalletRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.BankServiceInterfaces.PaymentServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.PaymentService
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentCallbackDataRepository _paymentCallbackDataRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletSpendMoneyRepository _walletSpendMoneyRepository;
        private readonly IWalletPushMoneyRepository _walletPushMoneyRepository;
        private readonly IPaymentIntegrationClientService _paymentIntegrationClientService;
        private readonly IPaymentNotificationClientService _paymentNotificationClientService;
        private readonly IPaymentInfoClientService _paymentInfoClientService;
        private readonly ICreateArchiveAndInvoiceRequestProducer _createArchiveRequestProducer;
        private readonly IUtilService _utilService;
        private readonly ICustomHttpUtilService _customHttpUtilService;
        private readonly IConfiguration _configuration;
        //private readonly IPaymentNotificationClientService _paymentNotificationClient;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IMapper mapper,
                           IPaymentRepository paymentRepository,
                           IPaymentCallbackDataRepository paymentCallbackDataRepository,
                           IUtilService utilService,
                           ILogger<PaymentService> logger,
                           IConfiguration configuration,
                           //IPaymentNotificationClientService paymentNotificationClient,
                           ICustomHttpUtilService customHttpUtilService,
                           IPaymentIntegrationClientService paymentIntegrationClientService,
                           IPaymentNotificationClientService paymentNotificationClientService,
                           IPaymentInfoClientService paymentInfoClientService,
                           IWalletRepository walletRepository,
                           IWalletSpendMoneyRepository walletSpendMoneyRepository,
                           IWalletPushMoneyRepository walletPushMoneyRepository,
                           ICreateArchiveAndInvoiceRequestProducer createArchiveRequestProducer) : base(
                           mapper)
        {
            _paymentRepository = paymentRepository;
            _paymentCallbackDataRepository = paymentCallbackDataRepository;
            _logger = logger;
            _utilService = utilService;
            _configuration = configuration;
            //_paymentNotificationClient = paymentNotificationClient;
            _customHttpUtilService = customHttpUtilService;
            _paymentIntegrationClientService = paymentIntegrationClientService;
            _paymentNotificationClientService = paymentNotificationClientService;
            _paymentInfoClientService = paymentInfoClientService;
            _walletRepository = walletRepository;
            _walletSpendMoneyRepository = walletSpendMoneyRepository;
            _walletPushMoneyRepository = walletPushMoneyRepository;
            _createArchiveRequestProducer = createArchiveRequestProducer;
        }
        /// <summary>
        /// cüzdan ile ödeme gerçekleşiyor
        /// </summary>
        /// <param name="paymentWalletRequest"></param>
        /// <returns></returns>
        public async Task<Result<PaymentWalletResponseDto>> PaymentWallet(PaymentWalletRequestDto paymentWalletRequest)
        {
            #region response dto oluşturuluyor
            var paymentWalletResponse = new PaymentWalletResponseDto();
            #endregion
            #region şarj işlemi ödemesi ise şarj işlemine ait ödeme verisi bulunuyor mu kontrol ediliyor
            if (paymentWalletRequest.PaymentReason == PaymentReasonEnum.CHARGE)
            {
                #region filter dto oluşturuluyor
                PaymentFilterDto paymentFilter = new PaymentFilterDto();
                paymentFilter.ChargeGuiId = paymentWalletRequest.ChargeGuiId;
                paymentFilter.PaymentStatus = PaymentStatusEnum .SUCCESSFUL;
                #endregion
                var checkPaymentExist = await _paymentRepository.GetPaymentAsNoTracking(paymentFilter).FirstOrDefaultAsync();
                #region işleme ait ödeme verisi mevcut
                if (checkPaymentExist != null)
                {
                    paymentWalletResponse.PaymentGuiId = checkPaymentExist.GuiId;
                    paymentWalletResponse.WalletSpendMoneyGuiId = checkPaymentExist.WalletSpendMoney.GuiId;
                    paymentWalletResponse.CompletedDate = checkPaymentExist.CompletedDate.GetValueOrDefault();
                    return new ErrorResult<PaymentWalletResponseDto>(paymentWalletResponse, PaymentErrorEnum.PAYMENT_WAS_COMPLETED);
                }
                #endregion
            }
            #endregion
            #region şimdiki zaman değişkeni oluşturuluyor
            var dateTimeNow = DateTime.Now;
            #endregion
            #region payment verisi oluşturuluyor
            var payment = _mapper.Map<Payment>(paymentWalletRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as Payment;
                    destData.CreatedDate = dateTimeNow;
                    destData.CompletedDate = dateTimeNow;
                    destData.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                    destData.GuiId = Guid.NewGuid() + "";
                    destData.PaymentMethod = PaymentMethodEnum.WALLET;
                    destData.PaymentChargeInfoJsonBase64 = _utilService.Base64Encode(paymentWalletRequest.PaymentChargeInfoJson);
                    destData.UserAdressJsonBase64 = _utilService.Base64Encode(paymentWalletRequest.UserAdressJson);
                });
            });
            #endregion
            #region şarj işlemi ödemesi
            if (paymentWalletRequest.PaymentReason == PaymentReasonEnum.CHARGE)
            {
                #region cüzdan verisi getiriliyor
                #region filter dto oluşturuluyor
                WalletFilterDto walletFilter = _mapper.Map<WalletFilterDto>(paymentWalletRequest);
                #endregion
                var wallet = await _walletRepository.GetWallet(walletFilter).FirstOrDefaultAsync();
                #endregion
                if (wallet != null)
                {
                    #region şarj işlem ücreti alınıyor
                    var amountTockenBase64Decoded = _utilService.Base64Decode(paymentWalletRequest.WalletAmountTockenGuiId);
                    var amountString = amountTockenBase64Decoded.Split('/')[1];
                    var amount = _utilService.ParseDecimal(amountString);
                    #endregion
                    #region cüzdan bakiyesi alınıyor
                    var walletAmountTockenBase64Decoded = _utilService.Base64Decode(wallet.AmountTockenGuiId);
                    var walletAmountString = walletAmountTockenBase64Decoded.Split('/')[1];
                    var walletAmount = _utilService.ParseDecimal(walletAmountString);
                    #endregion
                    #region cüzdan bakiyesi işlem için yeterli mi kontrol ediliyor
                    if (walletAmount < amount)
                    {
                        return new ErrorResult<PaymentWalletResponseDto>(paymentWalletResponse, PaymentErrorEnum.WALLET_AMOUNT_INSUFFICIENT);
                    }
                    #endregion
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
                    #region wallet spend money entity oluşturuluyor
                    WalletSpendMoney walletSpendMoney = new WalletSpendMoney();
                    walletSpendMoney.SpendAmount = amount;
                    walletSpendMoney.RefundedMoney = 0;
                    walletSpendMoney.Date = DateTime.Now;
                    walletSpendMoney.WalletId = wallet.Id;
                    walletSpendMoney.GuiId = Guid.NewGuid() + "";
                    walletSpendMoney.WalletProcessGuiId = paymentWalletRequest.WalletProcessGuiId;
                    payment.WalletSpendMoney = walletSpendMoney;
                    await _walletSpendMoneyRepository.InsertAsync(walletSpendMoney);
                    #endregion
                    #region veritabanına kayıt ediliyor
                    await _paymentRepository.InsertAsync(payment);
                    await _paymentRepository.SaveChangesAsync();
                    #endregion
                    #region rabbitmq ile bank apiye istek atılarak fatura oluşturma talebi iletiliyor
                    _createArchiveRequestProducer.archiveAndInvoiceCreateRequestCommand(new CreateArchiveAndInvoiceRequestCommand()
                    {
                        FirmGuiId = paymentWalletRequest.FirmGuiId,
                        PaymentGuiId = payment.GuiId,
                        UserAdressJsonBase64 = payment.UserAdressJsonBase64,
                        ProcessingUserAdressGuiId = paymentWalletRequest.ProcessingUserAdressGuiId,
                        PaymentChargeInfoJsonBase64 = payment.PaymentChargeInfoJsonBase64,
                    });
                    #endregion
                    #region response dto setleniyor
                    paymentWalletResponse.PaymentGuiId = payment.GuiId;
                    paymentWalletResponse.WalletSpendMoneyGuiId = walletSpendMoney.GuiId;
                    paymentWalletResponse.CompletedDate = payment.CompletedDate.GetValueOrDefault();
                    paymentWalletResponse.WalletAmount = wallet.WalletAmount;
                    #endregion
                    return new SuccessResult<PaymentWalletResponseDto>(paymentWalletResponse);
                }
                #region cüzdan bulunamadı
                else
                {
                    return new ErrorResult<PaymentWalletResponseDto>(paymentWalletResponse, PaymentErrorEnum.WALLET_NOT_FOUND);
                }
                #endregion
            }
            #endregion
            #region geçersiz ödeme sebebi
            else
            {
                return new ErrorResult<PaymentWalletResponseDto>(paymentWalletResponse, PaymentErrorEnum.INVALID_PAYMENT_REASON);
            }
            #endregion
        }
        /// <summary>
        /// Kredi kartı direk ödeme gerçekleşiyor
        /// </summary>
        /// <param name="paymentDirectDebitCardRequest"></param>
        /// <returns></returns>
        public async Task<Result<PaymentDirectDebitCardResponseDto>> PaymentDirectDebitCard(PaymentDirectDebitCardRequestDto paymentDirectDebitCardRequest)
        {
            #region response dto oluşturuluyor
            PaymentDirectDebitCardResponseDto paymentDirectDebitCardResponse = new PaymentDirectDebitCardResponseDto();
            #endregion
            #region cüzdana para aktarma ödemesi ise cüzdan getiriliyor
            Wallet? wallet = null;
            if (paymentDirectDebitCardRequest.PaymentReason == PaymentReasonEnum.WALLET)
            {
                #region cüzdan getiriliyor
                #region filter dto oluşturuluyor
                WalletFilterDto walletFilter = new WalletFilterDto();
                walletFilter.GuiId = paymentDirectDebitCardRequest.WalletGuiId;
                walletFilter.WalletTockenGuiId = paymentDirectDebitCardRequest.WalletTockenGuiId;
                #endregion
                wallet = await _walletRepository.GetWallet(walletFilter).FirstOrDefaultAsync();
                #endregion
                #region cüzdan bulunamadı
                if (wallet == null)
                {
                    return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.WALLET_NOT_FOUND);
                }
                #endregion
            }
            #endregion
            #region payment verisi oluşturuluyor
            var payment = _mapper.Map<Payment>(paymentDirectDebitCardRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as Payment;
                    destData.CreatedDate = DateTime.Now;
                    destData.PaymentStatus = PaymentStatusEnum.WAITING;
                    destData.GuiId = Guid.NewGuid() + "";
                    destData.PaymentMethod = PaymentMethodEnum.DEBIT_CARD;
                    destData.SecurityKey = _utilService.GetRandomString(32);
                    destData.PaymentChargeInfoJsonBase64 = _utilService.Base64Encode(paymentDirectDebitCardRequest.PaymentChargeInfoJson);
                    destData.UserAdressJsonBase64 = _utilService.Base64Encode(paymentDirectDebitCardRequest.UserAdressJson);
                });
            });
            #endregion
            #region payment verisi kayıt ediliyor
            var insertedPayment = await _paymentRepository.InsertAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            #endregion
            #region integration apiye istek atılarak ödeme gerçekleştiriliyor
            #region request dto oluşturuluyor
            PaymentDirectIntegrationRequestDto paymentDirectIntegrationRequest = _mapper.Map<PaymentDirectIntegrationRequestDto>(paymentDirectDebitCardRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentDirectIntegrationRequestDto;
                    destData.PaymentGuiId = insertedPayment.GuiId;
                });
            });
            #endregion
            var paymentDirectIntegrationResponse = await _paymentIntegrationClientService.PaymentDirectIntegration(paymentDirectIntegrationRequest);
            #endregion
            #region ödeme işlemi başarılı
            if (paymentDirectIntegrationResponse.ResultType == ResultType.Ok)
            {
                #region payment güncelleniyor
                DateTime dateTime = DateTime.Now;
                _paymentRepository.UpdateWithProperties(insertedPayment, new Expression<Func<Payment, object>>[] {
                    s => s.PaymentStatus,
                    s => s.CompletedDate,
                });
                insertedPayment.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                insertedPayment.CompletedDate = dateTime;

                await _paymentRepository.SaveChangesAsync();
                #endregion
                #region response dto setleniyor
                paymentDirectDebitCardResponse = _mapper.Map<PaymentDirectDebitCardResponseDto>(paymentDirectIntegrationResponse.Data, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as PaymentDirectDebitCardResponseDto;
                        destData.PaymentGuiId = payment.GuiId;
                        destData.CompletedDate = dateTime;
                    });
                });
                #endregion
                #region cüzdana para aktarma işlemi
                if (payment.PaymentReason == PaymentReasonEnum.WALLET)
                {
                    #region arttırılacak miktar alınıyor
                    var amountTockenBase64Decoded = _utilService.Base64Decode(paymentDirectDebitCardRequest.WalletAmountTockenGuiId);
                    var amountStringArray = amountTockenBase64Decoded.Split('/');
                    var amount = _utilService.ParseDecimal(amountStringArray[1]);
                    #endregion
                    if (amountStringArray[0] == wallet.GuiId && amountStringArray[2] == wallet.WalletTockenGuiId)
                    {
                        #region walletpushmoney oluşturuluyor
                        WalletPushMoney walletPushMoney = new WalletPushMoney();
                        walletPushMoney.GuiId = Guid.NewGuid().ToString();
                        walletPushMoney.WalletId = wallet.Id;
                        walletPushMoney.PaymentId = insertedPayment.Id;
                        walletPushMoney.PushAmount = amount;
                        walletPushMoney.WalletProcessGuiId = paymentDirectDebitCardRequest.WalletProcessGuiId;
                        #endregion
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
                        #region veritabanına kayıt ediliyor
                        await _walletPushMoneyRepository.InsertAsync(walletPushMoney);
                        await _paymentRepository.SaveChangesAsync();
                        #endregion
                        #region response dto setleniyor
                        paymentDirectDebitCardResponse.WalletPushMoneyGuiId = walletPushMoney.GuiId;
                        paymentDirectDebitCardResponse.WalletAmount = amount;
                        #endregion
                    }
                    #region geçersiz tutar
                    else
                    {
                        return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.INVALID_AMOUNT);
                    }
                    #endregion
                }
                #endregion
                #region şarj işlemi ödemesi
                else
                {
                    #region rabbitmq ile integration apiye istek atılarak fatura oluşturma talebi iletiliyor
                    _createArchiveRequestProducer.archiveAndInvoiceCreateRequestCommand(new CreateArchiveAndInvoiceRequestCommand()
                    {
                        FirmGuiId = paymentDirectDebitCardRequest.FirmGuiId,
                        PaymentGuiId = payment.GuiId,
                        UserAdressJsonBase64 = payment.UserAdressJsonBase64,
                        ProcessingUserAdressGuiId = paymentDirectDebitCardRequest.ProcessingUserAdressGuiId,
                        PaymentChargeInfoJsonBase64 = payment.PaymentChargeInfoJsonBase64,
                    });
                    #endregion
                }
                #endregion
                return new SuccessResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse);
            }
            #endregion
            #region ödeme işlemi başarısız
            else
            {
                #region payment güncelleniyor
                _paymentRepository.UpdateWithProperties(insertedPayment, new Expression<Func<Payment, object>>[] {
                    s => s.PaymentStatus,
                    s => s.CompletedDate,
                });
                insertedPayment.PaymentStatus = PaymentStatusEnum.FAILURE;
                insertedPayment.CompletedDate = DateTime.Now;
                await _paymentRepository.SaveChangesAsync();
                #endregion
                return PreparePaymentDirectDebitCardErrorHandle(paymentDirectDebitCardResponse, paymentDirectIntegrationResponse);
            }
            #endregion
        }
        /// <summary>
        /// Kredi kartı 3D ödeme başlatma isteği atılıyor
        /// </summary>
        /// <param name="paymentStartDebitCard3DRequest"></param>
        /// <returns></returns>
        public async Task<Result<PaymentStartDebitCard3DResponseDto>> PaymentStartDebitCard3D(PaymentStartDebitCard3DRequestDto paymentStartDebitCard3DRequest)
        {
            #region response dto oluşturuluyor
            PaymentStartDebitCard3DResponseDto paymentStartDebitCard3DResponse = new PaymentStartDebitCard3DResponseDto();
            #endregion
            #region şarj işlemi ödemesi ise şarj işlemine ait başarılı ödeme verisi bulunuyor mu kontrol ediliyor
            if (paymentStartDebitCard3DRequest.PaymentReason == PaymentReasonEnum.CHARGE)
            {
                #region filter dto oluşturuluyor
                PaymentFilterDto paymentFilter = new PaymentFilterDto();
                paymentFilter.ChargeGuiId = paymentStartDebitCard3DRequest.ChargeGuiId;
                paymentFilter.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                #endregion
                #region ödeme çekiliyor
                var checkPaymentExist = await _paymentRepository.GetPaymentAsNoTracking(paymentFilter).FirstOrDefaultAsync();
                #endregion
                #region işleme ait ödeme verisi mevcut
                if (checkPaymentExist != null)
                {
                    return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.PAYMENT_WAS_COMPLETED);
                }
                #endregion
            }
            #endregion
            #region payment callback data verisi oluşturuluyor
            PaymentCallbackData paymentCallbackData = _mapper.Map<PaymentCallbackData>(paymentStartDebitCard3DRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentCallbackData;
                    destData.CreatedDate = DateTime.Now;
                    destData.State = PaymentCallbackStateEnum.CREATED;
                });
            });
            #endregion
            #region payment verisi oluşturuluyor
            var payment = _mapper.Map<Payment>(paymentStartDebitCard3DRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as Payment;
                    destData.CreatedDate = DateTime.Now;
                    destData.PaymentStatus = PaymentStatusEnum.WAITING;
                    destData.GuiId = Guid.NewGuid() + "";
                    destData.PaymentMethod =  PaymentMethodEnum.DEBIT_CARD;
                    destData.SecurityKey = _utilService.GetRandomString(32);
                    destData.PaymentChargeInfoJsonBase64 = _utilService.Base64Encode(paymentStartDebitCard3DRequest.PaymentChargeInfoJson);
                    destData.UserAdressJsonBase64 = _utilService.Base64Encode(paymentStartDebitCard3DRequest.UserAdressJson);
                    destData.PaymentCallbackData = paymentCallbackData;
                });
            });
            #endregion
            #region payment ve callback data verisi kayıt ediliyor
            var insertedPayment = await _paymentRepository.InsertAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            #endregion
            #region şarj işlemi ödemesi
            if (paymentStartDebitCard3DRequest.PaymentReason == PaymentReasonEnum.CHARGE)
            {

            }
            #endregion
            #region cüzdana para aktarma ödemesi
            else if (paymentStartDebitCard3DRequest.PaymentReason == PaymentReasonEnum.WALLET)
            {
                #region cüzdan verisi getiriliyor
                #region filter dto oluşturuluyor
                WalletFilterDto walletFilter = new WalletFilterDto();
                walletFilter.GuiId = paymentStartDebitCard3DRequest.WalletGuiId;
                walletFilter.WalletTockenGuiId = paymentStartDebitCard3DRequest.WalletTockenGuiId;
                #endregion
                var wallet = await _walletRepository.GetWalletAsNoTracking(walletFilter).FirstOrDefaultAsync();
                #endregion
                if (wallet == null)
                {
                    return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.WALLET_NOT_FOUND);
                }
            }
            #endregion
            #region geçersiz ödeme sebebi
            else
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.AN_ERROR_OCCURRED);
            }
            #endregion
            #region integration apiye istek atılarak 3D ödeme işlemi başlatılıyor
            #region request dto oluşturuluyor
            PaymentStart3DIntegrationRequestDto paymentStart3DIntegrationRequest = _mapper.Map<PaymentStart3DIntegrationRequestDto>(paymentStartDebitCard3DRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentStart3DIntegrationRequestDto;
                    destData.PaymentId = payment.Id;
                    destData.PaymentGuiId = payment.GuiId;
                    destData.PaymentCallbackDataId = payment.PaymentCallbackData.Id;
                });
            });
            #endregion
            var paymentStart3DIntegrationResponse = await _paymentIntegrationClientService.PaymentStart3DIntegration(paymentStart3DIntegrationRequest);
            if (paymentStart3DIntegrationResponse.ResultType == ResultType.Ok)
            {
                #region response dto setleniyor
                paymentStartDebitCard3DResponse = _mapper.Map<PaymentStartDebitCard3DResponseDto>(paymentStart3DIntegrationResponse.Data, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as PaymentStartDebitCard3DResponseDto;
                        destData.SecurityKey = payment.SecurityKey;
                        if (destData.PaymentBankType == PaymentIntegrationBankTypeEnum.MOKA)
                        {
                            destData.Payment3DContentType = Payment3DContentTypeEnum.REDIRCET_URL;
                        }
                        else
                        {
                            destData.Payment3DContentType = Payment3DContentTypeEnum.HTML_CONTENT;
                        }
                    });
                });
                #endregion
            }
            #region 3D ödeme işlemi başlatılamadı
            else
            {
                #region payment güncelleniyor
                _paymentRepository.UpdateWithProperties(insertedPayment, new Expression<Func<Payment, object>>[] {
                        s => s.PaymentStatus,
                        s => s.CompletedDate,
                    });
                insertedPayment.PaymentStatus = PaymentStatusEnum.FAILURE;
                insertedPayment.CompletedDate = DateTime.Now;
                #endregion
                #region payment callback data güncelleniyor
                _paymentCallbackDataRepository.UpdateWithProperties(paymentCallbackData, new Expression<Func<PaymentCallbackData, object>>[] {
                        s => s.State,
                        s => s.ReceievedDate,
                    });
                paymentCallbackData.State = PaymentCallbackStateEnum.RECEIVED;
                paymentCallbackData.ReceievedDate = DateTime.Now;
                #endregion
                #region veritabanına kayıt ediliyor
                await _paymentRepository.SaveChangesAsync();
                #endregion
                #region response dto setleniyor
                paymentStartDebitCard3DResponse = _mapper.Map<PaymentStartDebitCard3DResponseDto>(paymentStart3DIntegrationResponse.Data, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as PaymentStartDebitCard3DResponseDto;
                        destData.SecurityKey = payment.SecurityKey;
                    });
                });
                #endregion
                return PreparePaymentStartDebitCard3DErrorHandle(paymentStartDebitCard3DResponse, paymentStart3DIntegrationResponse);
            }
            #endregion
          
            #endregion
            return new SuccessResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse);
        }
        /// <summary>
        /// Kredi kartı 3D ödeme tamamlanıyor
        /// </summary>
        /// <param name="paymentCompleteDebitCard3DRequest"></param>
        /// <returns></returns>
        public async Task<Result<PaymentCompleteDebitCard3DResponseDto>> PaymentCompleteDebitCard3D(PaymentCompleteDebitCard3DRequestDto paymentCompleteDebitCard3DRequest)
        {
            #region response dto oluşturuluyor
            PaymentCompleteDebitCard3DResponseDto paymentCompleteDebitCard3DResponse = new PaymentCompleteDebitCard3DResponseDto();
            #endregion
            #region payment getiriliyor
            #region request dto oluşturuluyor
            PaymentFilterDto paymentFilter = new PaymentFilterDto();
            paymentFilter.PaymentGuiId = paymentCompleteDebitCard3DRequest.PaymentGuiId;
            #endregion
            var payment = await _paymentRepository.GetPayment(paymentFilter).FirstOrDefaultAsync();
            #endregion
            if (payment != null)
            {
                #region callbackdata verisi bulunamadı
                if (payment.PaymentCallbackData == null)
                {
                    #region sonuç signalr ile mobil kullanıcıya bildiriliyor
                    await SendPaymentInfoMobilWithSignalr(payment, "Ödeme Geri Arama Bilgisi Bulunamadı", "0" , false);
                    #endregion
                    return new ErrorResult<PaymentCompleteDebitCard3DResponseDto>(paymentCompleteDebitCard3DResponse, PaymentErrorEnum.PAYMENT_CALLBACK_DATA_NOT_FOUND);
                }
                #endregion
                #region payment güncelleniyor
                _paymentRepository.UpdateWithProperties(payment, new Expression<Func<Payment, object>>[] {
                    s => s.PaymentStatus,
                    s => s.CompletedDate,
                });
                payment.PaymentStatus = paymentCompleteDebitCard3DRequest.PaymentStatus;
                payment.CompletedDate = paymentCompleteDebitCard3DRequest.CompletedDate;
                #endregion
                #region payment callback data güncelleniyor
                _paymentCallbackDataRepository.UpdateWithProperties(payment.PaymentCallbackData, new Expression<Func<PaymentCallbackData, object>>[] {
                    s => s.State,
                    s => s.ReceievedDate,
                });
                payment.PaymentCallbackData.State = PaymentCallbackStateEnum.RECEIVED;
                payment.PaymentCallbackData.ReceievedDate = DateTime.Now;
                #endregion
                #region veritabanına kayıt ediliyor
                await _paymentRepository.SaveChangesAsync();
                #endregion
                #region mobil apiye istek atılarak ödeme tamamlanıyor
                #region request dto oluşturuluyor
                CompleteMobilPaymentInfoDebitCard3DRequestDto completeMobilPaymentInfoDebitCard3DRequest = _mapper.Map<CompleteMobilPaymentInfoDebitCard3DRequestDto>(payment, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as CompleteMobilPaymentInfoDebitCard3DRequestDto;
                        destData.PaymentBankType = paymentCompleteDebitCard3DRequest.PaymentBankType;
                    });
                });
                #endregion
                var completePaymentInfoDebitCard3DResponse = await _paymentInfoClientService.CompletePaymentInfoDebitCard3D(completeMobilPaymentInfoDebitCard3DRequest);
                #endregion
                #region sonuç signalr ile mobil kullanıcıya bildiriliyor
                if (payment.PaymentStatus == PaymentStatusEnum.SUCCESSFUL)
                {
                    await SendPaymentInfoMobilWithSignalr(payment, "İşlem Başarılı","1",true);
                }
                else
                {
                    await SendPaymentInfoMobilWithSignalr(payment, "Ödeme Başarısız", "0", false);
                }
                #endregion
                #region rabbitmq ile integration apiye istek atılarak fatura oluşturma talebi iletiliyor
                if (payment.PaymentStatus == PaymentStatusEnum.SUCCESSFUL)
                {
                    _createArchiveRequestProducer.archiveAndInvoiceCreateRequestCommand(new CreateArchiveAndInvoiceRequestCommand()
                    {
                        FirmGuiId = payment.FirmGuiId,
                        PaymentGuiId = payment.GuiId,
                        UserAdressJsonBase64 = payment.UserAdressJsonBase64,
                        ProcessingUserAdressGuiId = payment.ProcessingUserAdressGuiId,
                        PaymentChargeInfoJsonBase64 = payment.PaymentChargeInfoJsonBase64,
                    });
                }
                #endregion
                return new SuccessResult<PaymentCompleteDebitCard3DResponseDto>(paymentCompleteDebitCard3DResponse);
            }
            #region payment bulunamadı
            else
            {
                return new ErrorResult<PaymentCompleteDebitCard3DResponseDto>(paymentCompleteDebitCard3DResponse, PaymentErrorEnum.PAYMENT_INFORMATION_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// ödeme durumu getiriliyor
        /// </summary>
        /// <param name="getPaymentStatusRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPaymentStatusResponseDto>> GetPaymentStatus(GetPaymentStatusRequestDto getPaymentStatusRequest)
        {
            #region response dto oluşturuluyor
            var getPaymentStatusResponse = new GetPaymentStatusResponseDto();
            #endregion
            #region ödeme bilgisi getiriliyor
            #region filter dto oluşturuluyor
            PaymentFilterDto paymentFilter = new PaymentFilterDto();
            paymentFilter.PaymentMethod = PaymentMethodEnum.DEBIT_CARD;
            paymentFilter.SecurityKey = getPaymentStatusRequest.SecurityKey;
            #endregion
            var payment = await _paymentRepository.GetOnlyPaymentAsNoTracking(paymentFilter).FirstOrDefaultAsync();
            #endregion
            if (payment != null)
            {
                #region response dto setleniyor
                getPaymentStatusResponse.PaymentStatus = payment.PaymentStatus;
                #endregion
                return new SuccessResult<GetPaymentStatusResponseDto>(getPaymentStatusResponse);
            }
            #region ödeme bulunamadı
            else
            {
                return new ErrorResult<GetPaymentStatusResponseDto>(getPaymentStatusResponse, PaymentErrorEnum.PAYMENT_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// ödeme bilgisi getiriliyor
        /// </summary>
        /// <param name="getPaymentRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPaymentDataResponseDto>> GetPayment(GetPaymentDataRequestDto getPaymentRequest)
        {
            #region response dto oluşturuluyor
            var getPaymentResponse = new GetPaymentDataResponseDto();
            #endregion
            #region ödeme bilgisi getiriliyor
            #region filter dto oluşturuluyor
            PaymentFilterDto paymentFilter = _mapper.Map<PaymentFilterDto>(getPaymentRequest);
            #endregion
            var payment = await _paymentRepository.GetPaymentAsNoTracking(paymentFilter).FirstOrDefaultAsync();
            #endregion
            if (payment != null)
            {
                #region response dto setleniyor
                getPaymentResponse.Payment = _mapper.Map<PaymentDto>(payment);
                #endregion
                return new SuccessResult<GetPaymentDataResponseDto>(getPaymentResponse);
            }
            #region ödeme bulunamadı
            else
            {
                return new ErrorResult<GetPaymentDataResponseDto>(getPaymentResponse, PaymentErrorEnum.PAYMENT_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// ödeme bilgileri getiriliyor
        /// </summary>
        /// <param name="getPaymentListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPaymentDataListResponseDto>> GetPaymentList(GetPaymentDataListRequestDto getPaymentListRequest)
        {
            #region response dto oluşturuluyor
            var getPaymentListResponse = new GetPaymentDataListResponseDto();
            getPaymentListResponse.PaymentList = new List<PaymentDto>();
            #endregion
            #region ödeme bilgileri getiriliyor
            #region filter dto oluşturuluyor
            PaymentFilterDto paymentFilter = _mapper.Map<PaymentFilterDto>(getPaymentListRequest);
            #endregion
            var paymentList = await _paymentRepository.GetPaymentAsNoTracking(paymentFilter).ToListAsync();
            #endregion
            _logger.LogError("payments:");
            _logger.LogError(System.Text.Json.JsonSerializer.Serialize(paymentFilter));
            #region response dto setleniyor
            getPaymentListResponse.PaymentList = _mapper.Map<List<PaymentDto>>(paymentList);
            #endregion
            return new SuccessResult<GetPaymentDataListResponseDto>(getPaymentListResponse);
        }
        /// <summary>
        /// kredi kartı ödemesi için payment callback data oluşturuluyor
        /// </summary>
        /// <param name="createPaymentCallbackDataRequest"></param>
        /// <returns></returns>
        public async Task<Result<CreatePaymentCallbackDataResponseDto>> CreatePaymentCallback(CreatePaymentCallbackDataRequestDto createPaymentCallbackDataRequest)
        {
            CreatePaymentCallbackDataResponseDto createPaymentCallbackDataResponse = new CreatePaymentCallbackDataResponseDto();
            #region payment data verisi oluşturuluyor
            var paymentCallbackData = _mapper.Map<PaymentCallbackData>(createPaymentCallbackDataRequest);
            var insertedData = await _paymentCallbackDataRepository.InsertAsync(paymentCallbackData);
            await _paymentCallbackDataRepository.SaveChangesAsync();
            #endregion
            createPaymentCallbackDataResponse.Id = insertedData.Id;
            return new SuccessResult<CreatePaymentCallbackDataResponseDto>(createPaymentCallbackDataResponse);
        }
        /// <summary>
        /// e-arşiv veya e-fatura için ödeme bilgisi getiriliyor
        /// </summary>
        /// <param name="getPaymentDetailForArchiveAndInvoiceRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPaymentDetailForArchiveAndInvoiceResponseDto>> GetPaymentDetailForArchiveAndInvoice(GetPaymentDetailForArchiveAndInvoiceRequestDto getPaymentDetailForArchiveAndInvoiceRequest)
        {
            #region response dto oluşturuluyor
            var getPaymentResponse = new GetPaymentDetailForArchiveAndInvoiceResponseDto();
            #endregion
            #region ödeme bilgisi getiriliyor
            #region filter dto oluşturuluyor
            PaymentFilterDto paymentFilter = new PaymentFilterDto();
            paymentFilter.PaymentGuiId = getPaymentDetailForArchiveAndInvoiceRequest.PaymentGuiId;
            #endregion
            var payment = await _paymentRepository.GetPaymentAsNoTracking(paymentFilter).FirstOrDefaultAsync();
            #endregion
            if (payment != null)
            {
                #region response dto setleniyor
                getPaymentResponse.Kdv = payment.Kdv.GetValueOrDefault();
                getPaymentResponse.PaidPrice = payment.PaidPrice;
                getPaymentResponse.PaymentDate = payment.CompletedDate.GetValueOrDefault();
                getPaymentResponse.PaymentGuiId = payment.GuiId;
                getPaymentResponse.PaymentMethod = payment.PaymentMethod;
                #endregion
                return new SuccessResult<GetPaymentDetailForArchiveAndInvoiceResponseDto>(getPaymentResponse);
            }
            #region ödeme bulunamadı
            else
            {
                return new ErrorResult<GetPaymentDetailForArchiveAndInvoiceResponseDto>(getPaymentResponse, PaymentErrorEnum.PAYMENT_NOT_FOUND);
            }
            #endregion
        }
        private Result<PaymentStartDebitCard3DResponseDto> PreparePaymentStartDebitCard3DErrorHandle(PaymentStartDebitCard3DResponseDto paymentStartDebitCard3DResponse, Result<PaymentStart3DIntegrationResponseDto> paymentStart3DIntegrationResponse)
        {
            if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_PAYMENT_DEALER_AUTHENTICATION_INVALID_REQUEST ||
        paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_PAYMENT_DEALER_AUTHENTICATION_INVALID_ACCOUNT ||
        paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_PAYMENT_DEALER_AUTHENTICATION_VIRTUAL_POS_NOT_FOUND ||
        paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_DEALER_PAYMENT_LIMITS_DAILY_DEALER_LIMIT_EXCEEDED ||
        paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_THREED_REQUIRED ||
       paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_PAYMENT_MUST_BE_AUTHORIZATION ||
        paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_AUTHORIZATION_FORBIDDEN_FOR_THIS_DEALER ||
         paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_POOL_PAYMENT_NOT_AVAILABLE_FOR_DEALER ||
          paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_POOL_PAYMENT_REQUURED_FOR_DEALER ||
           paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_CARD_TOKEN_CANNOT_USE_WITH_SAVE_CARD ||
            paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_CARD_TOKEN_NOT_FOUND ||
             paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_ONLY_CARD_TOKEN_OR_CARD_NUMBER ||
             paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_CHANNEL_PERMISSION_NOT_AVAILABLE ||
             paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_IP_ADDRESS_NOT_ALLOWED ||
             paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_INVALID_SUB_MERCHANT_NAME
       )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_DEALER_ERROR);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_INVALID_REQUEST ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_REDIRECT_URL_REQUIRED ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_INVALID_CURRENCY_CODE
                )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_DEALER_PROCESS_ERROR);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_TOKENIZATION_NOT_AVAILABLE_FOR_DEALER

                )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.CARD_STORAGE_NOT_AVAILABLE_FOR_DEALER);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_VIRTUAL_POS_NOT_AVAILABLE

              )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.VIRTUAL_POS_NOT_AVAILABLE_FOR_DEBIT_CARD);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_FOREIGN_CURRENCY_NOT_AVAILABLE_FOR_THIS_DEALER
             )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.FOREIGN_CURRENCY_NOT_AVAILABLE_FOR_THIS_DEALER);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION
            )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DAILY_CHECK_DEALER_PAYMENT_LIMITS_DAILY_CARD_LIMIT_EXCEEDED)
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.PAYMENT_LIMITS_DAILY_CARD_LIMIT_EXCEEDED);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_CARD_INFO_INVALID_CARD_INFO)
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.INVALID_CARD_INFO);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION)
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE_FOR_DEALER ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_INVALID_INSTALLMENT_NUMBER ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE_FOR_VIRTUAL_POS ||
                 paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_INVALID_INSTALLMENT_NUMBER ||
                 paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE_FOR_DEALER ||
                 paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE_FOR_VIRTUAL_POS ||
                 paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_DEALER_COMMISSION_RATE_NOT_FOUND ||
                 paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_3D_REQUEST_DEALER_GROUP_COMMISSION_RATE_NOT_FOUND)
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_EX)
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_GENERAL_ERROR);
            }
            else if (paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.SERVICE_NOT_EXIST_FOR_SELECTED_PAYMENT_INTEGRATION ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.SELECTED_PAYMENT_INTEGRATION_NOT_FOUND ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.PAYMENT_MOKA_INTEGRATION_SETTING_NOT_FOUND ||
                paymentStart3DIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.FIRM_PAYMENT_INTEGRATION_NOT_FOUND
                )
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentIntegrationErrorEnum.FAILED_TO_DO_DIRECT_PAYMENT);
            }
            else
            {
                return new ErrorResult<PaymentStartDebitCard3DResponseDto>(paymentStartDebitCard3DResponse, PaymentErrorEnum.AN_ERROR_OCCURRED);
            }
        }

        private Result<PaymentDirectDebitCardResponseDto> PreparePaymentDirectDebitCardErrorHandle(PaymentDirectDebitCardResponseDto paymentDirectDebitCardResponse, Result<PaymentDirectIntegrationResponseDto> paymentDirectIntegrationResponse)
        {
            if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_PAYMENT_DEALER_AUTHENTICATION_INVALID_REQUEST ||
                paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_PAYMENT_DEALER_AUTHENTICATION_INVALID_ACCOUNT ||
                paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_PAYMENT_DEALER_AUTHENTICATION_VIRTUAL_POS_NOT_FOUND ||
                paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_DEALER_PAYMENT_LIMITS_DAILY_DEALER_LIMIT_EXCEEDED ||
                paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_THREED_REQUIRED)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_DEALER_ERROR);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DAILY_CHECK_DEALER_PAYMENT_LIMITS_DAILY_CARD_LIMIT_EXCEEDED)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.PAYMENT_LIMITS_DAILY_CARD_LIMIT_EXCEEDED);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_CHECK_CARD_INFO_INVALID_CARD_INFO)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.INVALID_CARD_INFO);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE_FOR_DEALER ||
                paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_INVALID_INSTALLMENT_NUMBER ||
                paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_DO_DIRECT_PAYMENT_THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE_FOR_VIRTUAL_POS)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_EX || paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.MOKA_GENERAL_ERROR)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_GENERAL_ERROR);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.SERVICE_NOT_EXIST_FOR_SELECTED_PAYMENT_INTEGRATION)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentIntegrationErrorEnum.FAILED_TO_DO_DIRECT_PAYMENT);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.SELECTED_PAYMENT_INTEGRATION_NOT_FOUND)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentIntegrationErrorEnum.FAILED_TO_DO_DIRECT_PAYMENT);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.PAYMENT_MOKA_INTEGRATION_SETTING_NOT_FOUND)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentIntegrationErrorEnum.FAILED_TO_DO_DIRECT_PAYMENT);
            }
            else if (paymentDirectIntegrationResponse.ErrorCode == (int)PaymentIntegrationErrorEnum.FIRM_PAYMENT_INTEGRATION_NOT_FOUND)
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentIntegrationErrorEnum.FAILED_TO_DO_DIRECT_PAYMENT);
            }
            else
            {
                return new ErrorResult<PaymentDirectDebitCardResponseDto>(paymentDirectDebitCardResponse, PaymentErrorEnum.AN_ERROR_OCCURRED);
            }
        }
        private async Task SendPaymentInfoMobilWithSignalr(Payment payment, string status, string mdStatus, bool isSuccess)
        {
            await _paymentNotificationClientService.SendOnlyUser(new PaymentNotificationDto()
            {
                ConnectionId = payment.PaymentCallbackData.ConnectionId,
                Status = status,
                MdStatus = mdStatus,
                ChargeGuiId = payment.ChargeGuiId,
                IsSuccessful = true
            });
        }
    }
}

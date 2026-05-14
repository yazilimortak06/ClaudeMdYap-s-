using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.MobilDeviceContentDtos;
using Shared.Domain.Dto.ApiDto.MobilLoginDtos;
using Shared.Domain.Dto.ApiDto.UserCurrentDeviceDtos;
using Shared.Domain.Dto.ApiDto.UserDtos;
using Shared.Domain.Dto.NotificationDto.MobilConnectionDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestDtos;
using Shared.Domain.Dto.TockenDto.MobilUserDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginFormDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginSessionDtos;
using Shared.Domain.Entities.ApiEntities.GuestUserModule;
using Shared.Domain.Entities.ApiEntities.MobilDeviceModule;
using Shared.Domain.Entities.ApiEntities.UserModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.Errors.TockenErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.NotificationApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.TockenApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.GuestUserRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.MobilDeviceContentRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserCurrentDeviceRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.MobilLoginServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Services.MobilApiServices.MobilLogin
{
    public class MobilLoginService : BaseService, IMobilLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserCurrentDeviceRepository _userCurrentDeviceRepository;
        private readonly IMobilDeviceContentRepository _mobilDeviceContentRepository;
        private readonly IGuestUserRepository _guestUserRepository;
        private readonly IUtilService _utilService;
        private readonly IMobilUserClientService _mobilUserClientService;
        private readonly ICustomHttpUtilService _customHttpUtilService;
        private readonly IMobilConnectionClientService _mobilConnectionClientService;
        private readonly IConfiguration _configuration;
        private readonly IUserContextProvider _userContextProvider;

        public MobilLoginService(IMapper mapper,
                                IUtilService utilService,
                                ICustomHttpUtilService customHttpUtilService,
                                IUserContextProvider userContextProvider,
                                IUserCurrentDeviceRepository userCurrentDeviceRepository,
                                IConfiguration configuration,
                                IUserRepository userRepository,
                                IMobilDeviceContentRepository mobilDeviceContentRepository,
                                IGuestUserRepository guestUserRepository,
                                IMobilUserClientService mobilUserClientService,
                                IMobilConnectionClientService mobilConnectionClientService) : base(
                           mapper
                               )
        {
            _userRepository = userRepository;
            _userCurrentDeviceRepository = userCurrentDeviceRepository;
            _utilService = utilService;
            _customHttpUtilService = customHttpUtilService;
            _configuration = configuration;
            _userContextProvider = userContextProvider;
            _mobilDeviceContentRepository = mobilDeviceContentRepository;
            _guestUserRepository = guestUserRepository;
            _mobilUserClientService = mobilUserClientService;
            _mobilConnectionClientService = mobilConnectionClientService;
        }
        #region ekleme,güncelleme,silme

        #endregion
        /// <summary>
        /// Giriş işlemi için form hazırlanıyor
        /// </summary>
        /// <param name="mobilLoginFormRequest"></param>
        /// <returns></returns>
        public async Task<Result<MobilLoginFormResponseDto>> LoginForm(MobilLoginFormRequestDto mobilLoginFormRequest)
        {
            #region mobil device content çekiliyor
            #region filter dto oluşturuluyor
            MobilDeviceContentFilterDto mobilDeviceContentFilter = _mapper.Map<MobilDeviceContentFilterDto>(mobilLoginFormRequest);
            #endregion
            var mobilDeviceContent = await _mobilDeviceContentRepository.GetMobilDeviceContentAsNoTracking(mobilDeviceContentFilter).FirstOrDefaultAsync();
            #endregion
            if (mobilDeviceContent != null)
            {
                #region login formu için tocken api ye istek atılıyor
                var mobilUserLoginFormPrepareRequestDto = new MobilUserLoginFormPrepareRequestDto();
                mobilUserLoginFormPrepareRequestDto.Identifier = _utilService.DecryptMd5(JsonConvert.SerializeObject(mobilLoginFormRequest));
                mobilUserLoginFormPrepareRequestDto.MobilDeviceContentGuid = mobilDeviceContent.GuiId;
                mobilUserLoginFormPrepareRequestDto.IpAddress = _customHttpUtilService.GetHttpContext().IpAddress;
                var mobilUserLoginFormPrepareResponse = await _mobilUserClientService.MobilUserLoginFormPrepare(mobilUserLoginFormPrepareRequestDto);
                #endregion
                if (mobilUserLoginFormPrepareResponse.ResultType == ResultType.Ok)
                {
                    var mobilLoginFormResponse = _mapper.Map<MobilLoginFormResponseDto>(mobilUserLoginFormPrepareResponse.Data);
                    return new SuccessResult<MobilLoginFormResponseDto>(mobilLoginFormResponse);
                }
                else
                {
                    return new ErrorResult<MobilLoginFormResponseDto>(new MobilLoginFormResponseDto() { }, MobilLoginErrorEnum.MOBIL_LOGIN_FORM_CAN_NOT_CREATED);
                }
            }
            else
            {
                return new ErrorResult<MobilLoginFormResponseDto>(new MobilLoginFormResponseDto() { }, MobilLoginErrorEnum.MOBIL_DEVICE_CONTENT_CAN_NOT_FOUND);
            }
        }
        /// <summary>
        /// Giriş işlemi için kullanıcı kontrolü yapılıyor
        /// </summary>
        /// <param name="mobilLoginRequest"></param>
        /// <returns></returns>
        public async Task<Result<MobilLoginResponseDto>> LoginCheck(MobilLoginRequestDto mobilLoginRequest)
        {
            #region eğer android veya ios test yayıncısı ise kontrol ediliyor
            #region request dto oluşturuluyor
            MobilLoginForTestPublishersDto mobilLoginForTestPublishers = _mapper.Map<MobilLoginForTestPublishersDto>(mobilLoginRequest);
            #endregion
            var publisherLoginState = await LoginPublishers(mobilLoginForTestPublishers);
            if (!publisherLoginState)
            {
                return new ErrorResult<MobilLoginResponseDto>(new MobilLoginResponseDto() { }, MobilLoginErrorEnum.USER_CAN_NOT_FOUND);
            }
            #endregion
            mobilLoginRequest.Password = _utilService.DecryptMd5(mobilLoginRequest.Password);
            var userFilter = _mapper.Map<UserFilterDto>(mobilLoginRequest);
            userFilter.IsActive = true;
            var userEntity = await _userRepository.GetOnlyUser(userFilter).FirstOrDefaultAsync();
            if (userEntity != null)
            {
                #region notification apiye istek atılarak mobil connection verisi getiriliyor
                #region istek için request dto oluşturuluyor
                GetMobilConnectionRequestDto getMobilConnectionRequestDto = new GetMobilConnectionRequestDto();
                getMobilConnectionRequestDto.MobilUserGuiId = userEntity.MobilUserGuiId;
                #endregion
                #region notification api ye istek atılıyor
                var getMobilConnectionResponse = await _mobilConnectionClientService.GetMobilConnection(getMobilConnectionRequestDto);
                #endregion
                #endregion
                #region mobil user için tocken api ye istek atılıyor
                var loginCheckMobilUserRequest = new LoginCheckMobilUserRequestDto();
                loginCheckMobilUserRequest.MobilUserGuiId = userEntity.MobilUserGuiId;
                loginCheckMobilUserRequest.Password = mobilLoginRequest.Password;
                loginCheckMobilUserRequest.Phone = mobilLoginRequest.Phone;
                loginCheckMobilUserRequest.PhoneCountryCode = mobilLoginRequest.PhoneCountryCode;
                loginCheckMobilUserRequest.LoginFormKey = mobilLoginRequest.LoginFormKey;
                loginCheckMobilUserRequest.UserId = userEntity.Id;
                loginCheckMobilUserRequest.ConnectionId = getMobilConnectionResponse.Data != null ? getMobilConnectionResponse.Data.ConnectionId : "";
                var loginCheckMobilUserResponse = await _mobilUserClientService.MobilUserLoginCheck(loginCheckMobilUserRequest);
                #endregion
                var mobilLoginResponse = _mapper.Map<MobilLoginResponseDto>(loginCheckMobilUserResponse.Data);
                if (loginCheckMobilUserResponse.ResultType == ResultType.Ok)
                {
                    #region signalr için mobil connection id dönülüyor
                    mobilLoginResponse.ConnectionId = getMobilConnectionResponse.Data != null ? getMobilConnectionResponse.Data.ConnectionId : "";
                    #endregion
                    #region mobil device content getiriliyor
                    #region filter dto oluşturuluyor
                    MobilDeviceContentFilterDto mobilDeviceContentFilter = new MobilDeviceContentFilterDto();
                    mobilDeviceContentFilter.DeviceTocken = mobilLoginRequest.DeviceTocken;
                    #endregion
                    MobilDeviceContent mobilDeviceContent = await _mobilDeviceContentRepository.GetMobilDeviceContentAsNoTracking(mobilDeviceContentFilter).FirstOrDefaultAsync();
                    #endregion
                    #region kullanıcıya ait user current device verisi kontrol ediliyor, yoksa oluşturuluyor, varsa güncelleniyor
                    if (mobilDeviceContent != null)
                    {
                        #region filter dto oluşturuluyor
                        UserCurrentDeviceFilterDto userCurrentDeviceFilter = new UserCurrentDeviceFilterDto();
                        userCurrentDeviceFilter.MobilDeviceContentId = mobilDeviceContent.Id;
                        userCurrentDeviceFilter.UserId = userEntity.Id;
                        #endregion
                        var userCurrentDevice = await _userCurrentDeviceRepository.GetUserCurrentDevice(userCurrentDeviceFilter).FirstOrDefaultAsync();
                        #region user current device tablosuna kayıt yapılıyor
                        if (userCurrentDevice == null)
                        {
                            userCurrentDevice = new UserCurrentDevice();
                            userCurrentDevice.DeviceType = UserCurrentDeviceTypeEnum.MOBIL;
                            userCurrentDevice.State = UserCurrentDeviceStateEnum.ACTIVE;
                            userCurrentDevice.LastLoginDate = DateTime.Now;
                            userCurrentDevice.UserId = userEntity.Id;
                            userCurrentDevice.UserType = UserCurrentDeviceUserTypeEnum.USER;
                            userCurrentDevice.MobilDeviceContentId = mobilDeviceContent.Id;
                            await _userCurrentDeviceRepository.InsertAsync(userCurrentDevice);
                        }
                        #endregion
                        #region user current device güncelleniyor
                        else
                        {
                            _userCurrentDeviceRepository.UpdateWithProperties(userCurrentDevice, new Expression<Func<UserCurrentDevice, object>>[] {
                            s => s.LastLoginDate
                        });
                            userCurrentDevice.LastLoginDate = DateTime.Now;
                        }
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _userCurrentDeviceRepository.SaveChangesAsync();
                        #endregion
                    }
                    #endregion
                    #region response dto setleniyor
                    mobilLoginResponse.UserName = userEntity.Name;
                    mobilLoginResponse.UserSurname = userEntity.Surname;
                    #endregion
                    return new SuccessResult<MobilLoginResponseDto>(mobilLoginResponse);
                }
                else
                {
                    if (loginCheckMobilUserResponse.ErrorCode == (int)MobilUserTockenErrorEnum.MOBIL_USER_CAN_NOT_FOUND)
                    {
                        return new ErrorResult<MobilLoginResponseDto>(mobilLoginResponse, MobilLoginErrorEnum.USER_CAN_NOT_FOUND);
                    }
                    else if (loginCheckMobilUserResponse.ErrorCode == (int)MobilUserTockenErrorEnum.LOGIN_TIME_ERROR)
                    {
                        return new ErrorResult<MobilLoginResponseDto>(mobilLoginResponse, MobilLoginErrorEnum.LOGIN_TIME_ERROR);

                    }
                    else if (loginCheckMobilUserResponse.ErrorCode == (int)MobilUserTockenErrorEnum.LOGIN_FORM_CAN_NOT_FOUND)
                    {
                        return new ErrorResult<MobilLoginResponseDto>(mobilLoginResponse, MobilLoginErrorEnum.LOGIN_FORM_CAN_NOT_FOUND);
                    }
                    else
                    {
                        return new ErrorResult<MobilLoginResponseDto>(mobilLoginResponse, MobilLoginErrorEnum.USER_CAN_NOT_FOUND);
                    }
                }
            }
            else
            {
                return new ErrorResult<MobilLoginResponseDto>(new MobilLoginResponseDto() { }, MobilLoginErrorEnum.USER_CAN_NOT_FOUND);

            }
        }
        /// <summary>
        /// Misafir girişi gerçekleşiyor
        /// </summary>
        /// <param name="mobilGuestLoginRequest"></param>
        /// <returns></returns>
        public async Task<Result<MobilGuestLoginResponseDto>> GuestLogin(MobilGuestLoginRequestDto mobilGuestLoginRequest)
        {
            #region mobil device content e eşsiz olacak şekilde guest ekleniyor/güncelleniyor
            #region mobil device content çekiliyor
            #region filter dto oluşturuluyor
            MobilDeviceContentFilterDto mobilDeviceContentFilter = _mapper.Map<MobilDeviceContentFilterDto>(mobilGuestLoginRequest);
            #endregion
            var mobilDeviceContent = await _mobilDeviceContentRepository.GetMobilDeviceContentAsNoTracking(mobilDeviceContentFilter).FirstOrDefaultAsync();
            #endregion
            if (mobilDeviceContent != null)
            {
                #region misafir kullanıcı cihaza kayıtlı değilse kayıt ediliyor
                if (mobilDeviceContent.GuestUser == null)
                {
                    mobilDeviceContent.GuestUser = new GuestUser();
                    mobilDeviceContent.GuestUser.MobilDeviceContentId = mobilDeviceContent.Id;
                    mobilDeviceContent.GuestUser.GuiId = Guid.NewGuid() + "";
                    var insertedGuestUser = await _guestUserRepository.InsertAsync(mobilDeviceContent.GuestUser);
                    await _guestUserRepository.SaveChangesAsync();
                    #region guest user verisine id setleniyor
                    mobilDeviceContent.GuestUser.Id = insertedGuestUser.Id;
                    #endregion
                }
                #endregion
                
                #region tocken api ye istek atılarak cihaza göre eşsiz olan misafir kullanıcısı kayıdı/güncellemesi yapılıyor
                #region request dto oluşturuluyor
                MobilGuestLoginProcessRequestDto mobilGuestLoginProcessRequest = new MobilGuestLoginProcessRequestDto();
                mobilGuestLoginProcessRequest.Identifier = _utilService.DecryptMd5(JsonConvert.SerializeObject(mobilGuestLoginRequest));
                mobilGuestLoginProcessRequest.IpAddress = _customHttpUtilService.GetHttpContext().IpAddress;
                mobilGuestLoginProcessRequest.GuestUserGuid = mobilDeviceContent.GuestUser.GuiId;
                mobilGuestLoginProcessRequest.GuestUserId = mobilDeviceContent.GuestUser.Id;
                #endregion
                var mobilGuestLoginProcessResponse = await _mobilUserClientService.MobilGuestLoginProcess(mobilGuestLoginProcessRequest);
                #endregion
                if (mobilGuestLoginProcessResponse.ResultType == ResultType.Ok)
                {
                    MobilGuestLoginResponseDto mobilGuestLoginResponse = _mapper.Map<MobilGuestLoginResponseDto>(mobilGuestLoginProcessResponse.Data);
                    #region misafire ait user current device verisi kontrol ediliyor, yoksa oluşturuluyor, varsa güncelleniyor
                    #region filter dto oluşturuluyor
                    UserCurrentDeviceFilterDto userCurrentDeviceFilter = new UserCurrentDeviceFilterDto();
                    userCurrentDeviceFilter.MobilDeviceContentId = mobilDeviceContent.Id;
                    userCurrentDeviceFilter.GuestId = mobilDeviceContent.GuestUser.Id;
                    #endregion
                    var userCurrentDevice = await _userCurrentDeviceRepository.GetUserCurrentDevice(userCurrentDeviceFilter).FirstOrDefaultAsync();
                    #region user current device tablosuna kayıt yapılıyor
                    if (userCurrentDevice == null)
                    {
                        userCurrentDevice = new UserCurrentDevice();
                        userCurrentDevice.DeviceType = UserCurrentDeviceTypeEnum.MOBIL;
                        userCurrentDevice.State = UserCurrentDeviceStateEnum.ACTIVE;
                        userCurrentDevice.LastLoginDate = DateTime.Now;
                        userCurrentDevice.GuestId = mobilDeviceContent.GuestUser.Id;
                        userCurrentDevice.UserType = UserCurrentDeviceUserTypeEnum.GUEST;
                        userCurrentDevice.MobilDeviceContentId = mobilDeviceContent.Id;
                        await _userCurrentDeviceRepository.InsertAsync(userCurrentDevice);
                    }
                    #endregion
                    #region user current device güncelleniyor
                    else
                    {
                        _userCurrentDeviceRepository.UpdateWithProperties(userCurrentDevice, new Expression<Func<UserCurrentDevice, object>>[] {
                            s => s.LastLoginDate
                        });
                        userCurrentDevice.LastLoginDate = DateTime.Now;
                    }
                    #endregion
                    #region veritabanına kayıt ediliyor
                    await _userCurrentDeviceRepository.SaveChangesAsync();
                    #endregion
                    #endregion
                    return new SuccessResult<MobilGuestLoginResponseDto>(mobilGuestLoginResponse);
                }
                else
                {
                    if (mobilGuestLoginProcessResponse.ErrorCode == (int)MobilGuestTockenErrorEnum.DEVICE_CAN_NOT_FOUND)
                    {
                        return new ErrorResult<MobilGuestLoginResponseDto>(new MobilGuestLoginResponseDto() { }, MobilGuestLoginErrorEnum.DEVICE_CAN_NOT_FOUND);
                    }
                    else
                    {
                        return new ErrorResult<MobilGuestLoginResponseDto>(new MobilGuestLoginResponseDto() { }, MobilGuestLoginErrorEnum.AN_ERROR_OCCURRED);
                    }
                }

            }
            #endregion
            #region mobil device content bulunamadı
            else
            {
                return new ErrorResult<MobilGuestLoginResponseDto>(new MobilGuestLoginResponseDto() { }, MobilGuestLoginErrorEnum.DEVICE_CONTENT_CAN_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// Çıkış İşlemi Yapılıyor
        /// </summary>
        /// <returns></returns>
        public async Task<Result<MobilLogOutResponseDto>> LogOut()
        {
            #region çıkış işlemi için tocken api ye istek atılıyor
            MobilUserLogOutRequestDto mobilUserLogOut = new MobilUserLogOutRequestDto();
            mobilUserLogOut.DeviceGuiId = _userContextProvider.DeviceGuiId;
            mobilUserLogOut.DeviceIdentifier = _userContextProvider.DeviceIdentifier;
            mobilUserLogOut.LoginSessionId = _userContextProvider.LoginSessionId;
            mobilUserLogOut.LoginSessionJwtKey = _userContextProvider.LoginSessionJwtKey;
            mobilUserLogOut.LoginSessionKey = _userContextProvider.LoginSessionKey;
            mobilUserLogOut.MobilUserGuiId = _userContextProvider.MobilUserGuiId;
            mobilUserLogOut.MobilUserId = _userContextProvider.MobilUserId.GetValueOrDefault();
            mobilUserLogOut.RememberKey = _userContextProvider.RememberKey;
            mobilUserLogOut.UserId = _userContextProvider.UserId.GetValueOrDefault();
            var logOutResponse = await _mobilUserClientService.MobilUserLogOut(mobilUserLogOut);
            #endregion
            if (logOutResponse.ResultType == ResultType.Ok)
            {
                return logOutResponse;
            }
            else
            {
                return new ErrorResult<MobilLogOutResponseDto>(new MobilLogOutResponseDto() { State = false }, MobilLogOutErrorEnum.LOG_OUT_FAILED);

            }
        }
        #region private methods
        /// <summary>
        /// Android ve Ios yayıncısı için test girişi
        /// </summary>
        /// <param name="mobilLoginForTestPublishers"></param>
        /// <returns></returns>
        public async Task<bool> LoginPublishers(MobilLoginForTestPublishersDto mobilLoginForTestPublishers)
        {
            #region gelen şifre md5 e çevriliyor
            var md5Password = _utilService.DecryptMd5(mobilLoginForTestPublishers.Password);
            #endregion
            #region giriş bilgileri local bilgilerden kontrol ediliyor
            if ((mobilLoginForTestPublishers.Phone == _configuration.GetSection("PublishTestUserAndroid:Phone").Value && _configuration.GetSection("PublishTestUserAndroid:PasswordMd5").Value == md5Password) ||
                (mobilLoginForTestPublishers.Phone == _configuration.GetSection("PublishTestUserIos:Phone").Value && _configuration.GetSection("PublishTestUserIos:PasswordMd5").Value == md5Password))
            {
                #region telefona kayıtlı kullanıcı var mı kontrol ediliyor
                var userFilter = _mapper.Map<UserFilterDto>(mobilLoginForTestPublishers);
                userFilter.IsActive = true;
                var userEntityCheck = await _userRepository.GetOnlyUser(userFilter).AnyAsync();
                #region kayıtlı kullanıcı mevcut
                if (userEntityCheck)
                {
                    return true;
                }
                #endregion
                #region bilgiler ile kayıt yapılıyor
                else
                {
                    mobilLoginForTestPublishers.Password = _utilService.DecryptMd5(mobilLoginForTestPublishers.Password);
                    #region tocken apiye istek atılarak user mobil ve general user ekleniyor
                    var mobilUserInsertRequest = _mapper.Map<MobilUserInsertRequestDto>(mobilLoginForTestPublishers);
                    var mobilUserInsertResponse = await _mobilUserClientService.AddMobilUser(mobilUserInsertRequest);
                    #endregion
                    if (mobilUserInsertResponse.ResultType == ResultType.Ok)
                    {
                        var userCheck = await _userRepository.GetUserAsNoTracking(new UserMobilFilterDto() { MobilUserGuiId = mobilUserInsertResponse.Data.MobilUserGuiId, Phone = mobilLoginForTestPublishers.Phone }).AnyAsync();
                        if (!userCheck)
                        {
                            #region user verisi ekleniyor
                            var userEntity = _mapper.Map<User>(mobilLoginForTestPublishers, opt =>
                            {
                                opt.AfterMap((src, dest) =>
                                {
                                    var destData = dest as User;
                                    destData.TcNumber = mobilLoginForTestPublishers.Phone;
                                });
                            });
                            userEntity.MobilUserGuiId = mobilUserInsertResponse.Data.MobilUserGuiId;
                            userEntity.IsActive = true;
                            await _userRepository.InsertAsync(userEntity);
                            #endregion
                            #region notification apiye istek atılarak mobil kullanıcı için mobil connection verisi ekleniyor
                            MobilConnectionInsertOrUpdateRequestDto mobilConnectionInsertOrUpdateRequest = new MobilConnectionInsertOrUpdateRequestDto();
                            mobilConnectionInsertOrUpdateRequest.ConnectionId = _utilService.GetRandomString(64);
                            mobilConnectionInsertOrUpdateRequest.MobilUSerGuiId = mobilUserInsertResponse.Data.MobilUserGuiId;
                            var mobilConnectionInsertOrUpdateResponse = await _mobilConnectionClientService.InsertOrUpdateMobilConnection(mobilConnectionInsertOrUpdateRequest);
                            #endregion
                            if (mobilConnectionInsertOrUpdateResponse.ResultType == ResultType.Ok)
                            {
                                await _userRepository.SaveChangesAsync();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                #endregion

                #endregion

            }
            #endregion
            else
            {
                return true;
            }
        }
        #endregion
    }
}

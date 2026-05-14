using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.MobilUserProfileDtos;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.ApiDto.PanelUserDtos;
using Shared.Domain.Dto.ApiDto.UserDtos;
using Shared.Domain.Dto.MailSmsDto.UserRemoveSmsDtos;
using Shared.Domain.Entities.ApiEntities.UserModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Errors.MailSmsErrors;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.Errors.WebPanelErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.TockenApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.UserManagmentServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Services.PanelServices.UserManagment
{
    public class UserManagmentService : BaseService, IUserManagmentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly IMobilUserClientService _mobilUserClientService;
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;

        public UserManagmentService(IMapper mapper,
                            IConfiguration configuration,
                            IUtilService utilService,
                           IUserRepository userRepository, IMobilUserClientService mobilUserClientService, 
                           IChargeRepository chargeRepository) : base(
                           mapper
                               )
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _utilService = utilService;
            _mobilUserClientService = mobilUserClientService;
            _chargeRepository = chargeRepository;
        }

        #region ekleme,güncelleme,silme

        #endregion
        /// <summary>
        /// kullanıcı aktifliği güncelleniyor
        /// </summary>
        /// <param name="panelChangeUserStateRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelChangeUserStateResponseDto>> ChangeUserState(PanelChangeUserStateRequestDto panelChangeUserStateRequest)
        {
            #region response dto oluşturuluyor
            PanelChangeUserStateResponseDto panelChangeUserStateResponse = new PanelChangeUserStateResponseDto();
            #endregion
            #region kullanıcı getiriliyor
            #region filter dto oluşturuluyor
            UserFilterDto userFilter = _mapper.Map<UserFilterDto>(panelChangeUserStateRequest);
            #endregion
            var userEntity = await _userRepository.GetOnlyUser(userFilter).FirstOrDefaultAsync();
            #endregion
            if (userEntity != null)
            {
                #region kullanıcı durumu güncelleniyor
                _userRepository.UpdateWithProperties(userEntity, new Expression<Func<User, object>>[] {
                    s => s.IsActive
                });
                userEntity.IsActive = !userEntity.IsActive;
                #endregion
                #region veritabanına kayıt ediliyor
                await _userRepository.SaveChangesAsync();
                #endregion
            }
            return new SuccessResult<PanelChangeUserStateResponseDto>(panelChangeUserStateResponse);
        }
        /// <summary>
        /// panel için user verileri çekiliyor
        /// </summary>
        /// <param name="dataTableFilterModel"></param>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<PanelUserListItemDto>>> GetUserDataTablePanel(DataTableFilterModel<GetPanelUserDataTableRequestDto> dataTableFilterModel)
        {
            #region kullanıcı sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region kullanıcılar getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            #region filter dto oluşturuluyor
            UserFilterDto userFilter = _mapper.Map<UserFilterDto>(dataTableFilterModel.data);
            #endregion
            var users = _userRepository.GetOnlyUser(userFilter);
            var resultUser= users.ApplySorting(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective).AsNoTracking();
            #endregion
            #region response dto setleniyor
            toplamKayit = resultUser.Count();
            var userList = await resultUser.Skip(ofset).Take(recordPerPage).ToListAsync();
            var userListDto = _mapper.Map<List<PanelUserListItemDto>>(userList);
            var result = new DataTableResponseWrapper<PanelUserListItemDto>(toplamKayit, userListDto);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<PanelUserListItemDto>>(result);
        }
        /// <summary>
        /// panel için user verisi çekiliyor
        /// </summary>
        /// <param name="getPanelUserRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelUserResponseDto>> GetUser(GetPanelUserRequestDto getPanelUserRequest)
        {
            #region response dto oluşturuluyor
            GetPanelUserResponseDto getPanelUserResponse = new GetPanelUserResponseDto();
            #endregion
            #region kullanıcı çekiliyor
            #region filter dto oluşturuluyor
            UserFilterDto userFilter = _mapper.Map<UserFilterDto>(getPanelUserRequest);
            #endregion
            var userEntity = await _userRepository.GetOnlyUserAsNoTracking(userFilter).FirstOrDefaultAsync();
            #endregion
            #region response dto setleniyor
            getPanelUserResponse = _mapper.Map<GetPanelUserResponseDto>(userEntity);
            #endregion
            return new SuccessResult<GetPanelUserResponseDto>(getPanelUserResponse);
        }
        /// <summary>
        /// kullanıcı seçimi için kullanıcılar getiriliyor
        /// </summary>
        /// <param name="getUserForSelectListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetUserForSelectListResponseDto>> GetUsersForSelectList(GetUserForSelectListRequestDto getUserForSelectListRequest)
        {
            #region response dto oluşturuluyor
            GetUserForSelectListResponseDto getUserForSelectListResponse = new GetUserForSelectListResponseDto();
            getUserForSelectListResponse.UserList = new List<PanelUserSelectListItemDto>();
            #endregion
            #region kullanıcılar çekiliyor
            #region filter dto oluşturuluyor
            var userFilter = _mapper.Map<UserFilterDto>(getUserForSelectListRequest);
            #endregion
            var userEntityList = await _userRepository.GetOnlyUserAsNoTracking(userFilter).ToListAsync();
            #endregion
            #region response dto setleniyor
            getUserForSelectListResponse.UserList = _mapper.Map<List<PanelUserSelectListItemDto>>(userEntityList);
            #endregion
            return new SuccessResult<GetUserForSelectListResponseDto>(getUserForSelectListResponse);
        }
        /// <summary>
        /// Mobil kullanıcı kaldırılıyor
        /// </summary>
        /// <param name="panelRemoveUserRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelRemoveUserResponseDto>> RemoveUser(PanelRemoveUserRequestDto panelRemoveUserRequest)
        {
            #region response dto oluşturuluyor
            PanelRemoveUserResponseDto panelRemoveUserResponse = new PanelRemoveUserResponseDto();
            #endregion
            #region kullanıcı getiriliyor
            #region filter dto oluşturuluyor
            UserMobilFilterDto userFilter = _mapper.Map<UserMobilFilterDto>(panelRemoveUserRequest);
            #endregion
            #endregion
            var user = await _userRepository.GetUser(userFilter, null).FirstOrDefaultAsync();
            if (user != null)
            {
                #region ödeme yapılmamış işlem varsa kontrol ediliyor
                #region filter dto oluşturuluyor
                ChargeFilterDto chargeFilter = new ChargeFilterDto();
                chargeFilter.UserId = user.Id;
                chargeFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_FAIL, ChargeStateEnum.PAYMENT_BEING_RECEIVED };
                #endregion
                var unPaidCharge = await _chargeRepository.GetCharge(chargeFilter, null).AnyAsync();
                #endregion
                #region ödeme yapılmamış işlem mevcut
                if (unPaidCharge)
                {
                    return new ErrorResult<PanelRemoveUserResponseDto>(panelRemoveUserResponse, UserManagmentErrorEnum.UNPAID_CHARGE_PROCESS_EXIST);
                }
                #endregion
                #region kullanıcı kaldırılıyor
                _userRepository.UpdateWithPropertiesForProperty(user, new Expression<Func<User, object>>[] {
                    s => s.Deleted,
                    s => s.DeletedDate,
                });
                user.Deleted = true;
                user.DeletedDate = DateTime.Now;
                await _userRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<PanelRemoveUserResponseDto>(panelRemoveUserResponse);
            }
            else
            {
                return new ErrorResult<PanelRemoveUserResponseDto>(panelRemoveUserResponse, UserManagmentErrorEnum.USER_CAN_NOT_FOUND);
            }
        }
    }
}

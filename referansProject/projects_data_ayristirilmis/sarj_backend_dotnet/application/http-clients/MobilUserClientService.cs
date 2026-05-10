// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\HttpClients\HttpClientServices\TockenApiServices\MobilUserClientService.cs
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.TockenDto.ChangePasswordDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestLoginSessionDtos;
using Shared.Domain.Dto.TockenDto.MobilUserDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginFormDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginSessionDtos;
using Shared.Domain.HttpClients.HttpClientInterfaces.TockenApiInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.HttpClients.HttpClientServices.TockenApiServices
{
    public class MobilUserClientService : IMobilUserClientService
    {
        private readonly IGenericHttpClientService _genericHttpClientService;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        public MobilUserClientService(IGenericHttpClientService genericHttpClientService,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _genericHttpClientService = genericHttpClientService;
            _baseUrl = _configuration.GetSection("TockenApi:Url").Value;
        }

        public async Task<Result<MobilUserInsertResponseDto>> AddMobilUser(MobilUserInsertRequestDto mobilUserInsertRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilUserInsertResponseDto>>(mobilUserInsertRequest, _baseUrl, "MobilUser/AddMobilUser", null);
            return response;
        }

        public async Task<Result<MobilUserChangePasswordResponseDto>> ChangePassword(MobilUserChangePasswordRequestDto mobilUserChangePasswordRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilUserChangePasswordResponseDto>>(mobilUserChangePasswordRequest, _baseUrl, "MobilUser/ChangePassword", null);
            return response;
        }

        public async Task<Result<MobilUserChangePasswordVerifyInsertResponseDto>> ChangePasswordVerifyInsert(MobilUserChangePasswordVerifyInsertRequestDto mobilUserChangePasswordVerifyInsertRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilUserChangePasswordVerifyInsertResponseDto>>(mobilUserChangePasswordVerifyInsertRequest, _baseUrl, "MobilUser/ChangePasswordVerifyInsert", null);
            return response;
        }

        public async Task<Result<CheckMobilUserResponseDto>> CheckMobilUser(CheckMobilUserRequestDto checkMobilUserRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<CheckMobilUserResponseDto>>(checkMobilUserRequest, _baseUrl, "MobilUser/CheckMobilUser", null);
            return response;
        }

        public async Task<Result<MobilGuestLoginProcessResponseDto>> MobilGuestLoginProcess(MobilGuestLoginProcessRequestDto mobilGuestLoginProcessRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilGuestLoginProcessResponseDto>>(mobilGuestLoginProcessRequest, _baseUrl, "MobilUser/MobilGuestLoginProcess", null);
            return response;
        }

        public async Task<Result<MobilGuestLoginSessionCheckResponseDto>> MobilGuestLoginSessionCheck(MobilGuestLoginSessionCheckRequestDto mobilGuestLoginSessionCheckRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilGuestLoginSessionCheckResponseDto>>(mobilGuestLoginSessionCheckRequest, _baseUrl, "MobilUser/MobilGuestLoginSessionCheck", null);
            return response;
        }

        public async Task<Result<LoginCheckMobilUserResponseDto>> MobilUserLoginCheck(LoginCheckMobilUserRequestDto loginCheckMobilUserRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<LoginCheckMobilUserResponseDto>>(loginCheckMobilUserRequest, _baseUrl, "MobilUser/MobilUserLoginCheck", null);
            return response;
        }

        public async Task<Result<MobilUserLoginFormPrepareResponseDto>> MobilUserLoginFormPrepare(MobilUserLoginFormPrepareRequestDto mobilUserLoginFormPrepareRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilUserLoginFormPrepareResponseDto>>(mobilUserLoginFormPrepareRequest, _baseUrl, "MobilUser/MobilUserLoginFormPrepare", null);
            return response;
        }

        public async Task<Result<MobilUserLoginSessionCheckResponseDto>> MobilUserLoginSessionCheck(MobilUserLoginSessionCheckRequestDto loginCheckMobilUserRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilUserLoginSessionCheckResponseDto>>(loginCheckMobilUserRequest, _baseUrl, "MobilUser/MobilUserLoginSessionCheck", null);
            return response;
        }

        public async Task<Result<MobilUserLoginSessionUpdateResponseDto>> MobilUserLoginSessionUpdate(MobilUserLoginSessionUpdateRequestDto mobilUserLoginSessionUpdateRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilUserLoginSessionUpdateResponseDto>>(mobilUserLoginSessionUpdateRequest, _baseUrl, "MobilUser/MobilUserLoginSessionUpdate", null);
            return response;
        }

        public async Task<Result<MobilLogOutResponseDto>> MobilUserLogOut(MobilUserLogOutRequestDto mobilUserLogOutRequest)
        {
            var response = await _genericHttpClientService.PostJson<Result<MobilLogOutResponseDto>>(mobilUserLogOutRequest, _baseUrl, "MobilUser/MobilUserLogOut", null);
            return response;
        }
    }
}

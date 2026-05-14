// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\HttpClients\HttpClientInterfaces\TockenApiInterfaces\IMobilUserClientService.cs
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.Dto.TockenDto.ChangePasswordDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestLoginSessionDtos;
using Shared.Domain.Dto.TockenDto.MobilUserDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginFormDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginSessionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.HttpClients.HttpClientInterfaces.TockenApiInterfaces
{
    public interface IMobilUserClientService
    {
        Task<Result<MobilUserInsertResponseDto>> AddMobilUser(MobilUserInsertRequestDto mobilUserInsertRequest);
        Task<Result<CheckMobilUserResponseDto>> CheckMobilUser(CheckMobilUserRequestDto checkMobilUserRequest);
        Task<Result<MobilUserChangePasswordResponseDto>> ChangePassword(MobilUserChangePasswordRequestDto mobilUserChangePasswordRequest);
        Task<Result<LoginCheckMobilUserResponseDto>> MobilUserLoginCheck(LoginCheckMobilUserRequestDto loginCheckMobilUserRequest);
        Task<Result<MobilUserLoginFormPrepareResponseDto>> MobilUserLoginFormPrepare(MobilUserLoginFormPrepareRequestDto mobilUserLoginFormPrepareRequest);
        Task<Result<MobilUserChangePasswordVerifyInsertResponseDto>> ChangePasswordVerifyInsert(MobilUserChangePasswordVerifyInsertRequestDto mobilUserChangePasswordVerifyInsertRequest);
        Task<Result<MobilUserLoginSessionCheckResponseDto>> MobilUserLoginSessionCheck(MobilUserLoginSessionCheckRequestDto loginCheckMobilUserRequest);
        Task<Result<MobilUserLoginSessionUpdateResponseDto>> MobilUserLoginSessionUpdate(MobilUserLoginSessionUpdateRequestDto mobilUserLoginSessionUpdateRequest);
        Task<Result<MobilLogOutResponseDto>> MobilUserLogOut(MobilUserLogOutRequestDto mobilUserLogOutRequest);
        Task<Result<MobilGuestLoginProcessResponseDto>> MobilGuestLoginProcess(MobilGuestLoginProcessRequestDto mobilGuestLoginProcessRequest);
        Task<Result<MobilGuestLoginSessionCheckResponseDto>> MobilGuestLoginSessionCheck(MobilGuestLoginSessionCheckRequestDto mobilGuestLoginSessionCheckRequest);
    }
}

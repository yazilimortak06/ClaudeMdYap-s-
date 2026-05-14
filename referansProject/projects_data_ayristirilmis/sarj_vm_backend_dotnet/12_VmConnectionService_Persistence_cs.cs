// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Core\Applications\Vm.Application\Services\ConnectionManagement\VmConnectionService.Persistence.cs
// VmConnectionService - PARTIAL: Veritabanı kayıt ve güncelleme metodları

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.VmDto.VmConnectionDtos;
using Shared.Domain.Dto.VmDto.VmCpoConnectionDtos;
using Shared.Domain.Dto.VmDto.VmDeviceConnectionDtos;
using Shared.Domain.Dto.VmDto.VmServerConnectionDtos;
using Shared.Domain.Entities.VmEntities.VmCommandMessageModule;
using Shared.Domain.Enums.VmEnums;
using Shared.Domain.RepositoryInterfaces.VmRepositoryInterfaces.VmCommandMessageRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.ConnectionManagementsServiceInterfaces;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.DeviceManagementServiceInterfaces;
using System;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.Extentions;

namespace Vm.Application.Services.ConnectionManagement
{
    public partial class VmConnectionService
    {
        private async Task SaveVmCommandMessage(long? vmDeviceConnectionId, string identifier)
        {
            VmCommandMessage vmCommandMessage = new VmCommandMessage();
            vmCommandMessage.GuiId = Guid.NewGuid().ToString();
            vmCommandMessage.VmDeviceConnectionId = vmDeviceConnectionId;
            vmCommandMessage.Data = identifier + " connected successfully";
            vmCommandMessage.SenderType = VmCommandMessageTypeEnum.DEVICE;
            vmCommandMessage.ReceivedType = VmCommandMessageTypeEnum.VM;
            vmCommandMessage.CreatedDate = DateTime.UtcNow;
            vmCommandMessage.MessageDescription = VmCommandMessageTopicEnum.CONNECTED.ToDescriptionString();
            vmCommandMessage.Topic = VmCommandMessageTopicEnum.CONNECTED;
            vmCommandMessage.SenderIpAddress = _customHttpUtilService.GetHttpContext().IpAddress;
            using (var scope = _services.CreateScope())
            {
                IVmCommandMessageRepository vmCommandMessageRepository = scope.ServiceProvider.GetRequiredService<IVmCommandMessageRepository>();
                await vmCommandMessageRepository.InsertAsync(vmCommandMessage);
                await vmCommandMessageRepository.SaveChangesAsync();
            }
        }

        // NOT: Aşağıdaki metodlar şu an TODO — ileride implement edilecek
        private Task SaveDeviceCommandMessageDb(VmConnectionSessionDto vmConnectionSession, string messageType, string messageId, string payload, string action)
        {
            _logger.LogDebug($"[DB-TODO] SaveDeviceCommandMessageDb not implemented. Device={vmConnectionSession?.Identifier}, Action={action}, MessageId={messageId}");
            return Task.CompletedTask;
        }

        private Task SaveDeviceCommandCalculatedResponseMessageDb(VmConnectionSessionDto vmConnectionSession, string messageType, string messageId, string payload, string action)
        {
            _logger.LogDebug($"[DB-TODO] SaveDeviceCommandCalculatedResponseMessageDb not implemented. Device={vmConnectionSession?.Identifier}");
            return Task.CompletedTask;
        }

        private Task SaveCpoTriggerMessageDb(VmConnectionSessionDto vmConnectionSession, string messageType, string messageId, string payload, string action)
        {
            _logger.LogDebug($"[DB-TODO] SaveCpoTriggerMessageDb not implemented. Device={vmConnectionSession?.Identifier}");
            return Task.CompletedTask;
        }

        private Task SaveServerTriggerMessageDb(VmConnectionSessionDto vmConnectionSession, string messageType, string messageId, string payload, string action)
        {
            _logger.LogDebug($"[DB-TODO] SaveServerTriggerMessageDb not implemented. Device={vmConnectionSession?.Identifier}");
            return Task.CompletedTask;
        }

        private async Task UpdateCpoConnectionDb(VmConnectionSessionDto vmConnectionSession, bool connectionState)
        {
            UpdateVmCpoConnectionRequestDto updateVmCpoConnectionRequest = new UpdateVmCpoConnectionRequestDto();
            updateVmCpoConnectionRequest.VmDeviceConnectionId = vmConnectionSession.VmDeviceConnectionId;
            updateVmCpoConnectionRequest.Date = DateTime.UtcNow;
            updateVmCpoConnectionRequest.ConnectionState = connectionState;
            updateVmCpoConnectionRequest.ConnectedCpoOcppUrlAddress = vmConnectionSession.CpoOcppUrlAddress;
            using (var scope = _services.CreateScope())
            {
                IVmCpoConnectionManagementService vmCpoConnectionManagementService = scope.ServiceProvider.GetRequiredService<IVmCpoConnectionManagementService>();
                await vmCpoConnectionManagementService.UpdateVmCpoConnection(updateVmCpoConnectionRequest);
            }
        }

        private async Task UpdateServerConnectionDb(VmConnectionSessionDto vmConnectionSession, bool connectionState)
        {
            UpdateVmServerConnectionRequestDto updateVmServerConnectionRequest = new UpdateVmServerConnectionRequestDto();
            updateVmServerConnectionRequest.VmDeviceConnectionId = vmConnectionSession.VmDeviceConnectionId;
            updateVmServerConnectionRequest.Date = DateTime.UtcNow;
            updateVmServerConnectionRequest.ConnectionState = connectionState;
            updateVmServerConnectionRequest.ConnectedServerUrlAddress = vmConnectionSession.ServerOcppUrlAddress;
            using (var scope = _services.CreateScope())
            {
                IVmServerConnectionManagementService vmServerConnectionManagementService = scope.ServiceProvider.GetRequiredService<IVmServerConnectionManagementService>();
                await vmServerConnectionManagementService.UpdateVmServerConnection(updateVmServerConnectionRequest);
            }
        }

        private async Task UpdateDeviceConnectionDb(VmConnectionSessionDto vmConnectionSession, bool connectionState, ChargeDeviceInstantStateEnum? chargeDeviceInstantState)
        {
            UpdateVmDeviceConnectionRequestDto updateVmDeviceConnectionRequest = new UpdateVmDeviceConnectionRequestDto();
            updateVmDeviceConnectionRequest.Id = vmConnectionSession.VmDeviceConnectionId;
            updateVmDeviceConnectionRequest.Date = DateTime.UtcNow;
            updateVmDeviceConnectionRequest.ConnectionState = connectionState;
            using (var scope = _services.CreateScope())
            {
                IVmDeviceConnectionManagementService vmDeviceConnectionManagementService = scope.ServiceProvider.GetRequiredService<IVmDeviceConnectionManagementService>();
                await vmDeviceConnectionManagementService.UpdateVmDeviceConnection(updateVmDeviceConnectionRequest);
            }
        }
    }
}

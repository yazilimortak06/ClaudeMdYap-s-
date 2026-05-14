using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Dto.ApiDto.ChargeDtos
{
    public class ChargeFilterDto
    {
#nullable enable
        public long? Id { get; set; }
        public List<long>? IdList { get; set; }
        public long? StartTransactionProcessId { get; set; }
        public string? TransactionGuiId { get; set; }
        public long? StationId { get; set; }
        public string? GuiId { get; set; }
        public List<string>? GuiIdList { get; set; }
        public long? ContentLanguageId { get; set; }
        public long? ChargeDeviceConnectorId { get; set; }
        public long? ChargeDeviceId { get; set; }
        public long? ChargeDevicePowerTypeId { get; set; }
        public List<ChargeStateEnum>? ChargeStateList { get; set; }
        public ChargeStateEnum? ChargeState { get; set; }
        public ChargeStateEnum? ExceptChargeState { get; set; }
        public long? UserId { get; set; }
        public bool? Finished { get; set; }
        public long? CompanyId { get; set; }
        public long? FirmId { get; set; }
        public string? ConnectorName { get; set; }
        public string? DeviceName { get; set; }
        public string? SocketMovementGuiId { get; set; }
        public string? DeviceIdentifier { get; set; }
        public int? AutomaticPaymentTrialTimeRange { get; set; }
        public int? AutomaticPaymentFirstTrialTimeRange { get; set; }
        public int? Year { get; set; }
        public ChargeFirmTypeEnum? ChargeFirmType { get; set; }
        public List<long>? SelectedIdList { get; set; }
        public List<long>? UnSelectedIdList { get; set; }
        public bool? AllSelected { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? CountryId { get; set; }
        public long? CityId { get; set; }
        public long? TownId { get; set; }

#nullable disable
    }
}

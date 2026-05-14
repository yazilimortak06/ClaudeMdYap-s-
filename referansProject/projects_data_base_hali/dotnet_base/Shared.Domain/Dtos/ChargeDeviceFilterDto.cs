using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Dto.ApiDto.ChargeDeviceDtos
{
    public class ChargeDeviceFilterDto
    {
#nullable enable
        public long? Id { get; set; }
        public List<long>? IdList { get; set; }
        public string? Name { get; set; }
        public string? GuiId { get; set; }
        public string? Identifier { get; set; }
        public string? IdentifierContain { get; set; }
        public string? StationNameContain { get; set; }
        public string? StationName { get; set; }
        public long? StationId { get; set; }
        public List<long?>? StationIds { get; set; }
        public List<long>? InContains { get; set; }
        public List<long>? ExceptInContains { get; set; }
        public ChargeDeviceStateEnum? DeviceState { get; set; }
        public long? CompanyId { get; set; }
        public long? FirmId { get; set; }
        public ChargeDeviceInstantStateEnum? InstantState { get; set; }
        public bool? ConnectionState { get; set; }
        public int? LastHearthBeatLimitSecond { get; set; }
        public bool? IsTestStation { get; set; }
        public long? ContentLanguageId { get; set; }
        public long? CountryId { get; set; }
        public long? CityId { get; set; }
        public long? TownId { get; set; }
        public long? ChargeDevicePowerTypeId { get; set; }
        public string? FirmName { get; set; }
        public string? CompanyName { get; set; }
        public FirmIntegrationTypeEnum? FirmIntegrationType { get; set; }
#nullable disable
    }
}

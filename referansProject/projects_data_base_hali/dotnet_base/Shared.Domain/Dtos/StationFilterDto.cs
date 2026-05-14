using FrameworkCore.Bases.BaseDtos;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Dto.ApiDto.StationDtos
{
    public class StationFilterDto : BaseDto
    {
#nullable enable
        public long? Id { get; set; }
        public string? StationLocationGuiId { get; set; }
        public List<string>? StationLocationGuiIdList { get; set; }
        public long? ContentLanguageId { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

        public List<long>? SocketTypeId { get; set; }
        public List<long>? PowerTypeId { get; set; }
        public List<long>? PaymentMethod { get; set; }
        public bool? FavoriteStation { get; set; }
        public List<StationFilterStateEnum>? MobilStationFilterState { get; set; }
        public long? UserId { get; set; }
        public long? CompanyId { get; set; }
        public long? FirmId { get; set; }
        public long? FirmDataId { get; set; }
        public bool? IsIntegrated { get; set; }
        public List<long>? StationOpportunity { get; set; }
        public string? SearchValue { get; set; }
        public int? MinKw { get; set; }
        public int? MaxKw { get; set; }
        public int? MaxPrice { get; set; }
        public int? MinPrice { get; set; }
        public int? MinDistance { get; set; }
        public int? MaxDistance { get; set; }
        public int? StationRatingPoint { get; set; }
        public List<long>? FirmIdList { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Latitude { get; set; }
        public string? Longtitude { get; set; }

#nullable disable
    }
}

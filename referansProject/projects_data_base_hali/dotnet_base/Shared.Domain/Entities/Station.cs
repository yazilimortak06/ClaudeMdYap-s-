using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.ChargeDeviceModule;
using Shared.Domain.Entities.ApiEntities.CompanyModule;
using Shared.Domain.Entities.ApiEntities.FirmModule;
using Shared.Domain.Entities.ApiEntities.ReservationModule;
using Shared.Domain.Entities.ApiEntities.RouteProcessModule;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.StationModule
{
    [Table("Station", Schema = "RotaWatt")]
    public class Station : BaseEntity
    {
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get  ; set; }
        public bool IsIntegrated { get; set; }
        public bool IsTestStation { get; set; }
        public StationFirmTypeEnum StationFirmType { get; set; }
        public StationStateEnum State { get; set; }
        public virtual ICollection<StationLocationArea> StationLocationArea { get; set; }
#nullable enable
        [ForeignKey("Station_Company")]
        public long? CompanyId { get; set; }
        public Company? Company { get; set; }
        [ForeignKey("Station_Firm")]
        public long? FirmId { get; set; }
        public Firm? Firm { get; set; }
        public ReservationSuitableDate? ReservationSuitableDate { get; set; }
        public long? FirmDataId { get; set; }
        public string? FirmDataGuiId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public DateTime? FirmIntegrationUpdateDate { get; set; }
        public DateTime? FirmIntegrationCreatedDate { get; set; }

#nullable disable
        public virtual ICollection<StationContent> StationContent { get; set; }
        public virtual ICollection<StationPicture> StationPicture { get; set; }
        public virtual ICollection<ChargeDevice> ChargeDevice { get; set; }
        public virtual ICollection<StationPaymentMethod> StationPaymentMethod { get; set; }
        public virtual ICollection<StationFacility> StationFacility { get; set; } // imkanlar
        public virtual ICollection<StationRating> StationRating { get; set; }
        //public virtual ICollection<RouteStationPlan> RouteStationPlan { get; set; }
        //public virtual ICollection<ChargeDeviceReservation> ChargeDeviceReservation { get; set; }
        //public virtual ICollection<ChargeDeviceReservationTemp> ChargeDeviceReservationTemp { get; set; }
        public virtual ICollection<FavoriteStation> FavoriteStation { get; set; }
        public virtual ICollection<RouteStationProcess> RouteStationProcess { get; set; }
        //public virtual ICollection<StationReservationNotSuitableDates> StationReservationNotSuitableDates { get; set; }
        //public virtual ICollection<RouteLineStationTemp> RouteLineStationTemp { get; set; }
        public StationAddress StationAddress { get; set; }
        public StationInfo StationInfo { get; set; }
    }
}

using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.ChargeDevicePowerTypeModule;
using Shared.Domain.Entities.ApiEntities.ChargeDeviceSocketTypeModule;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.ChargeDeviceModule
{
    [Table("ChargeDeviceConnector", Schema = "RotaWatt")]
    public class ChargeDeviceConnector : BaseEntity
    {
        public string GuiId { get; set; }
        public string Name { get; set; }
        public string SocketNumber { get; set; }
        public string Identifier { get; set; }
        public int ConnectorNo { get; set; }
        public string SerialNumber { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public ChargeDeviceConnectorStateEnum State { get; set; }
        public double Kdv { get; set; }
        public bool IncludeKdv { get; set; }
        public decimal PriceWithKdv { get; set; }
        public decimal Price { get; set; }
        public int kW { get; set; }
        public ChargeDeviceConnectorWaitingPriceStateEnum WaitingPriceState { get; set; }
#nullable enable
        public ChargeDeviceConnectorInstantStateEnum? InstantState { get; set; }
        public string? PluggedSocketMovementGuiId { get; set; }
        public DateTime? LastInstantStateUpdatedDate { get; set; }
        public DateTime? ConfirmedPriceDate { get; set; }
        public decimal? WaitingPrice { get; set; }
        public decimal? WaitingPriceWithKdv { get; set; }
        public double? WaitingKdv { get; set; }
        public string? EpdkPriceInfoGuiId { get; set; } // epdk için
        public string? LastEpdkAvailabilityInfoGuiId { get; set; } // epdk için
        [ForeignKey("ChargeDevice_ChargeDeviceSocketType")]
        public long? ChargeDeviceSocketTypeId { get; set; }
        public ChargeDeviceSocketType? ChargeDeviceSocketType { get; set; }
        public ChargeDeviceConnectorGib? ChargeDeviceConnectorGib { get; set; }
#nullable disable
        [ForeignKey("ChargeDevice_ChargeDevicePowerType")]
        public long ChargeDevicePowerTypeId { get; set; }
        public ChargeDevicePowerType ChargeDevicePowerType { get; set; }
        [ForeignKey("ChargeDeviceConnector_ChargeDevice")]
        public long ChargeDeviceId { get; set; }
        public ChargeDevice ChargeDevice { get; set; }
        public virtual ICollection<Charge> Charge { get; set; }
        public virtual ICollection<ChargingConnectorReading> ChargingConnectorReading { get; set; }
        //public virtual ICollection<ChargeDeviceReservationTemp> ChargeDeviceReservationTemp { get; set; }
        //public virtual ICollection<ChargeDeviceReservation> ChargeDeviceReservation { get; set; }
        //public virtual ICollection<ChargeProcessTemp> ChargeProcessTemp { get; set; }
    }
}

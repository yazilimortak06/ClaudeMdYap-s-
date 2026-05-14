using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.ChargeDeviceMarkModule;
using Shared.Domain.Entities.ApiEntities.GibModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.ChargeDeviceModule
{
    [Table("ChargeDevice", Schema = "RotaWatt")]
    public class ChargeDevice : BaseEntity
    {
        public string Name { get; set; }
        public string GuiId { get; set; }
        public string Identifier { get; set; }
        public string OcppUrl { get; set; }
        public string SerialNumber { get; set; }
        public ChargeDeviceStateEnum State { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool ConnectionState { get; set; }
#nullable enable
        public ChargeDeviceInstantStateEnum? InstantState { get; set; }
        public DateTime? ConnectionDate { get; set; }
        public DateTime? DisconnectionDate { get; set; }
        public DateTime? LastInstantStateUpdatedDate { get; set; }
        [ForeignKey("ChargeDevice_Station")]
        public long? StationId { get; set; }
        public Station? Station { get; set; }
        [ForeignKey("ChargeDevice_ChargeDeviceMark")]
        public long? ChargeDeviceMarkId { get; set; }
        public ChargeDeviceMark? ChargeDeviceMark { get; set; }
        public EsuGibTaxPayer? EsuGibTaxPayer { get; set; }
#nullable disable
        public virtual ICollection<ChargeDeviceConnector> ChargeDeviceConnector { get; set; }
    }
}

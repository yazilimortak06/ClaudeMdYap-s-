using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.ChargeDevicePowerTypeModule;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.FirmModule
{
    [Table("FirmPrice", Schema = "RotaWatt")]
    public class FirmPrice : BaseEntity
    {
        public decimal Price { get; set; }
        public double Kw { get; set; }
        [ForeignKey("ChargeDevice_ChargeDevicePowerType")]
        public long ChargeDevicePowerTypeId { get; set; }
        public ChargeDevicePowerType ChargeDevicePowerType { get; set; }
        [ForeignKey("FirmPrice_Firm")]
        public long FirmId { get; set; }
        public Firm Firm { get; set; }
    }
}

using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.AdminModule;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.CompanyModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Entities.ApiEntities.SupportModule;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.FirmModule
{
    [Table("Firm", Schema = "RotaWatt")]
    public class Firm : BaseEntity
    {
        public string GuiId { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public double DividendPercantage { get; set; } // kar payı
        public DateTime CreatedDate { get; set; } 
        public bool IsActive { get; set; }
        public virtual ICollection<PanelAdmin> PanelAdmin { get; set; }
        public virtual ICollection<Station> Station { get; set; }
        public virtual ICollection<Company> Company { get; set; }
        public virtual ICollection<Charge> Charge { get; set; }
        public virtual ICollection<FirmPrice> FirmPrice { get; set; }
        public virtual ICollection<Support> Support { get; set; }
#nullable enable
        public DateTime? UpdatedDate { get; set; }
        public string? OcppUrlAddress { get; set; }
        public FirmIntegrationTypeEnum? IntegrationType { get; set; }
        public FirmLogo? FirmLogo { get; set; }
#nullable disable
    }
}

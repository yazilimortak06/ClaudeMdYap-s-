// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Domain\Entities\PlaceEntities\PlaceLanguage.cs
using FrameworkCore.Bases.BaseEntities;
using PixdinnCrm.Domain.Entities.LanguageEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixdinnCrm.Domain.Entities.PlaceEntities
{
    [Table("PlaceLanguage", Schema = "Crm")]
    public class PlaceLanguage : BaseEntity
    {
        public bool IsDefault { get; set; }

        [ForeignKey("PlaceLanguage_Place")]
        public long PlaceId { get; set; }
        public Place Place { get; set; }

        [ForeignKey("PlaceLanguage_ContentLanguage")]
        public long ContentLanguageId { get; set; }
        public ContentLanguage ContentLanguage { get; set; }
    }
}

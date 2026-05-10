// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Domain\Entities\PlaceEntities\Place.cs
using FrameworkCore.Bases.BaseEntities;
using PixdinnCrm.Domain.Entities.FileEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixdinnCrm.Domain.Entities.PlaceEntities
{
    [Table("Place", Schema = "Crm")]
    public class Place : BaseEntity
    {
        public string Name { get; set; }
        public string AboutUs { get; set; }
        public string Instagram { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LogoGuiId { get; set; }

        public virtual ICollection<PlacePicture> PlacePictures { get; set; }
        public virtual ICollection<PlaceLanguage> PlaceLanguages { get; set; }
    }
}

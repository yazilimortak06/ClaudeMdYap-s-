// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Domain\Entities\CategoryEntities\Category.cs
using FrameworkCore.Bases.BaseEntities;
using PixdinnCrm.Domain.Entities.PlaceEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixdinnCrm.Domain.Entities.CategoryEntities
{
    [Table("Category", Schema = "Crm")]
    public class Category : BaseEntity
    {
        public virtual ICollection<CategoryContent> CategoryContents { get; set; }
        public virtual ICollection<CategoryPicture> CategoryPictures { get; set; }

        public long PlaceId { get; set; }
        public Place Place { get; set; }
    }
}

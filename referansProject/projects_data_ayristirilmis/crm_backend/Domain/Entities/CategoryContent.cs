// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Domain\Entities\CategoryEntities\CategoryContent.cs
using FrameworkCore.Bases.BaseEntities;
using PixdinnCrm.Domain.Entities.LanguageEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixdinnCrm.Domain.Entities.CategoryEntities
{
    [Table("CategoryContent", Schema = "Crm")]
    public class CategoryContent : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey("CategoryContent_Category")]
        public long CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("CategoryContent_Language")]
        public long LanguageId { get; set; }
        public ContentLanguage Language { get; set; }
    }
}

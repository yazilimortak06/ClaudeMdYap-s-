// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Domain\Entities\ProductEntities\ProductContent.cs
using Domain.Entities;
using Domain.Entities.ProductEntities;
using FrameworkCore.Bases.BaseEntities;
using PixdinnCrm.Domain.Entities.LanguageEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixdinnCrm.Domain.Entities.ProductEntities
{
    [Table("ProductContent", Schema = "Crm")]
    public class ProductContent : BaseEntity
    {
        public string Name { get; set; }
        public string Detail { get; set; }

        [ForeignKey("ProductContent_Product")]
        public long ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("ProductContent_Language")]
        public long LanguageId { get; set; }
        public ContentLanguage Language { get; set; }
    }
}

// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Domain\Entities\ProductEntities\Product.cs
using FrameworkCore.Bases.BaseEntities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PixdinnCrm.Domain.Entities;
using PixdinnCrm.Domain.Entities.PlaceEntities;
using PixdinnCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities.ProductEntities
{
    [Table("Product", Schema = "Crm")]
    public class Product : BaseEntity
    {
        public decimal Price { get; set; }

        public virtual ICollection<ProductContent> ProductContents { get; set; }
        public virtual ICollection<ProductPicture> ProductPictures { get; set; }

        [ForeignKey("Product_Place")]
        public long PlaceId { get; set; }
        public Place Place { get; set; }
    }
}

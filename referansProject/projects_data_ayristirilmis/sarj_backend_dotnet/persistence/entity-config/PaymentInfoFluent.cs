// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Persistences\Api.Persistence\EntityFluent\PaymentInfoFluent.cs
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Domain.Entities.ApiEntities.PaymentInfoModule;

namespace Api.Persistence.EntityFluent
{
    public class PaymentInfoFluent : IEntityTypeConfiguration<PaymentInfo>
    {
        public void Configure(EntityTypeBuilder<PaymentInfo> builder)
        {
            builder
           .HasIndex(e => new { e.PaymentGuiId })
           .IsUnique()
           .HasFilter("[Deleted] = 0 and  [PaymentGuiId] is not null ");
        }
    }
}

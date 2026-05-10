// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Persistences\Api.Persistence\EntityFluent\UserFluent.cs
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.ApiEntities.UserModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Persistence.EntityFluent
{
    public class UserFluent : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
             .HasIndex(e => new { e.Phone, e.PhoneCountryCode })
             .IsUnique()
             .HasFilter("[Deleted] = 0 ");
       //     builder
       //    .HasIndex(e => new { e.TcNumber })
       //    .IsUnique()
       //.HasFilter("[Deleted] = 0");
        }
    }
}

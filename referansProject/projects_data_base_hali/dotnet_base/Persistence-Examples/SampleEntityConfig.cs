using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Persistence.EntityFluent
{
    public class ChargeFluent : IEntityTypeConfiguration<Charge>
    {
        public void Configure(EntityTypeBuilder<Charge> builder)
        {
            builder
             .HasIndex(e => new { e.PluggedSocketMovementGuiId })
             .IsUnique()
             .HasFilter("[Deleted] = 0 ");
        }
    }
}

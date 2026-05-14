// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Persistences\Api.Persistence\EntityFluent\StationAddressFluent.cs
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;
using NetTopologySuite.Geometries;
using Api.Persistence.DbContext;
using NetTopologySuite.IO;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;

namespace Api.Persistence.EntityFluent
{
    public class StationAddressFluent : IEntityTypeConfiguration<StationAddress>
    {
        public void Configure(EntityTypeBuilder<StationAddress> builder)
        {
            // Not: Coğrafi koordinat alanı için WKT dönüşümü yapılmaktadır.
            // Location alanı geography(Point) tipinde tutulmaktadır.
            // Aşağıdaki converter ile Point <-> WKT string dönüşümü sağlanmaktadır.

            var geometryConverter = new ValueConverter<Point, string>(
          v => new WKTWriter().Write(v),
          v => (Point)new WKTReader().Read(v)
      );

            // Örnek alternatif yaklaşımlar (yorum satırı olarak bırakılmıştır):
            // builder
            //     .Property(e => e.Location)
            //     .HasColumnType("geography(Point)")
            //     .HasConversion(v => $"{v.X},{v.Y}",
            //   v => new Point(double.Parse(v.Split(",", StringSplitOptions.None)[0]), double.Parse(v.Split(",", StringSplitOptions.None)[1])));

            // builder
            //     .Property(e => e.Location).HasComputedColumnSql("geography::Point(Latitude, Longitude, 4326)");
        }
    }
}

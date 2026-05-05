# sarj_backend_dotnet — FrameworkCore NuGet Paketleri

Bu liste `Framework/Core/FrameworkCore/` projesinin bağımlılıklarını kapsar.
Tüm microservice API projeleri bu framework'e referans verir.

---

## Temel Bağımlılıklar

| Paket | Versiyon | Kullanım Amacı |
|-------|----------|----------------|
| `Autofac` | 6.3.0 | IoC container, dependency injection |
| `Autofac.Extensions.DependencyInjection` | 7.2.0 | ASP.NET Core ile Autofac entegrasyonu |
| `Autofac.Extras.DynamicProxy` | 6.0.0 | AOP (Aspect Oriented Programming), interceptor desteği |
| `AutoMapper` | 11.0.0 | Object-to-object mapping (DTO ↔ Entity) |
| `EFCore.BulkExtensions` | 5.1.0 | EF Core toplu insert/update/delete operasyonları |
| `FluentValidation` | 10.3.5 | Model validation kuralları (fluent API) |
| `GeoAPI` | (latest uyumlu) | Coğrafi veri tipleri, NetTopologySuite desteği |
| `Hellang.Middleware.ProblemDetails` | 6.4.0 | RFC 7807 uyumlu hata yanıt formatı |
| `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` | 5.0.0 | API versioning + Swagger entegrasyonu |
| `Microsoft.EntityFrameworkCore.SqlServer` | 5.0.9 | EF Core SQL Server provider |
| `Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite` | 5.0.9 | Spatial data (koordinat, geometry) desteği |
| `Refit` | 5.2.1 | HTTP client interface'leri ile servisler arası iletişim |
| `Swashbuckle.AspNetCore.Swagger` | 6.2.3 | Swagger/OpenAPI dokümantasyonu |
| `MassTransit.RabbitMQ` | 7.3.1 | RabbitMQ üzerinden message bus (event-driven) |

---

## Platform

- Target Framework: `net5.0`
- ASP.NET Core: 5.0.x

---

## Notlar

- `Autofac.Extras.DynamicProxy` ile servis interceptor'ları (örn. logging, transaction interceptor) uygulanmaktadır.
- `EFCore.BulkExtensions` özellikle migration ve data seeding operasyonlarında performans için kullanılır.
- `GeoAPI` + `NetTopologySuite` şarj istasyonu koordinat verilerinin SQL Server'da spatial olarak saklanması için gereklidir.
- `Refit` servislerin birbirini HTTP üzerinden çağırması için type-safe HTTP client sağlar.
- `MassTransit.RabbitMQ` CQRS yerine event-driven mimari için tercih edilmiştir.

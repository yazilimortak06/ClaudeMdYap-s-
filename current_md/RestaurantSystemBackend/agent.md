# RestaurantSystemBackend — Agent

## Proje Bağlamı

Multi-tenant, modüler monolith restoran SaaS backend.
Stack: .NET 5.0 + EF Core 5.0.9 + SQL Server + Autofac DI.
3 API: Web.Api (ana), Token.Api (auth), File.Api (medya).

Çekirdek prensip: **Product = identity. Modüller = projection/configuration. OrderItem = snapshot.**

Detay için: `current_md/RestaurantSystemBackend/mimari_gelisen.md`.

## Aktif Görev

**Faz 1 Foundation Veritabanı (DEVAM EDIYOR — analiz onayı bekliyor)**

Yapılan:
- Faz 1 entity'leri oluşturuldu (Tenancy, Identity, Catalog, Menu, Subscription).
- `AuditedEntity`, `ITenantOwned` base'leri eklendi.
- `RestaurantDbContext` DbSet'leri ve global filter'lar (soft delete) güncellendi.
- EF EntityTypeConfiguration'ları yazıldı.

Kalan:
- Migration üret (`dotnet ef migrations add InitialCreate ...`).
- Seed data (default plan, default permissions, default roles).
- `muhammed_ali/analiz.md`'deki açık soruları tartış → `ortak/analiz.md` ye taşı.

## Bağlı Projeler ile Paylaşılan

- Shared klasör: `shared/RestaurantSystemBackend--RestaurantSystemPanel/`
- Shared klasör: `shared/RestaurantSystemBackend--RestaurantSystemQr/`

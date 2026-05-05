# RestaurantSystemBackend — Ortak

Her commit/push sonrası güncellenir.

## Son Değişiklikler

### 2026-05-05 — Faz 1 Foundation (muhammed_ali)
- Domain entity yapısı kuruldu: 16 entity (Tenancy + Identity + Catalog + Menu + Subscription).
- `AuditedEntity`, `ITenantOwned`, `IBranchOwned` taban sınıf/interface'leri.
- EF Core Fluent API konfigürasyonları (FK, index, max length, decimal precision, soft-delete query filter).
- `RestaurantDbContext` DbSet'leri + audit timestamp uygulayan SaveChanges override'ı.
- `Token.Persistence`'a `RefreshToken` entity'si.
- Solution build: 0 hata.

## Genel İlerleme

- [x] Mimari karar (modular monolith, entity konumu, base classes) — `mimari_gelisen.md`
- [x] Faz 1 entity'leri ve EF mapping'leri
- [x] DbContext güncellemeleri ve build doğrulaması
- [ ] DI register (Web.Api/Startup.cs) ve ilk migration
- [ ] Seed data (default Plans, Permissions, Roles)
- [ ] Açık soruların kesinleşmesi (`muhammed_ali/analiz.md` → `ortak/analiz.md`)
- [ ] Faz 1 Extension: Order snapshot, Tables, QR codes
- [ ] Faz 2: Pricing engine, Menu design engine, POS mapping

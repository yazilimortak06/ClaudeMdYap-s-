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

## Son Değişiklikler

### 2026-05-06 — Auth & Oturum Sistemi (ali)
- `JwtTokenService`, `AuthService`, `RefreshTokenRepository`, `UserRepositoryForToken` yazıldı.
- `AuthController` — login, refresh, logout, logout-all endpoint'leri.
- Login username/password tabanlı (email değil). `User` entity'sine `Username` eklendi.
- Login hatalıysa 500 değil 401 dönüyor.
- CORS middleware sırası düzeltildi: `UseRouting` → `UseCors` → `UseAuthentication`.
- `RefreshTokenRepository` Autofac `IUnitOfWork` cast sorunu çözüldü.
- Startup'ta auto-migration + `AdminUserSeeder` (admin / Admin@123).
- IIS Express launchSettings eklendi (44301/44302/44303).
- Her API'ye `Dockerfile`, solution root'a `docker-compose.yml`.

## Genel İlerleme

- [x] Mimari karar (modular monolith, entity konumu, base classes) — `mimari_gelisen.md`
- [x] Faz 1 entity'leri ve EF mapping'leri
- [x] DbContext güncellemeleri ve build doğrulaması
- [x] DI register ve migration (RestaurantSystemDb @ localhost)
- [x] Seed data (Plans, Permissions, Roles, Features, PlanFeatures)
- [x] Auth & oturum sistemi (Token.Api)
- [x] Docker desteği
- [ ] Açık soruların kesinleşmesi (`muhammed_ali/analiz.md` → `ortak/analiz.md`)
- [ ] Faz 1 Extension: Order snapshot, Tables, QR codes
- [ ] Faz 2: Pricing engine, Menu design engine, POS mapping

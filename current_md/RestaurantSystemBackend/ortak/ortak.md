# RestaurantSystemBackend — Ortak Durum

Her commit/push sonrası güncellenir. Genel ilerleme ve son değişiklikler.

## Son Güncelleme
(Henüz güncelleme yok — proje başlangıç aşamasında)

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
- [ ] Altyapı kurulumu
- [ ] Temel CRUD endpoint'leri
- [ ] Auth entegrasyonu
- [ ] SignalR hub
- [ ] Docker ortamı

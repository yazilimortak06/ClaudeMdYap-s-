# RestaurantSystemBackend — Agent

## Proje Bağlamı

Multi-tenant, modüler monolith restoran SaaS backend.
Stack: .NET 5.0 + EF Core 5.0.9 + SQL Server + Autofac DI.
3 API: Web.Api (ana), Token.Api (auth), File.Api (medya).

Çekirdek prensip: **Product = identity. Modüller = projection/configuration. OrderItem = snapshot.**

Detay için: `current_md/RestaurantSystemBackend/mimari_gelisen.md`.

## Aktif Görev

**Aktif görev yok — auth sistemi tamamlandı.**

Bir sonraki adım: Faz 1 Extension (Order snapshot, Tables, QR codes) veya açık soruların kesinleştirilmesi.

**Tamamlanan — Auth & Oturum Sistemi**

- `JwtTokenService` — userId, tenantId, branchId, roles claim'leri
- `AuthService` — login (bcrypt), refresh (rotation), logout, logout-all
- `RefreshTokenRepository`, `UserRepositoryForToken`
- `AuthController` (Token.Api) — 4 endpoint: login, refresh, logout, logout-all
- DI kayıtları + CORS middleware sırası düzeltildi (UseRouting → UseCors)
- Startup auto-migration + AdminUserSeeder (admin/Admin@123)
- `User` entity'sine `Username` alanı eklendi, login email → username'e çevrildi
- Login başarısız → 401 döner (exception değil)
- Docker desteği: her API'ye Dockerfile, docker-compose.yml

**Tamamlanan — Faz 1 Foundation**

- 16 entity (Tenancy, Identity, Catalog, Menu, Subscription)
- EF migration + seed data (roller, izinler, planlar, özellikler)
- IIS Express launchSettings (44301/44302/44303)

## Bağlı Projeler ile Paylaşılan

- Shared klasör: `shared/RestaurantSystemBackend--RestaurantSystemPanel/`
- Shared klasör: `shared/RestaurantSystemBackend--RestaurantSystemQr/`

# RestaurantSystemBackend — Muhammed Ali Logs

Muhammed Ali'nin backend projesindeki işlem geçmişi.

Format:
```
## [Tarih Saat] İşlem Başlığı
**Yapılan:** Ne yapıldı?
**Durum:** Tamamlandı / Devam ediyor / Askıda
```

---

## [2026-05-05] Faz 1 Foundation Veritabanı Yapısı

**Yapılan:**
- `Api.Persistence/Domain/` altında modüler yapı kuruldu (Common, Enums, Entities/, Configurations/).
- Base sınıflar eklendi: `AuditedEntity` (CreatedAt/UpdatedAt), `ITenantOwned`, `IBranchOwned`.
- Enum'lar eklendi: `EntityStatus`, `PriceChannel`, `FeatureType`, `SubscriptionStatus`.
- Entity'ler oluşturuldu (toplam 16):
  - **Tenancy:** Tenant, Branch
  - **Identity:** User, Role, Permission, RolePermission, UserRole
  - **Catalog:** Category, Product, ProductTranslation, ProductPrice
  - **Menu:** MenuEntity, MenuCategory, MenuItem
  - **Subscription:** Plan, Feature, PlanFeature, TenantSubscription, TenantFeatureOverride
- Her entity için `IEntityTypeConfiguration<T>` yazıldı (FK'lar, indeksler, max length, decimal precision, soft-delete query filter).
- `RestaurantDbContext` DbSet'leri ve audit timestamp uygulayan `SaveChanges` override'ı eklendi.
- `Token.Persistence`'a `RefreshToken` entity'si ve config'i eklendi.
- `dotnet build RestaurantSystem.sln` → **0 hata, 22 uyarı** (uyarılar mevcut framework kodu kaynaklı).

**Sonraki Adım:**
- `Web.Api/Startup.cs` içinde DbContext'leri DI'a register et.
- İlk migration: `dotnet ef migrations add InitialCreate -p src/Core/Persistences/Api.Persistence -s src/Presentation/Web.Api`
- Seed data planla (default Plan'lar: Starter/Pro/Premium; default Permission listesi; super-admin Role).
- `muhammed_ali/analiz.md` daki açık soruları tartış → onaylanırsa `ortak/analiz.md` ye taşı.

**Durum:** Tamamlandı (analiz onayı bekliyor).

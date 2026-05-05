# muhammed_ali — RestaurantSystemBackend

## Çalışılan Modül / Konu

Faz 1 Foundation — Veritabanı + Domain Entities + EF Konfigürasyonu.

## Ne Yapacak

`restaurant_saas_context` referansıyla aşağıdaki modüllerin entity'lerini ve EF mapping'lerini oluşturmak:

1. **Tenancy:** Tenant, Branch
2. **Identity:** User, Role, Permission, RolePermission, UserRole
3. **Catalog:** Category, Product, ProductTranslation, ProductPrice
4. **Menu:** Menu, MenuCategory, MenuItem
5. **Subscription:** Plan, Feature, PlanFeature, TenantSubscription, TenantFeatureOverride

Ek olarak `AuditedEntity`, `ITenantOwned` taban yapısı; `RestaurantDbContext` üzerinde DbSet'ler ve soft-delete global filter.

## İlerleme

- [x] Mimari karar (modular monolith, entity konumu, base classes)
- [x] AuditedEntity + ITenantOwned eklendi
- [x] Tenancy entity'leri (Tenant, Branch)
- [x] Identity entity'leri
- [x] Catalog entity'leri
- [x] Menu entity'leri
- [x] Subscription entity'leri
- [x] EF Configuration'lar
- [x] DbContext DbSet güncelleme + soft-delete global filter
- [ ] Migration üretme (kullanıcı SQL Server connection string doğrulasın)
- [ ] Seed data
- [ ] Açık soruları tartışıp `ortak/analiz.md` kesinleştir

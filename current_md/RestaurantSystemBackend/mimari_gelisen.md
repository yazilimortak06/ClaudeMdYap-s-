# RestaurantSystemBackend — Gelişen Mimari

> Proje ilerledikçe alınan kararlar doğrultusunda güncellenir. Her önemli mimari karar buraya eklenir.

## Alınan Kararlar

### 2026-05-05 — Faz 1 Foundation Mimari Çerçevesi

#### Mimari Stil
- **Modular Monolith** (referans: `restaurant_saas_context`).
- 3 ayrı API: `Web.Api` (ana iş kuralları), `Token.Api` (auth), `File.Api` (dosya/medya).
- Her API'nin kendi DbContext'i var: `RestaurantDbContext`, `TokenDbContext`, `FileDbContext` — aynı SQL Server DB'sine bakıyor (logical bounded contexts, fiziksel tek DB).

#### Stack
- .NET 5.0, EF Core 5.0.9, SQL Server.
- Autofac (DI), AutoMapper, FluentValidation, Hellang ProblemDetails, Refit (HTTP), Swashbuckle.

#### Domain Yerleşimi
- Entity'ler: `src/Core/Persistences/Api.Persistence/Domain/Entities/<Modül>/`
- EF Configuration'lar: `src/Core/Persistences/Api.Persistence/Domain/Configurations/<Modül>/`
- Sebep: `RestaurantDbContext` `ApplyConfigurationsFromAssembly(typeof(RestaurantDbContext).Assembly)` kullanıyor — entity ve config aynı assembly'de olmak zorunda.
- İleride `Api.Domain` projesi ayrılırsa migration mümkün; şimdilik mevcut yapıya uyduk.

#### Çekirdek Prensip — Product is Source of Truth
- `Product` çekirdek kimliktir. Modüllere ait ayrı ürün identity'si YOK.
- QR Menü, Table Order, Home Delivery, POS gibi modüller `Product`'ı projeksiyon/konfigürasyon tabloları üzerinden referanslar.
- `OrderItem` snapshot tutar (geçmiş bütünlük).

#### Multi-Tenancy
- Tenant seviyesinde scoping. Çoğu entity'de `tenant_id` taşınır.
- Platform super-admin kullanıcısı `tenant_id = null` olabilir.
- Tenant admin'in `tenant_id` var ama `branch_id = null` olabilir (multi-branch yetki).
- Branch user'ın hem `tenant_id` hem `branch_id` var.

#### Base Sınıflar
- `BaseEntity (FrameworkCore)`: `long Id`, `bool Deleted`. (Framework — değişmez.)
- `AuditedEntity : BaseEntity` (yeni): `DateTime CreatedAt`, `DateTime UpdatedAt`. Tüm tenant verileri buradan türer.
- `ITenantOwned` interface: `long TenantId` zorunluluğu — global filter ve cross-tenant koruma için.

#### Soft Delete
- `BaseEntity.Deleted` zaten var. `OnModelCreating`'de tüm entity'lere `HasQueryFilter(e => !e.Deleted)` global filter eklenir.

#### Para / Lokalizasyon
- Money: `decimal(18,4)` + ISO 4217 3-char `currency`.
- Language code: 5 char (örn: `tr-TR`).

#### Identity Yerleşimi
- `User`, `Role`, `Permission`, `RolePermission`, `UserRole` → `Api.Persistence` (iş kurallarına yakın).
- `RefreshToken` → `Token.Persistence` (auth detayı). Aynı DB, ayrı bounded context.

## Mimari Genel Görünüm

```
┌──────────────────────────────────────────────────────────────┐
│                  Presentation (REST APIs)                    │
│   Web.Api          Token.Api          File.Api               │
└─────────┬──────────────┬───────────────────┬─────────────────┘
          │              │                   │
┌─────────▼──┐    ┌──────▼──────┐    ┌───────▼───────┐
│ Api.Appl.  │    │ Token.Appl. │    │ File.Appl.    │
│ (use cases)│    │             │    │               │
└─────────┬──┘    └──────┬──────┘    └───────┬───────┘
          │              │                   │
┌─────────▼─────────┐ ┌──▼──────────┐ ┌──────▼────────┐
│ Api.Persistence   │ │ Token.Pers. │ │ File.Pers.    │
│  └ Domain/        │ │  └ Refresh  │ │  └ FileMeta   │
│    Entities/      │ │    Tokens   │ │               │
│    Configurations/│ │             │ │               │
└─────────┬─────────┘ └──┬──────────┘ └──────┬────────┘
          │              │                   │
          └──────────────┴───────────────────┘
                         │
                ┌────────▼────────┐
                │ SQL Server DB   │
                │ RestaurantSystemDb │
                └─────────────────┘

Çekirdek Modüller (Domain/Entities/):
- Tenancy        (Tenant, Branch)
- Identity       (User, Role, Permission, RolePermission, UserRole)
- Catalog        (Category, Product, ProductTranslation, ProductPrice)
- Menu           (Menu, MenuCategory, MenuItem)
- Subscription   (Plan, Feature, PlanFeature, TenantSubscription, TenantFeatureOverride)
```

## Notlar / Tartışmalar

- **Net upgrade:** .NET 5 EOL. Faz 1 stable olduktan sonra .NET 8 LTS'e geçiş için ayrı sohbet açılmalı.
- **Pricing engine:** Faz 2'de `pricing_rules`, `pricing_rule_conditions`, `pricing_rule_actions` eklenecek.
- **Menu design engine:** Faz 2'de `menu_themes`, `menu_design_rules` eklenecek.
- **Order snapshot:** Phase 1 extension'da `orders`, `order_items` (snapshot fields ile).
- **POS, Home Delivery, Table Order projeksiyonları:** Phase 1 extension+'da, ürünün üzerinden config tabloları olarak eklenecek (hiçbir zaman ayrı product identity oluşturulmaz).

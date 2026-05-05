# muhammed_ali — RestaurantSystemBackend Analiz Taslağı

> Bu dosya kişisel taslaktır. Kesinleştirme onayı verilirse `ortak/analiz.md` ye taşınacaktır.

## 2026-05-05 — Faz 1 Veritabanı Temeli (Inşa)

### Bağlam
`restaurant_saas_context (1).txt` referans alınarak modüler monolith mimaride çok kiracılı (multi-tenant) restoran SaaS platformunun **Faz 1 foundation** veritabanı yapısı kuruldu.

### Karar Verilenler (Önerilen — Onay Bekliyor)

1. **Stack:** Mevcut .NET 5.0 + EF Core 5.0.9 + SQL Server'da kalındı. Upgrade ayrı tartışma.
2. **Entity konumu:** `Api.Persistence/Domain/Entities/<Modül>/`
3. **Konfigürasyon konumu:** `Api.Persistence/Domain/Configurations/<Modül>/`
4. **Audit alanları:** `BaseEntity` framework koduna dokunmadı. Yeni `AuditedEntity : BaseEntity` (CreatedAt, UpdatedAt) eklendi.
5. **Tenant izolasyonu:** `ITenantOwned` interface eklendi. Global query filter altyapısı hazır ama henüz aktif değil — tenant resolver service eklendiğinde aktive edilecek.
6. **Soft delete:** `BaseEntity.Deleted` üzerinden çalışıyor. Global filter `OnModelCreating` içinde otomatik uygulandı.
7. **Identity bölünmesi:** User/Role/Permission `Api.Persistence`'da. RefreshToken `Token.Persistence`'da. İki context aynı DB'ye bakıyor.
8. **Product Category:** Tenant seviyesinde `Category` (product için) ve menu seviyesinde `MenuCategory` (menü organizasyonu) ayrı tablolar — context dokümanı bunu öneriyor.
9. **Variants/Options:** Faz 1 dışında bırakıldı (context: future).
10. **Money tipi:** `decimal(18,4)` + 3 karakterli `currency` kodu (ISO 4217).
11. **Çoklu dil:** `language_code` 5 char (örn: `tr-TR`, `en-US`).

### Açık Sorular (Tartışılacak — Kesinleşmeden Önce)

| # | Soru | Önerim | Etki |
|---|------|--------|------|
| 1 | Branch override: ürün adı/fiyat/görünürlük branch bazında değiştirilebilir mi? | Fiyat: evet (`product_prices` `branch_id`). Ad: hayır, sadece menü `override_name`. | Branch yönetim akışı |
| 2 | Menu kapsamı: bir menü tek branch'e mi yoksa birden çoğa mı bağlı? | Tek branch (`menus.branch_id` zorunlu). Tenant geneli için farklı menüler oluşturulur. | Menü yönetimi UX |
| 3 | POS senkronu: tek yön (bizden POS'a) mu çift yön mü? | Faz 1'de POS yok, ileride tek yön başlatılır. | POS modülü |
| 4 | Product silme: hard delete asla yok, sadece soft? | Sadece soft (BaseEntity zaten Deleted bool). | Veri tutarlılığı |
| 5 | Fiyat değişimi versiyonlama? | `product_prices` zaten `valid_from/valid_to` tutuyor — versiyonlama bu üzerinden. | Geçmiş izleme |
| 6 | Menü design rules nerede çalışacak: server-side mı client-side mı? | Server-side (kural değerlendirmesi backend'de, client'a sonuç JSON gider). | API tasarımı |
| 7 | QR menü public mi (login gerektirmez)? | Evet, public. Tenant context QR koddan resolve edilir. | Auth tasarımı |
| 8 | Table order staff onayı gerektiriyor mu kitchen/POS'a giderken? | Tenant ayarına bağlı (default: hayır, otomatik geçer). | Order workflow |
| 9 | Stok/inventory ileride gelecek mi? | Evet, ayrı modül. Faz 1 etkilemez. | Future module |
| 10 | Variants (boy, ekstra peynir vb.)? | Faz 2+. Faz 1 dışında. | Product yapısı |
| 11 | Vergi: ürün bazlı mı kanal bazlı mı? | Ürün bazlı (`products.tax_rate`). Kanal bazlı override gerekirse `product_prices` üzerinden. | Faturalandırma |
| 12 | Campaign vs dynamic pricing önceliği? | `pricing_rules.priority` field'ı belirler. Campaign default daha yüksek priority. | Pricing engine |
| 13 | Reporting: real-time mı periyodik mi? | Order snapshot tablosu üzerinden real-time read. Aggregate'ler periyodik. | Reporting modülü |

### Sonraki Adım
- Yukarıdaki açık soruları tartışıp `ortak/analiz.md` ye kesinleşmiş hali taşı.
- Migration üret: `dotnet ef migrations add InitialCreate -p src/Core/Persistences/Api.Persistence -s src/Presentation/Web.Api`
- Seed data planla (default plan, default permissions).

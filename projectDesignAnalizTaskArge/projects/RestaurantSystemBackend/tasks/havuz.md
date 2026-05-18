# RestaurantSystemBackend — Görev Havuzu

Kesinleşmiş ama henüz alınmamış görevler. İş alınınca `yapilacaklar.md`'ye taşınır.

---

## Temel Altyapı

- [ ] **Proje scaffolding** — Clean Architecture 5 katman kurulumu: Domain, Application, Infrastructure, Persistence, API. FrameworkCore referans projesi bağlantısı. _(Tahmini: 2 saat)_
- [ ] **Docker + docker-compose kurulumu** — SQL Server, RabbitMQ, Redis container'ları. Health check ve volume konfigürasyonu. _(Tahmini: 1 saat)_
- [ ] **EF Core DbContext + BaseEntity + soft-delete global filter** — `BaseEntity` (Id, CreatedAt, UpdatedAt, IsDeleted), `ApplicationDbContext`, soft-delete ve multi-tenancy global query filter'ları. _(Tahmini: 2 saat | Bağımlılık: Proje scaffolding)_
- [ ] **JWT Auth** — `TokenService` implementasyonu, Bearer middleware konfigürasyonu, refresh token desteği. _(Tahmini: 3 saat | Bağımlılık: DbContext)_
- [ ] **AutoMapper profilleri** — Menu, Order, Table domain nesneleri için DTO ↔ Entity mapping profilleri. _(Tahmini: 2 saat | Bağımlılık: Domain entity'leri)_
- [ ] **Swagger konfigürasyonu** — API versioning (v1), JWT auth butonu, XML doc entegrasyonu. _(Tahmini: 1 saat | Bağımlılık: Proje scaffolding)_

---

## Domain

- [ ] **Restaurant entity + CRUD** — `Restaurant` entity tasarımı, `RestaurantService`, CRUD endpoint'leri (GET, POST, PUT, DELETE). _(Tahmini: 3 saat | Bağımlılık: DbContext)_
- [ ] **MenuCategory entity + CRUD + sıralama** — `MenuCategory`, DisplayOrder alanı, sürükleme ile sıralama endpoint'i. _(Tahmini: 3 saat | Bağımlılık: Restaurant CRUD)_
- [ ] **MenuItem entity + CRUD + görsel yükleme** — `MenuItem`, FileAPI entegrasyonu ile resim yükleme, aktif/pasif toggle. _(Tahmini: 4 saat | Bağımlılık: MenuCategory CRUD, FileAPI)_
- [ ] **MenuItemOption entity + CRUD** — Ürün seçenekleri (boyut, sos vb.), `MenuItemOptionGroup` + `MenuItemOption`. _(Tahmini: 3 saat | Bağımlılık: MenuItem CRUD)_
- [ ] **Table entity + CRUD + QR token üretimi** — `Table`, `QrCode` entity, UUID token üretimi, QR içerik URL formatı. _(Tahmini: 3 saat | Bağımlılık: Restaurant CRUD)_
- [ ] **Order + OrderItem entity + CRUD** — `Order`, `OrderItem`, sepet'ten sipariş oluşturma endpoint'i, toplam tutar hesaplama. _(Tahmini: 5 saat | Bağımlılık: MenuItem, Table)_
- [ ] **OrderStatus geçiş servisi** — `Pending → Accepted → Preparing → Ready → Delivered → Paid` geçiş kuralları, geçersiz geçişleri reddeden servis. _(Tahmini: 3 saat | Bağımlılık: Order CRUD)_
- [ ] **User + Role yönetimi** — `User`, `Role` entity, kullanıcı kayıt/güncelleme, rol atama endpoint'leri. _(Tahmini: 4 saat | Bağımlılık: JWT Auth)_

---

## Servisler

- [ ] **FileAPI entegrasyonu** — Resim yükleme, silme, URL üretme. `IFileService` soyutlaması, local storage veya cloud (S3 uyumlu) implementasyonu. _(Tahmini: 3 saat | Bağımlılık: Proje scaffolding)_
- [ ] **SignalR Hub** — `OrderHub`: yeni sipariş olayı, durum güncelleme olayı. Panel bağlantısı için restoran bazlı group yönetimi. _(Tahmini: 4 saat | Bağımlılık: Order CRUD)_
- [ ] **NotificationService entegrasyonu** — Yeni sipariş geldiğinde push bildirimi (FCM veya benzeri). `INotificationService` soyutlaması. _(Tahmini: 3 saat | Bağımlılık: SignalR Hub)_
- [ ] **WorkerService** — Günlük rapor oluşturma job'u, eski log temizleme job'u, `IHostedService` implementasyonu. _(Tahmini: 3 saat | Bağımlılık: Order CRUD)_

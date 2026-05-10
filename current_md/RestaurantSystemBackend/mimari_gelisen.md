# RestaurantSystemBackend — Gelişen Mimari

> Proje ilerledikçe alınan kararlar doğrultusunda güncellenir.
> Her önemli mimari karar buraya eklenir; eski kararlar silinmez, üzerine not düşülür.

---

## Alınan Kararlar

### Tech Stack

| Alan | Seçilen Teknoloji | Gerekçe |
|---|---|---|
| Runtime | .NET 8 | LTS sürümü, minimal API + controller desteği |
| ORM | Entity Framework Core 8 | Code-first migration, LINQ desteği |
| IoC Container | Autofac | Interceptor desteği (transaction, logging AOP) |
| Mapping | AutoMapper | DTO ↔ Entity dönüşümü, profil tabanlı yapılandırma |
| Mesajlaşma | MassTransit + RabbitMQ | Event-driven mimari, retry/dead-letter kutusu |
| Auth | JWT Bearer | Stateless kimlik doğrulama, refresh token desteği |
| API Dokümantasyon | Swagger / Swashbuckle | Geliştirici deneyimi, endpoint keşfi |
| Containerization | Docker + docker-compose | Ortam bağımsızlığı, servis izolasyonu |
| Realtime | SignalR | Sipariş güncellemelerinin Panel ve QR'a anlık iletimi |

### Veritabanı

- **SQL Server** (üretim ve geliştirme ortamı).
- Her servis kendi veritabanına sahiptir; doğrudan cross-DB join yapılmaz.
- Migration'lar `RestaurantSystem.Persistence` projesinden yönetilir.
- Soft-delete için tüm entity'lerde `IsDeleted` alanı kullanılır;
  EF Core global query filter ile `IsDeleted = false` otomatik eklenir.
- Multi-tenant veri izolasyonu: `RestaurantId` kolonu her tenant entity'sinde bulunur;
  EF Core global query filter ile her sorguya otomatik tenant filtresi eklenir.

### Mimari

- **Clean Architecture** (4+1 katman + FrameworkCore).
- Katmanlar arası bağımlılık yönü: `API → Application → Domain`;
  `Persistence` yalnızca `Application` interface'lerini implement eder.
- `FrameworkCore` paylaşılan altyapı projesidir; tüm servislerce NuGet paketi olarak referans alınır.

### API

- **REST** tabanlı, versioned endpoint'ler: `/v1/...`
- Tüm response'lar `InnerResult<T>` sarmalayıcısında döner (FrameworkCore'dan gelir).
- Pagination için `PaginatedResponse<T>` kullanılır.
- Hata yönetimi: global exception middleware; `ProblemDetails` standardı.

### Auth

- **JWT Bearer** token; claims: `UserId`, `RestaurantId`, `Roles`, `Email`.
- Refresh token rotasyonu: her yenileme yeni bir refresh token üretir, eskisi iptal edilir.
- `[InnerRequestAttribute]` ile controller action'larında user context otomatik inject edilir.
- Public endpoint'ler (QR doğrulama, sipariş oluşturma): `[AllowAnonymous]`, QR token doğrulaması.

### Realtime

- **SignalR** Hub: `/hubs/orders`
- Bağlantı kimlik doğrulama: JWT token query string ile (`?access_token=...`).
- Gruplar: her restoran kendi SignalR grubuna katılır (`restaurant-{restaurantId}`).
- Yayımlanan eventler:
  - `OrderCreated` → Panel'deki tüm kullanıcılara
  - `OrderStatusChanged` → Panel + ilgili masa QR oturumuna
  - `OrderCancelled` → Panel'deki tüm kullanıcılara

---

## Katman Yapısı

```
RestaurantSystem.API          → HTTP katmanı (Controllers, Middleware, Filters, Hubs)
    ↓ referans alır
RestaurantSystem.Application  → Use-case katmanı (Services, DTOs, Interfaces, Validators, Mappings)
    ↓ referans alır
RestaurantSystem.Domain       → İş mantığı çekirdeği (Entities, Enums, Domain Events, Exceptions)

RestaurantSystem.Persistence  → Veri erişim katmanı (DbContext, Repositories, Migrations, Configurations)
    → Application interface'lerini implement eder
    → Domain'i referans alır

FrameworkCore                 → Paylaşılan altyapı (BaseEntity, BaseRepository, UnitOfWork,
                                InnerResult, JWT helpers, Autofac modülleri, Interceptors)
    → Tüm katmanlar tarafından referans alınır (NuGet paketi)
```

**Bağımlılık yönü özeti:**
- `API` → `Application` → `Domain` (tek yön, domain dışarıya bağımlı değil)
- `Persistence` → `Application` + `Domain` (interface'leri implement eder)
- `FrameworkCore` → kimseye bağımlı değil (en alt katman)

---

## Microservice Envanteri

| Servis | Sorumluluk | Port (Dev) |
|---|---|---|
| `RestaurantSystem.API` | Ana uygulama; menü, masa, sipariş, kullanıcı, auth, bildirim, SignalR | 5100 |
| `FileAPI` | Dosya yükleme/indirme/silme; image resize; URL üretimi | 5200 |
| `NotificationService` | E-posta, SMS, push bildirim gönderimi; RabbitMQ consumer | 5300 |
| `WorkerService` | Arka plan görevleri (yazıcı komutları, raporlama, cleanup) | — (hosted service) |
| `TokenService` | JWT üretim/doğrulama yardımcı servisi (FrameworkCore içinde kütüphane olarak da var) | 5400 |

**Servisler Arası İletişim:**
- `RestaurantSystem.API` → `FileAPI`: HTTP (dosya URL'si almak için)
- `RestaurantSystem.API` → RabbitMQ: event publish (`OrderCreatedEvent`, `OrderStatusChangedEvent` vb.)
- `NotificationService` ← RabbitMQ: event consume, bildirim gönder
- `WorkerService` ← RabbitMQ: event consume, yazıcı komutu / rapor işle

---

## Mimari Genel Görünüm

```
┌─────────────────────────────────────────────────────────────┐
│                    İstemciler                               │
│  [Panel (Angular/React)]  [QR App (React Native/Web)]       │
└────────────┬──────────────────────────┬────────────────────┘
             │ JWT + REST/SignalR        │ QR Token + REST
             ▼                          ▼
┌─────────────────────────────────────────────────────────────┐
│              RestaurantSystem.API  (:5100)                  │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌───────────┐  │
│  │ Order    │  │  Menu    │  │  Table   │  │ Auth      │  │
│  │Controller│  │Controller│  │Controller│  │Controller │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └─────┬─────┘  │
│       │              │              │               │        │
│  ┌────▼──────────────▼──────────────▼───────────────▼─────┐ │
│  │              Application Layer                         │ │
│  │   IOrderService  IMenuService  ITableService           │ │
│  │   AutoMapper Profiles    FluentValidation              │ │
│  └────────────────────────┬───────────────────────────────┘ │
│                           │                                  │
│  ┌────────────────────────▼───────────────────────────────┐ │
│  │              Persistence Layer                         │ │
│  │   RestaurantDbContext  (EF Core + SQL Server)          │ │
│  │   OrderRepository  MenuItemRepository  TableRepository │ │
│  │   UnitOfWork                                           │ │
│  └────────────────────────────────────────────────────────┘ │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              SignalR Hub (/hubs/orders)              │   │
│  └─────────────────────────────────────────────────────┘   │
└────────────────────┬────────────────────────────────────────┘
                     │ RabbitMQ Events
          ┌──────────▼──────────────────────────┐
          │          RabbitMQ                   │
          └──────┬───────────────┬──────────────┘
                 │               │
    ┌────────────▼────┐   ┌──────▼───────────┐
    │ NotificationSvc │   │  WorkerService   │
    │ (:5300)         │   │ (Printer, Report)│
    └─────────────────┘   └──────────────────┘

    ┌─────────────────────────────────────────┐
    │         FileAPI  (:5200)                │
    │  Upload / Resize / URL                  │
    └─────────────────────────────────────────┘

    ┌─────────────────────────────────────────┐
    │  FrameworkCore  (NuGet Package)         │
    │  BaseEntity, BaseRepository, UnitOfWork │
    │  InnerResult, JWT, Autofac Modules      │
    └─────────────────────────────────────────┘
```

---

## Notlar / Tartışmalar

### 2024-05-10 — Snapshot Kararı
OrderItem içinde `MenuItemName` ve `UnitPrice` snapshot olarak tutulacak.
Alternatif olarak audit log düşünüldü ama snapshot daha basit ve sorgulama açısından daha hızlı.

### 2024-05-10 — Public Endpoint Auth
Sipariş oluşturma endpoint'i (`POST /v1/orders`) JWT gerektirmeyecek; QR token yeterli olacak.
Rate limiting ile kötüye kullanım engellenecek (IP başına dakikada max 10 sipariş).

### 2024-05-10 — Tenant Filter Tasarımı
EF Core global query filter ile tenant izolasyonu sağlanacak. Ancak SuperAdmin için
`ICurrentUserService.IsSuperAdmin` kontrolüyle filter bypass edilecek.
Service katmanında ek kontrol (defense in depth) de yapılacak.

### 2024-05-10 — SignalR vs Polling
Sipariş durumu güncellemeleri için SignalR tercih edildi. QR müşteri tarafı için
uzun polling alternatif olarak değerlendirildi ancak SignalR daha az kaynak kullanacak.

### 2024-05-10 — FileAPI Ayrımı
Dosya yükleme ayrı bir serviste (FileAPI) tutulacak. Ana API sadece URL string saklayacak.
Bu sayede CDN entegrasyonu ilerleyen aşamada kolayca eklenebilir.

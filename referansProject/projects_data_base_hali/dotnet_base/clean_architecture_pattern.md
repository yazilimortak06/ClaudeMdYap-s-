# Clean Architecture Pattern — .NET Microservice Backend

## Katman Mimarisi Genel Bakış

Bu proje Clean Architecture + Microservice yapısını birleştirir. Her microservice kendi bağımsız katmanlarına sahiptir; ortak framework bir shared library olarak paketlenir.

```
┌─────────────────────────────────────────────────┐
│                 PRESENTATION                     │
│         (HTTP API — Controllers)                 │
├─────────────────────────────────────────────────┤
│                 APPLICATION                      │
│    (Business Logic — Services, Validators)      │
├─────────────────────────────────────────────────┤
│                  PERSISTENCE                     │
│   (Data Access — Repository, DbContext, EF)     │
├─────────────────────────────────────────────────┤
│                   DOMAIN                         │
│      (Entities, DTOs, Interfaces, Enums)         │
├─────────────────────────────────────────────────┤
│              FRAMEWORK CORE                      │
│   (Shared Base Classes, Utilities, Extensions)  │
└─────────────────────────────────────────────────┘
```

---

## Katman Detayları

### 1. Domain Katmanı
- **İçerik:** Entity sınıfları, DTO'lar, Enum'lar, Interface tanımları
- **Bağımlılık:** Hiçbir dış katmana bağımlı değildir
- **Kural:** İş mantığı içermez; sadece veri modelleri ve sözleşmeler

```
domain/
├── entities/       → Veritabanı entity sınıfları (BaseEntity'den türer)
├── dtos/           → Request/Response transfer objeleri
├── enums/          → Sabit değer listeleri
└── interfaces/     → Repository ve service arayüzleri
```

### 2. Application Katmanı
- **İçerik:** İş kuralları, service implementasyonları, validasyonlar
- **Bağımlılık:** Sadece Domain'e bağımlıdır (Persistence'a doğrudan bağlanmaz)
- **Kural:** UnitOfWork üzerinden repository erişimi; cross-cutting concern'ler interceptor ile

```
application/
├── services/       → Iş mantığı service implementasyonları
├── filters/        → Action filter'lar
├── extensions/     → Service registration extension metotları
├── interceptors/   → AOP interceptor'lar (logging, transaction)
└── message-bus/
    └── consumers/ → Message broker event consumer'ları
```

### 3. Persistence Katmanı
- **İçerik:** EF Core DbContext, Repository implementasyonları, Migrations
- **Bağımlılık:** Domain ve Application'a bağımlıdır
- **Kural:** Domain interface'lerini implement eder; Application katmanı bu implementasyonu bilmez

```
persistence/
├── db-context/     → EF Core DbContext sınıfı
├── entity-config/  → IEntityTypeConfiguration<T> (Fluent API)
├── migrations/     → EF Core migration dosyaları
└── repositories/   → IRepository implementasyonları
```

### 4. Presentation Katmanı
- **İçerik:** ASP.NET Core Controller'lar, Program.cs, Startup.cs
- **Bağımlılık:** Application'a bağımlıdır (Domain'e de dolaylı)
- **Kural:** İş mantığı içermez; sadece HTTP request'i parse eder ve service'e iletir

```
presentation/
├── controllers/    → API endpoint tanımları
├── program.cs      → Host konfigürasyonu
├── startup.cs      → Middleware + DI kayıtları (BaseStartup'tan türer)
├── appsettings.json
└── Dockerfile
```

### 5. Framework Core (Shared Library)
- **İçerik:** Tüm servisler tarafından paylaşılan base class'lar ve utility'ler
- **Paketleme:** NuGet paketi veya project reference olarak dağıtılır
- **Kural:** Domain-agnostic; herhangi bir domain'e özgü kod içermez

```
framework-core/
├── bases/
│   ├── base-entity/        → Id, CreatedAt, UpdatedAt gibi ortak alanlar
│   ├── base-repository/    → Generic CRUD + Bulk operasyonlar
│   ├── base-unit-of-work/  → Transaction yönetimi
│   ├── base-service/       → Ortak service davranışları
│   ├── base-startup/       → Ortak middleware kayıtları
│   └── base-dto/           → Ortak DTO sınıfları (paging, result wrapper)
├── wrapper-core/           → Result<T> pattern — HTTP yanıt standardı
├── filter-attribute-core/  → Request/Response logging filter
├── problem-detail-core/    → RFC 7807 hata formatı
└── utils/                  → Helper metotlar, extension'lar
```

---

## Dependency Flow (Bağımlılık Yönü)

```
Presentation → Application → Domain ← Persistence
                    ↑
              Framework Core
         (hepsi bu katmana bağımlı)
```

---

## Event-Driven Mimari

Servisler arası iletişim doğrudan HTTP yerine message bus üzerinden sağlanır:

```
Service A ─→ Publish Event ─→ [Message Bus] ─→ Consumer ─→ Service B
```

- Her servisin `application/message-bus/consumers/` klasöründe event consumer'ları bulunur
- Consumer'lar arka planda çalışır (hosted service veya worker)
- Event schema'ları `shared/shared-domain/` altında paylaşılır

---

## Servisler Arası Doğrudan HTTP İletişimi

Bazı işlemler için senkron HTTP çağrısı gerektiğinde `Refit` kullanılır:

```csharp
// Interface tanımla (Application katmanında)
public interface IServiceBClient
{
    [Post("/v1/Resource/GetById")]
    Task<Result<ResourceDto>> GetById(IdRequest request);
}

// Autofac ile kaydet ve inject et
```

---

## Cross-Cutting Concerns

| Concern | Yaklaşım |
|---------|---------|
| Logging | `RequestResponseFilterAttribute` (action filter) |
| Exception handling | `ProblemDetails` middleware |
| Validation | `FluentValidation` (application katmanında) |
| Caching | Service katmanında, repository üzerinde |
| Transaction | `UnitOfWork` + interceptor |
| Auth/AuthZ | JWT token, `InnerRequestAttribute` (servis kısıtlaması) |

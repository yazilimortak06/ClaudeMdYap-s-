# Generic Microservice Backend — Klasör Yapısı (Domain-soyutlanmış)

## Genel Bakış
.NET 5 tabanlı microservice backend. Ortak framework katmanı + birden fazla bağımsız servis içerir.

---

## framework/core/

```
framework/
└── core/
    └── framework-core/
        ├── bases/
        │   ├── base-attribute/
        │   ├── base-data-seeding/
        │   ├── base-dtos/
        │   ├── base-entities/
        │   ├── base-repository/
        │   ├── base-services/
        │   ├── base-token/
        │   ├── base-unit-of-work/
        │   └── startup-base/
        ├── core/
        │   ├── api/
        │   ├── data-properties/
        │   ├── enums/
        │   ├── extensions/
        │   ├── filter-attribute-core/
        │   ├── problem-detail-core/
        │   ├── repository/
        │   ├── unit-of-work-core/
        │   └── wrapper-core/
        └── utils/
            ├── entity-utils/
            ├── interface/
            ├── models/
            └── services/
```

---

## src/

### presentation/ (API Katmanı)

Her API: `controllers/`, `program.cs`, `startup.cs`, `appsettings.json`, `Dockerfile`

```
src/
└── presentation/
    ├── payment.api/          ← (domain: bank/ödeme)
    ├── web.api/              ← (domain: web portal)
    ├── mobile.api/           ← (domain: mobil uygulama)
    ├── logging.api/          ← (domain: log servisi)
    ├── notification.api/     ← (domain: bildirim)
    ├── background.api/       ← (domain: worker service API)
    ├── file.api/             ← (domain: dosya yönetimi)
    ├── messaging.api/        ← (domain: mail/SMS)
    ├── gateway.api/          ← (domain: API gateway)
    ├── geo-service.api/      ← (domain: harita/konum servisi)
    ├── integration.api/      ← (domain: 3. parti entegrasyon)
    ├── resource.api/         ← (domain: şarj istasyonu/kaynak)
    ├── protocol.api/         ← (domain: OCPP protokolü)
    ├── device.api/           ← (domain: VM/cihaz yönetimi)
    ├── auth.api/             ← (domain: token/kimlik doğrulama)
    └── background-worker/    ← (domain: arka plan worker)
```

### core/applications/

Her Application: `services/`, `filters/`, `extensions/`, `interceptors/`, `message-bus/consumers/`

```
src/
└── core/
    └── applications/
        ├── payment.application/
        ├── web.application/
        ├── portal.application/
        ├── file.application/
        ├── logging.application/
        ├── messaging.application/
        ├── notification.application/
        ├── auth.application/
        ├── background.application/
        ├── geo-service.application/
        ├── integration.application/
        ├── device.application/
        ├── protocol.application/
        ├── resource.application/
        ├── admin.application/
        ├── firm-integration.application/
        └── firm-integration-worker.application/
```

### core/persistences/

Her Persistence: `db-context/`, `entity-config/`, `migrations/`, `repositories/`

```
src/
└── core/
    └── persistences/
        ├── payment.persistence/
        ├── web.persistence/
        ├── portal.persistence/
        ├── file.persistence/
        ├── logging.persistence/
        ├── messaging.persistence/
        ├── notification.persistence/
        ├── auth.persistence/
        ├── background.persistence/
        ├── geo-service.persistence/
        ├── integration.persistence/
        ├── device.persistence/
        ├── protocol.persistence/
        ├── resource.persistence/
        └── admin.persistence/
```

### shared/

```
src/
└── shared/
    └── shared.domain/
```

### external-integration/ (Ayrı Alt Sistem)

```
src/
└── external-integration/
    ├── presentation/
    │   ├── integration.api/
    │   └── integration-worker/
    ├── applications/
    │   ├── integration.application/
    │   └── integration-worker.application/
    ├── domains/
    │   ├── integration.domain/
    │   └── integration-worker.domain/
    └── persistences/
        ├── integration.persistence/
        └── integration-worker.persistence/
```

---

## Katman Özeti

| Katman | Rol |
|--------|-----|
| `framework/core/` | Paylaşılan base class'lar, utility'ler |
| `presentation/` | HTTP API endpoint'leri (controller'lar) |
| `core/applications/` | İş mantığı, service implementasyonları |
| `core/persistences/` | Veritabanı erişimi, repository'ler |
| `shared/` | Tüm servisler arası paylaşılan domain nesneleri |
| `external-integration/` | 3. parti entegrasyon alt sistemi |

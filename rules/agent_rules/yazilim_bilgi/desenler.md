# Yazılım Tasarım Desenleri

Agent bu dosyayı okuyarak hangi problemde hangi deseni kullanacağını öğrenir.

---

## Creational (Oluşturucu) Desenler

### Singleton
Bir sınıftan yalnızca bir instance oluşturulur.
- **Ne zaman:** DB bağlantısı, config, logger, cache manager
- **Dikkat:** Global state yaratır, test edilebilirliği zorlaştırır. DI container ile yönet.

### Factory Method
Nesne oluşturma mantığını alt sınıflara bırakır.
- **Ne zaman:** Hangi nesnenin oluşturulacağı runtime'da belli olacaksa.
- **Örnek:** `NotificationFactory.create("email")` → `EmailNotification`

### Abstract Factory
Birbiriyle ilişkili nesne ailelerini oluşturur.
- **Ne zaman:** Birden fazla varyant olan bir nesne grubu varsa (örn: dark/light theme bileşenleri)

### Builder
Karmaşık nesneyi adım adım inşa eder.
- **Ne zaman:** Çok parametreli constructor yerine. Opsiyonel alanlar çoksa.
- **Örnek:** `QueryBuilder`, `HttpRequestBuilder`

---

## Structural (Yapısal) Desenler

### Repository
Veri erişimini soyutlar, iş mantığını DB'den ayırır.
- **Ne zaman:** Her zaman. Veri kaynağına (SQL, NoSQL, API) bağımsız servis katmanı için.

### Adapter
Uyumsuz interface'leri uyumlu hale getirir.
- **Ne zaman:** 3rd party kütüphane veya dış servis interface'i sisteminizle uyuşmuyorsa.

### Decorator
Nesneye davranış ekler — sınıfı değiştirmeden.
- **Ne zaman:** Logging, caching, validation, auth middleware
- **Örnek:** `CachedUserRepository` → `UserRepository` i wrap eder, sonuçları cache'ler.

### Facade
Karmaşık bir sisteme basit bir interface sunar.
- **Ne zaman:** Birden fazla servis/modülü koordine eden üst seviye bir operasyon varsa.
- **Örnek:** `OrderFacade.placeOrder()` → inventory, payment, notification servislerini koordine eder.

### Proxy
Gerçek nesneye erişimi kontrol eder.
- **Ne zaman:** Lazy loading, access control, logging, caching

---

## Behavioral (Davranışsal) Desenler

### Strategy
Algoritma ailesini tanımlar, runtime'da değiştirilebilir yapar.
- **Ne zaman:** Aynı işin birden fazla yapılış biçimi varsa.
- **Örnek:** `PaymentStrategy` → `CreditCardPayment`, `CashPayment`, `OnlinePayment`

### Observer
Bir nesne değişince bağımlılarını otomatik bilgilendirir.
- **Ne zaman:** Event-driven sistemler, domain events, pub/sub.
- **Örnek:** Sipariş verilince stock servisi, notification servisi, analytics servisi tetiklenir.

### Command
İşlemi nesne olarak kapsüller — undo/redo, queue, log için kullanılır.
- **Ne zaman:** İşlem geçmişi, undo/redo, task queue

### Chain of Responsibility
İsteği bir zincirden geçirir, uygun halka işler.
- **Ne zaman:** Middleware pipeline, validation zinciri, auth kontrolleri.

### Template Method
Algoritmanın iskeletini tanımlar, detayları alt sınıflara bırakır.
- **Ne zaman:** Aynı akış, farklı adım implementasyonları.

---

## Architectural (Mimari) Desenler

### MVC — Model View Controller
- **Model:** Veri ve iş mantığı
- **View:** Sunum katmanı
- **Controller:** Model ve View arasındaki koordinatör
- **Ne zaman:** Web uygulamaları

### MVVM — Model View ViewModel
- ViewModel, View'in ihtiyaç duyduğu veriyi hazırlar — View'den bağımsız test edilebilir.
- **Ne zaman:** Angular, React, mobile uygulamalar

### CQRS — Command Query Responsibility Segregation
Okuma (Query) ve yazma (Command) işlemlerini ayırır.
- **Ne zaman:** Okuma ve yazma yükü çok farklıysa, karmaşık query ihtiyacı varsa.

### Event Sourcing
State'i direkt kaydetmek yerine olayları (event) kaydeder. State olaylardan türetilir.
- **Ne zaman:** Audit log kritikse, geçmişe dönük analiz gerekiyorsa.

### Saga Pattern
Dağıtık sistemlerde uzun süreli transaction yönetimi.
- **Ne zaman:** Microservice'ler arası transaction (ödeme → stok → kargo)

---

## Ne Zaman Hangi Desen?

| Problem | Desen |
|---------|-------|
| Nesne oluşturma karmaşıklaştı | Factory, Builder |
| Birden fazla algoritma varyantı | Strategy |
| Bağımlı nesneleri haberdar et | Observer |
| Middleware zinciri kur | Chain of Responsibility |
| Veri erişimini soyutla | Repository |
| Davranış ekle, sınıfı değiştirme | Decorator |
| Karmaşık sistemi basitleştir | Facade |
| Uyumsuz interface'leri birleştir | Adapter |

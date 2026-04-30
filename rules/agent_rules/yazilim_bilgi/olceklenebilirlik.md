# Ölçeklenebilirlik Rehberi

Agent bu dosyayı okuyarak ölçeklenebilir mimari kararlarını nasıl vereceğini öğrenir.

---

## Temel Kavramlar

### Horizontal vs Vertical Scaling
- **Vertical (Scale-Up):** Makineyi güçlendir (daha fazla RAM/CPU). Limiti var, single point of failure.
- **Horizontal (Scale-Out):** Daha fazla instance ekle. Tercih edilmeli — sınırsız büyüme, yük dağılımı.
- **Tasarım gereği:** Her servis stateless olmalı — horizontal scaling için zorunlu.

### Stateless Tasarım
- Servis, istek arasında session state tutmamalı.
- State'i dışarıda tut: Redis (session), DB (kalıcı veri).
- Herhangi bir instance herhangi bir isteği karşılayabilmeli.

---

## Load Balancing
- Trafiği birden fazla instance'a dağıtır.
- Algoritma: Round Robin, Least Connections, IP Hash.
- Health check: sağlıksız instance'a trafik gönderme.
- **Session Sticky:** Mümkünse kaçın — stateless tasarımla gereksiz hale gelir.

---

## Veritabanı Ölçekleme

### Read Replica
- Okuma yoğun uygulamalarda: write → master, read → replica.
- Replica lag'ı izle — kritik okumalar master'dan yapılmalı.

### Connection Pooling
- Her servis instance'ı DB'ye doğrudan bağlanmaz — pool manager üzerinden (PgBouncer, HikariCP).

### Sharding
- Veriyi yatay böl — her shard farklı sunucuda.
- Karmaşık: join sorguları zorlaşır. İhtiyaç olmadan uygulanmaz.
- Önce: index optimizasyonu → read replica → cache → sonra sharding.

### Database per Service (Microservice için)
- Her microservice kendi DB'sine sahip — başkasının DB'sine direkt bağlanmaz.
- Servisler arası veri paylaşımı API üzerinden.

---

## Caching (Ölçeklenebilirlik Boyutu)

- DB yükünü azaltır — daha az instance ile daha fazla trafik karşılanır.
- Redis cluster: distributed cache, horizontal scaling.
- CDN: statik içerik ve public response cache — origin sunucuyu korur.

---

## Message Queue & Async Processing

- Ani yük artışlarında buffer görevi görür — spike'ları absorbe eder.
- Producer hızlı yazıp devam eder, consumer kendi hızında işler.
- Dead Letter Queue (DLQ): işlenemeyen mesajları kayıt altına al, kaybetme.
- **Idempotency:** Mesaj iki kez işlenirse sonuç aynı olmalı (at-least-once delivery).

---

## Microservice vs Monolith

### Monolith ile Başla
- Küçük takım, erken aşama → monolith daha hızlı geliştirme.
- Sınırlar monolith içinde modüler tutulursa gerektiğinde servis ayrılabilir.

### Ne Zaman Microservice?
- Farklı componentler çok farklı ölçekleme ihtiyacı duyduğunda.
- Takımlar bağımsız deploy etmek istediğinde.
- Servis sınırları netleştiğinde.

### Microservice Dikkat Noktaları
- Network latency artar — senkron çağrı zinciri kurmaktan kaçın.
- Distributed transaction zor — Saga pattern kullan.
- Observability zorunlu: distributed tracing (Jaeger, Zipkin), log aggregation.
- API Gateway: tek giriş noktası, auth, rate limit, routing.

---

## API Tasarımı (Ölçeklenebilirlik için)

- **Versioning:** `/api/v1/` — breaking change'ler yeni versiyona.
- **Idempotent endpoint'ler:** Aynı isteği N kez göndermek güvenli olmalı (özellikle POST için idempotency key).
- **Pagination zorunlu:** Sonsuz liste dönme.
- **Async operasyonlar:** Uzun işlem → job ID dön, client poll etsin veya webhook.

---

## Ölçeklenebilirlik Kontrol Sırası

Sorun büyüdükçe şu sırayla çöz — hepsini baştan yapma:

1. Index optimizasyonu
2. Query optimizasyonu
3. Caching (Redis)
4. Read replica
5. Async/queue mimarisi
6. Horizontal scaling (stateless servis)
7. Sharding / database ayrıştırma
8. Microservice mimarisine geçiş

# Performans Rehberi

Agent bu dosyayı okuyarak performans sorunlarını nasıl önleyeceğini ve çözeceğini öğrenir.

---

## Veritabanı Performansı

### N+1 Problemi — En Yaygın Hata
Bir liste çekip her eleman için ayrı query atmak.
```
# Yanlış — 1 + N query
orders = Order.findAll()
orders.forEach(o => o.user = User.findById(o.userId))  // N adet extra query

# Doğru — JOIN veya eager loading
orders = Order.findAll({ include: ['user'] })  // tek query
```
**Her zaman:** ORM kullanırken ilişkili veriyi eager load et. Query sayısını logla.

### İndeksleme
- WHERE, JOIN, ORDER BY, GROUP BY kolonlarına index ekle.
- Foreign key'lere index ekle.
- Composite index: sık birlikte kullanılan kolonlar için.
- **Aşırı index ekleme:** Write operasyonlarını yavaşlatır — sadece gerekli index.
- Sorgu planını incele: `EXPLAIN ANALYZE` (PostgreSQL), `EXPLAIN` (MySQL).

### Query Optimizasyonu
- `SELECT *` yerine sadece ihtiyaç duyulan kolonları çek.
- Büyük veri setlerinde `LIMIT` ve `OFFSET` yerine cursor tabanlı pagination kullan.
- Subquery yerine JOIN tercih et.
- Büyük işlemlerde transaction içinde batch işle.

### Connection Pooling
- DB bağlantısını her request'te aç/kapat — performans katili.
- Connection pool kullan (min: 5, max: 20 — yük testine göre ayarla).
- Pool tükenmesini izle — bekleme süresi artarsa pool büyüt veya query'leri optimize et.

---

## Caching Stratejileri

### Ne Zaman Cache?
- Sık okunan, az değişen veri (menü listesi, kategori listesi, config)
- Hesaplama maliyeti yüksek veri (rapor, aggregate)
- Dış servis çağrıları (3rd party API)

### Cache Katmanları
1. **In-memory (uygulama içi):** En hızlı. Küçük, sık erişilen veri. Dikkat: horizontal scaling'de her instance ayrı cache tutar.
2. **Redis / Memcached:** Dağıtık cache. Tüm instance'lar aynı cache'i kullanır. Session, rate limiting, distributed lock.
3. **CDN:** Statik dosyalar, public API response'ları.
4. **DB Query Cache:** ORM seviyesinde veya DB seviyesinde.

### Cache Stratejileri
- **Cache-Aside:** Önce cache bak, yoksa DB'den al, cache'e yaz.
- **Write-Through:** DB'ye yazarken aynı anda cache'e de yaz.
- **Write-Behind:** Önce cache'e yaz, async DB'ye flush.
- **TTL:** Her cache entry için uygun süre belirle — eski veri dönme riskini yönet.

### Cache Invalidation
- Veri güncellenince ilgili cache'i geçersiz kıl.
- Versioned cache key: `user:123:v2` — veri değişince version artır.
- **Dikkat:** Cache invalidation yanlışsa stale data dönebilir — kritik veride cache süresi kısa tut.

---

## Asenkron & Concurrency

### Ne Zaman Async?
- I/O operasyonları: DB, dosya okuma, dış API çağrısı — her zaman async.
- CPU-bound işlemler farklı thread/worker'a gönder.
- Kullanıcıyı bekletme: uzun işlemleri arka plana at, job queue kullan.

### Message Queue / Job Queue
- Email gönderme, bildirim, rapor üretme, resim işleme → queue'ya at, worker işlesin.
- Faydaları: kullanıcı anında yanıt alır, retry mekanizması, yük dağılımı.
- Araçlar: Redis (BullMQ), RabbitMQ, Kafka (yüksek throughput için).

### Race Condition
- Aynı kaynağa concurrent yazma — mutex/lock kullan.
- DB seviyesinde: SELECT FOR UPDATE, optimistic locking (version field).
- Distributed lock: Redis SETNX.

---

## API Performansı

### Pagination
- Hiçbir zaman tüm listeyi döndürme.
- Cursor tabanlı pagination (keyset pagination) büyük veri setleri için offset'ten daha hızlı.
- `limit` parametresine max değer koy (örn: max 100).

### Response Optimizasyonu
- Sadece ihtiyaç duyulan alanları döndür.
- Nested objeler gereksizse flat tut.
- Büyük response'lar için compression (gzip/brotli) aktif et.
- HTTP/2 kullan — multiplexing.

### Rate Limiting & Throttling
- Hem kullanıcıyı hem sistemi korur.
- Sliding window veya token bucket algoritması.

---

## Performans İzleme

- Slow query log aktif et — threshold belirle (örn: 100ms üstü logla).
- APM aracı kullan (response time, throughput, error rate).
- Her endpoint'in p95/p99 response süresini izle.
- Memory leak tespiti: memory kullanımı sürekli artıyorsa araştır.

---

## Checklist — Feature Yazarken Sor

- [ ] N+1 query var mı?
- [ ] Büyük liste döndürüyor mu? Pagination eklendi mi?
- [ ] Sık erişilen veri cache'lendi mi?
- [ ] Uzun süren işlem async'e alındı mı?
- [ ] DB kolonlarına gerekli index eklendi mi?
- [ ] `SELECT *` kullanıldı mı?

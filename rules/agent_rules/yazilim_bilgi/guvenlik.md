# Güvenlik Rehberi

Agent bu dosyayı okuyarak güvenli kod yazma standartlarını öğrenir. Her geliştirmede bu kurallar kontrol edilmeli.

---

## OWASP Top 10 — Bilinmesi Zorunlu

### 1. Injection (SQL, NoSQL, Command)
- **Asla** string concatenation ile SQL oluşturma.
- Her zaman parameterized query / prepared statement kullan.
- ORM kullanıyorsan raw query yazmaktan kaçın; zorunluysa parametreli yaz.
```
# Yanlış
query = "SELECT * FROM users WHERE id = " + userId

# Doğru
query = "SELECT * FROM users WHERE id = ?"  // parametre ayrı geçilir
```

### 2. Broken Authentication
- Şifreyi plain text kaydetme — `bcrypt`, `argon2` kullan (minimum cost: 12).
- Token'ları güvenli yerde sakla (httpOnly cookie, güvenli storage).
- JWT secret'ı güçlü tut, env variable'dan al, kod içine yazma.
- Token expiry her zaman kısa tut (access: 15dk, refresh: 7-30 gün).
- Brute force koruması: rate limiting + account lockout.

### 3. Sensitive Data Exposure
- Şifre, kart no, TC kimlik gibi veriler log'a yazılmaz.
- Response'da asla şifre hash'i bile gönderme.
- HTTPS zorunlu — HTTP redirect et.
- DB'de hassas veriyi encrypt et.

### 4. Broken Access Control
- Her endpoint'te yetki kontrolü yap — "bu kullanıcı bu kaynağa erişebilir mi?"
- IDOR'a karşı: kaynak ID'sini alırken o kaynağın gerçekten bu kullanıcıya ait olduğunu kontrol et.
- Role bazlı (RBAC) veya attribute bazlı (ABAC) yetkilendirme uygula.
- Admin endpoint'leri ayrı korunmalı.

### 5. Security Misconfiguration
- Debug mode production'da kapalı olmalı.
- Stack trace kullanıcıya gösterilmemeli — generic error message dön.
- Default şifre, default credential kullanma.
- Gereksiz port, servis, endpoint kapatılmalı.
- CORS whitelist — `*` kullanma.

### 6. XSS — Cross Site Scripting
- Kullanıcıdan gelen her veriyi HTML'e yazmadan önce escape et.
- Content Security Policy (CSP) header'ı ekle.
- Frontend'de `innerHTML` yerine `textContent` kullan.
- Cookie'lere `httpOnly` ve `SameSite` flag'i koy.

### 7. Insecure Deserialization
- Güvenilmeyen kaynaklardan gelen serialize edilmiş veriyi doğrudan deserialize etme.
- Input validation + type checking uygula.

### 8. Using Components with Known Vulnerabilities
- Dependency'leri düzenli güncelle.
- `npm audit`, güvenlik tarama araçları kullan.
- Kullanılmayan dependency'leri kaldır.

### 9. Insufficient Logging & Monitoring
- Auth başarısız, yetki ihlali, kritik hatalar mutlaka loglanmalı.
- Log'da şifre, token, kart no olmamalı.
- Alert mekanizması kur — anomali tespiti.

### 10. SSRF — Server Side Request Forgery
- Kullanıcıdan URL alıp server'dan istek atıyorsan whitelist kontrol et.
- Internal IP / localhost'a istek gitmesini engelle.

---

## Authentication & Authorization

### JWT Best Practices
- Secret key en az 256 bit, env'den alınmalı.
- Payload'a hassas veri koyma — decode edilebilir.
- `exp` (expiry) her zaman koy.
- Refresh token rotation uygula — kullanıldığında yenisini ver, eskisini geçersiz kıl.
- Token blacklist için Redis kullan (logout, şifre değişimi).

### Password Güvenliği
- bcrypt ile hash (cost factor ≥ 12).
- Minimum 8 karakter, büyük/küçük/rakam/özel karakter zorla.
- Şifre sıfırlamada token tabanlı flow (email ile), token kısa süreli olmalı (15-30 dk).
- Şifre geçmişi — son N şifre tekrar kullanılamasın.

### Rate Limiting
- Login endpoint'ine agresif rate limit (örn: 5 deneme / dakika / IP).
- API genel rate limit (örn: 100 istek / dakika / kullanıcı).
- Distributed sistemde Redis tabanlı rate limiter kullan.

---

## Input Validation

- Her input sunucu tarafında validate edilmeli — client validation yeterli değil.
- Tip, uzunluk, format, izin verilen değerler kontrol edilmeli.
- Whitelist yaklaşımı: izin verilenler dışındaki her şeyi reddet.
- File upload: tip kontrolü (magic bytes), boyut limiti, dosyayı web root dışına kaydet, execute izni verme.

---

## API Güvenliği

- Tüm endpoint'ler authentication gerektirmeli (public olanlar açıkça işaretlenmeli).
- API versioning yap — eski versiyonları zamanında kapat.
- Sensitive endpoint'lerde HTTPS zorunlu.
- Response header'larında güvenlik header'ları: `X-Content-Type-Options`, `X-Frame-Options`, `Strict-Transport-Security`.
- Swagger/API docs production'da kapalı veya korumalı olmalı.

---

## Checklist — Her Feature'da Sor

- [ ] Bu endpoint'e kim erişebilir? Yetki kontrolü var mı?
- [ ] Kullanıcıdan gelen her veri validate ediliyor mu?
- [ ] DB sorgusu parameterized mı?
- [ ] Hassas veri log'a yazılıyor mu?
- [ ] Error response stack trace içeriyor mu?
- [ ] Token/şifre response'da dönüyor mu?

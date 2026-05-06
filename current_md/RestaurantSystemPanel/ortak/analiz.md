# RestaurantSystemPanel — Kesinleşmiş Analizler

> Bu dosya doğrudan düzenlenmez. Sadece `analiz_rules.md` akışıyla güncellenir.
> Buradaki her madde tüm ekip için bağlayıcıdır.

---

## 2026-05-05 — Auth & Oturum Yapısı (Tüm Ekip İçin Bağlayıcı)

### Oturum Yönetimi

- **Access token** memory'de (Angular service) tutulur. localStorage'a yazılmaz.
- **Refresh token** httpOnly cookie veya localStorage'da tutulur (güvenlik tercihi: httpOnly cookie önerilir, backend bunu set eder).
- Access token süresi dolunca otomatik `POST /api/auth/refresh` çağrısı yapılır (interceptor ile).
- Refresh token da geçersizse kullanıcı login sayfasına yönlendirilir.

### Sayfalar (İlk Faz)

| Sayfa | Route | Açıklama |
|-------|-------|----------|
| Login | `/login` | Email + şifre formu, "Beni hatırla" checkbox'ı |
| Dashboard | `/dashboard` | Giriş sonrası ana ekran, kullanıcı bilgisi + çıkış butonu |

### Auth Guard

- Login olmadan `/dashboard` ve diğer korumalı route'lara erişilemez.
- Token yoksa veya geçersizse `/login`'e yönlendir.

### "Beni Hatırla"

- Checkbox işaretliyse login isteğinde `rememberMe: true` gönderilir.
- Backend refresh token süresini buna göre ayarlar (1 gün vs 30 gün).


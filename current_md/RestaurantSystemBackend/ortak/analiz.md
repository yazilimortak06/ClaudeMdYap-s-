# RestaurantSystemBackend — Kesinleşmiş Analizler

> Bu dosya doğrudan düzenlenmez. Sadece `analiz_rules.md` akışıyla güncellenir.
> Buradaki her madde tüm ekip için bağlayıcıdır.

---

## 2026-05-05 — Auth & Oturum Yapısı (Tüm Ekip İçin Bağlayıcı)

### Token Stratejisi

- **Access Token:** JWT, kısa ömürlü (15 dakika). Claim'lerde `userId`, `tenantId`, `branchId`, `roles` taşır.
- **Refresh Token:** Opak string, DB'de saklanır (`Token.Persistence.RefreshToken`), tek kullanımlık rotation. `ReplacedByToken` ile zincir takibi yapılır.
- **"Beni Hatırla" açık:** Refresh token süresi 30 gün. Kapalı: 1 gün.
- **Çoklu cihaz:** Bir kullanıcının birden fazla aktif refresh token'ı olabilir (cihaz başına ayrı kayıt).
- **Logout:** Sadece ilgili cihazın refresh token'ı revoke edilir. Diğer cihazlar etkilenmez.
- **Tüm cihazlardan çıkış:** Kullanıcıya ait tüm refresh token'lar revoke edilir.

### Login

- **Tek endpoint:** `POST /api/auth/login` — kullanıcı tipi farkı token claim'leriyle belirlenir.
- **Kimlik bilgisi:** Email + şifre (sosyal login yok).
- **Şifre hashleme:** bcrypt.
- **Yanıt:** `accessToken`, `refreshToken`, `expiresIn`.

### Kullanıcı Tipleri ve Claim Farkı

| Tip | tenantId | branchId | Örnek Rol |
|-----|----------|----------|-----------|
| Platform Super Admin | null | null | `super_admin` |
| Tenant Admin | var | null | `tenant_admin` |
| Branch User | var | var | `branch_staff` |

### Şifre Sıfırlama

- **Şimdilik yapılmayacak.** İleride email tabanlı eklenecek.

### Endpoint Listesi

| Method | Path | Açıklama |
|--------|------|----------|
| POST | `/api/auth/login` | Giriş — access + refresh token döner |
| POST | `/api/auth/refresh` | Access token yenile |
| POST | `/api/auth/logout` | Mevcut cihazdan çıkış |
| POST | `/api/auth/logout-all` | Tüm cihazlardan çıkış |


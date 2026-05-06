# RestaurantSystemPanel — Ortak

Her commit/push sonrası güncellenir.

## Son Değişiklikler

### 2026-05-06 — Auth Ekranları (ali)
- `AuthService`, `AuthInterceptor`, `AuthGuard`, `NoAuthGuard` yazıldı.
- `APP_INITIALIZER` ile sayfa yenileme sonrası oturum geri yükleme.
- Login sayfası: kullanıcı adı + şifre + "Beni Hatırla" formu.
- Dashboard sayfası: kullanıcı bilgisi + çıkış butonu.
- `ToastService` + `ToastComponent`: sağ üst köşe toast bildirimleri.
- CORS sorunu nedeniyle `AuthInterceptor`'da `Injector` lazy injection uygulandı.
- "Beni hatırla" mantığı: her iki durumda da localStorage, fark backend token süresi.
- `environment.ts`: `apiUrl = http://localhost:44302/api/v1`
- Angular build: 0 hata.

## Genel İlerleme

- [x] Auth ekranları (login, dashboard)
- [x] Oturum yönetimi (localStorage, APP_INITIALIZER, guard'lar)
- [x] Toast bildirim sistemi
- [x] Lazy load routing
- [ ] Diğer yönetim ekranları (tenant, branch, kullanıcı vs.)

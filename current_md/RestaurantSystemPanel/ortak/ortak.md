# RestaurantSystemPanel — Ortak Durum

Her commit/push sonrası güncellenir. Genel ilerleme ve son değişiklikler.

## Son Güncelleme
(Henüz güncelleme yok — proje başlangıç aşamasında)

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
- [ ] Altyapı kurulumu
- [ ] Temel CRUD endpoint'leri
- [ ] Auth entegrasyonu
- [ ] SignalR hub
- [ ] Docker ortamı

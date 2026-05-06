# ali — RestaurantSystemPanel

## Çalışılan Modül / Konu

Auth Ekranları — tamamlandı.

## Son Yaptıkları

- `AuthService` (localStorage, rememberMe → token süresi farkı)
- `AuthInterceptor` (Injector ile circular dep çözüldü, 401 otomatik refresh)
- `AuthGuard` (Observable, fallback refresh), `NoAuthGuard` (/login → /dashboard)
- `APP_INITIALIZER` ile sayfa yenileme oturum restore
- Login sayfası: kullanıcı adı + şifre + "Beni Hatırla"
- Dashboard sayfası: kullanıcı bilgisi + çıkış
- `ToastService` + `ToastComponent`: sağ üst toast bildirimleri
- `environment.ts`: Token.Api → http://localhost:44302/api/v1

## Sıradaki

- Diğer yönetim ekranları (tenant, branch, kullanıcı yönetimi)

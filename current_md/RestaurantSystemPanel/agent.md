# RestaurantSystemPanel — Agent Bağlamı

## Proje Bağlamı
Angular admin panel uygulaması. Backend API'yi tüketerek restoran yönetim arayüzünü sunar.
Tech: Angular | TypeScript | Angular Material | RxJS | SignalR Client | JWT

Angular tabanlı restoran SaaS yönetici paneli. Backend: RestaurantSystemBackend (Token.Api üzerinden auth).

## Aktif Görev
(Henüz görev alınmadı — havuz.md'den görev seçilmesi bekleniyor)

## Son Çalışma Notları

**Aktif görev yok — auth ekranları tamamlandı.**

**Tamamlanan — Auth Ekranları**

- `AuthService` — login, logout, refresh, localStorage token yönetimi
- `AuthInterceptor` — Bearer header, 401'de otomatik refresh (Injector ile circular dep çözüldü)
- `AuthGuard` — Observable tabanlı, fallback refresh desteği
- `NoAuthGuard` — oturum açıksa /login → /dashboard yönlendirir
- `APP_INITIALIZER` — sayfa yenilenince localStorage token ile oturum geri yüklenir
- Login sayfası (`/login`) — kullanıcı adı + şifre + "Beni Hatırla"
- Dashboard sayfası (`/dashboard`) — kullanıcı bilgisi + çıkış
- `ToastService` + `ToastComponent` — sağ üst köşe bildirim sistemi
- Routing — lazy load, guard entegrasyonu
- "Beni hatırla": fark sadece backend token süresi (1 gün vs 30 gün), ikisi de localStorage

## Bağlı Projeler ile Paylaşılan
- Shared klasör: `shared/RestaurantSystemBackend--RestaurantSystemPanel/`

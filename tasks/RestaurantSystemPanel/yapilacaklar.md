# RestaurantSystemPanel — Yapılacaklar

Aktif olarak üzerinde çalışılan veya çalışılacak görevler burada tutulur.

---

## Auth Ekranları — TAMAMLANDI

| # | Görev | Durum | Sorumlu |
|---|-------|-------|---------|
| 1 | `AuthService` — login, logout, token yönetimi (localStorage) | ✅ tamamlandı | ali |
| 2 | `AuthInterceptor` — Bearer header + 401 otomatik refresh (Injector) | ✅ tamamlandı | ali |
| 3 | `AuthGuard` — Observable tabanlı, fallback refresh | ✅ tamamlandı | ali |
| 4 | `NoAuthGuard` — oturum açıksa /login → /dashboard | ✅ tamamlandı | ali |
| 5 | `APP_INITIALIZER` — sayfa yenileme oturum restore | ✅ tamamlandı | ali |
| 6 | Login sayfası (`/login`) — kullanıcı adı + şifre + "Beni Hatırla" | ✅ tamamlandı | ali |
| 7 | Dashboard sayfası (`/dashboard`) — kullanıcı bilgisi + çıkış | ✅ tamamlandı | ali |
| 8 | `ToastService` + `ToastComponent` — bildirim sistemi | ✅ tamamlandı | ali |
| 9 | Routing — lazy load + guard entegrasyonu | ✅ tamamlandı | ali |

## Sıradaki

| # | Görev | Durum |
|---|-------|-------|
| 1 | Diğer yönetim ekranları (tenant, branch, kullanıcı yönetimi) | bekliyor |


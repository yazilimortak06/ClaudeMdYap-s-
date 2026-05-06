# Shared — RestaurantSystemBackend & RestaurantSystemPanel

Her iki projenin agentları bu klasörü okuyup yazabilir.

## Endpoint Tanımları

### Auth (Token.Api)

| Method | Path | Request | Response |
|--------|------|---------|----------|
| POST | `/api/auth/login` | `{ email, password, rememberMe }` | `{ accessToken, refreshToken, expiresIn, user: { id, fullName, email, tenantId, branchId, roles } }` |
| POST | `/api/auth/refresh` | `{ refreshToken }` | `{ accessToken, refreshToken, expiresIn }` |
| POST | `/api/auth/logout` | `{ refreshToken }` | `200 OK` |
| POST | `/api/auth/logout-all` | _(Authorization header yeterli)_ | `200 OK` |

## Kontratlar / Arayüz Anlaşmaları

- Access token süresi: **15 dakika**
- Refresh token süresi: `rememberMe=false` → **1 gün**, `rememberMe=true` → **30 gün**
- Access token taşıma: `Authorization: Bearer <token>` header
- 401 alındığında Panel otomatik refresh dener; refresh da başarısızsa `/login`'e yönlendirir
- Roller token claim'inde `roles` array olarak gelir

## Notlar

- Token.Api auth endpoint'lerinin host/portu `.env` ile yönetilecek (Panel tarafında environment config)


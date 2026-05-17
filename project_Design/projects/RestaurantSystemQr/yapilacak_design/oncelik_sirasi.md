# Öncelik Sırası — RestaurantSystemQr

---

## Ekran Listesi

| Öncelik | Ekran | Spec Dosyası | Durum | Atanan |
|---------|-------|-------------|-------|--------|
| P0 | QR Tarama / Giriş | `qr_tarama.md` | Bekliyor | - |
| P0 | Menü Listesi | `menu_listesi.md` | Bekliyor | - |
| P1 | Ürün Detay | `urun_detay.md` | Bekliyor | - |
| P1 | Kategori Filtresi | `kategori_filtresi.md` | Bekliyor | - |
| P2 | Sepet | `sepet.md` | Bekliyor | - |
| P2 | Sipariş Onay | `siparis_onay.md` | Bekliyor | - |
| P3 | Arama | `arama.md` | Bekliyor | - |

## Ortak Bileşenler

| Öncelik | Bileşen | Spec Dosyası | Durum |
|---------|---------|-------------|-------|
| P0 | Ürün Kartı | `_komp_urun_karti.md` | Bekliyor |
| P0 | Header (mobil) | `_komp_header.md` | Bekliyor |
| P1 | Kategori Filtresi | `_komp_kategori_filtresi.md` | Bekliyor |
| P2 | Sepet İkonu / Badge | `_komp_sepet_badge.md` | Bekliyor |

> **Agent Notu:** QR flow'un P0 ekranları doğrusal sıralıdır (tarama → liste → detay).
> Bu üçü olmadan uygulama anlamsız olur. Önce bunları tamamla.

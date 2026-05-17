# Öncelik Sırası — RestaurantSystemPanel

Geliştirici hangi ekranı önce implement edeceğini buradan öğrenir.
Tasarımcı veya analiz uzmanı bu listeyi günceller.

---

## Öncelik Sistemi

| Seviye | Anlamı |
|--------|--------|
| **P0 — Kritik** | Olmadan uygulama çalışmaz / teslim edilemez |
| **P1 — Yüksek** | Sprint'in temel hedefi |
| **P2 — Orta** | Sprint'te yapılırsa iyi olur |
| **P3 — Düşük** | Backlog'a atabiliriz |

---

## Ekran Listesi

| Öncelik | Ekran | Spec Dosyası | Durum | Atanan |
|---------|-------|-------------|-------|--------|
| P0 | Login | `login.md` | Bekliyor | - |
| P1 | Dashboard | `dashboard.md` | Bekliyor | - |
| P1 | Menü Yönetimi | `menu_yonetimi.md` | Bekliyor | - |
| P2 | Sipariş Listesi | `siparis_listesi.md` | Bekliyor | - |
| P2 | Masa Yönetimi | `masa_yonetimi.md` | Bekliyor | - |
| P3 | Ayarlar | `ayarlar.md` | Bekliyor | - |

> **Agent Notu:** Bu liste başlangıç taslağıdır. Analiz uzmanı gereksinim analizi tamamladıkça
> ekranlar eklenir / sıra değişir. Tasarım hazır olmadan P0/P1 haricinde geliştirme başlatma.

---

## Ortak Bileşenler (Ekrandan Bağımsız)

| Öncelik | Bileşen | Spec Dosyası | Durum |
|---------|---------|-------------|-------|
| P0 | Sidebar | `_komp_sidebar.md` | Bekliyor |
| P0 | Header | `_komp_header.md` | Bekliyor |
| P1 | Veri Tablosu | `_komp_veri_tablosu.md` | Bekliyor |
| P1 | Form Bileşenleri | `_komp_formlar.md` | Bekliyor |
| P2 | Grafik / Chart | `_komp_grafik.md` | Bekliyor |
| P2 | Bildirim / Toast | `_komp_bildirim.md` | Bekliyor |

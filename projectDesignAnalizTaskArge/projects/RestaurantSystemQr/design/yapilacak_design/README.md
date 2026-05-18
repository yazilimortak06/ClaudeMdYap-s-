# Yapılacak Design — RestaurantSystemQr

> **Geliştirici için:** Bu klasör, QR menü arayüzünü kodlamadan önce okuman gereken tek referans noktasıdır.
> Müşterinin telefonunda göreceği her ekran buraya spec olarak yazılmıştır.

---

## Bu Klasör Ne İçin?

`yapilacak_design/`, tasarımcı ile geliştirici arasındaki köprüdür.
RestaurantSystemQr müşteri yüzlü, mobil-öncelikli bir uygulamadır.
Tasarım kararları burada net ve uygulanabilir hale getirilir.

**Tasarımcı yazar → Geliştirici okur → Kod yazılır → Kabul kriterleri check edilir.**

---

## Dosya Adlandırma

Ekran bazlı: `<ekran_adi>.md`
Örn: `qr_tarama.md`, `menu_listesi.md`, `urun_detay.md`, `sepet.md`

Bileşen bazlı: `_komp_<isim>.md`
Örn: `_komp_urun_karti.md`, `_komp_kategori_filtresi.md`

---

## Ekran Spec Dosyası Şablonu

```markdown
# [Ekran Adı]

## Durum
[ ] Bekliyor  [ ] Tasarım Hazır  [ ] Geliştiriliyor  [x] Tamamlandı
Atanan Geliştirici: -
Tahmini Efor: -

## Tasarım Referansı
- Mevcut Design: project_Design/RestaurantSystemQr/mevcut_design/<dosya>
- Örnek: project_Design/RestaurantSystemQr/example_design_duzenlenmis/<dosya>

## Route / URL
-

## Amaç
Bu ekran ne içindir? (Müşteri perspektifinden yaz)

## Mobil Öncelik
Bu ekranın tasarımı önce mobile göre düşünülür.
Hangi ekran boyutu baz alındı? (320px / 375px / 414px)

## Layout
-

## Bileşenler

### [Bileşen 1 Adı]
- **Tür:**
- **Veri Kaynağı:**
- **Görsel Açıklama:**
- **Özel Notlar:**

## Davranışlar

| Aksiyon | Tetikleyici | Sonuç |
|---------|-------------|-------|

## Ekran Durumları

| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton / spinner |
| Boş | Boş durum mesajı |
| Hata | Hata ekranı |
| Dolu | Normal içerik |

## API Bağlantıları

| İşlem | Method | Endpoint |
|-------|--------|----------|

## Performans Notu
(Yavaş bağlantılarda ne olur? Önbellek var mı? Lazy load gerekiyor mu?)

## Kabul Kriterleri
- [ ] -
- [ ] -

## Notlar
-
```

---

## Durum Sistemi

| Durum | Anlamı |
|-------|--------|
| **Bekliyor** | Tasarım henüz hazır değil |
| **Tasarım Hazır** | Spec yazıldı, geliştirici alabilir |
| **Geliştiriliyor** | Üzerinde çalışılıyor |
| **Tamamlandı** | Kabul kriterleri karşılandı |

---

## Agent Notu

> RestaurantSystemQr müşterinin yediği lokantada telefonuyla açtığı şeydir.
> Yavaş internet, küçük ekran, elinde bir şeyler taşıyan kullanıcı —
> bu faktörler her tasarım kararını etkiler.
> Spec dosyalarına mutlaka "Performans Notu" ve "Mobil Öncelik" bölümlerini doldur.

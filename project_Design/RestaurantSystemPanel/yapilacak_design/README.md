# Yapılacak Design — RestaurantSystemPanel

> **Geliştirici için:** Bu klasör, arayüzü kodlamadan önce okuman gereken tek referans noktasıdır.
> Bir ekran implement etmeden önce o ekranın .md dosyasını oku, anla, sonra yaz.

---

## Bu Klasör Ne İçin?

`yapilacak_design/`, tasarımcı ile geliştirici arasındaki köprüdür.
Tasarımcı "ne yapılması gerektiğini" buraya yazar.
Geliştirici bunu okuyarak kodlar.

**Tasarımcı yazar → Geliştirici okur → Kod yazılır → Durum "Tamamlandı" işaretlenir.**

---

## Klasör Yapısı

```
yapilacak_design/
├── README.md               ← bu dosya
├── oncelik_sirasi.md       ← hangi ekranın önce yapılacağı
└── <ekran_adi>.md          ← her ekran için ayrı spec dosyası
```

---

## Dosya Adlandırma

Ekran bazlı: `<route_adi>.md`
Örn: `dashboard.md`, `menu_yonetimi.md`, `siparis_listesi.md`

Bileşen bazlı: `_komp_<isim>.md` (başına `_komp_` koy, ekrandan bağımsız bileşenler)
Örn: `_komp_veri_tablosu.md`, `_komp_sidebar.md`

---

## Ekran Spec Dosyası Şablonu

Her `.md` dosyası aşağıdaki formatı izler:

```markdown
# [Ekran Adı]

## Durum
[ ] Bekliyor  [ ] Tasarım Hazır  [ ] Geliştiriliyor  [x] Tamamlandı
Atanan Geliştirici: -
Tahmini Efor: -

## Tasarım Referansı
- Mevcut Design: project_Design/RestaurantSystemPanel/mevcut_design/<dosya>
- Örnek: project_Design/RestaurantSystemPanel/example_design_duzenlenmis/<dosya>
- Taslak: project_Design/RestaurantSystemPanel/taslak_design/<dosya>

## Route / URL
-

## Amaç (1-2 cümle)
Bu ekran ne içindir?

## Layout
Sayfa düzeni: (Sidebar + Content? Full-width? Dialog? vs.)

## Bileşenler

### [Bileşen 1 Adı]
- **Tür:** (tablo / form / kart / liste / grafik / vs.)
- **Veri Kaynağı:** hangi API endpoint'i
- **Alan Listesi:** gösterilecek / girilecek alanlar
- **Varsayılan Değerler:** -
- **Özel Notlar:** -

### [Bileşen 2 Adı]
...

## Davranışlar ve Etkileşimler

| Aksiyon | Tetikleyici | Sonuç |
|---------|-------------|-------|
| - | - | - |

## Ekran Durumları

| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton loader / spinner |
| Boş (veri yok) | Boş durum mesajı + CTA butonu |
| Hata | Hata mesajı + yeniden dene butonu |
| Dolu | Normal içerik |

## Form Validasyonları (varsa)

| Alan | Kural | Hata Mesajı |
|------|-------|-------------|
| - | - | - |

## API Bağlantıları

| İşlem | Method | Endpoint | Notlar |
|-------|--------|----------|--------|
| Veri çek | GET | /api/... | - |
| Kaydet | POST | /api/... | - |

## Responsive Davranış
- Masaüstü (>1200px): -
- Tablet (768–1200px): -
- Mobil (<768px): -

## Erişilebilirlik
- [ ] ARIA label'lar eklendi
- [ ] Klavye navigasyonu çalışıyor
- [ ] Renk kontrastı yeterli

## Kabul Kriterleri
Geliştirici bu maddeleri tamamladığında ekran "Tamamlandı" sayılır:
- [ ] -
- [ ] -

## Notlar / Özel Durumlar
-
```

---

## Durum Sistemi

| Durum | Anlamı |
|-------|--------|
| **Bekliyor** | Tasarım henüz hazır değil, geliştirici beklemeye devam |
| **Tasarım Hazır** | Spec yazıldı, geliştirici alabilir |
| **Geliştiriliyor** | Bir geliştirici üzerinde çalışıyor |
| **Tamamlandı** | Kod yazıldı, kabul kriterleri karşılandı |

---

## Agent Notu

> Bu klasörün varlığı kritik: tasarım ile geliştirme arasındaki "telefon kırık" oyununu engeller.
> Tasarımcı bir ekranı bitirdiğinde buraya yazar, geliştirici haberdar olur.
> Geliştirici "nasıl görünmeli" diye tahmin etmek zorunda kalmaz.
> `oncelik_sirasi.md` dosyasına da bak — hangi ekranın önce geleceği orada belirlenir.

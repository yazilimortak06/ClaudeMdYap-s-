# Design Kuralları — RestaurantSystemQr

> **Bu dosya ne için:** QR menü uygulamasının görsel dilini belirler.
> RestaurantSystemQr, müşterinin telefonuyla açtığı uygulamadır — müşteri yüzlü ve mobil önceliklidir.
>
> **Agent Yorumu:** Panel (RestaurantSystemPanel) ile QR arasındaki temel fark:
> Panel → veri yoğun, masaüstü, yönetici aracı.
> QR → görsel ağır, mobil-first, müşteri deneyimi.
> Bu dosyayı yazarken "restorana gitmiş, yemek siparişi vermek isteyen biri" gözünden bak.
> Yavaş internet, tek el kullanım, yemek kokusu ve acıkma durumu — tasarım bu ortama hitap eder.

---

## 1. Temel Prensipler

1. **Mobil önce gelir** — Tasarım her zaman 375px ekran genişliğinden başlar.
2. **Görsel ağırlık** — Yemek fotoğrafları öne çıkar. Bilgi yoğunluğu düşük tutulur.
3. **Dokunma dostu** — Tüm tıklanabilir alan minimum 44×44px olur.
4. **Hız** — Müşteri yemek beklerken uygulamayı kullanır. Her saniye önemlidir.
5. **Netlik** — Fiyat, ürün adı, içindekiler — bilgiler nette ve büyük görünür.
6. **Bağlantısız çalışabilirlik** — Menü bir kez yüklenince zayıf bağlantıda da okunabilir kalmalı.

---

## 2. Renk Paleti

> Yemek uygulamalarında renkler iştah açıcı olmalıdır. Soğuk mavi-gri tonlar genellikle uygun değildir.
> Sıcak tonlar (turuncu, kırmızı, sarı) iştah uyandırır; ama fazlası göz yorar.
> "Marka rengi + nötr arka plan + iştah açıcı vurgu" dengesi önerilir.

| CSS Token | Açıklama | Renk Adı | Hex |
|-----------|----------|----------|-----|
| `--color-primary` | Ana marka rengi | - | - |
| `--color-primary-dark` | Hover / pressed | - | - |
| `--color-accent` | Sepet, CTA, fiyat vurgusu | - | - |
| `--color-success` | Sipariş onayı, mevcut | - | - |
| `--color-error` | Stokta yok, hata | - | - |
| `--color-warning` | Dikkat (alerjen vs.) | - | - |
| `--color-bg-page` | Ana sayfa arka planı | - | - |
| `--color-bg-card` | Ürün kartı arka planı | - | - |
| `--color-text-primary` | Ana metin | - | - |
| `--color-text-secondary` | Açıklama, içindekiler | - | - |
| `--color-text-price` | Fiyat vurgusu | - | - |
| `--color-border` | Kart kenarlığı | - | - |
| `--color-overlay` | Fotoğraf üzeri overlay (okunabilirlik için) | - | rgba(0,0,0,0.35) |

**Renk Kuralları:**
- Fiyat her zaman `--color-text-price` ile vurgulanır — göz hemen fiyatı bulabilmeli.
- Stokta yok ürünler: fotoğraf `opacity: 0.4`, üzerinde "Mevcut Değil" overlay.
- `--color-error` yalnızca "stokta yok" ve gerçek hata durumlarında kullanılır.

---

## 3. Tipografi

> Mobil ekranda küçük yazı okumak zordur. Gövde metninin minimum 15px olması önerilir.
> Ürün adı ve fiyat — kullanıcının gözü her zaman bunlara gider; bu iki alan büyük ve okunaklı olur.

| CSS Token | Kullanım | Boyut | Ağırlık | Satır Yüksekliği |
|-----------|----------|-------|---------|-----------------|
| `--text-brand` | Restoran adı, ana başlık | 28px | 700 | 1.2 |
| `--text-category` | Kategori başlığı | 20px | 700 | 1.3 |
| `--text-product-name` | Ürün adı | 17px | 600 | 1.4 |
| `--text-price` | Fiyat | 18px | 700 | 1 |
| `--text-body` | Açıklama, içindekiler | 14px | 400 | 1.6 |
| `--text-badge` | Alerjen, etiket, badge | 12px | 500 | 1 |
| `--text-button` | Sepet butonu, CTA | 15px | 600 | 1 |
| `--text-caption` | Alt notlar, birim | 12px | 400 | 1.5 |

**Kural:** Açıklama metni 3 satırdan fazlaysa "Devamını Oku" ile kısalt.

---

## 4. Boşluk Sistemi

| CSS Token | Değer | Kullanım |
|-----------|-------|----------|
| `--space-1` | 4px | Badge iç padding, küçük boşluklar |
| `--space-2` | 8px | Kart içi küçük boşluk |
| `--space-3` | 12px | — |
| `--space-4` | 16px | Standart sayfa kenar boşluğu, kart padding |
| `--space-5` | 20px | Bölüm başlığı altı |
| `--space-6` | 24px | Kategori bölümleri arası |
| `--space-8` | 32px | Büyük bölümler |

**Sayfa Kenar Boşluğu:** 16px (her iki tarafta)
**Kart İç Padding:** 12–16px

---

## 5. Komponent Standartları

### 5.1 Ürün Kartı (Ana Komponent)

> QR menünün kalbi ürün kartıdır. Tasarımın %70'i buradan geçer.

**Fotoğraf:**
- En-boy oranı: 4:3 veya 16:9 (tutarlı olmalı)
- Stokta yok: `opacity: 0.4`, "Mevcut Değil" overlay
- Fotoğraf yoksa: `--color-bg-card` renginde placeholder ikon

**Bilgi Alanı:**
- Ürün adı (`--text-product-name`)
- Kısa açıklama (max 2 satır)
- Fiyat (`--text-price`) — her zaman görünür
- Etiketler: Vejetaryen / Acılı / Popüler (küçük badge'ler)
- "Sepete Ekle" veya "+" butonu

**Boyut:** Kart genişliği sayfa genişliği − 32px (kenar boşluğu).

### 5.2 Butonlar (Dokunma Dostu)

| Varyant | Kullanım | Min. Yükseklik |
|---------|----------|----------------|
| `primary` | "Sepete Ekle", "Sipariş Ver" | 44px |
| `secondary` | "İptal", "Geri" | 44px |
| `danger` | "Siparişten Çıkar" | 44px |
| `icon` | Sepet, favoriler, arama | 44×44px |
| `floating` | Sepet ikonu (sağ alt, FAB) | 56×56px |

**Floating Action Button (FAB):**
- Sağ alt köşede, sabit pozisyon
- Sepet ikonu + toplam ürün sayısı badge
- Sepete en az 1 ürün eklendiğinde görünür

### 5.3 Kategori Filtresi

- Yatay scroll eden pill/chip listesi (sayfa genişliğini aşar)
- Seçili kategori vurgulu (primary renk)
- Hafif shadow ile arka plandan ayrışır
- Sticky (sabit) — scroll edilirken de üstte kalır

### 5.4 Arama

- Tam sayfa veya overlay arama ekranı
- Anlık arama (her tuş vuruşunda filtrele)
- "X" ile temizle
- Sonuç yoksa: "Aramanızla eşleşen ürün bulunamadı" + öneri

### 5.5 Sepet / Modal

- Alt sayfadan kayarak açılan bottom sheet (mobil standardı)
- Ürün listesi + adedi düzenleme (+ / -)
- Toplam fiyat (sticky footer)
- "Sipariş Ver" butonu (primary, tam genişlik)

---

## 6. Fotoğraf Standartları

> QR menüde fotoğraf tasarımın yarısıdır. Kötü fotoğraf en iyi tasarımı mahveder.

- **Minimum çözünürlük:** 800×600px
- **Maksimum dosya boyutu:** 500KB (sıkıştırılmış)
- **Format:** WebP öncelikli, JPEG fallback
- **En-boy oranı:** Tüm ürünlerde tutarlı (4:3 önerilir)
- **Arka plan:** Nötr veya tarafsız — ürün ön planda
- **Fotoğraf yoksa:** Emoji değil, kategori ikonu ile placeholder

---

## 7. Animasyon

> Mobilde iyi animasyon "doğal hissettiren" animasyondur. Over-engineering yaratma.

| Durum | Süre | Easing |
|-------|------|--------|
| Sepet FAB giriş | 300ms | spring/bounce hafif |
| Bottom sheet açılma | 350ms | ease-out |
| Kart hover/press | 100ms | ease |
| Kategori geçişi | 200ms | ease |
| Skeleton → içerik | 300ms | ease-out |

---

## 8. Breakpoint'ler

QR uygulaması öncelikle mobil, ama masaüstüde de açılabilmelidir.

| İsim | Min | Hedef |
|------|-----|-------|
| `mobile-sm` | 320px | Küçük telefon |
| `mobile` | 375px | Standart iPhone — tasarım buradan başlar |
| `mobile-lg` | 414px | Büyük Android |
| `tablet` | 768px | Tablet — 2 sütunlu kart grid |
| `desktop` | 1024px | Masaüstü — 3 sütunlu kart grid, max-width: 720px ve ortala |

---

## 9. Performans Kuralları (Design Perspektifinden)

- Fotoğraflar lazy-load ile yüklenir (ekranın dışındakiler bekletilir)
- Skeleton loader — içerik gelene kadar alan korunur, layout kaymaları önlenir
- Kritik CSS (ilk görünen ekran) öncelikli yüklenir
- Tasarımda fotoğraf boyutuna dikkat: çok büyük görseller = yavaş yükleme

---

## 10. "Asla Yapma" Listesi

- Tıklanabilir alanı 44px altına düşürme (parmak dokunuşu ıskalanır)
- Fotoğraf olmadan ürün listeleme (yemek görsel motivasyona dayanır)
- Checkout'ta çok adım koyma (maksimum 2 adım: sepet + onay)
- Küçük font (15px altı gövde metin)
- Popup / interstitial reklam (müşteri zaten yemek sipariş etmek için burada)
- Yavaş yükleme animasyonu göstermeksizin boş ekran bırakma

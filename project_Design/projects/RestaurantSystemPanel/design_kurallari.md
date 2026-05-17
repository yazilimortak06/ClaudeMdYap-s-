# Design Kuralları — RestaurantSystemPanel

> **Tasarımcıya:** Bu dosya projenin görsel dilinin anayasasıdır.
> Belirlenen renk, tipografi, boşluk kuralı tüm ekranlara uygulanır.
> Değiştirmek istersen önce buraya yaz, sonra geliştiriciyi bilgilendir.
>
> **Geliştiriciye:** Bir renk veya boyut "nereden geldi?" diye sorarsan buraya bak.
> CSS değişkenlerini bu tablolardan üret.
>
> **Agent Yorumu:** RestaurantSystemPanel bir yönetim panelidir — masaüstü öncelikli, veri yoğun.
> En sık yapılan hata "her şeyi renkli yapmak". Aksine minimal renk kullan, bilgiyi ön plana çıkar.
> Tablo ve form tasarımları bu uygulamada kritik — buradan başla.

---

## 1. Temel Prensipler

1. **Netlik önce gelir** — Admin paneli işlevsel araçtır. Kullanıcı ne yapacağını ilk 3 saniyede anlamalıdır.
2. **Veri okunabilirliği** — Tablo ve form yoğun yapı. Tipografi hiyerarşisi şarttır.
3. **Tutarlılık** — Aynı işlev her yerde aynı görünür. "Sil" butonu her ekranda aynı renktedir.
4. **Hız** — Yöneticiler bu paneli sürekli kullanır. En sık yapılan işlem 3 tıklamayı geçmemeli.
5. **Renk anlam taşır** — Kırmızı = tehlike/silme, Yeşil = başarı/aktif. Bu anlam asla tersine çevrilmez.

---

## 2. Renk Paleti

> Not: Primary renk seçilirken `design_metni.md` deki ton bölümünü oku.
> Admin panelleri için neutral primer + güçlü bir accent önerilir.

| CSS Token | Açıklama | Renk Adı | Hex |
|-----------|----------|----------|-----|
| `--color-primary` | Ana renk — butonlar, aktif menü | - | - |
| `--color-primary-dark` | Hover/pressed durumu | - | - |
| `--color-primary-light` | Arka plan tonu, vurgu alanları | - | - |
| `--color-accent` | CTA, önemli badge | - | - |
| `--color-success` | Başarı, onay, aktif durum | Yeşil | - |
| `--color-warning` | Uyarı, dikkat | Sarı-turuncu | - |
| `--color-error` | Hata, silme, tehlike | Kırmızı | - |
| `--color-info` | Bilgi, yardım | Mavi | - |
| `--color-bg-page` | Ana sayfa arka planı | - | - |
| `--color-bg-sidebar` | Sidebar arka planı | - | - |
| `--color-bg-card` | Kart arka planı | - | - |
| `--color-text-primary` | Ana metin | - | - |
| `--color-text-secondary` | Açıklama, placeholder | - | - |
| `--color-text-disabled` | Pasif öğeler | - | - |
| `--color-border` | Genel kenarlık rengi | - | - |
| `--color-divider` | Bölücü çizgi | - | - |

**Zorunlu Kurallar:**
- `--color-error` yalnızca silme butonları, hata mesajları ve kritik uyarılarda kullanılır. Dekorasyon için kesinlikle kullanılamaz.
- Primary rengin 3'ten fazla tonu olmamasın: normal, dark (hover), light (arka plan).
- Metin/arka plan kontrast oranı minimum 4.5:1 (WCAG AA standardı).

---

## 3. Tipografi

> Admin panelinde 14px body standart değerdir. Gün boyu ekrana bakan kullanıcı için 12px altına inme.
> Hiyerarşiyi boyut değil ağırlık (font-weight) ile kur; çok büyük başlıklar panelde gereksiz yer kaplar.

| CSS Token | Kullanım | Font | Boyut | Ağırlık | Satır Yüksekliği |
|-----------|----------|------|-------|---------|-----------------|
| `--text-h1` | Sayfa başlığı | - | 28px | 700 | 1.3 |
| `--text-h2` | Bölüm başlığı | - | 22px | 600 | 1.4 |
| `--text-h3` | Kart başlığı | - | 18px | 600 | 1.4 |
| `--text-h4` | Alt bölüm | - | 16px | 600 | 1.5 |
| `--text-body` | Standart metin | - | 14px | 400 | 1.6 |
| `--text-body-sm` | Küçük açıklama, tablo içi | - | 12px | 400 | 1.5 |
| `--text-label` | Form etiketleri | - | 13px | 500 | 1.4 |
| `--text-caption` | Tablo başlıkları | - | 12px | 600 | 1.3 |
| `--text-button` | Buton metni | - | 14px | 500 | 1 |
| `--text-mono` | Kod / ID / hash gösterimi | monospace | 13px | 400 | 1.5 |

---

## 4. Boşluk Sistemi (8px Grid)

> 8px grid sistemi, geliştirici ile tasarımcı arasındaki "kaç pixel?" tartışmalarını bitirir.
> Tüm margin ve padding değerleri aşağıdaki tokenlardan biri olmalıdır.

| CSS Token | Değer | Kullanım |
|-----------|-------|----------|
| `--space-1` | 4px | İkon-metin arası, chip iç padding |
| `--space-2` | 8px | Küçük boşluklar, liste öğe arası |
| `--space-3` | 12px | Input iç padding |
| `--space-4` | 16px | Kart iç boşluğu, standart padding |
| `--space-6` | 24px | Bölümler arası, kart-kart arası |
| `--space-8` | 32px | Sayfa bölümleri arası |
| `--space-12` | 48px | Büyük bölüm ayırımları |
| `--space-16` | 64px | En büyük boşluk |

**Grid:**
- Sayfa içeriği: 12 sütun grid
- Sidebar: 240px (açık) / 64px (daraltılmış)
- İçerik alanı max-width: 1440px, ortada

---

## 5. Komponent Standartları

### 5.1 Butonlar

| Varyant | Ne Zaman? | Renk | Yükseklik |
|---------|-----------|------|-----------|
| `primary` | Ana aksiyon — Kaydet, Ekle, Onayla | `--color-primary` | 36–40px |
| `secondary` | İkincil — İptal, Geri, Vazgeç | Outline | 36–40px |
| `danger` | Silme, geri dönülemez işlem | `--color-error` | 36–40px |
| `ghost` | Düşük önem — Detay Gör, İndir | Sadece metin | 36px |
| `icon` | Tablo aksiyonları, kompakt alan | Şeffaf | 32×32px |

**Kurallar:**
- Bir form/bölümde tek `primary` buton. İkincisi varsa `secondary` olur.
- `danger` buton tıklandığında her zaman onay dialog'u açılır.
- Yükleme sırasında buton disabled + spinner — boyutu değişmez.
- Border radius: `6px`.

### 5.2 Input'lar

| Durum | Görünüm |
|-------|---------|
| Normal | `1px solid --color-border` |
| Focus | `2px solid --color-primary` |
| Hata | `2px solid --color-error` + altında hata mesajı |
| Disabled | `--color-bg-card` arka plan, cursor `not-allowed` |

- Yükseklik: `40px`
- Border radius: `6px`
- **Label zorunludur.** Placeholder label yerine geçemez (placeholder = ipucu).
- Hata mesajı blur anında gösterilir (submit'te değil — daha kullanıcı dostu).

### 5.3 Tablolar

> Panelin kalbi tablolardır. Kullanıcı saatlerce tabloya bakar — okunabilirlik hayat kalitesidir.

| Özellik | Değer |
|---------|-------|
| Satır yüksekliği | 48px (compact: 40px) |
| Başlık arka planı | Hafif gri fark |
| Zebra rengi | Opsiyonel (50% opaklık, çok kontrast olmasın) |
| Hover | `--color-primary-light` arka plan |
| Sıralama | Başlık tıklanabilir, yön oku |
| Sayfalama | 10 / 25 / 50 / 100 kayıt seçeneği |
| Boş durum | Ortalanmış ikon + mesaj + CTA butonu |
| Yükleme | Skeleton satırlar (sayfa boyutu sabit kalır) |

**Hizalama:** Sayılar ve para birimi sağa hizalı. Metin sola hizalı.

### 5.4 Kartlar

| Özellik | Değer |
|---------|-------|
| Border radius | `8px` |
| Gölge | `0 1px 4px rgba(0,0,0,0.08)` |
| Border | `1px solid --color-border` |
| Padding | `24px` (küçük kartlar: `16px`) |

### 5.5 Modallar

| Boyut | Genişlik | Kullanım |
|-------|----------|----------|
| Small | 400px | Onay dialog'ları |
| Medium | 600px | Kısa formlar |
| Large | 800px | Uzun formlar, detay görünümler |

- Backdrop tıklanınca kapanır (değişiklik varsa önce uyar).
- ESC kapatır.
- Footer düzeni: İptal (secondary, solda) | Onayla (primary/danger, sağda).

### 5.6 Badge / Chip (Durum Göstergesi)

| Durum | Renk |
|-------|------|
| Aktif / Başarılı | `--color-success` |
| Uyarı / Bekliyor | `--color-warning` |
| Pasif / Hata | `--color-error` |
| Taslak / Bilgi | `--color-info` |

- Border radius: `999px`
- Font: `--text-body-sm`, weight: 500

---

## 6. İkon Standartları

- **Kütüphane:** _______ (belirlenmeli)
- **Boyutlar:** 16px (küçük/satır içi) / 20px (standart) / 24px (büyük)
- **Renk:** Bulunduğu metnin rengiyle aynı. Aktif durumda `--color-primary`.
- **Yalnızca ikon kullanımı:** tooltip veya aria-label zorunludur.
- **İkon + metin arası:** 6–8px boşluk.

---

## 7. Animasyon

> Minimal, işlevsel animasyon. Dekoratif değil.

| Durum | Süre | Easing |
|-------|------|--------|
| Hover | 100–150ms | ease-out |
| Modal aç | 200ms | ease-out |
| Modal kapat | 150ms | ease-in |
| Sidebar toggle | 250ms | ease-in-out |
| Toast | 300ms | ease-out |
| Skeleton → İçerik | 300ms | ease-out |

**Kural:** `prefers-reduced-motion` varsa tüm animasyonlar devre dışı.
**Kural:** 500ms üzerinde animasyon yok.

---

## 8. Breakpoint'ler

| İsim | Min | Davranış |
|------|-----|---------|
| `xs` | 0 | Panel desteklemez |
| `sm` | 576px | Acil mobil görünüm |
| `md` | 768px | Tablet — sidebar daraltılmış |
| `lg` | 992px | Laptop — standart görünüm |
| `xl` | 1200px | Geniş masaüstü |
| `xxl` | 1440px | Ultra geniş — max-width ile sınırla |

---

## 9. Erişilebilirlik Minimumları

- Kontrast oranı: metin/arka plan minimum 4.5:1
- Her input'un üstünde label
- Her ikon için aria-label veya tooltip
- Tab sırası mantıklı sıralı
- Hata mesajları `aria-describedby` ile input'a bağlı

---

## 10. "Asla Yapma" Listesi

- Placeholder'ı label yerine kullanma
- Aynı sayfada 3'ten fazla farklı renk kullanma
- Yalnızca renkle anlam taşıma (renk körü kullanıcılar için ikon/metin ekle)
- `z-index: 9999` gibi keyfi değer yazma
- Destructive işlemde onay almama (silme her zaman confirm ister)
- 12px altında font kullanma

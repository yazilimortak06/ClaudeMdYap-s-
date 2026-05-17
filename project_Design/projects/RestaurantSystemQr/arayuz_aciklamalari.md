# Arayüz Açıklamaları — RestaurantSystemQr

> **Bu dosya ne için:** Figma olmadan her ekranın ne içereceğini metin olarak anlamak.
> Mobil-first bir QR menü uygulamasıdır. Ekran açıklamalarını telefon perspektifinden yaz.
>
> **Geliştirici:** Bu dosyayı oku → `yapilacak_design/` klasöründeki ekran spec'ini oku → kodla.
>
> **Agent Yorumu:** QR menü akışı çok kısa olmalıdır: Aç → Listele → Sepete Ekle → Sipariş Ver.
> Her ekran sadece bir işe odaklanır. Fazla özellik = müşteri kaçar.

---

## Şablon (Yeni Ekran Eklerken Kopyala)

```
### [Ekran Adı] — Route: /...

**Amaç:** Bu ekran ne içindir?

**Layout (Mobile 375px):**
Genel düzen açıklaması.

**İçerik:**
- [Bileşen]: ne gösterir, veri nereden gelir

**Kullanıcı Akışları:**
- [Aksiyon] → [Sonuç]

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton |
| Veri var | Normal |
| Hata | Hata ekranı |

**Performans Notu:**
**Özel Notlar:**
```

---

## Ortak Bileşenler

### Header (Sticky)

**İçerik:**
- Sol: Geri butonu (bir önceki ekran, varsa)
- Orta: Restoran adı veya ekran başlığı
- Sağ: Sepet ikonu + ürün sayısı badge

**Davranış:**
- Sticky — scroll edilirken üstte kalır
- Sepet badge: sepete ürün eklenince güncellenir, 0 ise gizlenir

---

### Kategori Filtresi (Sticky)

**Konum:** Header'ın hemen altında, yatay scroll pill listesi

**Davranış:**
- Seçili chip vurgulanır (primary renk)
- Chip tıklanınca o kategoriye scroll edilir
- Kullanıcı manual scroll ederken de aktif kategori güncellenir

---

### Ürün Kartı (Ana Komponent)

**İçerik:**
- Ürün fotoğrafı (4:3, lazy-load)
- Ürün adı, kısa açıklama (max 2 satır)
- Etiketler (Vejetaryen, Acılı, Popüler — badge)
- Fiyat + "+" butonu (min. 44×44px dokunma alanı)

**Stokta Yok:**
- Fotoğraf opacity: 0.4
- "Mevcut Değil" bandı
- "+" butonu disabled

---

### Floating Sepet Butonu (FAB)

**Konum:** Sağ alt köşe, fixed

**Davranış:**
- Sepet boşken gizli
- İlk ürün eklenince animasyonla belirir
- Tıklanınca sepet açılır

---

## Ekranlar

### Menü Listesi — Route: `/menu`

**Amaç:** Tüm menüyü kategoriler halinde listele.

**Layout:**
```
[Sticky Header]
[Sticky Kategori Filtresi]
[Scroll: Restoran Bilgisi (opsiyonel) + Kategori Bölümleri + Ürün Kartları]
[FAB: Sepet (sağ alt)]
```

**İçerik:**

*Restoran Bilgisi (liste üstünde, opsiyonel):*
- Banner fotoğrafı, restoran adı, çalışma saatleri, açıklama

*Kategori Bölümleri:*
- Kategori başlığı (büyük, bold)
- O kategorinin ürün kartları (tek sütun)

**Kullanıcı Akışları:**
- Kategori chip'i tıkla → o kategoriye scroll
- Ürün kartı tıkla → ürün detay
- "+" tıkla → sepete ekle, FAB güncellenir

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton kartlar |
| Normal | İçerik |
| Restoran kapalı | "Şu an sipariş alamıyoruz" banner, menü sadece görüntüleme |
| Hata | Tam ekran hata + Tekrar Dene |

**Performans Notu:**
İlk 5 ürün fotoğrafı öncelikli yüklenir, diğerleri lazy-load.
Menü verisi 10 dakika önbelleğe alınır.

---

### Ürün Detay — Route: `/product/:id`

**Amaç:** Ürünün büyük fotoğrafı, tam açıklaması, adet seçimi ve sepete ekleme.

**Layout:**
```
[Tam genişlik fotoğraf]
[Beyaz kart, fotoğrafın üzerine kaymış]
  Ürün adı | Fiyat
  Kısa açıklama
  Uzun açıklama (Devamını Gör)
  Alerjen bilgisi (opsiyonel)
  Etiketler
  Adet: (−) | Sayı | (+)
  [Sepete Ekle — tam genişlik, primary]
```

**Kullanıcı Akışları:**
- Adet değiştir → fiyat güncellenir
- Sepete Ekle → geri dön, FAB güncellenir, toast

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton |
| Normal | İçerik |
| Stokta yok | Fotoğraf soluk, "Mevcut Değil" disabled buton |

---

### Sepet — Bottom Sheet veya `/cart`

**Amaç:** Ürünleri gözden geçir, düzenle, siparişi ver.

**Layout:**
```
[Başlık: "Sepetim (N ürün)"]
[Ürün listesi: Fotoğraf | Ad | Adet(−/+) | Fiyat | Sil]
[Toplam alanı: Ara Toplam | Servis | TOPLAM]
[Not alanı (opsiyonel)]
["Sipariş Ver" — sticky footer, primary]
```

**Kullanıcı Akışları:**
- (−) ile 0'a düş → ürün animasyonla çıkar
- "Sipariş Ver" → onay ekranı

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Dolu | Normal |
| Boş | "Sepetiniz boş" + Menüye Dön |

---

### Sipariş Onay — `/order-confirm`

**Amaç:** Son kontrol. Özet göster, onayla.

**Layout:**
Masa no | Sipariş özeti | Toplam | "Siparişi Onayla" butonu

---

### Sipariş Başarı — `/order-success`

**Amaç:** "Siparişiniz alındı."

**Layout:**
Büyük başarı ikonu + "Siparişiniz alındı!" + Sipariş no + "Menüye Dön" butonu

---

> **Agent Notu:** Akış ne kadar kısa olursa o kadar iyi.
> QR menü için ideal: Aç → Listele → Sepet → Onayla → Teşekkür.
> Bu 5 adım yeterlidir. Her ek ekran müşteri kaybettirme riskidir.

# Arayüz Açıklamaları — RestaurantSystemPanel

> **Bu dosya ne için:** Figma olmadan, yalnızca metin okuyarak her ekranın ne içereceğini anlamak.
> Tasarımcı + analizci yazar, tüm ekip okur.
>
> **Geliştirici:** Bu dosyayı oku, sonra `yapilacak_design/` klasöründeki ilgili ekran spec'ini oku.
> Bu dosya "ne görünecek" der, spec dosyası "nasıl yapılacak" der.
>
> **Agent Yorumu:** Arayüz açıklaması yazmak tasarım yapmaktan önce gelir.
> Bir ekranı metin olarak tarif eden kişi, o ekranı tasarlarken çok daha az iterasyon yapar.

---

## Şablon (Yeni Ekran Eklerken Kopyala)

```
### [Ekran Adı] — Route: /...

**Amaç:** Bu ekran ne içindir? Kim kullanır? Ne zaman açılır?

**Erişim:**
- Nereden gelinir (sidebar/başka ekran)?
- Yetki kısıtı var mı?

**Layout:**
Genel düzen: (Sidebar + Başlık + Filtre + Tablo + Sayfalama gibi)

**İçerik Blokları:**
Her bilgi bloğu için:
- Bileşen: [ne gösterilecek]
- Veri: [nereden geliyor]
- Boş durum: [veri yoksa ne gösterilecek]

**Kullanıcı Akışları:**
- [Aksiyon] → [Sonuç]

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton |
| Veri var | Normal içerik |
| Veri yok | Boş durum mesajı + CTA |
| Hata | Hata mesajı + Yeniden Dene |

**Özel Notlar:** ...
```

---

## Ortak Bileşenler (Tüm Sayfalarda Var)

### Sidebar

**Amaç:** Tüm sayfalarda sabit, panel navigasyonu.

**İçerik (yukarıdan aşağıya):**
- Logo / Marka alanı (üst)
- Ana menü öğeleri (ikon + metin, gruplar halinde)
- Alt alan: Kullanıcı bilgisi + Çıkış

**Menü Öğesi Durumları:**
- Normal: ikon + metin
- Hover: hafif arka plan rengi
- Aktif (bulunulan sayfa): primary renk vurgusu (sol kenar çizgisi veya arka plan)
- Alt menüsü olan: tıklayınca expand/collapse

**Davranış:**
- Masaüstü: varsayılan açık (240px). Daraltılabilir (64px, sadece ikon).
- Tablet: kapalı başlar, hamburger ile açılır.
- Üst kısım scroll edilmez (sticky).

**Yetki Bazlı Gizleme:**
Kullanıcının yetkisi yoksa o menü öğesi hiç gösterilmez (disabled değil, gizli).

---

### Header (Üst Bar)

**İçerik:**
- Sol: Hamburger menü ikonu (tablet/mobil) | Sayfa başlığı
- Sağ: Bildirim ikonu (badge ile sayı) | Kullanıcı avatarı (dropdown)

**Kullanıcı Dropdown:**
- Profil adı + email
- Profil Düzenle
- Ayarlar
- Çıkış Yap

**Davranış:**
- Sticky (scroll ile kaybolmaz)
- Bildirim sayısı 0 ise badge gizlenir
- Bildirim tıklanınca liste dropdown açılır

---

### Toast / Bildirim

**Varyantlar:** success / error / warning / info

**Konum:** Sağ üst köşe, üst üste sıralanabilir

**Davranış:**
- 4 saniye sonra otomatik kapanır
- Hover'da timer durur
- X ikonu ile manuel kapatma
- Animasyonla giriş/çıkış

**Mesaj Kuralları:**
- Başarı: "Ürün başarıyla eklendi."
- Hata: "Kayıt sırasında hata oluştu. Tekrar dene."
- Uyarı: "Kaydedilmemiş değişiklikler var."

---

### Onay Dialog'u

Her silme ve geri dönülemez işlem öncesi çıkar.

**Yapı:**
- Başlık: "Silmek istediğine emin misin?"
- İçerik: Neyin silineceğini somut yaz. "**'Izgara Köfte'** menüden kalıcı olarak silinecek."
- Butonlar: İptal (secondary) | Sil (danger)

**Davranış:**
- ESC veya backdrop tıklaması = İptal
- Sil tıklandıktan sonra loading state, işlem bitince kapat + success toast

---

## Ekranlar

### Login — Route: `/auth/login`

**Amaç:** Kullanıcı kimlik doğrulama. Giriş yapılmışsa direkt dashboard'a yönlendir.

**Layout:**
İki sütun: Sol = marka/görsel alanı, Sağ = login formu
(veya tam ekran ortada kart — tasarımcı karar verir)

**İçerik:**
- Logo
- Başlık: "Hoş geldiniz" + kısa açıklama (opsiyonel)
- Email input (required)
- Şifre input + görünür/gizle toggle (required)
- "Beni hatırla" checkbox (opsiyonel)
- Giriş Yap butonu (primary, full-width)
- "Şifremi unuttum" linki (opsiyonel)

**Kullanıcı Akışları:**
- Form doldur + Giriş Yap → Yükleniyor → Başarı: `/dashboard` → Hata: hata mesajı
- "Şifremi Unuttum" → Email sıfırlama sayfasına git

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Boş form | Normal |
| Yükleniyor | Buton disabled + spinner |
| Hatalı bilgi | Form altında kırmızı hata mesajı |
| Başarı | /dashboard yönlendirmesi |
| Çok fazla deneme | Belirli süre kilitleme uyarısı |

**Özel Notlar:**
- Şifre alanı enter tuşuyla submit edilebilir.
- Beni hatırla seçilmişse kullanıcı bir sonraki açılışta direkt dashboard'a gider.

---

### Dashboard — Route: `/dashboard`

**Amaç:** Günlük operasyonun anlık özeti. "Şu an ne oluyor?" sorusunu yanıtlar.

**Erişim:** Login sonrası ilk açılan sayfa. Sidebar'dan erişilir.

**Layout:**
```
[Header]
[KPI Kartları — 4 adet, grid]
[Grafik Alanı — sol: çizgi grafik | sağ: bar grafik]
[Son Siparişler — tablo, max 10 satır]
```

**İçerik Blokları:**

*KPI Kartları:*
- Bugünkü sipariş sayısı (sayı + dün ile karşılaştırma oku)
- Bugünkü ciro (tutar + dün ile yüzde fark)
- Aktif masa sayısı (toplam / dolu)
- Bekleyen sipariş sayısı (kırmızı uyarı varsa vurgula)

*Grafikler:*
- Sol: Son 7 gün sipariş sayısı (çizgi grafik, X=gün, Y=adet)
- Sağ: En çok satan 5 ürün (yatay bar grafik, X=adet, Y=ürün adı)

*Son Siparişler:*
- Sütunlar: Sipariş No | Masa | Toplam | Durum | Saat
- Satıra tıklanınca sipariş detayına git
- "Tüm Siparişleri Gör" linki (sipariş listesi sayfasına)

**Kullanıcı Akışları:**
- KPI kartına tıkla → ilgili listeye git (örn: sipariş sayısına tıkla → sipariş listesi)
- Sipariş satırına tıkla → sipariş detayı

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton KPI kartlar + skeleton tablo |
| Yüklendi (veri var) | Normal içerik |
| Bugün sipariş yok | KPI'lar sıfır, grafik boş durum mesajı |
| API kısmen hata | Her bölüm bağımsız hata gösterir (diğerleri çalışmaya devam eder) |

---

### Menü Yönetimi — Route: `/menu`

**Amaç:** Kategorileri ve ürünleri yönet. Ekle, düzenle, sil, sırala, aktif/pasif yap.

**Layout:**
```
[Header: "Menü Yönetimi" başlığı | Sağda: "+ Kategori" ve "+ Ürün" butonları]
[Sol panel (240px): Kategori Listesi]
[Sağ alan: Seçili Kategori Ürünleri]
```

**İçerik:**

*Kategori Listesi (sol):*
- Kategori adı + ürün sayısı (badge)
- Aktif/pasif toggle
- Sağ tıklama veya üç nokta: Düzenle | Sil
- Sürükle-bırak sıralama (opsiyonel)
- "Kategori Ekle" butonu (listenin altında)

*Ürün Listesi (sağ):*
- Görünüm seçeneği: Liste (tablo) veya Kart grid
- Tablo sütunları: Fotoğraf | Ürün Adı | Fiyat | Durum | İşlemler
- Aktif/pasif toggle (inline)
- Düzenle ikonu → ürün düzenleme modal'ı
- Sil ikonu → onay dialog'u

**Kullanıcı Akışları:**
- "+ Ürün" tıkla → Ürün ekleme modal'ı (Ad, Fiyat, Kategori, Açıklama, Fotoğraf)
- Kaydet → modal kapanır, liste güncellenir, success toast
- Sil → onay dialog → onaylanınca → ürün listeden kaldırılır, success toast
- Toggle tıkla → anlık aktif/pasif değişir (kaydet butonu gerekmez)

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Kategori seçilmedi | Sağ panel: "Sol menüden bir kategori seç" |
| Kategori seçili, ürün yok | "Bu kategoride ürün yok. + Ürün Ekle" CTA |
| Yükleniyor | Skeleton |
| Hata | Hata mesajı + Yeniden Dene |

**Özel Notlar:**
- Fotoğraf yükleme: max 2MB, JPEG/PNG/WebP. Boyut kısıtı göster.
- Fiyat: Türk Lirası, virgüllü ondalık (örn: 45,90 TL).

---

### Sipariş Listesi — Route: `/orders`

**Amaç:** Tüm siparişleri listele, filtrele, detayına gir, durum güncelle.

**Layout:**
```
[Header: "Siparişler" başlığı | Dışa Aktar butonu]
[Filtre satırı: Tarih aralığı | Durum | Masa | Arama]
[Tablo: siparişler]
[Sayfalama]
```

**Tablo Sütunları:**
Sipariş No | Masa No | Ürün Özeti (ilk 2 ürün + "+N daha") | Toplam Tutar | Durum | Tarih/Saat | İşlemler

**Sipariş Durumları:**
- Yeni (sarı badge)
- Hazırlanıyor (mavi badge)
- Tamamlandı (yeşil badge)
- İptal Edildi (kırmızı badge)

**Kullanıcı Akışları:**
- Satıra tıkla → sipariş detay modal'ı
- Durum badge'ine tıkla → durum güncelleme dropdown
- "Dışa Aktar" → CSV veya Excel indir

**Durum Matrisi:**
| Durum | Görünüm |
|-------|---------|
| Yükleniyor | Skeleton tablo |
| Siparişler var | Normal tablo |
| Filtre sonucu boş | "Seçilen kriterlere uygun sipariş bulunamadı" mesajı |
| Hata | Hata mesajı |

---

> **Agent Notu:** Bu dosyaya ekran eklendikçe `yapilacak_design/oncelik_sirasi.md`
> güncellenmeli ve o ekran için `yapilacak_design/<ekran_adi>.md` oluşturulmalıdır.
> İki dosya birbirini tamamlar:
> `arayuz_aciklamalari.md` = "ne görünecek" (serbest dil, tasarımcı + analizci için)
> `yapilacak_design/<ekran>.md` = "nasıl yapılacak" (teknik spec, geliştirici için)

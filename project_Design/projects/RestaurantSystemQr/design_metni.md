# Design Metni — RestaurantSystemQr

> **Bu dosyanın amacı:** Tasarıma başlamadan önce "bu uygulama kim içindir ve nasıl hissettirmelidir?" sorusunu yanıtlamak.
>
> **Agent Yorumu:** QR menü uygulamaları genellikle "sadece menüyü gösterelim" diye basit yapılır.
> Ama iyi bir QR menü, müşterinin daha fazla sipariş vermesini sağlar (restoran açısından değer).
> Müşteri açısından ise: hız, görsellik ve güven hissi kritiktir.
> Bu dosya, ekibin bu hedefleri paylaşmasını sağlar.

---

## Ürünün Özeti

RestaurantSystemQr, müşterilerin restorana geldiklerinde QR kodu okutarak menüyü görüntüleyebildiği ve sipariş verebildiği mobil web uygulamasıdır.

**Müşterinin deneyimi:**
1. Masada QR kodu telefona okut
2. Menüyü gözden geçir (görseller, açıklamalar, fiyatlar)
3. Sepete ekle
4. Siparişi ver

---

## Hedef Kullanıcı

| Özellik | Detay |
|---------|-------|
| **Kim?** | - |
| **Yaş aralığı** | - |
| **Telefon türü** | Android + iOS, orta segment dahil |
| **İnternet bağlantısı** | Restoran Wi-Fi (zaman zaman yavaş) veya mobil veri |
| **Kullanım süresi** | 2–5 dakika (aktif sipariş verme) |
| **Ortam** | Restoranın içi — belki gürültülü, belki düşük ışık |
| **Motivasyon** | Hızlı sipariş ver ve yemeğini bekle |

---

## Genel Ruh ve Ton

**Bu uygulama şunları hissettirmelidir:**
- Sıcak ve davetkar (yemek ve iştah uyandırır)
- Hızlı ve sezgisel (kullanmayı öğrenmek gerekmez)
- Güvenilir (sipariş verdiğimde gidecek mi?)

**Bu uygulama BUNLARI HİSSETTİRMEMELİ:**
- Karmaşık (çok tıklama, çok ekran)
- Soğuk ve steril (hastane / banka hissi yok)
- Yavaş (yükleme bekletmesi)

---

## Görsel Kimlik Yönlendirmesi

**Renk Karakteri:**
Sıcak ve iştah açıcı tonlar (turuncu, kırmızı tonları iştahı uyarır — ama abartma).
Beyaz arka plan + güçlü bir marka rengi kombinasyonu yaygın ve işe yarar.

**Tipografi Karakteri:**
Büyük, okunaklı, koyu ağırlıklı başlıklar. Açıklamalar daha ince.
Mutfak/yemek temalı fontlar (çok stilize olanlar) yerine okunabilirliği öncelikle.

**Fotoğraf:**
Bu uygulamada fotoğraf tasarımın yarısıdır. İştah açıcı, gerçek ürün fotoğrafları.
Stok fotoğraf değil, gerçek menü ürünleri.

---

## Deneyim Hedefleri

1. **Sürtünmesiz menü tarama:** Kategoriler arası geçiş ve ürün görseline ulaşmak 1 tıkta.
2. **Güvenli sipariş:** Müşteri "siparişim gitti mi?" diye endişelenmemeli.
3. **Görsel karar kolaylığı:** Açıklama + fotoğraf + fiyat — müşteri hızla karar verebilmeli.
4. **Kolay sepet yönetimi:** Ürün ekleme/çıkarma 1 dokunuşta.

---

## Kısıtlamalar

> Design kararlarını etkileyen teknik veya işletmesel kısıtlamalar:

-

---

## Ne Olmayacak

- Hesap açma zorunluluğu (kayıtsız sipariş)
- Masaüstü öncelikli tasarım
- Çok adımlı, karmaşık ödeme akışı
- Popup reklam veya dikkat dağıtıcı öğeler

# Domainsel Kural Tanımlama Kuralları

---

## Amaç

Domain kuralı = Projeye özgü, değişmeden kalmasını istediğimiz iş veya teknik karar.

**Örnekler:**
- "MenuCategory silinirken içindeki aktif MenuItem'lar silinmez, arşivlenir (IsDeleted=false, IsArchived=true)"
- "Order.TotalAmount her zaman service katmanında hesaplanır, frontend'den gelmez ve güvenilmez"
- "OrderStatus geçişleri sadece izin verilen yönlerde yapılabilir: Pending→Accepted, Accepted→Preparing, Preparing→Ready, Ready→Delivered, Delivered→Paid. Geri geçiş yasaktır"
- "QR token doğrulanmadan sipariş endpoint'lerine erişilemez; token her istek ile birlikte gönderilir"

---

## Akış

### 1. Mevcut Kuralları Öğren
`rules/proje_domain_rules/<proje>.md` dosyasını oku. Var olan kuralları anla:
- Hangi entity'ler için kural var?
- Yeni kural mevcut bir kuralla çakışıyor mu?
- Yeni kural mevcut bir kuralı geçersiz kılıyor mu?

### 2. Kullanıcıyla Kural Tartış
- Kuralın tam kapsamını netleştir: hangi entity, hangi koşul, hangi davranış?
- Edge case'leri soruştur: "X silinirse Y ne olur?", "Z boşsa ne döner?"
- Kuralın mimari etkisini değerlendir: servis katmanı mı, validation mı, DB constraint mi?

### 3. Kuralı Yaz
Kural netleşince `rules/proje_domain_rules/<proje>.md` dosyasına ekle:
- İlgili entity başlığı altına yerleştir
- Kural cümlesi + kısa açıklama
- Kod örneği: ne yapılır / ne yapılmaz
- Uygulama yeri: Domain, Application (Service/Validator), Persistence

### 4. Mimari Etki Kontrolü
Kural mimari karar içeriyorsa `current_md/<proje>/mimari_gelisen.md` dosyasına da özet olarak ekle.

---

## Kurallar

- **Domain kuralı kod örneği içermelidir.** Sadece metin yeterli değil; en az bir "yapılır" veya "yapılmaz" kod bloğu eklenmeli.
- **Çakışma kontrolü zorunlu.** Kural eklenmeden önce mevcut kurallar taranır. Çakışma varsa kullanıcıya bildirilir ve önce çözüme kavuşturulur.
- **`proje_mimari_rules/` dosyaları bu modda değiştirilmez.** Mimari kurallar ayrı bir süreçle güncellenir; domain kural tanımlama seansında sadece `proje_domain_rules/` dosyası düzenlenir.
- **Kural geriye dönük uyumluluğu.** Mevcut koda etki eden bir kural eklenirse, etkilenen yerlerin listesi kullanıcıya sunulur.
- **Kural sahibi.** Her kural hangi tarihte, hangi bağlamda eklendi bilgisini içermeli (yorum satırı olarak).

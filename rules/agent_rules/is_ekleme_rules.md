# İş Ekleme Kuralları

---

## Akış

### 1. Mevcut Durumu Kontrol Et
- `current_md/<proje>/<kullanici>/current.md` oku — bu kullanıcı için devam eden iş var mı?
- `current_md/<proje>/ortak/ortak.md` oku — tüm projenin genel durumuna bak, başka aktif işler neler?

### 2. Yeni İş Seçimi
Kullanıcıya iki seçenek sun:
- **Seçenek A:** `tasks/<proje>/havuz.md`'den mevcut bir görevi al
- **Seçenek B:** Yeni bir iş tanımla (henüz havuzda olmayan)

### 3. Yeni İş Tanımlama (Seçenek B)
1. Önce `tasks/<proje>/planlama.md`'ye ekle — tartışma formatında yaz
2. Kullanıcıyla tartış: kapsam, bağımlılıklar, tahmini süre
3. Kapsam netleşince `tasks/<proje>/havuz.md`'ye taşı
4. Kullanıcı onayı alındıktan sonra `yapilacaklar.md`'ye geçilir

### 4. İş Alımı
Kullanıcı bir iş almaya karar verdiğinde sırayla:
1. `tasks/<proje>/havuz.md`'den ilgili satırı `yapilacaklar.md`'ye taşı
2. `current_md/<proje>/<kullanici>/current.md`'yi güncelle (aktif görev, başlangıç zamanı)
3. `agent.md`'yi güncelle (aktif görev adı ve bağlam özeti)

---

## Kurallar

- **Kullanıcı onayı olmadan `havuz.md` → `yapilacaklar.md` taşıma yapma.** Kullanıcı açıkça "Bu işi alalım" veya "Başlayalım" demedikçe iş aktarılmaz.
- **`yapilacaklar.md`'de bir kullanıcıya ait birden fazla aktif iş olabilir** — paralel çalışma desteklenir, ancak bağımlılıklı işler sırayla alınmalıdır.
- **Bağımlılık kontrolü:** İş alınmadan önce `havuz.md`'deki bağımlılık alanına bakılır. Bağımlı iş tamamlanmamışsa uyarı ver.
- **İş boyutu:** Tek seferde alınan iş 1-8 saat arasında olmalıdır. Daha büyük işler alt görevlere bölünür.

---

## İş Tamamlama

İş tamamlandığında sırayla:

1. `tasks/<proje>/yapilacaklar.md`'den ilgili satırı kaldır
2. `current_md/<proje>/ortak/ortak.md`'yi güncelle — tamamlanan modül/feature ekle
3. `current_md/<proje>/<kullanici>/current.md`'yi temizle veya sonraki göreve güncelle
4. `logs/<proje>/` altında log dosyasına tamamlanan iş kaydını ekle (tarih, görev adı, çıktı)
5. `agent.md`'yi güncelle

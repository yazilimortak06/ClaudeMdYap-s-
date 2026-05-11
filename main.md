# main.md

Bu proje, Claude yönetim projesidir. Birden fazla alt proje (backend, web, vs.) içerir ve tüm ekip üyelerinde ortak olarak bulunacaktır.

---

## Agent Akışı — Başlangıç Noktası

Bu dosyayı okur okumaz aşağıdaki adımları sırayla uygula:

### Adım 1 — Kullanıcıyı Belirle

1. `settings/users/` klasörünü tara — her `.md` dosyası bir kullanıcıdır.
2. Kullanıcıları numaralandırılmış seçenekler halinde göster, en sona "Yeni kullanıcıyım" ekle.
3. Seçimi bekle.

**Yeni kullanıcı seçilirse:**
- İsim al (kişi + makine, örn: `saidMasaustu`).
- Tip sor: `gelistirici` / `is_analisti` / `yonetici` / `tester`
- `settings/users/<isim>.md` dosyasını `settings/settings.md` deki şemaya göre oluştur.

**Mevcut kullanıcı seçilirse:**
- `settings/users/<isim>.md` dosyasını oku → `tip` alanını not et.

> `is_analisti`, `yonetici`, `tester` özel tiplerdir — kullanıcı listesinde tek seferlik görünürler, kişiye özgü değildir.

---

### Adım 2 — Proje Seç

1. `settings/projects.md` dosyasındaki proje listesini oku.
2. Kullanıcıya hangi proje üzerinde çalışacağını sor.
3. Seçilen proje `settings/projects.md` de **yoksa** → `rules/proje_ekleme_rules.md` kurallarını uygula.
4. Kullanıcı tipi `gelistirici` ise: projenin local pathi `settings/users/<isim>.md` de **tanımlı değilse** → sor:
   > "**<ProjeAdı>** için local klasör pathin nedir? (örn: C:\Projects\RestaurantSystemBackend)"
   
   Alınan pathi `settings/users/<isim>.md` dosyasına kaydet.

> `is_analisti`, `yonetici`, `tester` için path sormaya gerek yok — doğrudan devam et.

---

### Adım 3 — Bağlam Yükle

Kullanıcı tipine göre ilgili klasörü belirle:

| Tip | Klasör |
|-----|--------|
| `gelistirici` | `current_md/<proje>/<kullaniciAdi>/` |
| `is_analisti` | `current_md/<proje>/is_analisti/` |
| `yonetici` | `current_md/<proje>/yonetici/` |
| `tester` | `current_md/<proje>/tester/` |

**Geliştirici için oku:**
- `current_md/<proje>/agent.md`
- `current_md/<proje>/mimari_gelisen.md`
- `current_md/<proje>/<kullaniciAdi>/current.md`
- `current_md/<proje>/<kullaniciAdi>/ilerleme.md`
- `current_md/<proje>/<kullaniciAdi>/is_listesi.md`
- `current_md/_ilerleme/<kullaniciAdi>.md`

**İş analisti için oku:**
- `current_md/<proje>/is_analisti/current.md`
- `current_md/<proje>/is_analisti/private/notlar.md`
- `current_md/<proje>/is_analisti/public/sorular/README.md`

**Yönetici için oku:**
- `current_md/<proje>/yonetici/current.md`
- `current_md/<proje>/yonetici/public/ilerleme_raporu.md`
- `current_md/<proje>/yonetici/public/sprint_plani.md`
- `current_md/_ilerleme/ortak.md`

**Tester için oku:**
- `current_md/<proje>/tester/current.md`
- `current_md/<proje>/tester/public/bug_raporlari/README.md`
- `current_md/<proje>/tester/public/regression_listesi/README.md`

Kısa özet sun: "Kaldığın yer şu — ne yapmak istiyorsun?"

---

### Adım 4 — Mod Seç (Tipe Göre)

#### Geliştirici
```
1) Geliştirme
2) Analiz
3) Domainsel kural tanımlama
4) İş ekleme
```
| Mod | Ek Okunacak Dosyalar |
|-----|----------------------|
| Geliştirme | `rules/proje_mimari_rules/<tip>.md`, `rules/proje_domain_rules/<proje>.md`, `current_md/<proje>/<kullaniciAdi>/kurallar.md`, `current_md/<proje>/<kullaniciAdi>/takilmalar.md` |
| Analiz | `current_md/<proje>/<kullaniciAdi>/analiz.md`, `current_md/<proje>/ortak/analiz.md`, `rules/agent_rules/analiz_rules.md` |
| Domainsel kural tanımlama | `rules/proje_domain_rules/<proje>.md`, `current_md/<proje>/mimari_gelisen.md` |
| İş ekleme | `current_md/<proje>/<kullaniciAdi>/backlog.md`, `current_md/<proje>/ortak/ortak.md` |

#### İş Analisti
```
1) Analiz Yap
2) Task Oluştur
3) Backlog Düzenle
4) Gereksinim Tanımla
5) Kullanıcı Akışı Çiz
6) Soru Sor / Tartış
```
Tüm çıktılar: taslak → `private/taslaklar/`, kesinleşince → `public/<ilgili_klasör>/`

#### Yönetici
```
1) İlerleme Görüntüle
2) İş Ata
3) Sprint Planla
4) Risk Takibi
5) Rapor Oluştur
6) Toplantı Notu Al
```
Okur: tüm kullanıcıların `ilerleme.md`, `is_listesi.md`, `_ilerleme/ortak.md`
Yazar: `public/ilerleme_raporu.md`, `public/is_atamalari.md`, `public/sprint_plani.md`

#### Tester
```
1) Test Senaryosu Yaz
2) Bug Raporu Oluştur
3) Test Sonucu Gir
4) Regresyon Listesi Güncelle
5) Çözülen Bug'ı Kapat
```
Okur: `is_analisti/public/test_senaryolari/` (kabul kriterleri için)
Yazar: `tester/public/<ilgili_klasör>/`

---

## Oturum Sonu — Güncelleme Zorunluluğu

| Tip | Güncellenecek Dosyalar |
|-----|----------------------|
| Geliştirici | `current.md`, `ilerleme.md`, `is_listesi.md`, `is_notlari.md`, `mimari_kararlar.md`, `current_md/<proje>/mimari_gelisen.md`, `_ilerleme/<kullaniciAdi>.md`, `_ilerleme/ortak.md` |
| İş Analisti | `current.md`, `private/notlar.md`, taşınan çıktılar `public/` altına |
| Yönetici | `current.md`, `public/ilerleme_raporu.md`, `public/sprint_plani.md`, `_ilerleme/ortak.md` |
| Tester | `current.md`, ilgili `public/` dosyaları |

---

## Mimari Katmanlar

| Katman | Dosya | Açıklama |
|--------|-------|----------|
| Değişmez kurallar | `rules/proje_mimari_rules/<tip>.md` | Özel sohbet olmadan değiştirilemez |
| Gelişen mimari (proje) | `current_md/<proje>/mimari_gelisen.md` | Herkes için geçerli, kararlarla büyür |
| Gelişen mimari (kişi) | `current_md/<proje>/<kullaniciAdi>/mimari_kararlar.md` | Kişinin çalışması sırasında ortaya çıkan kararlar |
| Domain kuralları | `rules/proje_domain_rules/<proje>.md` | Projeye özgü domainsel kurallar |

---

## Referans Dosyalar

| Dosya | Açıklama |
|-------|----------|
| `settings/settings.md` | Kullanıcı dosyası şeması, tipler ve genel kurallar |
| `settings/projects.md` | Tüm projelerin listesi (tek kaynak) |
| `settings/users/` | Her kullanıcının kendi `.md` dosyası |
| `rules/proje_ekleme_rules.md` | Yeni proje eklerken uygulanacak adımlar |
| `current_md/_ilerleme/` | Kişi bazlı tüm projeler özet ilerleme |

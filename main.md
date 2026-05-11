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
- `settings/users/<isim>.md` dosyasını `settings/settings.md` deki şemaya göre oluştur.

**Mevcut kullanıcı seçilirse:**
- `settings/users/<isim>.md` dosyasını oku.

### Adım 2 — Proje Seç

1. `settings/projects.md` dosyasındaki proje listesini oku.
2. Kullanıcıya hangi proje üzerinde çalışacağını sor.
3. Seçilen proje `settings/projects.md` de **yoksa** → `rules/proje_ekleme_rules.md` kurallarını uygula.
4. Seçilen projenin local pathi kullanıcı dosyasında **tanımlı değilse** → kullanıcıya sor:
   > "**<ProjeAdı>** için local klasör pathin nedir? (örn: C:\Projects\RestaurantSystemBackend)"
   
   Alınan pathi `settings/users/<isim>.md` dosyasına kaydet, sonra devam et.

### Adım 3 — Bağlam Yükle

Kullanıcı ve proje belirlendikten sonra şu dosyaları oku:

| Dosya | Amaç |
|-------|------|
| `current_md/<proje>/agent.md` | Agent bağlamı |
| `current_md/<proje>/mimari_gelisen.md` | Projenin büyüyen mimari kararları |
| `current_md/<proje>/<kullanıcı>/current.md` | Kullanıcının nerede kaldığı |
| `current_md/<proje>/<kullanıcı>/ilerleme.md` | Genel ilerleme durumu |
| `current_md/<proje>/<kullanıcı>/is_listesi.md` | Aktif iş listesi |
| `current_md/_ilerleme/<kullanıcı>.md` | Tüm projelerdeki özet durum |

Kullanıcıya kısa bir özet sun: "Kaldığın yer şu, aktif işlerin şunlar — ne yapmak istiyorsun?"

### Adım 4 — Mod Seç

```
Ne yapmak istiyorsunuz?
1) Geliştirme
2) Analiz
3) Domainsel kural tanımlama
4) İş ekleme
```

Seçime göre ek dosyaları yükle:

| Mod | Ek Okunacak Dosyalar |
|-----|----------------------|
| Geliştirme | `rules/proje_mimari_rules/<tip>.md`, `rules/proje_domain_rules/<proje>.md`, `current_md/<proje>/<kullanıcı>/kurallar.md`, `current_md/<proje>/<kullanıcı>/takilmalar.md` |
| Analiz | `current_md/<proje>/<kullanıcı>/analiz.md`, `current_md/<proje>/ortak/analiz.md`, `rules/agent_rules/analiz_rules.md` |
| Domainsel kural tanımlama | `rules/proje_domain_rules/<proje>.md`, `current_md/<proje>/mimari_gelisen.md` |
| İş ekleme | `current_md/<proje>/<kullanıcı>/backlog.md`, `current_md/<proje>/ortak/ortak.md` |

---

## Oturum Sonu — Güncelleme Zorunluluğu

Her oturum bitiminde şu dosyaları güncelle:

| Dosya | Ne Güncellenir |
|-------|----------------|
| `current_md/<proje>/<kullanıcı>/current.md` | Nerede kaldı, bir sonraki adım ne |
| `current_md/<proje>/<kullanıcı>/ilerleme.md` | Tamamlananlar, devam edenler |
| `current_md/<proje>/<kullanıcı>/is_listesi.md` | İş durumları |
| `current_md/<proje>/<kullanıcı>/is_notlari.md` | Oturumda alınan teknik notlar |
| `current_md/<proje>/mimari_gelisen.md` | Oturumda alınan mimari kararlar |
| `current_md/<proje>/<kullanıcı>/mimari_kararlar.md` | Kullanıcının çalışması sırasında ortaya çıkan mimari kararlar |
| `current_md/_ilerleme/<kullanıcı>.md` | Tüm projeler özet tablo |
| `current_md/_ilerleme/ortak.md` | Ortak genel durum tablosu |

Takılınan bir nokta olduysa `takilmalar.md`'ye ekle.
Yeni bir kural keşfedildiyse `kurallar.md`'ye ekle.

---

## Mimari Katmanlar

| Katman | Dosya | Açıklama |
|--------|-------|----------|
| Değişmez kurallar | `rules/proje_mimari_rules/<tip>.md` | Özel sohbet olmadan değiştirilemez |
| Gelişen mimari (proje) | `current_md/<proje>/mimari_gelisen.md` | Proje kararlarıyla büyür, herkes için geçerli |
| Gelişen mimari (kişi) | `current_md/<proje>/<kullanıcı>/mimari_kararlar.md` | Kişinin çalışması sırasında ortaya çıkan kararlar |
| Domain kuralları | `rules/proje_domain_rules/<proje>.md` | Projeye özgü domainsel kurallar |

---

## Referans Dosyalar

| Dosya | Açıklama |
|-------|----------|
| `settings/settings.md` | Kullanıcı dosyası şeması ve genel kurallar |
| `settings/projects.md` | Tüm projelerin listesi (tek kaynak) |
| `settings/users/` | Her kullanıcının kendi `.md` dosyası |
| `rules/proje_ekleme_rules.md` | Yeni proje eklerken uygulanacak adımlar |
| `current_md/_ilerleme/` | Kişi bazlı tüm projeler özet ilerleme |

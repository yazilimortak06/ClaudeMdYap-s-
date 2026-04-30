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
4. Seçilen projenin local pathi kullanıcı dosyasında **tanımlı değilse** → kullanıcıdan pathi al ve kullanıcı dosyasına kaydet.

### Adım 3 — Ne Yapmak İstiyorsun?

Kullanıcıya her zaman şu menüyü göster:

```
Ne yapmak istiyorsunuz?
1) Geliştirme
2) Analiz
3) Domainsel kural tanımlama
4) İş ekleme
```

Seçime göre ilgili dosyaları yükle ve o moda geç:

| Mod | Okunacak Dosyalar |
|-----|-------------------|
| Geliştirme | `current_md/<proje>/agent.md`, `current_md/<proje>/mimari_gelisen.md`, `rules/proje_mimari_rules/<tip>.md`, `rules/proje_domain_rules/<proje>.md` |
| Analiz | `current_md/<proje>/<kullanıcı>/analiz.md` (taslak), `current_md/<proje>/ortak/analiz.md` (kesinleşmiş). Akış için bkz: `rules/analiz_rules.md` |
| Domainsel kural tanımlama | `rules/proje_domain_rules/<proje>.md`, `current_md/<proje>/mimari_gelisen.md` |
| İş ekleme | `current_md/<proje>/<kullanıcı>/current.md`, `current_md/<proje>/ortak/ortak.md` |

---

## Mimari Katmanlar

| Katman | Dosya | Açıklama |
|--------|-------|----------|
| Değişmez kurallar | `rules/proje_mimari_rules/<tip>.md` | Özel sohbet olmadan değiştirilemez |
| Gelişen mimari | `current_md/<proje>/mimari_gelisen.md` | Proje kararlarıyla büyür |
| Domain kuralları | `rules/proje_domain_rules/<proje>.md` | Projeye özgü domainsel kurallar |

---

## Referans Dosyalar

| Dosya | Açıklama |
|-------|----------|
| `settings/settings.md` | Kullanıcı dosyası şeması ve genel kurallar |
| `settings/projects.md` | Tüm projelerin listesi (tek kaynak) |
| `settings/users/` | Her kullanıcının kendi `.md` dosyası |
| `rules/proje_ekleme_rules.md` | Yeni proje eklerken uygulanacak adımlar |

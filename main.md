# main.md

Bu proje, Claude yönetim projesidir. Birden fazla alt proje içerir ve tüm ekip üyelerinde ortaktır.

---

## Agent Akışı

Bu dosyayı okur okumaz adımları sırayla uygula.

---

### Adım 0 — Mod Seç

> "Normal modda mı yoksa **Worktree** modunda mı çalışmak istiyorsun?"
> - **Normal** — Tek agent, standart akış → Adım 1'e geç
> - **Worktree** — Paralel multi-agent çalışma (her rol-proje ayrı terminal)

**Worktree seçildi:** `rules/worktree_rules.md` oku ve uygula. Aşağıdaki adımları atla.

---

### Adım 1 — Kim Olduğunu Belirle

1. `settings/users/` klasörünü tara.
2. Kullanıcıları listele + en sona "Yeni kullanıcıyım" ekle.
3. Seçimi bekle.

**Yeni kullanıcı:**
- İsim al.
- Tip sor: `normal` mi `yonetici` mi?
- `settings/users/<isim>.md` oluştur.

**Mevcut kullanıcı:**
- `settings/users/<isim>.md` oku → `Tip` alanını not et.

---

### Adım 2 — Proje Seç

1. `settings/projects.md` oku → proje listesi + **"Tüm Projeler"** seçeneği ekle.
2. Kullanıcıya sor.
3. Seçilen proje listede yoksa → `rules/proje_ekleme_rules.md` uygula.

**Kullanıcı `ortak` ise (proje seçimi sonrası):**
- Sor: > "Şu an ortak hesabı kim kullanıyor? (ali / muhammed_ali / said / ...)"
- Verilen ismi `aktif_kullanici` olarak not et.
- `settings/users/ortak.md` → `### <aktif_kullanici>` bölümünden o projenin satırını oku.
- Oturum boyunca `aktif_kullanici` adıyla çalış (log, notlar vb. bu isimle tutulur).

---

### Adım 3 — Rol (Yönetici değilse)

**Rol her oturumda sorulur.** Aynı kişi birden fazla terminal açıp farklı proje/rol ile çalışabilir.
Kullanıcı dosyasındaki kayıtlı rol "öneri" olarak gösterilir ama her oturumda seçim alınır.

`settings/users/<isim>.md` içinde seçilen projenin Rol sütununu oku, sonra sor:

> "**<ProjeAdı>** için bu oturumda hangi rolle çalışıyorsun?  
> *(Son kayıtlı: **<rol>** — aynıysa Enter'a bas, farklıysa yaz)*  
> `gelistirici` / `tester` / `analiz_uzmani` / `task_uzmani` / `tasarimci` / `arge_muhendisi` / `yazilim_mimari` / `kalite_muhendisi`"

Seçilen rol → kullanıcı dosyasının proje satırına kaydet.

**Yönetici tipi kullanıcılar bu adımı atlar.**

**Ortak kullanıcı:** `aktif_kullanici`'nın o proje satırına göre aynı işlem uygulanır.

**Gelistirici rolü seçildi ve path tanımsızsa:**
- Sor: > "**<ProjeAdı>** için local path nedir?"
- Path'i kullanıcı dosyasına kaydet.

> Diğer roller için path sorma.

---

### Adım 4 — Bağlam Yükle

**Rol → Okunacak Dosyalar:**

| Rol | Okunacak |
|-----|---------|
| gelistirici | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/<isim>/current.md`, `ilerleme.md`, `is_listesi.md`, `kurallar.md` |
| tester | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/tester/current.md`, `tester/ilerleme.md`, `tester/public/bug_raporlari/README.md`, `tester/public/regression_listesi/README.md` |
| analiz_uzmani | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/is_analisti/current.md`, `is_analisti/ilerleme.md`, `is_analisti/public/sorular/README.md`, `is_analisti/public/analizler/README.md` |
| task_uzmani | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/task_uzmani/current.md`, `task_uzmani/ilerleme.md`, `tasks/<proje>/havuz.md`, `tasks/<proje>/yapilacaklar.md` |
| tasarimci | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/tasarimci/current.md`, `tasarimci/ilerleme.md`, `project_Design/<proje>/design_kurallari.md`, `project_Design/<proje>/arayuz_aciklamalari.md` |
| arge_muhendisi | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/arge_muhendisi/current.md`, `arge_muhendisi/ilerleme.md`, `project_Design/<proje>/mevcut_design/README.md`, `arge_muhendisi/public/arastirmalar/README.md` |
| yazilim_mimari | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/yazilim_mimari/current.md`, `yazilim_mimari/ilerleme.md`, `yazilim_mimari/public/mimariler/README.md`, `yazilim_mimari/public/kararlar/README.md` |
| kalite_muhendisi | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/kalite_muhendisi/current.md`, `kalite_muhendisi/ilerleme.md`, `kalite_muhendisi/public/kalite_raporlari/README.md`, `kalite_muhendisi/public/standartlar/README.md` |
| yonetici | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/yonetici/current.md`, `yonetici/ilerleme.md`, `yonetici/public/ilerleme_raporu.md`, `yonetici/public/sprint_plani.md` |
| Tüm Projeler | + `current_md/_ilerleme/<isim>.md`, `current_md/_ilerleme/ortak.md` |

**Oturum Klasörü Oluştur (Adım 4 başında):**
`claude_context/` altında şu isimde klasör aç:
`YYYY-MM-DD_HHmm_<isim>_<ProjeKisa>_<rol>/`
İçine `meta.md`, `oturum.md`, `durum.md` oluştur. Şablon için `claude_context/README.md` oku.
Bu klasör bu terminale aittir — terminal kapanınca pasif kalır.

**Log Oku:** `logs/<ProjeAdı>/<isim>.md` dosyası varsa en üstteki girişi oku — Özet, Yarım Kalanlar, Blokerlar kısmını bağlama ekle. Tüm dosyayı okuma.

Kısa özet: "Kaldığın yer şu — ne yapmak istiyorsun?"

---

### Adım 5 — Mod Menüsü (Role Göre)

#### Geliştirici
```
1) Geliştirme
2) Analiz
3) Domain kural tanımlama
4) İş ekleme
```

#### Tester
```
1) Test Senaryosu Yaz
2) Bug Raporu Oluştur
3) Test Sonucu Gir
4) Regresyon Listesi Güncelle
5) Bug Kapat
```
*Okur:* `is_analisti/public/test_senaryolari/` (kabul kriterleri)
*Yazar:* `current_md/<proje>/tester/public/`

#### Analiz Uzmanı
```
1) Analiz Yap
2) Gereksinim Tanımla
3) Kullanıcı Akışı Çiz
4) Soru Sor / Tartış
```
*Taslak:* `current_md/<proje>/is_analisti/private/taslaklar/`
*Nihai:* `current_md/<proje>/is_analisti/public/`

#### Task Uzmanı
```
1) Task Oluştur (analizden / manuel)
2) Bug Task Aç (testerdan gelen bug'dan)
3) Backlog Düzenle / Sırala
4) Sprint Oluştur / Güncelle
5) Tartışma Notu Al (analiz uzmanı / tasarımcı ile)
6) Task Ata
```
*Okur:* `is_analisti/public/` (analiz ve gereksinimler), `tester/public/bug_raporlari/` (bug'lar), `tasarimci/public/` (tasarım kararları)
*Yazar:* `task_uzmani/public/tasklar/`, `task_uzmani/public/sprintler/`, `task_uzmani/public/bug_tasklari/`, `task_uzmani/public/backlog/`, `task_uzmani/public/tartismalar/`

#### Tasarımcı
```
1) Ekran Tasarımı Yap
2) Komponent Tanımla
3) Stil Rehberi Güncelle
4) Kullanıcı Akışı Çiz
5) Prototip Ekle
```
*Okur:* `is_analisti/public/akislar/`, `project_Design/<proje>/` (design merkezi)
*Taslak:* `current_md/<proje>/tasarimci/private/taslaklar/`
*Nihai:* `tasarimci/public/`, `project_Design/<proje>/` (mevcut_design, design_kurallari, arayuz_aciklamalari)

#### Arge Mühendisi
```
1) Araştırma Başlat
2) Prototip / PoC Yap
3) Teknoloji Değerlendir
4) Teknik Rapor Yaz
5) Öneri Oluştur
6) Bulguları Paylaş
```
*Okur:* `referansProject/analiz/`, `rules/`, `is_analisti/public/`, mimari dosyaları, `project_Design/<proje>/`
*Taslak:* `current_md/<proje>/arge_muhendisi/private/taslaklar/`
*Nihai:* `arge_muhendisi/public/`, `project_Design/<proje>/arge_design/`

#### Yazılım Mimarı
```
1) Mimari Tasarla
2) Mimari Gözden Geçir
3) Teknoloji Kararı Al
4) Entegrasyon Deseni Tanımla
5) ADR (Mimari Karar) Oluştur
6) Mimari Doküman Yaz
```
*Okur:* `rules/proje_mimari_rules/`, `current_md/<proje>/mimari_gelisen.md`, `referansProject/`, `arge_muhendisi/public/`
*Taslak:* `current_md/<proje>/yazilim_mimari/private/taslaklar/`
*Nihai:* `current_md/<proje>/yazilim_mimari/public/`
*Günceller:* `current_md/<proje>/mimari_gelisen.md` (karar kesinleşince)

#### Kalite Mühendisi
```
1) Kalite Analizi Yap
2) Kalite Standardı Tanımla
3) Metrik Oluştur / Güncelle
4) Teknik Borç Analizi
5) Kalite Raporu Yaz
6) Test Stratejisi Belirle
```
*Okur:* `tester/public/`, `rules/yazilim_bilgi/test_stratejileri.md`, `rules/yazilim_bilgi/prensipler.md`
*Taslak:* `current_md/<proje>/kalite_muhendisi/private/taslaklar/`
*Nihai:* `current_md/<proje>/kalite_muhendisi/public/`

#### Yönetici
```
1) İlerleme Görüntüle
2) İş Ata
3) Sprint Planla
4) Risk Takibi
5) Rapor Oluştur
6) Toplantı Notu Al
```
*Okur:* tüm rol klasörlerinin `ilerleme.md`, `_ilerleme/ortak.md`
*Yazar:* `current_md/<proje>/yonetici/public/`

---

## Oturum Sonu

Her oturum bitiminde güncelle. **Tüm roller için:** `claude_context/<oturum>/oturum.md` ve `durum.md` güncelle.

**Log Yaz (tüm roller zorunlu):** `rules/log_rules.md` formatında `logs/<ProjeAdı>/<isim>.md` dosyasının en üstüne yeni bir giriş ekle.

| Rol | Dosyalar |
|-----|---------|
| gelistirici | `current.md`, `ilerleme.md`, `is_listesi.md`, `is_notlari.md`, `mimari_kararlar.md`, `mimari_gelisen.md`, `_ilerleme/<isim>.md`, `_ilerleme/ortak.md` |
| tester | `tester/current.md`, `tester/ilerleme.md`, ilgili `tester/public/` dosyaları |
| analiz_uzmani | `is_analisti/current.md`, `is_analisti/ilerleme.md`, taslaktan `public/`'a taşıma |
| task_uzmani | `task_uzmani/current.md`, `task_uzmani/ilerleme.md`, `tasks/<proje>/` dosyaları |
| tasarimci | `tasarimci/current.md`, `tasarimci/ilerleme.md`, taslaktan `public/`'a taşıma, `project_Design/<proje>/yapilacak_design/` güncellemeleri |
| arge_muhendisi | `arge_muhendisi/current.md`, `arge_muhendisi/ilerleme.md`, taslaktan `public/`'a taşıma |
| yazilim_mimari | `yazilim_mimari/current.md`, `yazilim_mimari/ilerleme.md`, taslaktan `public/`'a taşıma, `mimari_gelisen.md` |
| kalite_muhendisi | `kalite_muhendisi/current.md`, `kalite_muhendisi/ilerleme.md`, taslaktan `public/`'a taşıma |
| yonetici | `yonetici/current.md`, `yonetici/ilerleme.md`, `ilerleme_raporu.md`, `sprint_plani.md`, `_ilerleme/ortak.md` |

---

## Mimari Katmanlar

| Katman | Dosya |
|--------|-------|
| Değişmez | `rules/proje_mimari_rules/<tip>.md` |
| Gelişen (proje) | `current_md/<proje>/mimari_gelisen.md` |
| Gelişen (kişi) | `current_md/<proje>/<isim>/mimari_kararlar.md` |
| Domain | `rules/proje_domain_rules/<proje>.md` |

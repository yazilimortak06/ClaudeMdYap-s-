# main.md

Bu proje, Claude yönetim projesidir. Birden fazla alt proje içerir ve tüm ekip üyelerinde ortaktır.

---

## Agent Akışı

Bu dosyayı okur okumaz adımları sırayla uygula.

---

### Adım 1 — Kim Olduğunu Belirle

1. `settings/users/` klasörünü tara.
2. Kullanıcıları listele + en sona "Yeni kullanıcıyım" ekle.
3. Seçimi bekle.

**Yeni kullanıcı:**
- İsim al.
- Tip sor: `normal` mi `yonetici` mi?
- Normal ise → Rol sor: `gelistirici` / `tester` / `analiz_uzmani` / `task_uzmani` / `tasarimci`
- `settings/users/<isim>.md` oluştur.

**Mevcut kullanıcı:**
- `settings/users/<isim>.md` oku → `Tip` ve `Rol` alanlarını not et.

---

### Adım 2 — Rol (Yönetici değilse)

Kullanıcı dosyasındaki `Rol` varsayılan olarak göster:
> "Rolün **gelistirici** olarak kayıtlı. Bu oturumda aynı rolle mi devam ediyorsun?"

Değiştirmek isterse → yeni rol seç ve dosyaya kaydet.

**Yönetici tipi kullanıcılar bu adımı atlar.**

---

### Adım 3 — Proje Seç

1. `settings/projects.md` oku → proje listesi + **"Tüm Projeler"** seçeneği ekle.
2. Kullanıcıya sor.
3. Seçilen proje listede yoksa → `rules/proje_ekleme_rules.md` uygula.

**Kullanıcı `ortak` ise (proje seçimi sonrası):**
- Sor: > "Şu an ortak hesabı kim kullanıyor? (ali / muhammed_ali / said / ...)"
- Verilen ismi `aktif_kullanici` olarak not et.
- `settings/users/ortak.md` → `### <aktif_kullanici>` bölümünden o projenin path'ini oku.
- `settings/users/ortak.md` → `### <aktif_kullanici>` bölümüne bak.
  - **Path `-` veya tanımsızsa → devam etme, MUTLAKA sor:**
    > "**<aktif_kullanici>** için **<ProjeAdı>** local path nedir?"
    
    Cevabı al → `settings/users/ortak.md` içinde `### <aktif_kullanici>` tablosuna kaydet → sonra devam et.
- Oturum boyunca `aktif_kullanici` adıyla çalış (log, notlar vb. bu isimle tutulur).

**Normal kullanıcı, rol `gelistirici`, path tanımlı değilse:**
- Sor: > "**<ProjeAdı>** için local path nedir?"
- Path'i `settings/users/<isim>.md` e kaydet.

> Diğer roller ve yönetici için path sorma.

---

### Adım 4 — Bağlam Yükle

**Rol / Proje → Okunacak Dosyalar:**

| Durum | Okunacak |
|-------|---------|
| Tüm roller | `current_md/<proje>/mimari_gelisen.md`, `current_md/<proje>/<isim>/current.md` |
| gelistirici | + `current_md/<proje>/<isim>/ilerleme.md`, `is_listesi.md`, `kurallar.md` |
| tester | + `current_md/<proje>/tester/public/bug_raporlari/README.md`, `regression_listesi/README.md` |
| analiz_uzmani | + `current_md/<proje>/is_analisti/public/sorular/README.md`, `analiz_uzmani/public/analizler/README.md` |
| task_uzmani | + `tasks/<proje>/havuz.md`, `tasks/<proje>/yapilacaklar.md` |
| tasarimci | + `current_md/<proje>/tasarimci/public/tasarimlar/README.md`, `stiller/README.md` |
| yonetici | + `current_md/<proje>/yonetici/public/ilerleme_raporu.md`, `sprint_plani.md` |
| Tüm Projeler | + `current_md/_ilerleme/<isim>.md`, `current_md/_ilerleme/ortak.md` |

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
1) Task Oluştur
2) Backlog Düzenle
3) Havuza Ekle
4) Task Ata
```
*Yazar:* `tasks/<proje>/`, `current_md/<proje>/is_analisti/public/tasklar/`

#### Tasarımcı
```
1) Ekran Tasarımı Yap
2) Komponent Tanımla
3) Stil Rehberi Güncelle
4) Kullanıcı Akışı Çiz
5) Prototip Ekle
```
*Taslak:* `current_md/<proje>/tasarimci/private/taslaklar/`
*Nihai:* `current_md/<proje>/tasarimci/public/`

#### Yönetici
```
1) İlerleme Görüntüle
2) İş Ata
3) Sprint Planla
4) Risk Takibi
5) Rapor Oluştur
6) Toplantı Notu Al
```
*Okur:* tüm kullanıcıların `ilerleme.md`, `_ilerleme/ortak.md`
*Yazar:* `current_md/<proje>/yonetici/public/`

---

## Oturum Sonu

Her oturum bitiminde güncelle:

| Rol | Dosyalar |
|-----|---------|
| gelistirici | `current.md`, `ilerleme.md`, `is_listesi.md`, `is_notlari.md`, `mimari_kararlar.md`, `mimari_gelisen.md`, `_ilerleme/<isim>.md`, `_ilerleme/ortak.md` |
| tester | `current.md`, ilgili `tester/public/` dosyaları |
| analiz_uzmani | `current.md`, taslaktan `public/`'a taşıma |
| task_uzmani | `current.md`, `tasks/<proje>/` dosyaları |
| tasarimci | `current.md`, taslaktan `public/`'a taşıma |
| yonetici | `current.md`, `ilerleme_raporu.md`, `sprint_plani.md`, `_ilerleme/ortak.md` |

---

## Mimari Katmanlar

| Katman | Dosya |
|--------|-------|
| Değişmez | `rules/proje_mimari_rules/<tip>.md` |
| Gelişen (proje) | `current_md/<proje>/mimari_gelisen.md` |
| Gelişen (kişi) | `current_md/<proje>/<isim>/mimari_kararlar.md` |
| Domain | `rules/proje_domain_rules/<proje>.md` |

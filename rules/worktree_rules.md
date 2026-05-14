# worktree_rules.md

`main.md` Adım 0'da "Worktree" seçildiğinde bu dosya okunur ve uygulanır.

---

## Adım WT-1 — Kullanıcı Seç

1. `settings/users/` klasörünü tara.
2. Kullanıcıları listele + en sona "Yeni kullanıcıyım" ekle.
3. Seçimi bekle.

**Yeni kullanıcı:** `main.md` Adım 1'deki yeni kullanıcı akışını uygula, sonra WT-2'ye geç.

**Mevcut kullanıcı:** `settings/users/<isim>.md` oku → Tip alanını not et.
- `## Bilgisayar:` section'larını tara → listele + "Yeni bilgisayar ekle" seçeneği ekle.
- Seçimi al → `aktif_bilgisayar` olarak not et.

---

## Adım WT-2 — Worktree Seç veya Oluştur

1. `worktree/<isim>/` klasörünü tara.
2. Her alt klasörde `worktree_aciklamasi.md` oku → açıklama + ilerleme satırını al.
3. Listele:

```
Mevcut Worktree'ler:
  1) said-genel-worktree — "Backend + Panel + Menü paralel geliştirme" | İlerleme: %30
  2) said-analiz-worktree — "Q1 analiz sprint" | İlerleme: %80
  + Yeni worktree oluştur
```

Klasör yoksa veya içi boşsa sadece "Yeni worktree oluştur" seçeneğini göster.

**Yeni worktree:**
- İsim al (örn: `said-sprint3`, boşluk ve Türkçe karakter kullanma)
- Açıklama al (ne için, hangi amaçla)

---

## Adım WT-3 — Rol-Proje Seçimi (Döngü)

1. `settings/projects.md` oku → proje listesini al.
2. Rol listesi: `gelistirici`, `tester`, `analiz_uzmani`, `task_uzmani`, `tasarimci`, `arge_muhendisi`, `yazilim_mimari`, `kalite_muhendisi`
3. Sor:

> "Hangi rol ile hangi projede çalışacaksın?  
> Rol: gelistirici / tester / analiz_uzmani / ...  
> Proje: (listeyi göster)"

4. Seçimi listeye ekle. Klasör adını hesapla: `<rol>-<projeKisa>` (örn: `gelistirici-backend`, `gelistirici-menuAngular`).
5. Sor:

> "Eklendi. Başka bir rol-proje eklemek ister misin? (e/h)"
- **e** → 3. adıma dön
- **h** → WT-4'e geç

**Path kontrolü (sadece geliştirici rolü):**
Geliştirici seçildi ve `settings/users/<isim>.md` de o projenin Path'i `-` ise:
> "**<ProjeAdı>** için local path nedir?"
Path'i kullanıcı dosyasına kaydet.

**Otomatik:** Koordinatör her zaman eklenir, kullanıcıdan onay alınmaz.

---

## Adım WT-4 — Yapıyı Kur

**Ana klasör:** `worktree/<isim>/<worktree-adı>/`

**Oluşturulacak dosyalar:**

```
<worktree-adı>/
├── worktree_aciklamasi.md
├── koordinator/
│   ├── current.md
│   ├── hafiza.md
│   ├── agent_ortak.md
│   └── ilerleme.md
└── <rol>-<projeKisa>/          ← her seçilen rol-proje için
    ├── current.md
    ├── ilerleme.md
    └── hafiza.md
```

**Kural:** Klasör zaten varsa mevcut dosyaları koru, sadece eksik olanları oluştur.

### Dosya Şablonları

**`worktree_aciklamasi.md`:**
```
# Worktree: <worktree-adı>

- **Açıklama:** <kullanıcının girdiği açıklama>
- **Oluşturulma:** <tarih>
- **İlerleme:** %0

## Rol-Proje Listesi
| Klasör | Rol | Proje |
|--------|-----|-------|
| koordinator | koordinatör | — |
| <rol>-<projeKisa> | <rol> | <proje> |
```

**`koordinator/current.md`:**
```
# Koordinatör — Aktif Durum

## Kim Ne Yapıyor
| Klasör | Proje | Durum | Son Güncelleme |
|--------|-------|-------|----------------|
(her rol-proje için bir satır — başlangıçta boş)

## Sıradaki Ortak Adımlar
—

## Blokerlar
—
```

**`koordinator/agent_ortak.md`:**
```
# Agent Ortak Bilgiler

Bu dosyayı tüm agentlar okur. Paylaşılan her karar buraya eklenir.

## API Uç Noktaları
—

## Ortak Veri Modelleri
—

## Ortak Kararlar
—

## Bağımlılıklar (Kim Kimi Bekliyor)
—
```

**`koordinator/hafiza.md`:**
```
# Koordinatör — Genel Hafıza

Worktree genelinde geçerli kararlar, bağlam ve geçmiş.

## Temel Kararlar
—

## Geçmiş Notlar
—
```

**`koordinator/ilerleme.md`:**
```
# Worktree Genel İlerleyişi

| Klasör | Tamamlanan | Devam Eden | Bloke |
|--------|-----------|------------|-------|
—
```

**`<rol>-<projeKisa>/current.md`:**
```
# <rol> — <proje> — Aktif Durum

## Şu An Ne Yapıyorum
—

## Sıradaki Adımlar
—

## Notlar
—
```

---

## Adım WT-5 — Özet ve Başlatma

Yapıyı göster:

```
Worktree hazır: worktree/<isim>/<worktree-adı>/
├── koordinator/
├── <rol>-<projeKisa>/
└── ...
```

Sor:

> "Worktree'yi şimdi başlatayım mı? Koordinatör tüm agentları otomatik başlatır. (e/h)"

**e → WT-6'ya geç.**
**h → Kullanıcı istediğinde "başlat" diyebilir.**

---

## Adım WT-6 — Agentları Başlat (Koordinatör)

Koordinatör, her rol-proje için birer subagent spawn eder. Hepsi paralel çalışır.

Her subagent için şu prompt kullanılır:

```
Sen <rol> rolünde <proje> projesinde çalışan bir agentsın.
Çalışma klasörün: worktree/<isim>/<worktree-adı>/<rol>-<projeKisa>/

Başlangıçta şunları oku:
1. current.md — kaldığın yer
2. ilerleme.md — genel ilerleme
3. hafiza.md — geçmiş bağlam
4. ../koordinator/agent_ortak.md — ortak kararlar ve API bilgileri

Özet çıkar, kullanıcının yönlendirmesini bekle.
Paylaşılan yeni bir karar üretirsen ../koordinator/agent_ortak.md'e ekle.
Oturum sonunda current.md, ilerleme.md, hafiza.md'yi güncelle.
```

Tüm subagentlar başlatıldıktan sonra koordinatör kendisi de başlar:

---

## Koordinatör Agent Davranışı

### Başlangıç
1. Tüm `<rol>-<projeKisa>/current.md` ve `ilerleme.md` dosyalarını oku.
2. `koordinator/agent_ortak.md` oku.
3. `koordinator/current.md` güncelle (kim ne durumda tablosu).
4. Özet sun: "Genel durum şu — ne yapmamı istiyorsun?"

### Çalışma Sırasında
- Herhangi bir agent yeni paylaşılan bilgi üretirse → `agent_ortak.md` güncelle.
- API uç noktası / veri modeli / ortak karar belirlendiyse → `agent_ortak.md` e ekle.
- Bloker varsa → ilgili `<rol>-<proje>/current.md` e not düş.
- Kullanıcı "şu agenta şunu söyle" derse → ilgili subagenta mesaj gönder.

### Oturum Sonu
- `koordinator/current.md`, `ilerleme.md`, `hafiza.md` güncelle.
- `worktree_aciklamasi.md` deki İlerleme yüzdesini güncelle.

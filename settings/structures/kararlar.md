# Yapısal Kararlar

Bu dosya, Claude yönetim projesinin yapısını şekillendiren tasarım kararlarını içerir.
Yeni bir karar alındığında buraya eklenir.

---

## Klasör Yapısı

```
/
├── main.md                        # Geliştiricilerin giriş noktası
├── structure_main.md              # Bu projenin yapısını geliştirmek için giriş noktası
├── settings/
│   ├── settings.md                # Kullanıcı dosyası şeması ve genel kurallar
│   ├── projects.md                # Tüm projelerin listesi (tek kaynak)
│   ├── users/                     # Her kullanıcının .md dosyası (kişi+makine adı)
│   └── structures/                # Bu projenin yapısal karar ve dökümanları
├── rules/
│   ├── agent_main_rules.md        # Agent'ın genel davranış kuralları (giriş noktası)
│   ├── agent_rules/               # Mod bazlı detaylı kural dosyaları
│   │   ├── analiz_rules.md
│   │   ├── gelistirme_rules.md
│   │   ├── is_ekleme_rules.md
│   │   ├── domain_kural_tanimlama_rules.md
│   │   ├── proje_ekleme_rules.md
│   │   └── yazilim_bilgi/         # Yazılım rehberleri (desenler, prensipler, güvenlik, performans...)
│   │       ├── desenler.md
│   │       ├── prensipler.md
│   │       ├── guvenlik.md
│   │       ├── performans.md
│   │       ├── olceklenebilirlik.md
│   │       └── test_stratejileri.md
│   ├── tecrube/                   # Proje/kişi bazlı hata ve ders birikimi
│   │   ├── ortak/genel.md         # Proje bağımsız genel dersler
│   │   └── <ProjeAdı>/
│   │       ├── ortak.md           # O projeye ait ortak dersler
│   │       ├── ali.md
│   │       ├── muhammed_ali.md
│   │       └── said.md
│   ├── proje_mimari_rules/        # Tip bazlı DEĞİŞMEZ mimari kurallar
│   └── proje_domain_rules/        # Her projeye özgü domain kuralları
├── logs/                          # İşlem geçmişi (tecrübe değil, log)
│   └── <ProjeAdı>/
│       ├── ali.md
│       ├── muhammed_ali.md
│       └── said.md
├── current_md/
│   ├── _ilerleme/                 # Kişi bazlı tüm projeler özet
│   │   ├── ali.md
│   │   ├── muhammed_ali.md
│   │   ├── said.md
│   │   └── ortak.md               # Herkesin tüm projelerdeki durumu
│   └── <ProjeAdı>/
│       ├── agent.md               # Agent bağlamı, aktif görev
│       ├── mimari_gelisen.md      # Proje geneli kararlarla büyüyen mimari (herkes için geçerli)
│       ├── ortak/
│       │   ├── ortak.md           # Her commit/push sonrası güncellenir
│       │   ├── analiz.md          # Kesinleşmiş analizler (bağlayıcı)
│       │   ├── ilerleme.md        # Ortak proje ilerleyişi
│       │   ├── is_listesi.md      # Ortak iş listesi
│       │   ├── backlog.md         # Ortak backlog
│       │   ├── kurallar.md        # Ortak kurallar
│       │   ├── takilmalar.md      # Ortak takılınan noktalar
│       │   ├── is_notlari.md      # Ortak iş notları
│       │   ├── notlar.md          # Ortak genel notlar
│       │   └── mimari_kararlar.md # Ortak gelişen mimari kararlar
│       └── <kullanıcı>/
│           ├── current.md         # Çalışılan modül, ne yapacak, ilerleme
│           ├── analiz.md          # Kişisel analiz taslağı
│           ├── ilerleme.md        # Kişinin proje ilerleyişi
│           ├── is_listesi.md      # Aktif iş listesi
│           ├── backlog.md         # Sonra yapılacaklar
│           ├── kurallar.md        # Kişinin keşfettiği kurallar
│           ├── takilmalar.md      # Takılınan noktalar
│           ├── is_notlari.md      # Oturum bazlı teknik notlar
│           ├── notlar.md          # Genel notlar, fikirler
│           └── mimari_kararlar.md # Çalışma sırasında ortaya çıkan mimari kararlar
├── shared/
│   └── <ProjeA>--<ProjeB>/        # Bağlantılı proje çiftleri için ortak alan
│       └── shared.md
├── tasks/
│   └── <ProjeAdı>/
│       ├── planlama.md            # Tartışmalar, kesinleşmemiş görevler, fikirler
│       ├── havuz.md               # Kesinleşmiş ama henüz alınmamış görevler
│       └── yapilacaklar.md        # Aktif yapılacak / yapılan işler
├── referansProject/
│   ├── referans_projects.md       # Master liste: tüm projeler, platform, tech, özet
│   ├── projects_data/             # Ham proje klasörleri (kullanılınca silinecek)
│   ├── projects_data_ayristirilmis/
│   │   └── <anonim_isim>/         # Yapısal pattern dosyaları (domain genellikle çıkarılmış)
│   │       ├── klasor_yapisi.md
│   │       ├── controller_pattern.cs / .ts
│   │       ├── persistence_pattern.md
│   │       └── ... (proje yapısına göre değişir)
│   ├── projects_data_base_hali/
│   │   ├── angular_base/          # Angular base proje (sarj_qr_web baz alınmış)
│   │   └── dotnet_base/           # .NET base proje (sarj_backend_dotnet baz alınmış)
│   └── analiz/
│       └── <anonim_isim>/
│           ├── analiz.md          # Detaylı analiz — GERÇEK KOD İÇERİR (domain-intact, secret-stripped)
│           └── rules.md           # Bu projeden çıkarılan tekrar edilebilir kurallar
```

---

## Alınan Kararlar

### Kullanıcı Sistemi
- Kullanıcı adı kişi + makine kombinasyonudur (örn: `saidMasaustu`)
- Aynı kişi farklı makinelerde farklı dosyalarla temsil edilebilir
- `settings/users/<isim>.md` her kullanıcının local proje pathlerini içerir

### Proje Sistemi
- `settings/projects.md` tek kaynaktır (proje adı, tipi, bağlı proje)
- Yeni proje eklemek için `rules/proje_ekleme_rules.md` izlenir
- Projeler arası iletişim `shared/` klasörü üzerinden opsiyoneldir

### Mimari Katmanlar
- **Değişmez:** `rules/proje_mimari_rules/<tip>.md` — özel sohbet olmadan değiştirilemez
- **Gelişen:** `current_md/<proje>/mimari_gelisen.md` — kararlarla büyür
- **Domain:** `rules/proje_domain_rules/<proje>.md` — domainsel keskin kurallar

### Analiz Akışı
- Taslak: `<kullanıcı>/analiz.md` — kişisel, bağlayıcı değil
- Kesinleşme: tartışma sonrası onaylanırsa `ortak/analiz.md` ye eklenir
- `ortak/analiz.md` doğrudan düzenlenmez, herkes için bağlayıcıdır

### Task Yönetimi
- Her proje için `tasks/<proje>/` altında 3 dosya: `planlama.md`, `havuz.md`, `yapilacaklar.md`
- Akış: planlama → (kesinleşince) → havuz → (iş alınınca) → yapilacaklar
- Proje üstü genel tasks klasörü yok, sadece proje bazlı

### Kullanıcı Rolleri (Proje Bazlı — Kartezyen Çarpım)
Bir kişi farklı projeler için farklı rollere sahip olabilir. Rol bilgisi kullanıcı dosyasındaki `Projeler` tablosuna (Proje | Rol | Path) kaydedilir. Rol sorusu Adım 3'te gelir, proje seçiminden SONRA.

Her rol klasörünün tam donanımı: `current.md`, `ilerleme.md`, `kurallar.md` (root) + `private/` (notlar, tartisma, taslaklar) + `public/` (role özgü çıktılar).

- `gelistirici` — kişi bazlı, `current_md/<proje>/<isim>/`. Path sorulur.
- `analiz_uzmani` — klasör adı `is_analisti/`. `public/`: analizler, gereksinimler, akislar, sorular, tasklar, test_senaryolari
- `tester` — `public/`: test_senaryolari, bug_raporlari, test_sonuclari, regression_listesi, cozulmus_buglar
- `task_uzmani` — `public/`: tasklar, sprintler, bug_tasklari, backlog, tartismalar. Ayrıca `tasks/<proje>/`
- `tasarimci` — `public/`: tasarimlar, komponentler, stiller, kullanici_akislari, prototipler
- `arge_muhendisi` — Araştırma, PoC, teknoloji değerlendirme. `public/`: arastirmalar, prototipler, teknik_raporlar, teknoloji_degerlendirme, oneriler
- `yazilim_mimari` — ADR, mimari tasarım. `public/`: mimariler, kararlar, entegrasyon_desenleri, mimari_dokumanlar. `mimari_gelisen.md` güncelleme yetkisi bu roldedir.
- `kalite_muhendisi` — Kalite standartları, metrikler, teknik borç. `public/`: kalite_raporlari, standartlar, metrikler, teknik_borc
- `yonetici` — Tip bazlı (Adım 3 atlanır). `public/`: ilerleme_raporu, is_atamalari, sprint_plani, riskler, toplanti_notlari

> Rol tanımları ve klasör yapıları: `rules/roller/<rol>.md`

### Agent Akışı (main.md)
1. Kullanıcıyı belirle (isim + tip: normal / yonetici) — rol SORULMAZ
2. Proje seç (yoksa ekle)
3. Proje bazlı rol göster/sor (yönetici hariç); gelistirici rolüyse path al
4. Role göre bağlam yükle
5. Role göre mod menüsü göster

### Referans Proje Sistemi
- Ham proje klasörleri `projects_data/<ProjeAdı>/` altına bırakılır (karışık, sırasız olabilir)
- Agent `projects_data/` tarar → platform, tech stack, yapı çıkarır → `referans_projects.md` doldurur
- Her referans proje için `referansProject/analiz/<ProjeAdı>/analiz.md` ve `rules.md` oluşturulur
- `analiz.md`: detaylı mimari analiz + **gerçek domain kod blokları** (domain-intact, secret-stripped — parola/env/connectionstring yok)
- `rules.md`: bu projeden üretilen, geliştirme sırasında uygulanabilir kurallar (kod örnekli)
- `projects_data_ayristirilmis/<ProjeAdı>/`: yapısal pattern dosyaları (bazıları domain-stripped, bazıları domain-intact)
- Hem yapı kurulumu sırasında hem `main.md` agent akışında (geliştirme modu) bu dosyalar okunabilir/güncellenebilir
- Kullanıcı sözlü olarak da ekleme yapabilir, agent ilgili `analiz.md` / `rules.md` günceller
- **Kullanım:** "Bu sayfayı şu referans projedeki gibi yap" denildiğinde → `analiz/` altındaki ilgili analiz.md okunur, kod bloğu referans alınır

### project_Design Klasörü
- Backend hariç frontend projeler için merkezi design alanı: `project_Design/<ProjeAdı>/`
- Kapsam: RestaurantSystemPanel, RestaurantSystemQr
- **Okuma:** Tüm roller
- **Yazma:** `tasarimci` (tüm klasörler), `arge_muhendisi` (arge_design/), `analiz_uzmani` (design_metni, arayuz_aciklamalari)
- Yapı:
  - `design_kurallari.md` — renk, tipografi, boşluk, komponent standartları
  - `design_metni.md` — tasarımın ruh/ton/kimlik metin hali (analiz_uzmani yazar)
  - `arayuz_aciklamalari.md` — her ekran ve bileşenin detaylı açıklaması
  - `example_design/` — referans/ilham alınan ham tasarımlar (screenshot, link)
  - `example_design_duzenlenmis/` — notlanmış, uyarlama kararları eklenmiş referanslar
  - `mevcut_design/` — projenin gerçek mevcut tasarım durumu
  - `arge_design/` — arge mühendisinin deneysel tasarım araştırmaları
  - `gecici/` — geçici/çer çöp dosyalar, periyodik temizlenir

### hamExample Klasörü (project_Design altında, proje bazlı değil)
- Konum: `project_Design/hamExample/` — proje ayrımı yapmadan sistemin bütününü ilgilendiren ham materyaller
- **Amaç:** Hangi projeye ait olduğu belirsiz veya her iki projeyi kapsayan dökümanların "önce at, sonra analiz et" alanı
- **Yapı:** Flat (alt klasör yok) — `example1`, `example2` gibi dosyalar doğrudan klasörde
- **Kural:** Bu klasöre dosyaları HEP KULLANICI koyar, Claude eklemez
- **Analiz akışı:** tasarimci / arge_muhendisi / analiz_uzmani rolleri, mod menüsünden önce "projeye devam mı, genel analiz mi?" sorar → analiz seçilirse dosyalar listelenir, analiz edilir, kullanıcıdan proje ataması istenir:
  - Panel → `project_Design/RestaurantSystemPanel/example_design_duzenlenmis/`
  - QR → `project_Design/RestaurantSystemQr/example_design_duzenlenmis/`
  - Genel / her ikisi de → `project_Design/genel/example_design_duzenlenmis/`
- **Temizlik:** Analizi tamamlanan materyaller ilgili proje klasörüne taşınır, `hamExample/` periyodik temizlenir
- `example_design/` farkı: `example_design/` proje bazlı, curated referanslar içerir; `hamExample/` proje-üstü, ham, sırasız döküman deposudur

### Bilgisayar Bazlı Path Sistemi
- Kullanıcı dosyalarında rol ve path ayrıldı: `## Projeler` tablosu rol tutar (bilgisayardan bağımsız), `## Bilgisayar: <ad>` section'ları path tutar
- Adım 1'de kullanıcı seçimi sonrası "Hangi bilgisayardasın?" sorusu eklendi — mevcut bilgisayarlar listelenir + "Yeni bilgisayar ekle"
- Yeni bilgisayardan ilk girişte section oluşturulur, path'ler `-` ile başlar, geliştirici rolünde doldurulur
- `aktif_bilgisayar` oturum boyunca not edilir — path okuma/yazma bu section'a göre yapılır
- Aynı kural `ortak.md`'de de geçerli: her kişinin kendi section'ı içinde `### Bilgisayar: <ad>` yapısı kullanılır
- `worktree_rules.md` WT-1 adımına da bilgisayar sorusu eklendi

### Log Sistemi
- Her agent, her oturum sonunda `logs/<ProjeAdı>/<isim>.md` dosyasına giriş yazar (zorunlu)
- Format `rules/log_rules.md`'de tanımlı — Claude'un hızlı bağlam kurabilmesi için optimize edilmiş
- Yeni girişler en üste eklenir — Claude sadece ilk bloğu okur, tüm geçmişi değil
- Oturum başında (Adım 4): son log girişinin Özet + Yarım Kalanlar + Blokerlar kısmı okunur
- Worktree koordinatörü: `logs/<ProjeAdı>/koordinator_<isim>.md` yazar, Agent Durumu tablosu içerir
- Log ≠ tecrübe: log işlem kaydıdır, `rules/tecrube/` ise öğrenilen dersleri içerir

### Worktree (Multi-Agent) Sistemi
- `main.md` Adım 0'da mod seçimi eklendi: Normal / Worktree
- Worktree seçilince `rules/worktree_rules.md` akışı devreye girer
- **Tek terminal, çok agent:** Koordinatör ana agent olarak çalışır, her rol-proje için subagent spawn eder
- Klasör yapısı: `worktree/<kullanıcıAdı>/<worktree-adı>/koordinator/` + `<rol>-<projeKisa>/`
- Koordinatör dosyaları: `current.md` (kim ne yapıyor), `hafiza.md`, `agent_ortak.md` (paylaşılan API/kararlar), `ilerleme.md`
- Her rol-proje dosyaları: `current.md`, `ilerleme.md`, `hafiza.md`
- Worktree seçimi: mevcut listeyi açıklama + ilerleme ile göster, ya seç ya yeni oluştur
- Rol-proje seçimi döngüseldir, koordinatör otomatik eklenir
- `agent_ortak.md`: tüm agentların okuyup yazdığı ortak alan (API uç noktaları, veri modelleri, bağımlılıklar)

### Git & Branch Stratejisi
- Pull alırken conflict varsa yerel değişiklikler öncelikli: `git pull -X ours`
- Periyodik push hedefi: `said_local` branch'i
- Branch isimlendirme: `<kullanıcıAdı>_local` formatı (ali-laptop-local, muhammedali_local, said_local)

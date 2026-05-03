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
│   └── <ProjeAdı>/
│       ├── agent.md               # Agent bağlamı, aktif görev
│       ├── mimari_gelisen.md      # Kararlarla büyüyen mimari döküman
│       ├── ortak/
│       │   ├── ortak.md           # Her commit/push sonrası güncellenir
│       │   └── analiz.md          # Kesinleşmiş analizler (bağlayıcı)
│       └── <kullanıcı>/
│           ├── current.md         # Çalışılan modül, ne yapacak, ilerleme
│           └── analiz.md          # Kişisel analiz taslağı
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
│   │   └── <anonim_isim>/         # Yapısal dosyalar, hassas bilgi temizlenmiş
│   ├── projects_data_base_hali/
│   │   ├── angular_base/          # Angular base proje (sarj_qr_web baz alınmış)
│   │   └── dotnet_base/           # .NET base proje (sarj_backend_dotnet baz alınmış)
│   └── <anonim_isim>/
│       ├── analiz.md              # Detaylı analiz ve çıkarımlar
│       └── rules.md               # Bu projeden çıkarılan kurallar
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

### Agent Akışı (main.md)
1. Kullanıcıyı belirle
2. Proje seç (yoksa ekle, path yoksa al)
3. Mod seç: Geliştirme / Analiz / Domainsel kural tanımlama / İş ekleme

### Referans Proje Sistemi
- Ham proje klasörleri `projects_data/<ProjeAdı>/` altına bırakılır (karışık, sırasız olabilir)
- Agent `projects_data/` tarar → platform, tech stack, yapı çıkarır → `referans_projects.md` doldurur
- Her referans proje için `referansProject/<ProjeAdı>/analiz.md` ve `rules.md` oluşturulur
- `analiz.md`: mimari kararlar, dikkat çeken desenler, çıkarımlar (detaylı)
- `rules.md`: bu projeden üretilen, geliştirme sırasında uygulanabilir kurallar
- Hem yapı kurulumu sırasında hem `main.md` agent akışında (geliştirme modu) bu dosyalar okunabilir/güncellenebilir
- Kullanıcı sözlü olarak da ekleme yapabilir, agent ilgili `analiz.md` / `rules.md` günceller

# Task Uzmanı Rolü

## Tanım
Task oluşturan, backlog yöneten, sprint planlayan, iş atayan ekip üyesi. Analizden gelen gereksinimleri somut task'lara dönüştürür.

## Klasör Yapısı
```
current_md/<proje>/task_uzmani/
├── current.md
├── ilerleme.md
├── kurallar.md
├── private/
│   ├── notlar.md
│   ├── tartisma.md       # Analiz uzmanı / tasarımcı ile tartışma notları
│   └── taslaklar/
└── public/
    ├── tasklar/           # Tanımlanmış tasklar
    ├── sprintler/         # Sprint planları
    ├── bug_tasklari/      # Testerdan gelen bug'lardan açılan tasklar
    ├── backlog/           # Sıralanmış ürün backlog'u
    └── tartismalar/       # Ekiple yapılan tartışma kayıtları

tasks/<proje>/
├── planlama.md            # Tartışmalar, kesinleşmemiş görevler, fikirler
├── havuz.md               # Kesinleşmiş, alınmamış görevler
└── yapilacaklar.md        # Aktif görevler (kim ne yapıyor)
```

## Task Akışı
`planlama.md` → (kesinleşince) → `havuz.md` → (alınınca) → `yapilacaklar.md`

## Mod Menüsü
1. Task Oluştur (analizden / manuel)
2. Bug Task Aç (testerdan gelen bug'dan)
3. Backlog Düzenle / Sırala
4. Sprint Oluştur / Güncelle
5. Tartışma Notu Al (analiz uzmanı / tasarımcı ile)
6. Task Ata

## Okur
- `is_analisti/public/` (analiz ve gereksinimler)
- `tester/public/bug_raporlari/` (bug'lar)
- `tasarimci/public/` (tasarım kararları)

## Yazar
- `task_uzmani/public/tasklar/`, `sprintler/`, `bug_tasklari/`, `backlog/`, `tartismalar/`
- `tasks/<proje>/`

## Oturum Sonu Güncellenecekler
`task_uzmani/current.md`, `task_uzmani/ilerleme.md`, `tasks/<proje>/` dosyaları

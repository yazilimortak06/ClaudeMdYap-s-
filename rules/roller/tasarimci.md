# Tasarımcı Rolü

## Tanım
UI/UX tasarımı yapan, komponentler tanımlayan, stil rehberi yöneten ekip üyesi. Tasarım kararları `public/` ile tüm ekiple paylaşılır.

## Klasör Yapısı
```
current_md/<proje>/tasarimci/
├── current.md
├── ilerleme.md
├── kurallar.md
├── private/
│   ├── notlar.md
│   ├── tartisma.md       # Analiz uzmanı / geliştirici ile tasarım tartışmaları
│   └── taslaklar/        # Ham eskizler, deneme tasarımları
└── public/
    ├── tasarimlar/        # Kesinleşmiş ekran tasarımları
    ├── komponentler/      # Komponent tanımları ve kuralları
    ├── stiller/           # Renk, tipografi, spacing rehberi
    ├── kullanici_akislari/ # UX akış diyagramları
    └── prototipler/       # Etkileşimli prototip linkleri / açıklamaları
```

## Mod Menüsü
1. Ekran Tasarımı Yap
2. Komponent Tanımla
3. Stil Rehberi Güncelle
4. Kullanıcı Akışı Çiz
5. Prototip Ekle

## Okur
- `is_analisti/public/akislar/` (kullanıcı akışları)
- `is_analisti/public/gereksinimler/`
- `current_md/<proje>/mimari_gelisen.md`
- `project_Design/<proje>/design_kurallari.md`
- `project_Design/<proje>/design_metni.md`
- `project_Design/<proje>/arayuz_aciklamalari.md`
- `project_Design/<proje>/example_design/` (referans tasarımlar)
- `project_Design/<proje>/example_design_duzenlenmis/`
- `project_Design/<proje>/arge_design/` (arge önerileri)

## Yazar
- Taslak: `tasarimci/private/taslaklar/`
- Nihai: `tasarimci/public/`
- `project_Design/<proje>/design_kurallari.md`
- `project_Design/<proje>/design_metni.md`
- `project_Design/<proje>/arayuz_aciklamalari.md`
- `project_Design/<proje>/mevcut_design/`
- `project_Design/<proje>/example_design_duzenlenmis/`
- `project_Design/<proje>/gecici/`

## Oturum Sonu Güncellenecekler
`tasarimci/current.md`, `tasarimci/ilerleme.md`, taslaktan `public/`'a taşıma, ilgili `project_Design/<proje>/` dosyaları

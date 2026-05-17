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
- `project_Design/projects/<proje>/design_kurallari.md`
- `project_Design/projects/<proje>/design_metni.md`
- `project_Design/projects/<proje>/arayuz_aciklamalari.md`
- `project_Design/projects/<proje>/example_design/` (referans tasarımlar)
- `project_Design/projects/<proje>/example_design_duzenlenmis/`
- `project_Design/projects/<proje>/arge_design/` (arge önerileri)
- `project_Design/hamExample/` (ham, proje-üstü materyaller)

## hamExample Bilgisi
- `project_Design/hamExample/` — proje-üstü ham materyal deposu
- **Bu klasöre dosyaları HEP KULLANICI koyar.** Claude buraya dosya eklemez.
- Dosyalar flat yapıda durur (alt klasör yok), kullanıcı `example1`, `example2` vb. şeklinde atar

## Yazar
- Taslak: `tasarimci/private/taslaklar/`
- Nihai: `tasarimci/public/`
- `project_Design/projects/<proje>/design_kurallari.md`
- `project_Design/projects/<proje>/design_metni.md`
- `project_Design/projects/<proje>/arayuz_aciklamalari.md`
- `project_Design/projects/<proje>/mevcut_design/`
- `project_Design/projects/<proje>/example_design_duzenlenmis/`
- `project_Design/projects/<proje>/gecici/`
- `project_Design/projects/<proje>/yapilacak_design/` (geliştirici buraya bakarak UI yapar — spec dosyaları)
- `project_Design/projects/<proje>/yapilacak_design/oncelik_sirasi.md`

## Son Adım Sorusu (Zorunlu)
Mod menüsü gösterilmeden hemen önce sorulur:

> "Projelerde mi çalışmak istiyorsun yoksa genel tasarım/analiz mi yapacaksın?"
> 1. **Projeye devam** → normal mod menüsünü göster
> 2. **Genel / hamExample** →
>    - `project_Design/hamExample/` klasörünü tara
>    - Her materyali her iki projenin `example_design_duzenlenmis/` klasörlerine karşı kontrol et — karşılığı yoksa "analiz edilmemiş (yeni)" say
>    - Kullanıcıya say ve listele: **"Şu an X adet materyal var: Example1, Example2... Hangisi üzerinde çalışalım?"**
>    - Seçim yapılınca analiz et → kullanıcıdan proje ataması al:
      - `RestaurantSystemPanel` → `project_Design/projects/RestaurantSystemPanel/example_design_duzenlenmis/`
      - `RestaurantSystemQr` → `project_Design/projects/RestaurantSystemQr/example_design_duzenlenmis/`
      - `Genel / her ikisi de` → `project_Design/genel/example_design_duzenlenmis/`

## Oturum Sonu Güncellenecekler
`tasarimci/current.md`, `tasarimci/ilerleme.md`, taslaktan `public/`'a taşıma, ilgili `project_Design/<proje>/` dosyaları, `claude_context/<oturum>/oturum.md`

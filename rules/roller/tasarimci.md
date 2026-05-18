# Tasarımcı Rolü

## Tanım
UI/UX tasarımı yapan, komponentler tanımlayan, stil rehberi yöneten ekip üyesi.

## Klasör Yapısı
```
current_md/<proje>/tasarimci/       # agent takip (çıktı buraya gelmez)
├── current.md
├── ilerleme.md
├── kurallar.md
└── private/
    ├── notlar.md
    ├── tartisma.md
    └── taslaklar/                  # ham eskizler (kesinleşince design/ ye taşı)

projectDesignAnalizTaskArge/projects/<proje>/design/   # TÜM çıktılar buraya
├── design_kurallari.md
├── design_metni.md
├── arayuz_aciklamalari.md
├── example_design/
├── example_design_duzenlenmis/
├── mevcut_design/
├── arge_design/
├── taslak_design/
├── gecici/
└── yapilacak_design/               # gelistirici buradan okur
    └── oncelik_sirasi.md
```

## Mod Menüsü
1. Ekran Tasarımı Yap
2. Komponent Tanımla
3. Stil Rehberi Güncelle
4. Kullanıcı Akışı Çiz
5. Prototip Ekle

## Okur
- `projectDesignAnalizTaskArge/projects/<proje>/analiz/` (analiz_uzmani çıktıları)
- `projectDesignAnalizTaskArge/projects/<proje>/design/`
- `projectDesignAnalizTaskArge/hamExample/`
- `current_md/<proje>/mimari_gelisen.md`

## Yazar
- Taslak: `tasarimci/private/taslaklar/`
- Nihai: `projectDesignAnalizTaskArge/projects/<proje>/design/`

## hamExample Bilgisi
- `projectDesignAnalizTaskArge/hamExample/` — proje-üstü ham materyal deposu
- **Bu klasöre dosyaları HEP KULLANICI koyar.** Claude buraya dosya eklemez.

## Son Adım Sorusu (Zorunlu)
Mod menüsü gösterilmeden hemen önce sorulur:

> "Projelerde mi çalışmak istiyorsun yoksa genel tasarım/analiz mi yapacaksın?"
> 1. **Projeye devam** → normal mod menüsünü göster
> 2. **Genel / hamExample** →
>    - `projectDesignAnalizTaskArge/hamExample/` klasörünü tara
>    - Her materyali `projects/*/design/example_design_duzenlenmis/` ile karşılaştır — yoksa "yeni"
>    - **"Şu an X adet materyal var: Example1, ... Hangisi üzerinde çalışalım?"**
>    - Analiz et → kullanıcıdan proje ve tip ataması al:
>      - Panel design → `projects/RestaurantSystemPanel/design/example_design_duzenlenmis/`
>      - QR design → `projects/RestaurantSystemQr/design/example_design_duzenlenmis/`
>      - Genel → `genel/design/example_design_duzenlenmis/`

## Oturum Sonu Güncellenecekler
`tasarimci/current.md`, `tasarimci/ilerleme.md`, ilgili `design/` dosyaları

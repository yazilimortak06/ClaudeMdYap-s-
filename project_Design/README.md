# project_Design

Backend hariç tüm frontend projelerinin design merkezi.

## Klasör Yapısı

```
project_Design/
├── RestaurantSystemPanel/
└── RestaurantSystemQr/
    ├── design_kurallari.md         # Renk, tipografi, boşluk, komponent standartları
    ├── design_metni.md             # Tasarımın metin hali — ruh, ton, kimlik
    ├── arayuz_aciklamalari.md      # Her ekran ve bileşenin detaylı açıklaması
    ├── example_design/             # Referans / ilham alınan tasarımlar (screenshot, link)
    ├── example_design_duzenlenmis/ # İşlenmiş, notlanmış referans tasarımlar
    ├── mevcut_design/              # Mevcut projenin gerçek tasarım durumu
    ├── arge_design/                # Arge mühendisinin deneysel tasarım araştırmaları
    ├── taslak_design/              # Ham taslaklar (kağıt foto, Paint çizimi, karalama)
    └── gecici/                     # Geçici / çer çöp tasarım dosyaları
```

## Kim Okuyabilir?
Tüm roller.

## Kim Değiştirebilir?
- `tasarimci` → tüm klasörler
- `arge_muhendisi` → `arge_design/`
- `analiz_uzmani` → `design_metni.md`, `arayuz_aciklamalari.md`

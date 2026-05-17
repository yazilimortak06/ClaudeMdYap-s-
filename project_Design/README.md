# project_Design

Backend hariç tüm frontend projelerinin design merkezi.

## Klasör Yapısı

```
project_Design/
├── projects/
│   ├── RestaurantSystemPanel/
│   └── RestaurantSystemQr/
│       ├── design_kurallari.md         # Renk, tipografi, boşluk, komponent standartları
│       ├── design_metni.md             # Tasarımın metin hali — ruh, ton, kimlik
│       ├── arayuz_aciklamalari.md      # Her ekran ve bileşenin detaylı açıklaması
│       ├── example_design/             # Referans / ilham alınan tasarımlar (screenshot, link)
│       ├── example_design_duzenlenmis/ # İşlenmiş, notlanmış referans tasarımlar
│       ├── mevcut_design/              # Mevcut projenin gerçek tasarım durumu
│       ├── arge_design/                # Arge mühendisinin deneysel tasarım araştırmaları
│       ├── taslak_design/              # Ham taslaklar (kağıt foto, Paint çizimi, karalama)
│       └── gecici/                     # Geçici / çer çöp tasarım dosyaları
├── genel/
│   └── example_design_duzenlenmis/    # Her iki projeyi kapsayan veya proje ataması belirsiz analizler
└── hamExample/                         # Kullanıcının ham materyal deposu (flat yapı, Claude eklemez)
```

## Kim Okuyabilir?
Tüm roller.

## Kim Değiştirebilir?
- `tasarimci` → `projects/<proje>/` tüm klasörler
- `arge_muhendisi` → `projects/<proje>/arge_design/`
- `analiz_uzmani` → `projects/<proje>/design_metni.md`, `projects/<proje>/arayuz_aciklamalari.md`
- `tasarimci / arge_muhendisi / analiz_uzmani` → `genel/` (hamExample analizi sonrası)

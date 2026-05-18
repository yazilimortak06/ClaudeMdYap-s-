# projectDesignAnalizTaskArge

**Geliştirici harici rollerin tüm çıktılarının merkezi.**

## Üç Kapsam

| Kapsam | Klasör | Roller |
|--------|--------|--------|
| Geliştirme | `current_md/`, kod repo | `gelistirici` |
| Design+Analiz+Task+Arge | `projectDesignAnalizTaskArge/` | `tasarimci`, `analiz_uzmani`, `task_uzmani`, `arge_muhendisi` |
| Test | ayrı (current_md) | `tester` |

## Klasör Yapısı

```
projectDesignAnalizTaskArge/
├── projects/
│   ├── RestaurantSystemPanel/
│   │   ├── design/                     # tasarimci çıktıları
│   │   │   ├── design_kurallari.md
│   │   │   ├── design_metni.md
│   │   │   ├── arayuz_aciklamalari.md
│   │   │   ├── example_design/
│   │   │   ├── example_design_duzenlenmis/
│   │   │   ├── mevcut_design/
│   │   │   ├── arge_design/            # arge'nin design araştırmaları
│   │   │   ├── taslak_design/
│   │   │   ├── gecici/
│   │   │   └── yapilacak_design/       # gelistirici buradan okur
│   │   ├── analiz/                     # analiz_uzmani çıktıları
│   │   │   ├── analizler/
│   │   │   ├── gereksinimler/
│   │   │   ├── akislar/
│   │   │   ├── sorular/
│   │   │   └── test_senaryolari/
│   │   ├── tasks/                      # task_uzmani çıktıları
│   │   │   ├── planlama.md
│   │   │   ├── havuz.md
│   │   │   └── yapilacaklar.md
│   │   └── arge/                       # arge_muhendisi çıktıları
│   │       ├── arastirmalar/
│   │       ├── prototipler/
│   │       ├── teknik_raporlar/
│   │       ├── teknoloji_degerlendirme/
│   │       └── oneriler/
│   ├── RestaurantSystemQr/
│   │   └── (aynı yapı)
│   └── RestaurantSystemBackend/
│       └── tasks/
├── genel/
│   ├── design/
│   │   └── example_design_duzenlenmis/
│   ├── analiz/
│   └── arge/
└── hamExample/                         # kullanıcının ham materyal deposu (Claude eklemez)
```

## Yazma Yetkileri
- `tasarimci` → `projects/<proje>/design/`
- `analiz_uzmani` → `projects/<proje>/analiz/`
- `task_uzmani` → `projects/<proje>/tasks/`
- `arge_muhendisi` → `projects/<proje>/arge/` ve `projects/<proje>/design/arge_design/`
- hamExample analizi sonrası → `projects/<proje>/<tip>/` veya `genel/<tip>/`

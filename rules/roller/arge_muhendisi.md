# Arge Mühendisi Rolü

## Tanım
Yeni teknolojileri araştıran, Proof-of-Concept yapan, inovasyon önerileri sunan ekip üyesi. Keşfeder, dener, raporlar; üretim kodu yazmaz.

## Klasör Yapısı
```
current_md/<proje>/arge_muhendisi/   # agent takip
├── current.md
├── ilerleme.md
├── kurallar.md
└── private/
    ├── notlar.md
    ├── tartisma.md
    └── taslaklar/                   # ham araştırma notları

projectDesignAnalizTaskArge/projects/<proje>/arge/    # TÜM çıktılar buraya
├── arastirmalar/
├── prototipler/
├── teknik_raporlar/
├── teknoloji_degerlendirme/
└── oneriler/

projectDesignAnalizTaskArge/projects/<proje>/design/arge_design/   # design araştırmaları
```

## İki Çalışma Modu

### Proje Modu
Projenin mevcut durumunu araştırarak katkı yapar.

### Bağımsız Mod
`projectDesignAnalizTaskArge/hamExample/` veya diğer kaynakları okuyarak bağımsız araştırma yapar.

## Mod Menüsü
1. Araştırma Başlat
2. Prototip / PoC Yap
3. Teknoloji Değerlendir
4. Teknik Rapor Yaz
5. Öneri Oluştur
6. hamExample Analizi

## Okur
- `projectDesignAnalizTaskArge/projects/<proje>/analiz/`
- `projectDesignAnalizTaskArge/projects/<proje>/design/`
- `projectDesignAnalizTaskArge/hamExample/`
- `referansProject/analiz/`
- `rules/`
- `current_md/<proje>/mimari_gelisen.md`

## Yazar
- Taslak: `arge_muhendisi/private/taslaklar/`
- Nihai: `projectDesignAnalizTaskArge/projects/<proje>/arge/`
- Design araştırmaları: `projectDesignAnalizTaskArge/projects/<proje>/design/arge_design/`

## hamExample Bilgisi
- `projectDesignAnalizTaskArge/hamExample/` — proje-üstü ham materyal deposu
- **Bu klasöre dosyaları HEP KULLANICI koyar.** Claude buraya dosya eklemez.

## Son Adım Sorusu (Zorunlu)
Mod menüsü gösterilmeden hemen önce sorulur:

> "Projelerde mi çalışmak istiyorsun yoksa genel arge/araştırma mı yapacaksın?"
> 1. **Projeye devam** → normal mod menüsünü göster
> 2. **Genel / hamExample** →
>    - `projectDesignAnalizTaskArge/hamExample/` klasörünü tara
>    - Her materyali `projects/*/arge/` ile karşılaştır — yoksa "yeni"
>    - **"Şu an X adet materyal var: Example1, ... Hangisi üzerinde çalışalım?"**
>    - Analiz et → kullanıcıdan proje ataması al:
>      - Panel → `projects/RestaurantSystemPanel/arge/`
>      - QR → `projects/RestaurantSystemQr/arge/`
>      - Genel → `genel/arge/`

## Oturum Sonu Güncellenecekler
`arge_muhendisi/current.md`, `arge_muhendisi/ilerleme.md`, ilgili `arge/` dosyaları

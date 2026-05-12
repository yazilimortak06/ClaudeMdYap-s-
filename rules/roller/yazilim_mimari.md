# Yazılım Mimarı Rolü

## Tanım
Sistem mimarisini tasarlayan, teknoloji kararları veren, entegrasyon desenlerini tanımlayan ekip üyesi. Aldığı kararlar `mimari_gelisen.md` aracılığıyla tüm ekip için bağlayıcıdır.

## Ne Yapar?
- Yüksek seviyeli sistem mimarisini tasarlar ve belgeler
- Architecture Decision Records (ADR) oluşturur
- Katmanlar arası entegrasyon desenlerini belirler
- Ölçeklenebilirlik, güvenlik ve sürdürülebilirlik kararları alır
- Mimari gözden geçirmeler (code/design review) yönetir
- `mimari_gelisen.md` üzerinde değişiklik yetkisi bu roldedir

## Klasör Yapısı
```
current_md/<proje>/yazilim_mimari/
├── current.md
├── ilerleme.md
├── kurallar.md
├── private/
│   ├── notlar.md
│   ├── tartisma.md              # Geliştirici / arge ile tartışma notları
│   └── taslaklar/               # Henüz kesinleşmemiş mimari taslakları
└── public/
    ├── mimariler/                # Sistem mimari diyagramları ve açıklamaları
    ├── kararlar/                 # ADR'lar (Architecture Decision Records)
    ├── entegrasyon_desenleri/    # Servisler arası entegrasyon desenleri
    └── mimari_dokumanlar/        # Genel mimari belgeler
```

## Mod Menüsü
1. Mimari Tasarla
2. Mimari Gözden Geçir
3. Teknoloji Kararı Al
4. Entegrasyon Deseni Tanımla
5. ADR (Mimari Karar) Oluştur
6. Mimari Doküman Yaz

## Okur
- `rules/proje_mimari_rules/` (değişmez kurallar)
- `current_md/<proje>/mimari_gelisen.md`
- `referansProject/analiz/`
- `arge_muhendisi/public/` (araştırma bulguları)
- `is_analisti/public/gereksinimler/`

## Yazar
- Taslak: `yazilim_mimari/private/taslaklar/`
- Nihai: `yazilim_mimari/public/`
- Onaylanınca: `current_md/<proje>/mimari_gelisen.md` günceller

## Oturum Sonu Güncellenecekler
`yazilim_mimari/current.md`, `yazilim_mimari/ilerleme.md`, taslaktan `public/`'a taşıma, `mimari_gelisen.md`

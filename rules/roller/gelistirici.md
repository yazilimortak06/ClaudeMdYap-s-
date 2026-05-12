# Geliştirici Rolü

## Tanım
Kod yazan, özellik geliştiren, bug düzelten ekip üyesi. Proje başına birden fazla geliştirici olabilir — her biri kendi kişisel klasöründe çalışır.

## Klasör Yapısı
```
current_md/<proje>/<isim>/
├── current.md          # Çalışılan modül, aktif görev, bir sonraki adım
├── ilerleme.md         # Proje üzerindeki genel ilerleme
├── is_listesi.md       # Aktif iş listesi
├── backlog.md          # Sonra yapılacaklar
├── kurallar.md         # Keşfedilen teknik kurallar (o projeye özgü)
├── analiz.md           # Kişisel analiz taslağı
├── is_notlari.md       # Oturum bazlı teknik notlar
├── notlar.md           # Genel notlar, fikirler
├── takilmalar.md       # Takılınan noktalar
└── mimari_kararlar.md  # Çalışma sırasında ortaya çıkan mimari kararlar
```

## Mod Menüsü
1. Geliştirme
2. Analiz
3. Domain kural tanımlama
4. İş ekleme

## Okur
- `current_md/<proje>/mimari_gelisen.md`
- `rules/proje_mimari_rules/`
- `rules/proje_domain_rules/<proje>.md`
- `current_md/<proje>/ortak/`

## Yazar
- Kendi `current_md/<proje>/<isim>/` klasörü
- `current_md/<proje>/mimari_gelisen.md` (mimari karar kesinleşince)
- `current_md/<proje>/ortak/` (commit/push sonrası)

## Oturum Sonu Güncellenecekler
`current.md`, `ilerleme.md`, `is_listesi.md`, `is_notlari.md`, `mimari_kararlar.md`, `mimari_gelisen.md`, `_ilerleme/<isim>.md`, `_ilerleme/ortak.md`

## Notlar
- Path sadece bu rol için sorulur (diğer rollerde sorulmaz)
- Birden fazla kişi aynı projede geliştirici olabilir

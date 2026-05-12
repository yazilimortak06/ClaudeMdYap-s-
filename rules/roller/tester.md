# Tester Rolü

## Tanım
Test senaryoları yazan, bug raporlayan, regresyon listesini yöneten ekip üyesi. Proje başına bir tester klasörü vardır.

## Klasör Yapısı
```
current_md/<proje>/tester/
├── current.md
├── ilerleme.md
├── kurallar.md
├── private/
│   ├── notlar.md
│   ├── tartisma.md
│   └── taslaklar/
└── public/
    ├── test_senaryolari/
    ├── bug_raporlari/
    ├── test_sonuclari/
    ├── regression_listesi/
    └── cozulmus_buglar/
```

## Mod Menüsü
1. Test Senaryosu Yaz
2. Bug Raporu Oluştur
3. Test Sonucu Gir
4. Regresyon Listesi Güncelle
5. Bug Kapat

## Okur
- `is_analisti/public/test_senaryolari/` (kabul kriterleri)
- `is_analisti/public/gereksinimler/`
- `is_analisti/public/akislar/`

## Yazar
- `current_md/<proje>/tester/public/`

## Oturum Sonu Güncellenecekler
`tester/current.md`, `tester/ilerleme.md`, ilgili `tester/public/` dosyaları

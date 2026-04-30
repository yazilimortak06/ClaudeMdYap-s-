# Analiz Kuralları

## Akış

1. **Taslak Analiz**
   - Kullanıcı ve agent birlikte analiz yapar.
   - Çalışma alanı: `current_md/<proje>/<kullanıcı>/analiz.md`
   - Bu dosya kişiseldir, taslaktır, bağlayıcı değildir.

2. **Tartışma & Karar**
   - Analiz tamamlandığında kullanıcıya sor:
     > "Bu analizi kesinleştirip ortak analize ekleyelim mi?"
   - Evet → Adım 3
   - Hayır → taslakta kalır, herhangi bir bağlayıcılığı yoktur.

3. **Kesinleştirme**
   - Onaylanan analiz `current_md/<proje>/ortak/analiz.md` dosyasına eklenir.
   - Eklenirken tarih ve kısa başlık ile kayıt altına alınır.
   - Bu dosyadaki her madde **tüm ekip için bağlayıcıdır.**

## Kural

- `ortak/analiz.md` doğrudan düzenlenmez. Sadece kesinleşme süreci üzerinden güncellenir.
- Kesinleşmiş bir analizi geri almak veya değiştirmek için ayrı bir tartışma gereklidir.

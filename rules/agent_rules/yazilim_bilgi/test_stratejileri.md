# Test Stratejileri

Agent bu dosyayı okuyarak test yazma standartlarını ve stratejilerini öğrenir.

---

## Test Piramidi

```
        [ E2E ]          az sayıda, yavaş, pahalı
      [ Integration ]    orta sayıda
    [   Unit Tests   ]   çok sayıda, hızlı, ucuz
```

- **Unit:** Tek bir fonksiyon/sınıf, izole, hızlı. Dış bağımlılık mock'lanır.
- **Integration:** Birden fazla katmanın birlikte çalışması (servis + DB, controller + servis).
- **E2E:** Sistemin baştan sona gerçek ortamda test edilmesi.

---

## Unit Test

### Ne Test Edilir?
- İş mantığı içeren fonksiyonlar.
- Edge case'ler (null, empty, boundary değerler).
- Hata durumları (exception fırlatılıyor mu?).

### Kurallar
- Her test tek bir şeyi test eder.
- Test isimleri ne test edildiğini açıklar: `should_returnError_when_emailIsInvalid`
- AAA pattern: **Arrange** (hazırlık) → **Act** (çalıştır) → **Assert** (doğrula).
- Test birbirinden bağımsız olmalı — sıra önemli olmamalı.
- Deterministik olmalı — her çalışmada aynı sonuç.

### Mocking
- Dış bağımlılıkları (DB, dış API, email servisi) mock'la.
- Mock aşırı kullanımından kaçın — çok fazla mock test değerini düşürür.
- Davranış test edilmeli, implementasyon detayı değil.

---

## Integration Test

### Ne Test Edilir?
- Servis + Repository birlikte çalışması.
- Controller → Servis → DB akışı.
- Dış servis entegrasyonları.

### Kurallar
- Gerçek DB kullan (test DB), production verisi değil.
- Her test öncesi DB'yi temiz duruma getir (transaction rollback veya truncate).
- Test sonrası temizlik yap.

---

## E2E Test

### Ne Test Edilir?
- Kritik kullanıcı akışları (kayıt, login, sipariş verme).
- Happy path + en kritik hata senaryoları.

### Kurallar
- Az tut — bakım maliyeti yüksek.
- Staging ortamında çalıştır.
- Flaky test'leri düzelt veya kaldır — güvenilmez test, hiç yoktan beter.

---

## TDD — Test Driven Development

Önce test yaz, sonra kodu:
1. **Red:** Başarısız olan testi yaz.
2. **Green:** Testi geçirecek minimum kodu yaz.
3. **Refactor:** Kodu iyileştir, test hâlâ geçmeli.

**Faydası:** Tasarımı test edilebilir yapar, gereksiz kod yazmayı önler.

---

## Test Coverage

- %100 coverage hedef değil — kritik iş mantığı %80+ olmalı.
- Coverage'ı yüksek tutmak için anlamsız test yazma.
- Ölçülmesi gereken: branch coverage (her if/else dalı test ediliyor mu?).

---

## Test Edilebilir Kod Yazma

- Dependency Injection kullan — mock enjekte edilebilsin.
- Sınıfları küçük tut — büyük sınıf test edilmesi zor.
- Static method'lardan kaçın — mock'lanamaz.
- Global state kullanma — testler arası sızıntı yapar.
- Pure function'lar yaz — aynı input, aynı output, side effect yok.

---

## Checklist

- [ ] İş mantığı içeren her fonksiyon için unit test var mı?
- [ ] Edge case'ler (null, empty, max) test edildi mi?
- [ ] Hata durumları (exception) test edildi mi?
- [ ] Integration test kritik akışları kapsıyor mu?
- [ ] Testler bağımsız çalışabiliyor mu?

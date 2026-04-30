# Yazılım Prensipleri

Agent bu dosyayı okuyarak her projede uygulaması gereken temel yazılım prensiplerini öğrenir.

---

## SOLID

### S — Single Responsibility Principle
Her sınıf, modül veya fonksiyonun tek bir sorumluluğu olmalıdır.
- **Yanlış:** `UserService` hem kullanıcı kaydı yapıyor hem email gönderiyor hem loglama yapıyor.
- **Doğru:** `UserService` sadece kullanıcı işlemleri, `EmailService` email, `AuditLogger` loglama.
- **İpucu:** "Bu sınıf neden değişir?" sorusunun birden fazla cevabı varsa SRP ihlali var.

### O — Open/Closed Principle
Kod yeni davranışa açık, mevcut koda değişikliğe kapalı olmalıdır.
- Yeni özellik eklerken mevcut kodu değiştirmek yerine genişlet.
- Interface/abstract class kullan, implementasyonu dışarıdan enjekte et.
- **İpucu:** Yeni bir `if/switch` bloğu ekliyorsan OCP ihlali ihtimali yüksek.

### L — Liskov Substitution Principle
Alt sınıf, üst sınıfın yerine geçebilmelidir — davranış bozulmadan.
- Override ettiğin metot, üst sınıfın sözleşmesini (precondition/postcondition) bozmamalı.
- **Yanlış:** `Square extends Rectangle` — `setWidth` her iki boyutu değiştirince davranış bozulur.

### I — Interface Segregation Principle
Büyük interface yerine küçük, odaklı interface'ler tercih edilmeli.
- Sınıf kullanmadığı metotları implement etmek zorunda kalmamalı.
- **İpucu:** Interface'de "Bu sınıf için gereksiz metot var mı?" diye sor.

### D — Dependency Inversion Principle
Yüksek seviyeli modüller düşük seviyeli modüllere bağımlı olmamalı; ikisi de soyutlamalara bağımlı olmalı.
- Concrete class'a değil, interface'e bağlan.
- Dependency Injection kullan (constructor injection tercih edilir).

---

## DRY — Don't Repeat Yourself
Aynı bilgi veya mantık sistemde yalnızca bir yerde temsil edilmeli.
- Kod kopyalama değil, ortak fonksiyon/servis/utility çıkar.
- **Dikkat:** Yüzeysel benzerlik DRY ihlali değildir. İki şey aynı nedenden değişiyorsa birleştir, farklı nedenlerden değişiyorsa ayrı tut.

## KISS — Keep It Simple, Stupid
En basit çözüm doğru çözümdür. Gereksiz karmaşıklıktan kaçın.
- 5 satırda çözülecek şey için abstract factory zinciri kurma.
- Kod okunabilirliği, zekice yazılmışlığın önünde gelir.

## YAGNI — You Aren't Gonna Need It
İhtiyaç olmayan şeyi ekleme. "Belki lazım olur" diye kod yazma.
- Şu an gerekmeyen abstraction, interface, config parametresi ekleme.
- İhtiyaç doğduğunda ekle — şimdiden değil.

---

## Clean Architecture Katmanları

```
[ Domain / Entity ]          ← iş kuralları, dış bağımlılık yok
[ Use Case / Application ]   ← uygulama mantığı, domain'e bağımlı
[ Interface / Adapter ]      ← controller, presenter, gateway
[ Infrastructure / Framework ] ← DB, HTTP, UI, dış servisler
```

**Bağımlılık yönü:** Dıştan içe. Infrastructure domain'e bağımlı, domain hiçbir şeye bağımlı değil.

---

## Repository Pattern
Veri erişim mantığını iş mantığından ayırır.
- Servisler repository interface'ine bağlanır, somut DB implementasyonuna değil.
- Test edilebilirlik artar — mock repository kullanılabilir.

## Separation of Concerns
Her katmanın kendi sorumluluğu var:
- **Controller:** HTTP request/response, validation, route yönetimi
- **Service:** İş mantığı
- **Repository:** Veri erişimi
- **Model/Entity:** Veri yapısı ve domain kuralları

---

## Kod Kalitesi Prensipleri

- **Anlamlı isimler:** `getUserById` > `getUser` > `get`. `isActive` > `flag`.
- **Küçük fonksiyonlar:** Bir fonksiyon bir iş yapar, 20-30 satırı geçmemeli.
- **Az parametre:** 3'ten fazla parametre varsa nesne al.
- **Erken return:** Nested if yerine guard clause kullan.
- **Comment değil, kod konuşsun:** İyi isimlendirilmiş kod comment'e ihtiyaç duymaz. Comment varsa "neden" açıklamalı, "ne" değil.

# qr_menu_mvc — DBservices Pattern

## Genel Açıklama

`DBservices/` klasörü, EF 6 context üzerinden veri erişimini saran bir servis katmanıdır. Controller'lar doğrudan EF context kullanmak yerine bu servis sınıflarını çağırır. Repository pattern'in basit, DI-free versiyonudur.

---

## Servis Sınıfı Yapısı

Her servis sınıfı şu yapıyı izler:

```csharp
// DBservices/UrunDBService.cs
using System.Collections.Generic;
using System.Linq;
using PixDinn.Models;  // EF otomatik üretilen modeller

namespace DBservices
{
    public class UrunDBService
    {
        // Her metot kendi context'ini oluşturur ve dispose eder
        private QrMenuEntities GetContext() => new QrMenuEntities();

        // ---------------------------------------------------------------
        // READ işlemleri
        // ---------------------------------------------------------------

        public List<Urun> GetByMekan(int mekanId)
        {
            using (var ctx = GetContext())
            {
                return ctx.Urunler
                          .Where(u => u.KategoriId != null)
                          .Join(ctx.Kategoriler,
                                u => u.KategoriId,
                                k => k.Id,
                                (u, k) => new { Urun = u, Kategori = k })
                          .Where(x => x.Kategori.MekanId == mekanId && !x.Urun.IsDeleted)
                          .OrderBy(x => x.Urun.SiraNo)
                          .Select(x => x.Urun)
                          .ToList();
            }
        }

        public Urun GetById(int id)
        {
            using (var ctx = GetContext())
            {
                return ctx.Urunler.Find(id);
            }
        }

        public List<Urun> GetByKategori(int kategoriId)
        {
            using (var ctx = GetContext())
            {
                return ctx.Urunler
                          .Where(u => u.KategoriId == kategoriId && u.IsActive && !u.IsDeleted)
                          .OrderBy(u => u.SiraNo)
                          .ToList();
            }
        }

        // ---------------------------------------------------------------
        // WRITE işlemleri
        // ---------------------------------------------------------------

        public void Create(UrunViewModel model)
        {
            using (var ctx = GetContext())
            {
                var urun = new Urun
                {
                    KategoriId = model.KategoriId,
                    Ad = model.Ad,
                    Aciklama = model.Aciklama,
                    Fiyat = model.Fiyat,
                    ResimPath = model.ResimPath,
                    SiraNo = model.SiraNo,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = System.DateTime.Now
                };
                ctx.Urunler.Add(urun);
                ctx.SaveChanges();
            }
        }

        public void Update(UrunViewModel model)
        {
            using (var ctx = GetContext())
            {
                var urun = ctx.Urunler.Find(model.Id);
                if (urun == null) return;

                urun.Ad = model.Ad;
                urun.Aciklama = model.Aciklama;
                urun.Fiyat = model.Fiyat;
                urun.SiraNo = model.SiraNo;
                urun.IsActive = model.IsActive;
                if (!string.IsNullOrEmpty(model.ResimPath))
                    urun.ResimPath = model.ResimPath;
                urun.UpdatedAt = System.DateTime.Now;

                ctx.SaveChanges();
            }
        }

        public void SoftDelete(int id)
        {
            using (var ctx = GetContext())
            {
                var urun = ctx.Urunler.Find(id);
                if (urun == null) return;

                urun.IsDeleted = true;
                urun.IsActive = false;
                ctx.SaveChanges();
            }
        }

        // ---------------------------------------------------------------
        // Sıralama güncelleme
        // ---------------------------------------------------------------

        public void UpdateOrder(List<int> orderedIds)
        {
            using (var ctx = GetContext())
            {
                for (int i = 0; i < orderedIds.Count; i++)
                {
                    var urun = ctx.Urunler.Find(orderedIds[i]);
                    if (urun != null)
                        urun.SiraNo = i + 1;
                }
                ctx.SaveChanges();
            }
        }
    }
}
```

---

## MenuDBService — Public Menü Sorguları

```csharp
// DBservices/MenuDBService.cs
public class MenuDBService
{
    private QrMenuEntities GetContext() => new QrMenuEntities();

    // QR kod ile erişilen public menü
    public PublicMenuViewModel GetPublicMenu(string mekanKod)
    {
        using (var ctx = GetContext())
        {
            var mekan = ctx.Mekanlar
                           .FirstOrDefault(m => m.MekanKod == mekanKod && m.IsActive);
            if (mekan == null) return null;

            var kategoriler = ctx.Kategoriler
                                 .Where(k => k.MekanId == mekan.Id && k.IsActive)
                                 .OrderBy(k => k.SiraNo)
                                 .ToList();

            var urunler = ctx.Urunler
                             .Join(ctx.Kategoriler,
                                   u => u.KategoriId,
                                   k => k.Id,
                                   (u, k) => new { Urun = u, KategoriMekanId = k.MekanId })
                             .Where(x => x.KategoriMekanId == mekan.Id && x.Urun.IsActive)
                             .OrderBy(x => x.Urun.SiraNo)
                             .Select(x => x.Urun)
                             .ToList();

            return new PublicMenuViewModel
            {
                MekanAd = mekan.Ad,
                MekanLogo = mekan.LogoPath,
                Kategoriler = kategoriler,
                Urunler = urunler
            };
        }
    }

    public PublicMenuViewModel GetMenuByCategory(string mekanKod, int kategoriId)
    {
        using (var ctx = GetContext())
        {
            var mekan = ctx.Mekanlar.FirstOrDefault(m => m.MekanKod == mekanKod);
            if (mekan == null) return null;

            var urunler = ctx.Urunler
                             .Where(u => u.KategoriId == kategoriId && u.IsActive)
                             .OrderBy(u => u.SiraNo)
                             .ToList();

            return new PublicMenuViewModel
            {
                MekanAd = mekan.Ad,
                AktifKategoriId = kategoriId,
                Urunler = urunler
            };
        }
    }
}
```

---

## AdminUserDBService — Authentication

```csharp
// DBservices/AdminUserDBService.cs
public class AdminUserDBService
{
    private QrMenuEntities GetContext() => new QrMenuEntities();

    // Kullanıcı adı ve şifre hash ile doğrulama
    public AdminUser Authenticate(string username, string password)
    {
        using (var ctx = GetContext())
        {
            // UYARI: Gerçek projede şifre hash kullan
            // BCrypt.Net veya PBKDF2 gibi güvenli hash algoritması
            var passwordHash = HashPassword(password);

            return ctx.AdminUsers
                      .FirstOrDefault(u => u.Username == username
                                      && u.PasswordHash == passwordHash
                                      && u.IsActive);
        }
    }

    private string HashPassword(string password)
    {
        // Basit örnek — gerçekte BCrypt kullan
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return System.Convert.ToBase64String(bytes);
        }
    }
}
```

---

## EF Context Kullanım Notları

### Neden `using` Bloğu?

Her metot kendi `using (var ctx = GetContext())` bloğunu açar. Bu:
- Connection'ı kısa tutar
- EF context state sorunlarını önler
- Dispose edilmeyen context birikmesini önler

**Dezavantaj:** İlişkili entity'ler için lazy loading `using` bloğu dışında çalışmaz. Tüm gerekli veriler aynı `using` içinde yüklenmeli.

### Eager Loading Pattern

```csharp
// Include ile ilişkili veri yükle
return ctx.Mekanlar
          .Include("Kategoriler")          // EF 6 string-based
          .Include("Kategoriler.Urunler")  // Nested include
          .FirstOrDefault(m => m.Id == id);
```

### Modernizasyon Notu

EF Core ile geçiş yapılırsa:
```csharp
// EF Core — lambda-based include
return await ctx.Mekanlar
                .Include(m => m.Kategoriler)
                    .ThenInclude(k => k.Urunler)
                .FirstOrDefaultAsync(m => m.Id == id);
```

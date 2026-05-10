# RestaurantSystem Backend – Domain Kuralları

Bu dosya, RestaurantSystem projesinin backend tarafına ait domain bilgilerini, entity modellerini,
iş kurallarını, naming convention'larını ve servis/endpoint yapılarını tanımlar.
Claude bu dosyayı okuyarak backend kodunu üretirken doğru domain kararlarını verir.

---

## 1. Entity Listesi ve C# Modelleri

Tüm entity'ler `BaseEntity<TId>` sınıfından kalıtım alır.
`BaseEntity` FrameworkCore katmanında tanımlıdır ve şu alanları içerir:

```csharp
// FrameworkCore/Domain/BaseEntity.cs
public abstract class BaseEntity<TId>
{
    public TId Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false; // soft-delete
}
```

Multi-tenant entity'ler ayrıca `ITenantEntity` interface'ini implement eder:

```csharp
// FrameworkCore/Domain/ITenantEntity.cs
public interface ITenantEntity
{
    Guid RestaurantId { get; set; } // hangi restorana ait olduğunu belirtir
}
```

---

### 1.1 Restaurant

Sistemdeki her restoranı temsil eder. Multi-tenant mimarinin kök entity'sidir.
Tüm diğer tenant-owned entity'ler `RestaurantId` alanı üzerinden bu entity'ye bağlıdır.

```csharp
// Domain/Entities/Restaurant.cs
public class Restaurant : BaseEntity<Guid>
{
    public string Name { get; set; }           // restoran adı
    public string Slug { get; set; }           // URL-friendly benzersiz tanımlayıcı (qr-menu linkinde kullanılır)
    public string? LogoUrl { get; set; }        // logo dosya yolu (FileAPI üzerinden)
    public string? CoverImageUrl { get; set; }  // kapak görseli
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<MenuCategory> MenuCategories { get; set; }
    public ICollection<Table> Tables { get; set; }
    public ICollection<User> Users { get; set; }
    public ICollection<Printer> Printers { get; set; }
    public ICollection<Setting> Settings { get; set; }
}
```

---

### 1.2 MenuCategory

Menü kategorilerini temsil eder (Başlangıçlar, Ana Yemekler, İçecekler vb.).
Bir kategorinin alt kategorisi olabilir (self-referencing).

```csharp
// Domain/Entities/MenuCategory.cs
public class MenuCategory : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }      // hangi restorana ait
    public string Name { get; set; }            // kategori adı
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; } = 0;     // sıralama önceliği
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; } // alt kategori için üst kategori referansı

    // Navigation properties
    public Restaurant Restaurant { get; set; }
    public MenuCategory? ParentCategory { get; set; }
    public ICollection<MenuCategory> SubCategories { get; set; }
    public ICollection<MenuItem> MenuItems { get; set; }
}
```

---

### 1.3 MenuItem

Menüdeki her bir ürünü (yemek, içecek vb.) temsil eder.

```csharp
// Domain/Entities/MenuItem.cs
public class MenuItem : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public Guid MenuCategoryId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true; // anlık stok durumu
    public int? PreparationTimeMinutes { get; set; } // tahmini hazırlık süresi (dakika)

    // Navigation properties
    public Restaurant Restaurant { get; set; }
    public MenuCategory MenuCategory { get; set; }
    public ICollection<MenuItemOption> Options { get; set; }
}
```

---

### 1.4 MenuItemOption

Bir menü öğesine ait seçenekleri tanımlar (boy seçimi, ek malzeme, pişirme tercihi vb.).
Her seçenek bir grup altında toplanır (ör. "Boy Seçimi" grubu altında: Küçük, Orta, Büyük).

```csharp
// Domain/Entities/MenuItemOption.cs
public class MenuItemOption : BaseEntity<Guid>
{
    public Guid MenuItemId { get; set; }
    public string GroupName { get; set; }        // seçenek grubu adı (ör. "Boy Seçimi")
    public string Name { get; set; }             // seçenek adı (ör. "Büyük")
    public decimal AdditionalPrice { get; set; } = 0; // ek ücret (0 ise ücretsiz)
    public bool IsRequired { get; set; } = false;     // zorunlu seçim mi
    public bool IsMultiSelect { get; set; } = false;  // çoklu seçim izni
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public MenuItem MenuItem { get; set; }
}
```

---

### 1.5 Table

Restorandaki fiziksel masaları temsil eder.

```csharp
// Domain/Entities/Table.cs
public class Table : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }            // masa adı/numarası (ör. "Masa 5", "VIP-1")
    public int Capacity { get; set; } = 4;      // kaç kişilik
    public string? Description { get; set; }    // konum/açıklama (ör. "Bahçe - Köşe")
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Restaurant Restaurant { get; set; }
    public QrCode? QrCode { get; set; }         // masa ile ilişkili QR kod
    public ICollection<Order> Orders { get; set; }
}
```

---

### 1.6 QrCode

Her masaya ait QR kodu bilgilerini tutar.
QR kod tarandığında kullanıcı RestaurantSystem QR projesine yönlendirilir.

```csharp
// Domain/Entities/QrCode.cs
public class QrCode : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public Guid TableId { get; set; }            // hangi masaya ait
    public string Token { get; set; }            // benzersiz QR token (GUID tabanlı)
    public string QrImageUrl { get; set; }       // üretilen QR görsel dosya yolu
    public bool IsActive { get; set; } = true;
    public DateTime? LastScannedAt { get; set; } // son taranma zamanı

    // Navigation properties
    public Restaurant Restaurant { get; set; }
    public Table Table { get; set; }
}
```

**QR Token Üretim Kuralı:**
`{restaurantSlug}/{tableId}/{uniqueToken}` formatında URL oluşturulur.
Örnek: `https://qr.restaurantsystem.com/kebapci/table-5/a1b2c3d4`

---

### 1.7 Order

Bir masadan verilen siparişi temsil eder. Sipariş yaşam döngüsünün merkez entity'sidir.

```csharp
// Domain/Entities/Order.cs
public class Order : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public Guid TableId { get; set; }
    public string OrderCode { get; set; }         // okunabilir sipariş kodu (ör. "ORD-20240510-0042")
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }      // hesaplanan toplam tutar
    public string? CustomerNote { get; set; }     // müşteri notu
    public string? CancelReason { get; set; }     // iptal gerekçesi (Cancelled durumunda)
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public Guid? AssignedWaiterId { get; set; }   // siparişten sorumlu garson (opsiyonel)

    // Navigation properties
    public Restaurant Restaurant { get; set; }
    public Table Table { get; set; }
    public User? AssignedWaiter { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
```

---

### 1.8 OrderItem

Bir sipariş içindeki her bir ürün kalemini temsil eder.

```csharp
// Domain/Entities/OrderItem.cs
public class OrderItem : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public string MenuItemName { get; set; }      // sipariş anındaki ürün adı snapshot'ı
    public decimal UnitPrice { get; set; }        // sipariş anındaki birim fiyat snapshot'ı
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }       // UnitPrice * Quantity + seçenek fiyatları
    public string? Note { get; set; }             // kalem bazında özel not

    // Navigation properties
    public Order Order { get; set; }
    public MenuItem MenuItem { get; set; }
    public ICollection<OrderItemOption> OrderItemOptions { get; set; }
}
```

**Snapshot Kuralı:** `MenuItemName` ve `UnitPrice` alanları sipariş oluşturulurken
o anki değerden kopyalanır. Sonraki fiyat/isim değişikliklerinden etkilenmez.

---

### 1.9 OrderItemOption

Bir sipariş kalemi için seçilen seçenekleri kaydeder.

```csharp
// Domain/Entities/OrderItemOption.cs
public class OrderItemOption : BaseEntity<Guid>
{
    public Guid OrderItemId { get; set; }
    public Guid MenuItemOptionId { get; set; }
    public string OptionGroupName { get; set; }  // snapshot
    public string OptionName { get; set; }       // snapshot
    public decimal AdditionalPrice { get; set; } // snapshot

    // Navigation properties
    public OrderItem OrderItem { get; set; }
    public MenuItemOption MenuItemOption { get; set; }
}
```

---

### 1.10 User

Sisteme giriş yapabilen kullanıcıları tanımlar. Restaurant personeli (Admin, Manager, Waiter, Kitchen)
ve sistem yöneticileri (SuperAdmin) bu entity üzerinden yönetilir.

```csharp
// Domain/Entities/User.cs
public class User : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }        // SuperAdmin için null olabilir
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public Restaurant? Restaurant { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

---

### 1.11 Role

Sistemdeki rolleri tanımlar. Roller sistem genelinde tanımlıdır, tenant'a özgü değildir.

```csharp
// Domain/Entities/Role.cs
public class Role : BaseEntity<Guid>
{
    public string Name { get; set; }         // SuperAdmin, Admin, Manager, Waiter, Kitchen
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; }
}

// Ara tablo (many-to-many)
public class UserRole : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public User User { get; set; }
    public Role Role { get; set; }
}
```

---

### 1.12 Printer

Restorandaki yazıcıları (mutfak yazıcısı, kasa yazıcısı vb.) tanımlar.

```csharp
// Domain/Entities/Printer.cs
public class Printer : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }             // ör. "Mutfak Yazıcısı", "Kasa"
    public string IpAddress { get; set; }        // ağ yazıcısı IP adresi
    public int Port { get; set; } = 9100;
    public PrinterType Type { get; set; }        // Kitchen, Cashier, Bar
    public bool IsActive { get; set; } = true;
    public bool IsOnline { get; set; } = false;  // anlık durum

    public Restaurant Restaurant { get; set; }
}

public enum PrinterType
{
    Kitchen = 1,   // mutfak yazıcısı
    Cashier = 2,   // kasa/hesap yazıcısı
    Bar = 3        // bar yazıcısı
}
```

---

### 1.13 Setting

Restoran bazlı konfigürasyon ayarlarını anahtar-değer çifti olarak saklar.

```csharp
// Domain/Entities/Setting.cs
public class Setting : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public string Key { get; set; }              // ör. "OrderNotificationSound", "AutoAcceptOrders"
    public string Value { get; set; }            // string olarak saklanır, uygulama tarafında parse edilir
    public string? Description { get; set; }
    public SettingType Type { get; set; }        // tip bilgisi parse için

    public Restaurant Restaurant { get; set; }
}

public enum SettingType
{
    String = 1,
    Boolean = 2,
    Integer = 3,
    Decimal = 4,
    Json = 5
}
```

---

### 1.14 FileRecord

FileAPI servisi tarafından yönetilen dosya meta bilgilerini tutar.
Entity adı `FileRecord` olarak kullanılır; C# `System.IO.File` ile çakışma önlenir.

```csharp
// Domain/Entities/FileRecord.cs
public class FileRecord : BaseEntity<Guid>
{
    public string OriginalName { get; set; }     // kullanıcının yüklediği orijinal dosya adı
    public string StoredName { get; set; }       // sistemin atadığı benzersiz dosya adı
    public string Path { get; set; }             // sunucudaki göreli yol
    public string Url { get; set; }              // dışarıya açık tam URL
    public string ContentType { get; set; }      // MIME type (image/jpeg, application/pdf vb.)
    public long SizeBytes { get; set; }
    public string? UploadedByUserId { get; set; }
    public Guid? TenantId { get; set; }          // hangi restorana ait (null ise genel)
}
```

---

### 1.15 Notification

Sistem içindeki bildirimleri (yeni sipariş, sipariş durumu değişikliği vb.) saklar.
SignalR ile gerçek zamanlı iletim için kullanılır; bu entity kalıcı log görevi görür.

```csharp
// Domain/Entities/Notification.cs
public class Notification : BaseEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; set; }
    public Guid? TargetUserId { get; set; }      // null ise restoran geneline yayın
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string? Payload { get; set; }         // JSON - ilgili entity bilgisi (OrderId vb.)
    public bool IsRead { get; set; } = false;

    public Restaurant Restaurant { get; set; }
    public User? TargetUser { get; set; }
}

public enum NotificationType
{
    NewOrder = 1,           // yeni sipariş geldi
    OrderStatusChanged = 2, // sipariş durumu değişti
    OrderCancelled = 3,     // sipariş iptal edildi
    LowStock = 4,           // düşük stok uyarısı
    SystemAlert = 5         // genel sistem uyarısı
}
```

---

## 2. OrderStatus Enum

Sipariş yaşam döngüsündeki tüm durumları tanımlar.

```csharp
// Domain/Enums/OrderStatus.cs
public enum OrderStatus
{
    Pending    = 1, // Beklemede    – müşteri siparişi verdi, restoran henüz kabul etmedi
    Accepted   = 2, // Kabul Edildi – restoran siparişi aldı, mutfağa iletildi
    Preparing  = 3, // Hazırlanıyor – mutfak hazırlamaya başladı
    Ready      = 4, // Hazır        – servis edilmeyi bekliyor
    Delivered  = 5, // Teslim Edildi – masaya götürüldü
    Paid       = 6, // Ödendi       – hesap kapandı
    Cancelled  = 7  // İptal Edildi  – herhangi bir aşamada iptal
}
```

**Geçerli Durum Geçişleri:**

| Mevcut Durum | İzin Verilen Sonraki Durum(lar) | Aktör |
|---|---|---|
| Pending | Accepted, Cancelled | Panel (Admin/Manager/Waiter) |
| Accepted | Preparing, Cancelled | Panel (Admin/Manager) |
| Preparing | Ready, Cancelled | Panel (Kitchen) |
| Ready | Delivered, Cancelled | Panel (Waiter) |
| Delivered | Paid | Panel (Admin/Manager/Waiter) |
| Paid | — | Terminal durum |
| Cancelled | — | Terminal durum |

**Kural:** Domain katmanında `Order.CanTransitionTo(OrderStatus newStatus)` metodu
bu geçiş tablosunu enforce eder. Geçersiz geçiş `OrderStatusTransitionException` fırlatır.

```csharp
// Domain/Entities/Order.cs – durum geçiş kuralı
public bool CanTransitionTo(OrderStatus newStatus)
{
    return (Status, newStatus) switch
    {
        (OrderStatus.Pending,   OrderStatus.Accepted)   => true,
        (OrderStatus.Pending,   OrderStatus.Cancelled)  => true,
        (OrderStatus.Accepted,  OrderStatus.Preparing)  => true,
        (OrderStatus.Accepted,  OrderStatus.Cancelled)  => true,
        (OrderStatus.Preparing, OrderStatus.Ready)      => true,
        (OrderStatus.Preparing, OrderStatus.Cancelled)  => true,
        (OrderStatus.Ready,     OrderStatus.Delivered)  => true,
        (OrderStatus.Ready,     OrderStatus.Cancelled)  => true,
        (OrderStatus.Delivered, OrderStatus.Paid)       => true,
        _ => false
    };
}
```

---

## 3. Business Kuralları

### 3.1 Sipariş Akışı

1. **Sipariş Oluşturma (QR Projesi → Backend):**
   - Müşteri QR kodu tarar → QR token doğrulanır → `TableId` ve `RestaurantId` resolve edilir.
   - Müşteri menüden ürün seçer, seçeneklerini belirler, notu ekler.
   - QR projesi `POST /v1/orders` çağrısı yapar (JWT gerektirmez, QR token yeterlidir).
   - Backend sipariş oluşturur, `OrderCode` üretir, `Status = Pending` olarak kaydeder.
   - `OrderCreatedEvent` publish edilir → Panel'e SignalR bildirimi gider.

2. **Sipariş Kabul (Panel → Backend):**
   - Panel kullanıcısı (Admin/Manager/Waiter) siparişi kabul eder.
   - `PATCH /v1/orders/{orderId}/status` → `{ "status": "Accepted" }`.
   - `OrderStatusChangedEvent` publish edilir → mutfak yazıcısına yazdırma komutu gider.

3. **Hazırlık ve Teslim:**
   - Mutfak `Preparing`, `Ready` geçişlerini yapar.
   - Garson `Delivered` geçişini yapar.
   - Her geçişte SignalR ile Panel ve QR projesine bildirim gider.

4. **Ödeme:**
   - `Paid` durumuna geçiş sadece `Delivered` sonrasında yapılabilir.
   - Ödeme gerçekleşince `OrderPaidEvent` publish edilir → raporlama worker'a iletilir.

### 3.2 QR Kodu ile Masa Bağlantısı

- Her masa için tek aktif `QrCode` kaydı olabilir.
- QR yeniden üretildiğinde eski `QrCode.IsActive = false` yapılır, yeni oluşturulur.
- QR token endpoint'i: `GET /v1/tables/qr/{token}` – token geçerliyse masa bilgilerini döner.
- QR endpoint'i JWT gerektirmez; sadece `IsActive = true` ve `Table.IsActive = true` kontrol edilir.
- Müşteri tarayıcısında oturum tutulmaz; her sipariş QR token + session ID ile ilişkilendirilir.

### 3.3 Multi-Tenant Kuralları

- Her request'te JWT token içindeki `RestaurantId` claim'i tenant context'i belirler.
- Tüm veritabanı sorgularına otomatik `WHERE RestaurantId = @currentRestaurantId` filtresi eklenir.
  Bu işlem `ITenantFilter` global query filter'ı üzerinden EF Core tarafından uygulanır.
- SuperAdmin tüm tenant'ları görebilir; `RestaurantId` claim'i null olan kullanıcılar
  tenant filter'dan muaf tutulur.
- Bir tenant'ın verisi başka bir tenant kullanıcısına hiçbir şekilde döndürülemez.
  Bu kural service katmanında da ayrıca doğrulanır (defense in depth).

### 3.4 Menü Yönetimi Kuralları

- Aktif siparişlerde referans edilen bir `MenuItem` silinemez (soft-delete zorunlu).
- `MenuItem.IsAvailable = false` yapılarak anlık olarak menüden kaldırılabilir.
- Fiyat değişikliği mevcut açık siparişleri etkilemez (snapshot mekanizması).
- Kategori silinmeden önce tüm alt öğeler farklı kategoriye taşınmalı veya silinmeli.

### 3.5 Yazıcı Kuralları

- Sipariş `Accepted` durumuna geçtiğinde ilgili restoranın `PrinterType.Kitchen` yazıcısına
  otomatik yazdırma yapılır.
- Sipariş `Paid` durumuna geçtiğinde `PrinterType.Cashier` yazıcısına hesap fişi gönderilir.
- Yazıcı offline ise hata loglanır, işlem başarısız sayılmaz (fire-and-forget).

---

## 4. Naming Conventions (.NET 8)

| Yapı | Kural | Örnek |
|---|---|---|
| Class | PascalCase | `MenuItemOption`, `OrderService` |
| Interface | `I` prefix + PascalCase | `IOrderService`, `IMenuRepository` |
| Method | PascalCase | `GetMenuItemsByCategory`, `UpdateOrderStatus` |
| Property | PascalCase | `TotalAmount`, `IsActive` |
| Private field | `_camelCase` | `_orderRepository`, `_mapper` |
| Parameter | camelCase | `restaurantId`, `menuItemId` |
| Local variable | camelCase | `orderItems`, `totalPrice` |
| Enum | PascalCase değerler | `OrderStatus.Preparing` |
| DTO (Request) | `{Action}{Entity}Request` | `CreateOrderRequest`, `UpdateMenuItemRequest` |
| DTO (Response) | `{Entity}Response` veya `{Entity}DetailResponse` | `OrderResponse`, `MenuItemDetailResponse` |
| Repository | `{Entity}Repository` | `OrderRepository`, `MenuItemRepository` |
| Service | `{Entity}Service` veya `{Domain}Service` | `OrderService`, `MenuService` |
| Controller | `{Entity}Controller` | `OrderController`, `TableController` |
| Event | `{Entity}{Action}Event` | `OrderCreatedEvent`, `OrderStatusChangedEvent` |
| Migration | EF Core otomatik timestamp prefix | `20240510120000_AddTableCapacityColumn` |

---

## 5. Service Interface Örnekleri

### 5.1 IMenuService

```csharp
// Application/Interfaces/IMenuService.cs
public interface IMenuService
{
    // Kategori işlemleri
    Task<IEnumerable<MenuCategoryResponse>> GetCategoriesAsync(Guid restaurantId);
    Task<MenuCategoryDetailResponse> GetCategoryByIdAsync(Guid categoryId);
    Task<MenuCategoryResponse> CreateCategoryAsync(CreateMenuCategoryRequest request);
    Task<MenuCategoryResponse> UpdateCategoryAsync(Guid categoryId, UpdateMenuCategoryRequest request);
    Task DeleteCategoryAsync(Guid categoryId);
    Task<MenuCategoryResponse> ReorderCategoryAsync(Guid categoryId, int newSortOrder);

    // Menü öğesi işlemleri
    Task<IEnumerable<MenuItemResponse>> GetItemsByCategoryAsync(Guid categoryId);
    Task<MenuItemDetailResponse> GetItemByIdAsync(Guid menuItemId);
    Task<MenuItemResponse> CreateItemAsync(CreateMenuItemRequest request);
    Task<MenuItemResponse> UpdateItemAsync(Guid menuItemId, UpdateMenuItemRequest request);
    Task DeleteItemAsync(Guid menuItemId);
    Task<MenuItemResponse> ToggleItemAvailabilityAsync(Guid menuItemId, bool isAvailable);

    // Seçenek işlemleri
    Task<IEnumerable<MenuItemOptionResponse>> GetOptionsByItemAsync(Guid menuItemId);
    Task<MenuItemOptionResponse> CreateOptionAsync(Guid menuItemId, CreateMenuItemOptionRequest request);
    Task<MenuItemOptionResponse> UpdateOptionAsync(Guid optionId, UpdateMenuItemOptionRequest request);
    Task DeleteOptionAsync(Guid optionId);
}
```

### 5.2 IOrderService

```csharp
// Application/Interfaces/IOrderService.cs
public interface IOrderService
{
    // Listeleme ve sorgulama
    Task<PaginatedResponse<OrderResponse>> GetOrdersAsync(GetOrdersQuery query);
    Task<OrderDetailResponse> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderResponse>> GetActiveOrdersByTableAsync(Guid tableId);

    // Sipariş oluşturma (QR projesi çağırır, JWT gerektirmez)
    Task<OrderDetailResponse> CreateOrderAsync(CreateOrderRequest request);

    // Durum yönetimi (Panel çağırır)
    Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request);
    Task<OrderResponse> CancelOrderAsync(Guid orderId, CancelOrderRequest request);

    // Kalem güncelleme (sadece Pending/Accepted durumunda)
    Task<OrderDetailResponse> AddOrderItemAsync(Guid orderId, AddOrderItemRequest request);
    Task RemoveOrderItemAsync(Guid orderId, Guid orderItemId);
}
```

### 5.3 ITableService

```csharp
// Application/Interfaces/ITableService.cs
public interface ITableService
{
    // Masa CRUD
    Task<IEnumerable<TableResponse>> GetTablesAsync(Guid restaurantId);
    Task<TableDetailResponse> GetTableByIdAsync(Guid tableId);
    Task<TableResponse> CreateTableAsync(CreateTableRequest request);
    Task<TableResponse> UpdateTableAsync(Guid tableId, UpdateTableRequest request);
    Task DeleteTableAsync(Guid tableId);

    // QR işlemleri
    Task<QrCodeResponse> GetQrCodeByTokenAsync(string token);
    Task<QrCodeResponse> GenerateQrCodeAsync(Guid tableId);
    Task DeactivateQrCodeAsync(Guid tableId);

    // Masa durumu (anlık sipariş bilgisi)
    Task<TableStatusResponse> GetTableStatusAsync(Guid tableId);
    Task<IEnumerable<TableStatusResponse>> GetAllTableStatusesAsync(Guid restaurantId);
}
```

---

## 6. DTO Örnekleri

### 6.1 Request DTO'ları

```csharp
// Application/DTOs/Order/CreateOrderRequest.cs
public class CreateOrderRequest
{
    public string QrToken { get; set; }                     // QR kodu token'ı (masa/restoran tespiti için)
    public List<CreateOrderItemRequest> Items { get; set; }
    public string? CustomerNote { get; set; }
}

public class CreateOrderItemRequest
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public List<Guid>? SelectedOptionIds { get; set; }      // seçilen MenuItemOption ID'leri
    public string? Note { get; set; }
}

// Application/DTOs/Order/UpdateOrderStatusRequest.cs
public class UpdateOrderStatusRequest
{
    public OrderStatus NewStatus { get; set; }
    public string? CancelReason { get; set; }               // Cancelled için zorunlu
}

// Application/DTOs/Menu/CreateMenuItemRequest.cs
public class CreateMenuItemRequest
{
    public Guid MenuCategoryId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? PreparationTimeMinutes { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### 6.2 Response DTO'ları

```csharp
// Application/DTOs/Order/OrderResponse.cs
public class OrderResponse
{
    public Guid Id { get; set; }
    public string OrderCode { get; set; }
    public Guid TableId { get; set; }
    public string TableName { get; set; }
    public OrderStatus Status { get; set; }
    public string StatusDisplay { get; set; }   // Türkçe durum adı
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Application/DTOs/Order/OrderDetailResponse.cs – detay endpoint'i için
public class OrderDetailResponse : OrderResponse
{
    public string? CustomerNote { get; set; }
    public List<OrderItemResponse> Items { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

// Application/DTOs/Menu/MenuItemResponse.cs
public class MenuItemResponse
{
    public Guid Id { get; set; }
    public Guid MenuCategoryId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int SortOrder { get; set; }
}
```

---

## 7. Endpoint Yapısı (Route Convention)

Tüm endpoint'ler `/v1/` prefix'i ile başlar. Kaynak adları çoğul ve kebab-case kullanılır.

```
# Menü Kategorileri
GET    /v1/menu-categories                             → tüm kategoriler (restaurantId JWT'den alınır)
GET    /v1/menu-categories/{categoryId}                → kategori detayı
POST   /v1/menu-categories                             → yeni kategori
PUT    /v1/menu-categories/{categoryId}                → kategori güncelle
DELETE /v1/menu-categories/{categoryId}                → kategori sil

# Menü Öğeleri
GET    /v1/menu-items?categoryId={id}                  → kategoriye göre öğeler
GET    /v1/menu-items/{menuItemId}                     → öğe detayı
POST   /v1/menu-items                                  → yeni öğe
PUT    /v1/menu-items/{menuItemId}                     → öğe güncelle
DELETE /v1/menu-items/{menuItemId}                     → öğe sil
PATCH  /v1/menu-items/{menuItemId}/availability        → stok durumu güncelle

# Menü Öğesi Seçenekleri
GET    /v1/menu-items/{menuItemId}/options             → öğenin seçenekleri
POST   /v1/menu-items/{menuItemId}/options             → seçenek ekle
PUT    /v1/menu-items/{menuItemId}/options/{optionId}  → seçenek güncelle
DELETE /v1/menu-items/{menuItemId}/options/{optionId}  → seçenek sil

# Masalar
GET    /v1/tables                                      → tüm masalar
GET    /v1/tables/{tableId}                            → masa detayı
POST   /v1/tables                                      → yeni masa
PUT    /v1/tables/{tableId}                            → masa güncelle
DELETE /v1/tables/{tableId}                            → masa sil
GET    /v1/tables/{tableId}/status                     → masa anlık durumu
GET    /v1/tables/statuses                             → tüm masaların durumu

# QR Kodlar
POST   /v1/tables/{tableId}/qr-code                   → QR kod üret/yenile
DELETE /v1/tables/{tableId}/qr-code                   → QR kodu deaktive et
GET    /v1/qr/{token}                                  → QR token doğrula (JWT gerekmez)

# Siparişler
GET    /v1/orders                                      → sipariş listesi (filtreli, sayfalı)
GET    /v1/orders/{orderId}                            → sipariş detayı
POST   /v1/orders                                      → yeni sipariş (JWT gerekmez, QR token yeterli)
PATCH  /v1/orders/{orderId}/status                     → sipariş durumu güncelle
DELETE /v1/orders/{orderId}                            → sipariş iptal (Cancelled'a çeker)
POST   /v1/orders/{orderId}/items                      → sipariş kalemi ekle
DELETE /v1/orders/{orderId}/items/{orderItemId}        → sipariş kalemi sil

# Auth
POST   /v1/auth/login                                  → giriş yap → JWT döner
POST   /v1/auth/refresh                                → token yenile
POST   /v1/auth/logout                                 → çıkış yap (refresh token iptal)

# Kullanıcılar
GET    /v1/users                                       → kullanıcı listesi
POST   /v1/users                                       → kullanıcı ekle
PUT    /v1/users/{userId}                              → kullanıcı güncelle
DELETE /v1/users/{userId}                              → kullanıcı sil

# Yazıcılar
GET    /v1/printers                                    → yazıcı listesi
POST   /v1/printers                                    → yazıcı ekle
PUT    /v1/printers/{printerId}                        → yazıcı güncelle
DELETE /v1/printers/{printerId}                        → yazıcı sil

# Ayarlar
GET    /v1/settings                                    → tüm ayarlar
PUT    /v1/settings/{key}                              → ayar güncelle

# Bildirimler
GET    /v1/notifications                               → bildirim listesi (sayfalı)
PATCH  /v1/notifications/{notificationId}/read         → okundu işaretle
PATCH  /v1/notifications/read-all                      → hepsini okundu işaretle
```

---

## 8. Bağımlı Proje Bağlantıları

### 8.1 RestaurantSystem Backend → RestaurantSystem Panel

Backend, Panel projesine şu hizmetleri sağlar:

- **Tüm yönetim endpoint'leri:** Menü, masa, sipariş, kullanıcı, yazıcı, ayar CRUD işlemleri.
- **JWT kimlik doğrulama:** Panel kullanıcıları `/v1/auth/login` ile token alır.
- **Sipariş yönetimi:** Panel, siparişleri listeler, durum günceller, iptal eder.
- **SignalR Hub (`/hubs/orders`):** Yeni sipariş geldiğinde ve durum değiştiğinde
  Panel'e anlık bildirim gönderilir. Panel bu hub'a bağlanarak canlı sipariş takibi yapar.
- **Dosya yükleme:** Panel, `FileAPI` üzerinden resim yükler; backend sadece URL'yi saklar.
- **Yazıcı yönetimi:** Panel yazıcıları tanımlar; backend yazıcılara komut gönderir.

**Panel'in kullandığı temel endpoint'ler:**
```
/v1/auth/login, /v1/orders (GET/PATCH), /v1/menu-* (CRUD),
/v1/tables (CRUD), /v1/printers (CRUD), /v1/settings, /v1/notifications
```

### 8.2 RestaurantSystem Backend → RestaurantSystem QR

Backend, QR projesine şu hizmetleri sağlar:

- **QR Token Doğrulama:** `GET /v1/qr/{token}` → masa adı, restoran bilgisi, menü erişim izni.
- **Public Menü Endpoint'leri:** JWT gerektirmez; QR token ile çağrılır.
  - `GET /v1/menu-categories?restaurantId={id}` – aktif kategoriler
  - `GET /v1/menu-items?categoryId={id}` – aktif ve stokta olan ürünler
- **Sipariş Oluşturma:** `POST /v1/orders` – QR token body'de gönderilir, JWT gerekmez.
- **Sipariş Durumu Takibi:** SignalR Hub veya `GET /v1/orders/{orderId}` ile müşteri
  siparişinin durumunu takip edebilir.

**QR projesinin kullandığı temel endpoint'ler:**
```
GET  /v1/qr/{token}          → masa/restoran resolve
GET  /v1/menu-categories     → menü kategorileri (public)
GET  /v1/menu-items          → menü öğeleri (public)
POST /v1/orders              → sipariş ver (public, QR token ile)
GET  /v1/orders/{orderId}    → sipariş durumu takip (public, orderId ile)
```

**Güvenlik Notu:** Public endpoint'lerde rate limiting uygulanır. QR token süresi
`Setting` tablosundan yönetilir (varsayılan: 24 saat).

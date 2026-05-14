# crm_backend — Kod Kuralları ve Pattern'ler

Bu dosya, PixdinnCrm projesinin gerçek kaynak kodundan çıkarılan somut kural ve pattern'leri içerir.
Her kural gerçek kod örneğiyle desteklenmiştir.

---

## Kural 1 — BaseEntity: Id + Deleted ile Başla

Her domain entity'si `BaseEntity`'den türer. `Id` long, `Deleted` bool (soft delete) varsayılan false.

```csharp
// FrameworkCore/Bases/BaseEntity/BaseEntity.cs
public class BaseEntity : IEntity
{
    public long Id { get; set; }
    public bool Deleted { get; set; } = false;
}

// Kullanım:
[Table("Place", Schema = "Crm")]
public class Place : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public virtual ICollection<PlacePicture> PlacePictures { get; set; }
}
```

**Kural:** Tüm tablolarda schema = "Crm" kullan. `[Table("TableName", Schema = "Crm")]` attribute zorunlu.

---

## Kural 2 — Çok Dilli İçerik: Content Entity Pattern

Her içerik alanı olan entity için ayrı Content entity'si oluştur. Ana entity basit kalır, dil bazlı içerik ayrı tabloda tutulur.

```csharp
// Ana entity — dil bağımsız veriler
[Table("Product", Schema = "Crm")]
public class Product : BaseEntity
{
    public decimal Price { get; set; }         // Dil bağımsız
    public long PlaceId { get; set; }
    public Place Place { get; set; }
    public virtual ICollection<ProductContent> ProductContents { get; set; }
    public virtual ICollection<ProductPicture> ProductPictures { get; set; }
}

// Content entity — dil bazlı içerik
[Table("ProductContent", Schema = "Crm")]
public class ProductContent : BaseEntity
{
    public string Name { get; set; }    // Türkçe veya İngilizce ad
    public string Detail { get; set; }  // Açıklama

    [ForeignKey("ProductContent_Product")]
    public long ProductId { get; set; }
    public Product Product { get; set; }

    [ForeignKey("ProductContent_Language")]
    public long LanguageId { get; set; }
    public ContentLanguage Language { get; set; }
}

// ContentLanguage — sistem genelinde dil tanımları
[Table("ContentLanguage", Schema = "Crm")]
public class ContentLanguage : BaseEntity
{
    public string Name { get; set; }     // "Türkçe"
    public string Prefix { get; set; }   // "tr"
    public int Order { get; set; }
    public bool IsDefault { get; set; }
}
```

**Kural:** Aynı pattern Category + CategoryContent, Place + PlaceLanguage için de geçerlidir. Ağır JOIN yerine default dil filtresi: `cc.Language.IsDefault`.

---

## Kural 3 — ForeignKey İsimlendirmesi

ForeignKey constraint isimleri `"ParentTable_ChildTable"` formatında explicit tanımlanır:

```csharp
[Table("PlaceLanguage", Schema = "Crm")]
public class PlaceLanguage : BaseEntity
{
    [ForeignKey("PlaceLanguage_Place")]          // Format: ChildTable_ParentTable
    public long PlaceId { get; set; }
    public Place Place { get; set; }

    [ForeignKey("PlaceLanguage_ContentLanguage")]
    public long ContentLanguageId { get; set; }
    public ContentLanguage ContentLanguage { get; set; }
}

[Table("MenuProduct", Schema = "Crm")]
public class MenuProduct : BaseEntity
{
    [ForeignKey("MenuProduct_MenuProduct")]       // Self-referencing
    public long? ParentCategoryId { get; set; }
    public MenuProduct? ParentCategory { get; set; }

    [ForeignKey("MenuProduct_Product")]
    public long? ProductId { get; set; }
    public Product? Product { get; set; }

    [ForeignKey("MenuProduct_Menu")]
    public long MenuId { get; set; }
    public Menu Menu { get; set; }
}
```

**Kural:** Nullable FK (`long?`) optional ilişkiyi, non-nullable (`long`) zorunlu ilişkiyi belirtir.

---

## Kural 4 — Cascade Delete Döngüsü: NoAction ile Çöz

Bir entity birden fazla FK içeriyorsa MSSQL cascade delete döngüsü hatası verir. Çözüm: OnModelCreating içinde `DeleteBehavior.NoAction`.

```csharp
// Persistence/DbContext/PixdinnCrmDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // MenuProduct 4 farklı tabloyla ilişkili → cascade döngüsü kırılmalı
    modelBuilder.Entity<MenuProduct>()
        .HasOne(e => e.Product)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<MenuProduct>()
        .HasOne(e => e.Category)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<MenuProduct>()
        .HasOne(e => e.ParentCategory)  // Self-referencing
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<MenuProduct>()
        .HasOne(e => e.Menu)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);
}
```

**Kural:** Self-referencing tree yapılarında `HasOne().WithMany().OnDelete(DeleteBehavior.NoAction)` zorunludur.

---

## Kural 5 — Repository: IQueryable Döndür, Materyalizasyon Service'te

Repository metodları `IQueryable<T>` döndürür. `ToList()`, `FirstOrDefault()`, `Count()` çağrısı her zaman servis katmanında yapılır. Bu JOIN kompozisyonuna olanak tanır.

```csharp
// Repository — sadece IQueryable döndürür
public class PlaceRepository : ConnectedRepository<Place>, IPlaceRepository
{
    public IQueryable<Place> GetPlaces(PlaceFilterDto placeFilterDto)
    {
        Expression<Func<Place, bool>> predicatePlace = s => s.Deleted == false;
        if (!String.IsNullOrEmpty(placeFilterDto.PlaceName))
            predicatePlace = predicatePlace.And(s => s.Name.Contains(placeFilterDto.PlaceName));
        if (!String.IsNullOrEmpty(placeFilterDto.PlaceEmail))
            predicatePlace = predicatePlace.And(s => s.Email.Contains(placeFilterDto.PlaceEmail));
        return _appDbContext.Place.Where(predicatePlace).AsNoTracking();
    }
}

// Service — IQueryable'ı JOIN'leyip materyalize eder
public async Task<Result<PlaceDto>> GetPlaceById(long id)
{
    // ToList/FirstOrDefault burada çağrılır, repository'de değil
    var placesEntity = await _placeRepository.GetPlaceById(id).FirstOrDefaultAsync();
    var placeDto = _mapper.Map<PlaceDto>(placesEntity);
    return new SuccessResult<PlaceDto>(placeDto);
}
```

---

## Kural 6 — Expression Predicate Chaining (And Extension)

Filtreler tip-güvenli `And` extension metodu ile zincir oluşturur. If bloğu içinde `predicate = predicate.And(...)` pattern'i kullanılır.

```csharp
// Repository filtreleme pattern'i
public IQueryable<ProductContent> GetProductsWithContent(ProductContentWithProductFilterDto dto)
{
    // Temel predicate: soft-delete kontrolü
    Expression<Func<ProductContent, bool>> predicate = pc => !pc.Deleted && pc.Product != null;

    // Opsiyonel filtreler zincir şeklinde eklenir
    if (dto.LanguageId != null)
        predicate = predicate.And(pc => pc.LanguageId == dto.LanguageId);
    else
        predicate = predicate.And(pc => pc.Language.IsDefault); // Dil verilmezse varsayılan dil

    if (dto.ProductName != null)
        predicate = predicate.And(pc => pc.Name.Contains(dto.ProductName));

    if (dto.PlaceId != null)
        predicate = predicate.And(pc => pc.Product.PlaceId == dto.PlaceId);

    if (dto.ProductId != null)
        predicate = predicate.And(pc => pc.Product.Id == dto.ProductId);

    return _appDbContext.ProductContent.Where(predicate)
        .Include(pc => pc.Product)
        .Include(pc => pc.Product.ProductPictures.Take(1)); // Sadece 1 resim
}
```

**Kural:** Her repository metoduna karşılık bir FilterDto sınıfı oluştur. Filtre parametreleri sınıfta toplanır.

---

## Kural 7 — Soft Delete: DeleteWithStateRange

Fiziksel silme yapılmaz. `Deleted = true` set edilerek güncelleme yapılır. Framework'te standart implementasyon:

```csharp
// FrameworkCore/Bases/BaseRepository/ConnectedRepository.cs
public override void DeleteWithStateRange(IEnumerable<TEntity> entities)
{
    entities.ToList().ForEach(item =>
    {
        item.Deleted = true;  // Fiziksel silme yok, soft delete
    });
    Update(entities);  // EF Core Update çağrısı
}

// Tekil soft delete:
public override void DeleteWithState(object id)
{
    var entity = _dbSet.Find(id);
    entity.Deleted = true;
    if (entity != null) Update(entity);
}

// Kullanım (service katmanında):
_productPictureRepository.DeleteWithStateRange(productPictureEntityDelete);
await _productRepository.SaveChangesAsync();
```

**Kural:** Tüm sorgularda `where Deleted == false` filtresi eklenir. Hiçbir sorgu silinen kayıtları getirmez.

---

## Kural 8 — Partial Update: UpdateWithProperties

Tüm entity kolonu güncellenmez. Sadece belirtilen property'ler güncellenir. EF Core `Entry.State = Unchanged + Property.IsModified = true` kullanılır.

```csharp
// Framework implementasyonu
public override void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties)
{
    _dbContext.Entry(entity).State = EntityState.Unchanged;
    foreach (var property in properties)
        _dbContext.Entry(entity).Property(property).IsModified = true;
    // SaveChanges çağrıldığında sadece belirtilen kolonlar UPDATE edilir
}

// Kullanım 1: Tek entity, birden fazla property
_productContentRepository.UpdateWithProperties(
    productContentEntityUpdate.ToArray(),
    new Expression<Func<ProductContent, object>>[] {
        s => s.Detail,
        s => s.Name
    });

// Kullanım 2: IsDefault gibi boolean toggle (PlaceLanguageService)
_placeLanguageRepository.UpdateWithProperties(defaultEntity,
    new Expression<Func<PlaceLanguage, object>>[] { s => s.IsDefault });
defaultEntity.IsDefault = false;
// UpdateWithProperties State'i Unchanged yapar, sonra IsDefault'u modified işaretler
// entity.IsDefault = false set etmek UPDATE sözcüğünde yeni değeri verir
await _placeLanguageRepository.SaveChangesAsync();
```

---

## Kural 9 — DataTable Pattern: Sayfalama + Sıralama

Tüm liste endpoint'leri `DataTableFilterModel<TFilter>` alır ve `DataTableResponseWrapper<TDto>` döner.

```csharp
// Generic DataTable request
public class DataTableFilterModel<T>
{
    public T data { get; set; }                // Domain filtresi (PlaceFilterDto, CategoryDataTableFilterPanelDto vb.)
    public string? orderDirective { get; set; } // "asc" / "desc"
    public string? orderProperty { get; set; }  // "Name" / "Price" / "Place.Name"
    public int? pageNumber { get; set; }         // 0-based
    public int? recordPerPage { get; set; }
}

// Generic DataTable response
public class DataTableResponseWrapper<T>
{
    public List<T> data { get; set; }
    public int recordCount { get; set; }         // Toplam kayıt sayısı (sayfalama için)
}

// Service'te standart kullanım pattern'i
public Result<DataTableResponseWrapper<PlaceDataTableDto>> GetPlacesForDataTable(
    DataTableFilterModel<PlaceFilterDto> placeFilterDto)
{
    var recordPerPage = placeFilterDto.recordPerPage.GetValueOrDefault();
    var pageNumber = placeFilterDto.pageNumber.GetValueOrDefault();
    int ofset = pageNumber * recordPerPage;

    var query = _placeRepository.GetPlaces(placeFilterDto.data);
    query = query.ApplySorting(placeFilterDto.orderProperty, placeFilterDto.orderDirective).AsNoTracking();

    int toplamKayit = query.Count();                              // COUNT sorgusu
    var list = query.Skip(ofset).Take(recordPerPage).ToList();    // SELECT sorgusu

    var dto = _mapper.Map<List<PlaceDataTableDto>>(list);
    return new SuccessResult<DataTableResponseWrapper<PlaceDataTableDto>>(
        new DataTableResponseWrapper<PlaceDataTableDto>(toplamKayit, dto));
}
```

---

## Kural 10 — Dinamik Sıralama: ApplySorting

String property adından Expression.Lambda oluşturarak `OrderBy` / `OrderByDescending` uygulayan generic metot:

```csharp
// ConnectedRepository.cs — ApplySorting implementasyonu
public override IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities)
{
    if (!string.IsNullOrEmpty(directive) && !string.IsNullOrEmpty(property))
    {
        // "Place.Name" gibi nested property desteği için Split('.') kullanılır
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var body = property.Split('.').Aggregate((Expression)parameter, Expression.Property);
        if (body.Type.IsValueType) body = Expression.Convert(body, typeof(object));
        var selector = Expression.Lambda<Func<TEntity, object>>(body, parameter);

        return directive == "asc"
            ? entities.OrderBy(selector)
            : entities.OrderByDescending(selector);
    }
    return entities.OrderBy(i => i.Id); // Varsayılan: Id'ye göre sırala
}

// Kullanım (service içinde, IQueryable üzerinde):
var sortedQuery = query.ApplySorting(dto.orderProperty, dto.orderDirective).AsNoTracking();
```

**Kural:** ApplySorting her zaman AsNoTracking'den önce çağrılır. AsNoTracking sayfalama öncesi yapılır.

---

## Kural 11 — Result<T> Pattern: Tüm Servisler Bu Döner

Servis metodları exception fırlatmaz. `SuccessResult<T>` veya `ErrorResult<T>` döner.

```csharp
// Başarı durumu
public async Task<Result<bool>> CreateNewPlace(PlaceInsertDto placeUpdateRequestDto)
{
    var placeToBeAdded = _mapper.Map<Place>(placeUpdateRequestDto);
    await _placeRepository.InsertAsync(placeToBeAdded);
    var effectedRowCount = await _placeRepository.SaveChangesAsync();

    if (effectedRowCount > 0)
        return new SuccessResult<bool>(true);   // ErrorCode=0, ResultType=Ok
    else
        return new ErrorResult<bool>(false, PlaceErrorEnum.PLACE_INSERT_ERROR);
        // ErrorCode=(int)enum, ErrorMesssage=[Description] attribute değeri
}

// Hata durumu — enum ile hata kodu ve mesaj
public async Task<Result<bool>> DeleteCategoryById(long id)
{
    _categoryRepository.Delete(id);
    var result = await _categoryRepository.SaveChangesAsync();
    if (result == 1) return new SuccessResult<bool>(true);
    return new ErrorResult<bool>(false, CategoryErrorEnum.CATEGORY_DELETE_ERROR);
}

// Controller'da this.FromResult() extension ile IActionResult'a dönüştürülür
[HttpPost]
public async Task<IActionResult> Add(PlaceInsertDto placeDto)
{
    var result = await _placeService.CreateNewPlace(placeDto);
    return this.FromResult(result); // Result<T> → IActionResult (200 OK veya 4xx/5xx)
}
```

---

## Kural 12 — ErrorEnum: [Description] ile Hata Mesajları

Hata kodları enum olarak tanımlanır. [Description] attribute ile Türkçe mesaj verilir. `ErrorResult<T>` mesajı otomatik çeker.

```csharp
// Validation hataları: 1-99
// Service hataları: 100+
public enum ProductErrorEnum
{
    [Description("Fiyat Sıfır Olamaz")]
    PRICE_ZERO_ERROR = 1,

    [Description("Place Id Null Olamaz")]
    NULL_PLACE_ID_ERROR = 2,

    [Description("Ürün Id Sıfırdan Büyük Olamaz")]
    WRONG_INSERT_ID = 4,

    [Description("Primary Key Üzerinden Product Bulunamadı")]
    PRODUCT_NOT_FOUND_ERROR = 99,

    [Description("Ürün Eklenemedi")]
    PRODUCT_INSERT_ERROR = 100,
}

// Menu iş kuralı hataları
public enum MenuErrorEnum
{
    [Description("Bu Kısımda Kategori Mevcut Olduğu İçin Ürün Ekleyemezsiniz")]
    MENU_PRODUCT_INSERT_ERROR = 100,

    [Description("Bu Kısımda Ürün Mevcut Olduğu İçin Kategori Ekleyemezsiniz")]
    MENU_CATEGORY_INSERT_ERROR = 101,

    [Description("Bir Hata Meydana Geldi")]
    MENU_PRODUCT_OR_CATEGORY_INSERT_ERROR = 102,
}

// ErrorResult: enum'dan kod ve mesaj alır
new ErrorResult<bool>(false, MenuErrorEnum.MENU_CATEGORY_INSERT_ERROR)
// → ErrorCode = 101, ErrorMesssage = "Bu Kısımda Ürün Mevcut Olduğu İçin Kategori Ekleyemezsiniz"
```

---

## Kural 13 — FluentValidation: Kayıt + ValidateFilter Attribute

Validator'lar `AbstractValidator<TDto>` sınıfından türer. `FluentApiRegister.cs` içinde kayıt yapılır. Controller'larda `[ValidateFilter]` attribute ile tetiklenir.

```csharp
// Validator tanımı
public class PlaceValidator : AbstractValidator<PlaceUpdateRequestDto>
{
    private readonly ICustomHttpUtilService _customHttpUtilService;

    public PlaceValidator(ICustomHttpUtilService customHttpUtilService)
    {
        _customHttpUtilService = customHttpUtilService;
        var httpContext = _customHttpUtilService.GetHttpContext();

        // Genel kural
        RuleFor(p => p.Name)
            .NotNull()
            .WithMessageNormal(PlaceErrorEnum.NAME_NULL_ERROR); // Enum'dan mesaj alır

        // Action adına göre koşullu kural
        if (httpContext.ActionName == "Update")
        {
            RuleFor(p => p.Id).NotNull().WithMessageNormal(PlaceErrorEnum.NAME_NULL_ERROR);
        }
    }
}

// Kayıt (Application/Extentions/FluentApiRegister.cs)
public static IServiceCollection AddFluentValidators(this IServiceCollection services)
{
    services.AddTransient<IValidator<ProductInsertDto>, ProductUpdateRequestDtoValidator>();
    services.AddTransient<IValidator<PlaceUpdateRequestDto>, PlaceValidator>();
    services.AddTransient<IValidator<PlaceLanguageDto>, PlaceLanguageValidator>();
    services.AddTransient<IValidator<CategoryAddDto>, CategoryAddValidator>();
    return services;
}

// Controller'da kullanım
[HttpPost]
[ValidateFilter]  // FluentValidation tetikler, hata varsa 400 döner
public async Task<IActionResult> Add(PlaceInsertDto placeDto) { ... }
```

---

## Kural 14 — Id==0 ile Insert/Update Ayırımı

Güncelleme DTO'larında `Id == 0` → yeni kayıt, `Id != 0` → güncelleme ayırımı yapılır. Hem insert hem update tek endpoint'e gönderilir.

```csharp
// CategoryService.UpdateCategory
public async Task<Result<bool>> UpdateCategory(CategoryUpdateRequestDto dto)
{
    // Id == 0 olanlar yeni eklenecek
    var categoryContentEntitiesToInsert = _mapper.Map<List<CategoryContent>>(
        dto.CategoryContents.Where(x => x.Id == 0).ToList());

    // Id != 0 olanlar güncellenecek
    var categoryContentEntitiesToUpdate = _mapper.Map<List<CategoryContent>>(
        dto.CategoryContents.Where(x => x.Id != 0)).ToList();

    // Silme listesi ayrı DTO'da gelir
    var categoryPicturesToDelete = _mapper.Map<List<CategoryPicture>>(dto.CategoryPicturesDeleted);

    _categoryContentRepository.Insert(categoryContentEntitiesToInsert);

    _categoryContentRepository.UpdateWithProperties(
        categoryContentEntitiesToUpdate.ToArray(),
        new Expression<Func<CategoryContent, object>>[] { cc => cc.Name }); // Partial update

    _categoryPictureRepository.Delete(categoryPicturesToDelete);

    await _categoryRepository.SaveChangesAsync();
    return new SuccessResult<bool>(true);
}
```

**Kural:** Update DTO'sunda hem `CategoryContents` (mevcut + yeni) hem `CategoryPicturesDeleted` (silinecekler) alanları bulunur.

---

## Kural 15 — Medya Dosyaları: GUID Referans (MediaGuiId)

Entity'ler dosyaya doğrudan FK ile bağlanmaz. FileAPI'den dönen GUID string entity'de `MediaGuiId` alanında saklanır. Bu servisleri birbirinden ayırır.

```csharp
// Entity'de sadece GUID string
[Table("ProductPicture", Schema = "Crm")]
public class ProductPicture : BaseEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public string MediaGuiId { get; set; }  // FileAPI'deki dosyaya GUID referans
}

// Controller'da dosya yükleme pattern'i
[HttpPost]
public async Task<IActionResult> AddProductPicture(IFormFile file)
{
    var fileApiUrl = _configuration.GetSection("FileApi:Url").Value;
    IRefitService refitFile = RestService.For<IRefitService>(fileApiUrl);

    UploadFileRequest uploadFileRequest = new UploadFileRequest();
    uploadFileRequest.GroupList = JsonConvert.DeserializeObject<List<GroupData>>(Request.Form["fileGroup"]);

    using (var memoryStream = new MemoryStream())
    {
        await file.CopyToAsync(memoryStream);
        uploadFileRequest.File = memoryStream.ToArray();
    }
    uploadFileRequest.FileName = file.FileName;

    var response = await refitFile.UploadPicture(uploadFileRequest);
    // response.FileKey = GUID string → ProductPicture.MediaGuiId olarak saklanır

    return this.FromResult(new SuccessResult<UploadFileResponse>(
        new UploadFileResponse { FileKey = response.FileKey }));
}
```

---

## Kural 16 — Menu İş Kuralı: Aynı Seviyede Tip Kontrolü

Bir menü seviyesine aynı anda hem CATEGORY hem PRODUCT eklenemez. Servis katmanında iş kuralı:

```csharp
public async Task<Result<bool>> CreateNewMenuProduct(MenuProductInsertDto menuProductInsert)
{
    if (menuProductInsert.Type == MenuProductTypeEnum.CATEGORY)
    {
        // Aynı menu + level kombinasyonunda PRODUCT var mı kontrol et
        var menuKontrol = _menuProductRepository.Where(x =>
            x.MenuId == menuProductInsert.MenuId &&
            x.Level == menuProductInsert.Level &&
            x.Type == MenuProductTypeEnum.PRODUCT &&
            !x.Deleted).FirstOrDefault();

        if (menuKontrol == null)
        {
            // Güvenli, kategori eklenebilir
            var entity = _mapper.Map<MenuProduct>(menuProductInsert);
            _menuProductRepository.Insert(entity, InsertStrategy.OnlytMain);
            await _menuProductRepository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }
        return new ErrorResult<bool>(false, MenuErrorEnum.MENU_CATEGORY_INSERT_ERROR);
    }
    else if (menuProductInsert.Type == MenuProductTypeEnum.PRODUCT)
    {
        // Aynı level'da CATEGORY var mı kontrol et
        var menuKontrol = _menuProductRepository.Where(x =>
            x.MenuId == menuProductInsert.MenuId &&
            x.Level == menuProductInsert.Level &&
            x.Type == MenuProductTypeEnum.CATEGORY &&
            !x.Deleted).FirstOrDefault();

        if (menuKontrol == null)
        {
            var entity = _mapper.Map<MenuProduct>(menuProductInsert);
            _menuProductRepository.Insert(entity, InsertStrategy.OnlytMain);
            await _menuProductRepository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }
        return new ErrorResult<bool>(false, MenuErrorEnum.MENU_PRODUCT_INSERT_ERROR);
    }
    return new ErrorResult<bool>(false, MenuErrorEnum.MENU_PRODUCT_OR_CATEGORY_INSERT_ERROR);
}
```

---

## Kural 17 — Varsayılan Dil: Default Language Toggle

PlaceLanguage'da `IsDefault` flag'i sadece bir kayıtta true olabilir. Değiştirmek için eski default'u false yapıp yeni default'u true yap:

```csharp
public async Task<Result<bool>> SetDefaultPlaceLanguage(PlaceSetDefaultLanguageDto dto)
{
    // 1. Mevcut default'u bul ve false yap
    var defaultEntity = _placeLanguageRepository
        .Where(x => !x.Deleted && x.PlaceId == dto.placeId && x.IsDefault)
        .FirstOrDefault();

    if (defaultEntity != null)
    {
        // Partial update: sadece IsDefault kolonu güncellenir
        _placeLanguageRepository.UpdateWithProperties(defaultEntity,
            new Expression<Func<PlaceLanguage, object>>[] { s => s.IsDefault });
        defaultEntity.IsDefault = false;  // C# nesnesindeki değeri de güncelle
    }

    // 2. Yeni default'u bul ve true yap
    var placeLanguageEntity = await _placeLanguageRepository.FindAsync(dto.id);
    if (placeLanguageEntity != null)
    {
        _placeLanguageRepository.UpdateWithProperties(placeLanguageEntity,
            new Expression<Func<PlaceLanguage, object>>[] { s => s.IsDefault });
        placeLanguageEntity.IsDefault = true;
    }

    await _placeLanguageRepository.SaveChangesAsync(); // Tek SaveChanges ile her iki güncelleme
    return new SuccessResult<bool>(true);
}
```

---

## Kural 18 — Idempotent Duplicate Kontrolü

Yeni kayıt eklerken önce unique constraint kontrolü yap. Zaten varsa başarı döndür (idempotent):

```csharp
public async Task<Result<bool>> createNewPlaceLanguage(PlaceLanguageDto placeLanguageDto)
{
    // Duplicate kontrolü: ContentLanguageId + PlaceId kombinasyonu unique olmalı
    var existing = _placeLanguageRepository
        .Where(x => !x.Deleted &&
                    x.ContentLanguageId == placeLanguageDto.ContentLanguageId &&
                    x.PlaceId == placeLanguageDto.PlaceId)
        .FirstOrDefault();

    if (existing == null)
    {
        // Yeni kayıt oluştur
        var entity = _mapper.Map<PlaceLanguage>(placeLanguageDto);
        await _placeLanguageRepository.InsertAsync(entity);
        var count = await _placeLanguageRepository.SaveChangesAsync();

        if (count > 0)
            return new SuccessResult<bool>(true);
        return new ErrorResult<bool>(false, PlaceLanguageErrorEnum.PLACE_LANGUAGE_INSERT_ERROR);
    }

    // Zaten var — hata değil, başarı döndür (idempotent)
    return new SuccessResult<bool>(true);
}
```

---

## Kural 19 — Çok Tablolu JOIN: Service'te LINQ

Karmaşık JOIN sorguları servis katmanında yapılır. Repository'ler basit filtrelenmiş IQueryable döndürür, JOIN servis'te birleştirilir:

```csharp
// MenuService.GetListMenuProduct — 7-way LEFT JOIN
public Result<List<MenuProductListDto>> GetListMenuProduct(MenuProductFilterDto menuProductFilter)
{
    // Her kaynak ayrı repository'den IQueryable olarak alınır
    var menuProducts = _menuProductRepository.ListMenuProduct(menuProductFilter);
    var productNameDefault = _productContentRepository.GetProductsWithContent(new ProductContentWithProductFilterDto());
    var categoryNameDefault = _categoryContentRepository.GetContentWithCategory(new CategoryWithContentFilterDto());
    var productMainPicture = _productPictureRepository.GetProductPictures(new ProductPictureFilterDto());
    var categoryMainPicture = _categoryPictureRepository.GetCategoryPictures(new CategoryPictureFilterDto());
    var menuList = _menuRepository.GetMenus();
    var placeList = _placeRepository.GetPlaces();

    // Servis'te LINQ JOIN ile tek sorguya birleştirilir
    var menuProductQuery = from menuProduct in menuProducts
                           join productContent in productNameDefault
                           on menuProduct.ProductId equals productContent.ProductId
                           into productContents
                           from productContent in productContents.DefaultIfEmpty()  // LEFT JOIN
                           join categoryContent in categoryNameDefault
                           on menuProduct.CategoryId equals categoryContent.CategoryId
                           into categoryContents
                           from categoryContent in categoryContents.DefaultIfEmpty()  // LEFT JOIN
                           // ... devam eden joinler ...
                           select new MenuProductListDto
                           {
                               // Hem product hem category için: null coalesce
                               Name = productContent.Name != null ? productContent.Name : categoryContent.Name,
                               Code = productPicture != null ? productPicture.MediaGuiId : categoryPicture.MediaGuiId,
                               // ...
                           };

    return new SuccessResult<List<MenuProductListDto>>(menuProductQuery.ToList());
}
```

---

## Kural 20 — Startup Konfigürasyonu: Bölgeli Yapı

Startup ConfigureServices içinde her bölüm `#region` ile ayrılır. Sıra önemlidir:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    #region Db configurasyonları ayarlanıyor
    // UnitOfWork + DbContext
    services.AddPixdinnDbService<PixdinnCrmDbContext>(dbcontextOptions);
    #endregion

    #region Api configurasyonları ayarlanıyor
    // API versiyonlama + Swagger + ProblemDetails + CORS + FluentValidation disable
    services.AddPixdinnApiService(Configuration, WebHostEnvironment, policy, apiUrl);
    #endregion

    #region Automapper configurasyonları ayarlanıyor
    // ProjectPrefix ile dll tarama → profile kayıt
    services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);
    #endregion

    #region Temel framework servisleri
    // Singleton: IAuthorizeService, ILoggerService, IUtilService, IFileApiWebRequest
    services.AddFrameworkServices();
    #endregion

    #region Validator
    services.AddFluentValidators(); // Tüm validator'lar buraya
    #endregion

    #region MassTransit-RabbitMQ configuration
    services.AddRabbitMqServices(new RabbitMqConfigModel()
    {
        HostAddress = GetAppSettingValue("EventBusSettings:HostAddress")
    });
    #endregion

    #region refit
    // FileAPI client
    services.AddRefitClient<IRefitService>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(GetAppSettingValue("FileApi:Url")));
    #endregion

    // JSON Enum → string serialize
    services.AddControllers().AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
}

// Autofac container: Assembly tarama ile otomatik kayıt
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.ConfigureRepositories(ApiOptions); // *Repository sınıfları
    builder.ConfigureServices(ApiOptions);     // *Service sınıfları
}
```

---

## Kural 21 — ConnectedRepository: DbContext Cast Kalıbı

Repository sınıfları `ConnectedRepository<TEntity>` türer. Spesifik DbContext'e erişim için private property ile cast yapılır:

```csharp
public class PlaceRepository : ConnectedRepository<Place>, IPlaceRepository
{
    // _dbContext (base'den gelen) → PixdinnCrmDbContext'e cast
    private PixdinnCrmDbContext _appDbContext { get => _dbContext as PixdinnCrmDbContext; }

    public PlaceRepository(IUnitOfWork dbContext) : base(dbContext) { }

    // _appDbContext üzerinden spesifik DbSet erişimi
    public IQueryable<Place> GetPlaces()
    {
        return _appDbContext.Place.AsNoTracking();
    }
}
```

**Kural:** Repository constructor parametresi her zaman `IUnitOfWork dbContext` dir. `base(dbContext)` çağrısı zorunludur.

---

## Kural 22 — Excel Bulk Import: RabbitMQ Async Pipeline

Büyük veri setleri için senkron işleme yerine asenkron pipeline:

```csharp
// 1. Ana API: Excel parse → RabbitMQ publish (fire-and-forget)
// ProductService.CreateNewProductExcel:
public async Task<Result<ExcelProductParseResponseModel>> CreateNewProductExcel(
    ExcelProductParseRequestModel excelProduct)
{
    var productDto = _mapper.Map<List<ProductInsertDto>>(excelProduct.ExcelProducts);
    var productEntity = _mapper.Map<List<Product>>(productDto);
    await _productRepository.InsertAsync(productEntity);  // Bulk insert
    await _productRepository.SaveChangesAsync();
    return new SuccessResult<ExcelProductParseResponseModel>(
        new ExcelProductParseResponseModel { State = true });
}

// 2. FileAPI: Excel dosyası yükle → parse → RabbitMQ publish
// FileController.ParseProductInsertExcel:
public IActionResult ParseProductInsertExcel(ParseProductInsertExcelRequestDto req)
{
    req.FilePath = _fileService.GetExcelFilePath(req.GuiId).Remove(0, 1);
    _publishEndpoint.Publish<ParseProductInsertExcelRequestDto>(req);  // Async
    return this.FromResult(new SuccessResult<bool>(true));  // Anında döner
}

// 3. FileAPI Consumer: Excel satırlarını işle
// PrefetchCount = 1 → tek seferde 1 mesaj işlenir (throttling)
cfg.ReceiveEndpoint(EventBusConstants.ExcelProductInsertQueue, c =>
{
    c.ConfigureConsumer<ExcelProductInsertConsumer>(ctx);
    c.PrefetchCount = 1;
});
```

---

## Kural 23 — RabbitMQ: Publisher-Only ve Consumer Ayrımı

Ana CRM API sadece publisher'dır. Consumer'lar ayrı servislerde:

```csharp
// Ana API Startup — Consumer YOK
services.AddMassTransit(config => {
    // config.AddConsumer<...>() YOK — sadece publisher
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqConfigModel.HostAddress);
        // ReceiveEndpoint YOK
    });
});

// FileAPI Startup — Consumer VAR
services.AddMassTransit(config => {
    config.AddConsumer<ExcelProductInsertConsumer>(); // Consumer kayıtlı
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.ExcelProductInsertQueue, c =>
        {
            c.ConfigureConsumer<ExcelProductInsertConsumer>(ctx);
            c.PrefetchCount = 1;
        });
    });
});

// Publish event (Controller veya Service'te):
await _publishEndpoint.Publish<TestEvent>(new TestEvent { Name = testName });
await _publishEndpoint.Publish<NotificationEvent>(notificationEvent);
await _publishEndpoint.Publish<ExceptionEvent>(new ExceptionEvent { ... });
```

---

## Kural 24 — Controller Route: Versiyonlu URL

Tüm controller'lar versiyonlu route kullanır. Swagger `OperationId` her metoda explicit verilir:

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]  // URL: /v1/Place/Add
public class PlaceController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "Add")]     // Swagger'da unique ID
    [ValidateFilter]                             // FluentValidation
    public async Task<IActionResult> Add(PlaceInsertDto placeDto) { ... }

    [HttpGet("{id:int}")]  // Route constraint: id integer olmalı
    [SwaggerOperation(OperationId = "GetCategoryById")]
    public async Task<ActionResult> GetCategoryById(long id) { ... }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(OperationId = "Delete")]
    public async Task<ActionResult> Delete(long id) { ... }
}
```

---

## Kural 25 — Update Form Verisi: ContentLanguage LEFT JOIN

Güncelleme formu açıldığında her dil için bir satır döner (kayıtlı içerik varsa dolu, yoksa boş):

```csharp
// ProductService — güncelleme formu için ContentLanguage + ProductContent LEFT JOIN
private IQueryable<PrdouctContentFormInputDto> GetContentLanguageWithProductContent(long productId)
{
    var contentLanguageList = _contentLanguageRepository.GetContentLanguages(new ContentLanguageFilterDto());
    var productContentList = _productContentRepository.GetContents(
        new ProductContentFilterDto() { ProductId = productId });

    // LEFT JOIN: tüm diller gelir, kayıtlı içerik yoksa null
    return from contentLanguage in contentLanguageList
           join productContent in productContentList
           on new { A = contentLanguage.Id, B = productId }
           equals new { A = productContent.LanguageId, B = productContent.ProductId }
           into productContents
           from productContent in productContents.DefaultIfEmpty()  // LEFT JOIN

           select new PrdouctContentFormInputDto
           {
               ContentLanguageId = contentLanguage.Id,
               ProductContentId = productContent.Id,      // 0 ise yeni eklenecek
               ProductId = productContent.ProductId,
               LanguageName = contentLanguage.Name,
               ProductDetail = productContent.Detail,      // null ise boş alan gösterilir
               ProductName = productContent.Name,
               Prefix = contentLanguage.Prefix,
               IsDefault = contentLanguage.IsDefault,
               Order = contentLanguage.Order
           };
}
// Sonuç: Sistemde 3 dil varsa 3 satır döner. ProductContentId=0 olanlar forumda yeni içerik anlamına gelir.
```

---

## Kural 26 — Enum Serializasyonu: JsonStringEnumConverter

API'ler enum değerlerini integer değil, string olarak serialize/deserialize eder:

```csharp
// Startup.cs
services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Sonuç: ActivePassive.ACTIVE JSON'da "ACTIVE" olarak gelir, 1 olarak değil
// MenuProduct.Type → "PRODUCT" veya "CATEGORY" (integer 1 veya 2 değil)
// Menu.State → "ACTIVE" veya "PASSIVE"
```

---

## Kural 27 — Kaçınılacaklar (Anti-Pattern'ler)

**1. Exception Yutma**
```csharp
// YAPMA:
try { ... }
catch (Exception ee) { } // Boş catch — exception yutulur
return new SuccessResult<bool>(true);

// YAP:
try { ... }
catch (Exception ex)
{
    _logger.LogError(ex, "UpdateProduct hatası");
    return new ErrorResult<bool>(false, ProductErrorEnum.PRODUCT_INSERT_ERROR);
}
```

**2. SQL Debug Console'a**
```csharp
// YAPMA (production'da):
optionsBuilder.LogTo(Console.WriteLine); // Tüm SQL'ler konsola

// YAP (development'ta):
if (env.IsDevelopment())
    optionsBuilder.LogTo(Console.WriteLine);
```

**3. Debug Satırları Kodda**
```csharp
// YAPMA:
var query = queryJoinedTables.ToQueryString(); // Debug satırı, kaldır
var cc = menuProductQuery.ToQueryString();     // Bu da kaldırılmalı

// YAP: Debug araçları veya loglama kullan
```

**4. Credential Kaynak Kodunda**
```json
// YAPMA:
"Password": "AliMuammer.!299"  // appsettings.json'da plain text

// YAP:
"Password": ""  // Boş bırak, environment variable'dan al
// Docker Compose override'da: env var olarak ver
```

**5. DI'ye Kayıtlı Servis Yerine Runtime Oluşturma**
```csharp
// YAPMA:
IRefitService refitFile = RestService.For<IRefitService>(fileApiUrl); // Runtime oluşturma

// YAP:
// Constructor'da inject et:
private readonly IRefitService _refitService;
public PlaceController(IRefitService refitService) { _refitService = refitService; }
// Kullanım:
var response = await _refitService.UploadPicture(uploadFileRequest);
```

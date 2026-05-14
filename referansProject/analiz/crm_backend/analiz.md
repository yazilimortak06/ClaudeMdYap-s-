# crm_backend — Kapsamlı Kod Analizi

## 1. Platform & Tech Stack

| Katman | Teknoloji |
|---|---|
| Runtime | .NET 5.0 |
| Web Framework | ASP.NET Core 5 Web API |
| ORM | Entity Framework Core 5 (Code-First, MSSQL) |
| DI Container | Autofac (Microsoft DI üzerine) |
| Mapper | AutoMapper |
| Validation | FluentValidation |
| Message Broker | MassTransit + RabbitMQ |
| HTTP Client | Refit (typed HTTP client) |
| API Versioning | Microsoft.AspNetCore.Mvc.Versioning (URL segment) |
| API Dokümantasyon | Swashbuckle (Swagger) |
| Problem Details | Hellang.Middleware.ProblemDetails |
| Loglama | MongoDB (ayrı log servisi) |
| File Storage | Ayrı FileAPI microservice |
| Token/Auth | Ayrı TokenService microservice (ASP.NET Identity) |
| Containerization | Docker Compose |

---

## 2. Proje Yapısı (tam klasör ağacı)

```
PixdinnCrmProjectBackEnd/
├── API/                                  ← Ana CRM Web API
│   ├── Controllers/
│   │   ├── CategoryController.cs
│   │   ├── ContentLanguageController.cs
│   │   ├── MenuController.cs
│   │   ├── PlaceController.cs
│   │   ├── PlaceLanguageController.cs
│   │   ├── ProductController.cs
│   │   └── TestEventPubllishController.cs
│   ├── Program.cs
│   ├── Startup.cs
│   └── appsettings.json
│
├── Application/                          ← İş mantığı katmanı
│   ├── Services/
│   │   ├── Category/CategoryService.cs
│   │   ├── ContentLanguage/ContentLanguageService.cs
│   │   ├── File/MediaFileService.cs
│   │   ├── Menu/MenuService.cs
│   │   ├── Places/PlaceLanguageService.cs
│   │   ├── Places/PlaceService.cs
│   │   └── Product/ProductService.cs
│   ├── ValidatorWrappers/
│   │   ├── CategoryValidators/CategoryAddValidator.cs
│   │   ├── PlaceLanguageValidators/PlaceLanguageValidator.cs
│   │   ├── PlaceValidators/PlaceValidator.cs
│   │   └── ProductValidators/ProductUpdateRequestDtoValidator.cs
│   └── Extentions/FluentApiRegister.cs
│
├── Domain/                               ← Domain modeli
│   ├── Entities/
│   │   ├── CategoryEntities/
│   │   │   ├── Category.cs
│   │   │   ├── CategoryContent.cs
│   │   │   └── CategoryPicture.cs
│   │   ├── FileEntities/MediaFile.cs
│   │   ├── LanguageEntities/ContentLanguage.cs
│   │   ├── MenuEntities/
│   │   │   ├── Menu.cs
│   │   │   └── MenuProduct.cs
│   │   ├── PlaceEntities/
│   │   │   ├── Place.cs
│   │   │   ├── PlaceLanguage.cs
│   │   │   └── PlacePicture.cs
│   │   └── ProductEntities/
│   │       ├── Product.cs
│   │       ├── ProductContent.cs
│   │       └── ProductPicture.cs
│   ├── Dto/                              ← 50+ DTO sınıfı
│   ├── Enums/
│   │   ├── DbEnums/ (CategoryEnum, GeneralEnum, MediaFileEnum, MenuProductEnum, ProductEnum)
│   │   └── ErrorEnums/ (CategoryErrorEnum, MediaFileErrorEnum, MenuErrorEnum, PlaceErrorEnum, PlaceLanguageErrorEnum, ProductErrorEnum)
│   └── Interfaces/
│       ├── Repositories/ (13 repository interface)
│       └── Services/ (7 service interface)
│
├── Persistence/                          ← Veri erişim katmanı
│   ├── DbContext/PixdinnCrmDbContext.cs
│   ├── Repositories/
│   │   ├── CategoryRepositories/ (CategoryRepository, CategoryContentRepository, CategoryPictureRepository)
│   │   ├── ContentLanguageRepositories/ContentLanguageRepository
│   │   ├── MediaFileRepositories/MediaFileRepository
│   │   ├── MenuProductRepositories/MenuProductRepository
│   │   ├── MenuRepositories/MenuRepository
│   │   ├── PlaceLanguageRepositories/PlaceLanguageRepository
│   │   ├── PlaceRepositories/ (PlaceRepository, PlacePictureRepository)
│   │   └── ProductRepositories/ (ProductRepository, ProductContentRepository, ProductPictureRepository)
│   └── Migrations/ (18 migration dosyası)
│
├── FrameworkCore/                        ← Özel framework (shared lib)
│   ├── Bases/
│   │   ├── BaseDbContext/ (IMultipleDbProjectBaseDbContext vb.)
│   │   ├── BaseDto/ (Result wrapper'lar, pagination)
│   │   ├── BaseEntity/BaseEntity.cs
│   │   ├── BaseRepository/ (BaseRepository.cs, ConnectedRepository.cs)
│   │   ├── BaseService/BaseService.cs
│   │   ├── BaseUnitOfWork/UnitOfWork.cs
│   │   └── StartupBase/ (BaseStartup, ApiServiceBaseCollectionExtensions)
│   └── FrameworkCore/
│       ├── FileApi/ (IFileApiWebRequest, FileApiWebRequest, models)
│       ├── RabbitMq/ (Events, Models, EventBusConstants)
│       ├── Refit/ (IRefitFileService)
│       ├── Repository/IRepository.cs
│       ├── WrapperCore/ (Result, SuccessResult, ErrorResult, DataTableResponseWrapper, DataTableFilterModel)
│       └── Utils/ (IUtilService, ICustomHttpUtilService, LinqExpression)
│
├── CommonDb/                             ← Paylaşımlı DbContext'ler
│   └── DbContext/
│       ├── RabbitMqDbContext.cs
│       └── WorkerServiceDbContext.cs
│
├── FileAPI/                              ← Dosya yükleme microservice
│   ├── Controllers/FileController.cs
│   ├── Controllers/FileViewController.cs
│   ├── Startup.cs (RabbitMQ ExcelProductInsertConsumer içerir)
│   └── appsettings.json
│
├── WorkerService/                        ← Background worker (BackgroundService)
│   ├── Worker.cs
│   └── Program.cs
│
├── ExternalServiceApi/TokenService/      ← JWT Auth microservice
│   ├── Core/TokenService.Domain/ (AppUser, UserRefreshToken, UserRole, IAuthenticationService)
│   ├── Core/TokenService.Application/ (AuthenticationService, UserService)
│   ├── Core/TokenService.Persistence/ (TokenServiceDbContext, UserRefreshTokenRepository)
│   └── Presentation/TokenService.API/ (AuthController, UserController, TestController)
│
├── PixdinCrm.MobilApi/                   ← Mobil API (ayrı proje)
├── PixdinnCrmLogService.API/             ← Log microservice (MongoDB consumer)
├── PixdinnCrmNotificationService.API/    ← Bildirim microservice
├── TockenService.API/                    ← Eski token service (duplicate, yeni ExternalServiceApi altında)
├── docker-compose.yml
└── docker-compose.override.yml
```

---

## 3. Domain — Entity'ler (TÜM, tam C# kodu)

### BaseEntity (tüm entity'lerin tabanı)

```csharp
namespace FrameworkCore.Bases.BaseEntities
{
    /// <summary>
    /// Bir Entity nin alması gereken en temel class
    /// </summary>
    public class BaseEntity : IEntity
    {
        public long Id { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
```

### Place.cs

```csharp
[Table("Place", Schema = "Crm")]
public class Place : BaseEntity
{
    public string Name { get; set; }
    public string AboutUs { get; set; }
    public string Instagram { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string LogoGuiId { get; set; }

    // Medya dosyaları artık GUID string ile referanslanıyor (eski FK yorumda)
    // public long PictureFileId { get; set; }
    // public MediaFile PictureFile { get; set; }

    public virtual ICollection<PlacePicture> PlacePictures { get; set; }
    public virtual ICollection<PlaceLanguage> PlaceLanguages { get; set; }
}
```

### PlaceLanguage.cs

```csharp
[Table("PlaceLanguage", Schema = "Crm")]
public class PlaceLanguage : BaseEntity
{
    public bool IsDefault { get; set; }

    [ForeignKey("PlaceLanguage_Place")]
    public long PlaceId { get; set; }
    public Place Place { get; set; }

    [ForeignKey("PlaceLanguage_ContentLanguage")]
    public long ContentLanguageId { get; set; }
    public ContentLanguage ContentLanguage { get; set; }
}
```

### PlacePicture.cs

```csharp
[Table("PlacePicture", Schema = "Crm")]
public class PlacePicture : BaseEntity
{
    [ForeignKey("PlacePicture_Place")]
    public long PlaceId { get; set; }
    public Place Place { get; set; }

    public string MediaGuiId { get; set; }  // FileAPI'deki dosyaya GUID referans
}
```

### Category.cs

```csharp
[Table("Category", Schema = "Crm")]
public class Category : BaseEntity
{
    public virtual ICollection<CategoryContent> CategoryContents { get; set; }
    public virtual ICollection<CategoryPicture> CategoryPictures { get; set; }

    public long PlaceId { get; set; }
    public Place Place { get; set; }
}
```

### CategoryContent.cs

```csharp
[Table("CategoryContent", Schema = "Crm")]
public class CategoryContent : BaseEntity
{
    public string Name { get; set; }

    [ForeignKey("CategoryContent_Category")]
    public long CategoryId { get; set; }
    public Category Category { get; set; }

    [ForeignKey("CategoryContent_Language")]
    public long LanguageId { get; set; }
    public ContentLanguage Language { get; set; }
}
```

### CategoryPicture.cs

```csharp
[Table("CategoryPicture", Schema = "Crm")]
public class CategoryPicture : BaseEntity
{
    [ForeignKey("CategoryPicture_Category")]
    public long CategoryId { get; set; }
    public Category Category { get; set; }

    public string MediaGuiId { get; set; }  // FileAPI'deki dosyaya GUID referans
}
```

### Product.cs

```csharp
[Table("Product", Schema = "Crm")]
public class Product : BaseEntity
{
    public decimal Price { get; set; }

    public virtual ICollection<ProductContent> ProductContents { get; set; }
    public virtual ICollection<ProductPicture> ProductPictures { get; set; }

    [ForeignKey("Product_Place")]
    public long PlaceId { get; set; }
    public Place Place { get; set; }
}
```

### ProductContent.cs

```csharp
[Table("ProductContent", Schema = "Crm")]
public class ProductContent : BaseEntity
{
    public string Name { get; set; }
    public string Detail { get; set; }

    [ForeignKey("ProductContent_Product")]
    public long ProductId { get; set; }
    public Product Product { get; set; }

    [ForeignKey("ProductContent_Language")]
    public long LanguageId { get; set; }
    public ContentLanguage Language { get; set; }
}
```

### ProductPicture.cs

```csharp
[Table("ProductPicture", Schema = "Crm")]
public class ProductPicture : BaseEntity
{
    [ForeignKey("ProductPicture_Product")]
    public long ProductId { get; set; }
    public Product Product { get; set; }

    public string MediaGuiId { get; set; }  // FileAPI'deki dosyaya GUID referans
}
```

### Menu.cs

```csharp
[Table("Menu", Schema = "Crm")]
public class Menu : BaseEntity
{
    public string MenuName { get; set; }
    public ActivePassive State { get; set; }  // ACTIVE=1, PASSIVE=2

    [ForeignKey("Menu_Place")]
    public long PlaceId { get; set; }
    public Place Place { get; set; }
}
```

### MenuProduct.cs

```csharp
[Table("MenuProduct", Schema = "Crm")]
public class MenuProduct : BaseEntity
{
    public int Level { get; set; }

#nullable enable
    [ForeignKey("MenuProduct_MenuProduct")]
    public long? ParentCategoryId { get; set; }
    public MenuProduct? ParentCategory { get; set; }  // Self-referencing tree

    [ForeignKey("MenuProduct_Product")]
    public long? ProductId { get; set; }
    public Product? Product { get; set; }

    [ForeignKey("MenuProduct_Category")]
    public long? CategoryId { get; set; }
    public Category? Category { get; set; }
#nullable disable

    public int Order { get; set; }

    [ForeignKey("MenuProduct_Menu")]
    public long MenuId { get; set; }
    public Menu Menu { get; set; }

    public MenuProductTypeEnum Type { get; set; }  // PRODUCT=1, CATEGORY=2
}
```

### ContentLanguage.cs

```csharp
[Table("ContentLanguage", Schema = "Crm")]
public class ContentLanguage : BaseEntity
{
    public string Name { get; set; }    // "Türkçe", "English"
    public string Prefix { get; set; }  // "tr", "en"
    public int Order { get; set; }
    public bool IsDefault { get; set; }
}
```

### MediaFile.cs

```csharp
[Table("MediaFile", Schema = "Crm")]
public class MediaFile : BaseEntity
{
    public string Code { get; set; }
    public FileTypeEnum Type { get; set; }             // SMALL, MEDIUM, BIG
    public FileMediaTypeEnum MediaType { get; set; }   // PICTURE, VIDEO, PDF, GIF, EXCEL, WORLD
    public long ContentSize { get; set; }
    public string Name { get; set; }
    public string ManuelName { get; set; }
    public long Size { get; set; }
    public long SizeX { get; set; }
    public long SizeY { get; set; }
    public int CompressedRate { get; set; }
    public FileExtentionEnum FileExtentionEnum { get; set; }  // PNG, JPEG, PDF
}
// NOT: DbContext'te DbSet<MediaFile> yorum satırında — aktif kullanılmıyor
```

---

## 4. Domain — Interface'ler (TÜM, tam kod)

### IRepository<TEntity> (FrameworkCore)

```csharp
public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class, IEntity
{
    // CRUD
    TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll);
    void Insert(params TEntity[] entities);
    void Insert(IEnumerable<TEntity> entities);
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task InsertAsync(params TEntity[] entities);
    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity, UpdateStrategy updateStrategy = UpdateStrategy.UpdateAll);
    void Update(params TEntity[] entities);
    void Update(IEnumerable<TEntity> entities);
    void Delete(object id);
    void Delete(TEntity entity, DeleteStrategy deleteStrategy = DeleteStrategy.MainIfRequiredAddChilds);
    void Delete(params TEntity[] entities);
    void Delete(IEnumerable<TEntity> entities);
    void DeleteWithState(object id);               // soft delete by id
    void DeleteWithStateRange(IEnumerable<TEntity> entities);  // soft delete range
    void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties);
    void UpdateWithProperties(TEntity[] entity, Expression<Func<TEntity, object>>[] properties);
    void UpdateWithPropertiesForProperty(TEntity entity, Expression<Func<TEntity, object>>[] properties);
    void UpdateWithPropertiesForProperty(TEntity[] entities, Expression<Func<TEntity, object>>[] properties);

    // Query
    IQueryable<TEntity> GetAll();
    IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault);
    Task<IList<TEntity>> GetAllAsync();
    TEntity Find(params object[] keyValues);
    ValueTask<TEntity> FindAsync(params object[] keyValues);
    TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null, ...);
    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, ...);
    bool Exists(Expression<Func<TEntity, bool>> selector = null);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null);

    // Aggregations
    int Count(Expression<Func<TEntity, bool>> predicate = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
    long LongCount(Expression<Func<TEntity, bool>> predicate = null);
    decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

    // UnitOfWork
    int SaveChanges();
    Task<int> SaveChangesAsync();

    // Sorting
    IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities);
}
```

### IPlaceRepository

```csharp
public interface IPlaceRepository : IRepository<Place>
{
    IQueryable<Place> GetPlaces(PlaceFilterDto placeFilterDto);
    IQueryable<Place> GetPlaces();
    IQueryable<Place> GetPlaceById(long placeId);
}
```

### IPlaceService

```csharp
public interface IPlaceService
{
    Task<Result<bool>> CreateNewPlace(PlaceInsertDto placeDto);
    Task<Result<bool>> UpdatePlace(PlaceUpdateRequestDto placeDto);
    Result<DataTableResponseWrapper<PlaceDataTableDto>> GetPlacesForDataTable(DataTableFilterModel<PlaceFilterDto> placeFilterDto);
    Task<Result<PlaceDto>> GetPlaceById(long id);
    Task<Result<PlaceUpdateRequestDto>> GetPlaceByIdForUpdateRequest(long id);
}
```

### ICategoryService

```csharp
public interface ICategoryService
{
    Task<Result<bool>> AddNewCategory(CategoryAddDto dto);
    Result<DataTableResponseWrapper<CategoryDataTableDto>> GetCategoryByDatatableDto(DataTableFilterModel<CategoryDataTableFilterPanelDto> dto);
    Task<Result<bool>> DeleteCategoryById(long id);
    Task<Result<CategoryUpdateRequestDto>> GetCategoryById(long Id);
    Task<Result<bool>> UpdateCategory(CategoryUpdateRequestDto dto);
    Task<List<CategoryDataTableDto>> GetListCategoryForMenu(long placeId);
}
```

### IProductService

```csharp
public interface IProductService
{
    Task<Result<bool>> CreateNewProduct(ProductInsertDto productDto);
    Task<Result<ExcelProductParseResponseModel>> CreateNewProductExcel(ExcelProductParseRequestModel excelProduct);
    Task<Result<bool>> UpdateProduct(ProductUpdateRequestDto productDto);
    Task<Result<ProductUpdateRequestDto>> GetProductForUpdate(long id);
    Result<DataTableResponseWrapper<ProductDataTableDto>> GetProductDataTablePanel(DataTableFilterModel<ProductDataTableFilterPanelDto> dataTableFilterModel);
    List<ProductDataTableDto> GetListProductForMenu(long placeId);
}
```

### IMenuService

```csharp
public interface IMenuService
{
    Task<Result<MenuUpdateFormDto>> GetMenuUpdateFormData(long menuId);
    Task<Result<bool>> CreateNewMenuProduct(MenuProductInsertDto menuProductInsertDto);
    Task<Result<MenuDto>> CreateNewMenu(MenuDto menuDto);
    Result<List<MenuProductListDto>> GetListMenuProduct(MenuProductFilterDto menuProductFilter);
    Result<DataTableResponseWrapper<MenuDataTableDto>> GetMenuForDataTablePanel(DataTableFilterModel<MenuDataTableFilterDto> dataTableFilterModel);
    Task<List<MenuProductListMobilDto>> GetListMenuProductForMobil(MenuProductListMobilFilterDto menuProductListMobilFilter);
}
```

### IContentLanguageService

```csharp
public interface IContentLanguageService
{
    Result<List<ContentLanguageDto>> GetAllContentLanguageDto();
    Result<List<ContentLanguageDto>> GetContenLanguageNotInPlace(long placeId);
}
```

### IPlaceLanguageService

```csharp
public interface IPlaceLanguageService
{
    Result<List<PlaceLanguageDto>> GetPlaceLanguageByPlaceId(long id);
    Result<List<PlaceLanguageDto>> GetPlaceLanguageByPlaceIdWithContentLanguage(long id);
    Task<Result<bool>> SetDefaultPlaceLanguage(PlaceSetDefaultLanguageDto dto);
    Task<Result<bool>> createNewPlaceLanguage(PlaceLanguageDto placeLanguageDto);
    Task<Result<bool>> deletePlaceLanguage(List<PlaceLanguageDto> placeLanguageDto);
}
```

### IMediaFileService

```csharp
public interface IMediaFileService
{
    Task<Result<CreateMediaFileResponseDto>> CreateNewMediaFile(MediaFileDto mediaFileDto);
}
```

---

## 5. Domain — Enum'lar + DTO'lar (TÜM, tam kod)

### DbEnums

```csharp
// GeneralEnum.cs
public enum ActivePassive
{
    ACTIVE = 1,
    PASSIVE = 2
}

// CategoryEnum.cs
public enum CategoryPictureType
{
    PREVIEW = 1,
    DETAILED = 2,
    EXTRA = 3
}

// MediaFileEnum.cs
public enum FileMediaTypeEnum
{
    PICTURE = 1,
    VIDEO = 2,
    PDF = 3,
    GIF = 4,
    EXCEL = 5,
    WORLD = 6
}

public enum FileTypeEnum
{
    SMALL = 1,
    MEDIUM = 2,
    BIG = 3
}

public enum FileExtentionEnum
{
    PNG = 1,
    JPEG = 2,
    PDF = 3
}

// MenuProductEnum.cs
public enum MenuProductTypeEnum
{
    PRODUCT = 1,
    CATEGORY = 2
}

// ProductEnum.cs
public enum ProductPictureType
{
    PREVIEW = 1,
    DETAILED = 2
}
```

### ErrorEnums

```csharp
// PlaceErrorEnum.cs
public enum PlaceErrorEnum
{
    [Description("Place Name Null Bırakılamaz!")]
    NAME_NULL_ERROR = 1,
    [Description("Bir Adet Varsayılan Dil Seçebilirsiniz")]
    LANGUAGE_IS_DEFAULT_ERROR = 2,
    [Description("Mekan Eklenemedi")]
    PLACE_INSERT_ERROR = 100,
}

// CategoryErrorEnum.cs
public enum CategoryErrorEnum
{
    [Description("Kategori Bulunamadı")]
    CATEGORY_ID_ERROR = 1,
    [Description("Kategori Eklenemedi")]
    CATEGORY_INSERT_ERROR = 100,
    [Description("Kategori Silinemedi")]
    CATEGORY_DELETE_ERROR = 101,
}

// ProductErrorEnum.cs
public enum ProductErrorEnum
{
    [Description("Fiyat Sıfır Olamaz")]
    PRICE_ZERO_ERROR = 1,
    [Description("Place Id Null Olamaz")]
    NULL_PLACE_ID_ERROR = 2,
    [Description("Product Id Null Olamaz")]
    WRONG_PRIMARY_KEY = 3,
    [Description("Ürün Id Sıfırdan Büyük Olamaz")]
    WRONG_INSERT_ID = 4,
    [Description("Primary Key Üzerinden Product Bulunamadı")]
    PRODUCT_NOT_FOUND_ERROR = 99,
    [Description("Ürün Eklenemedi")]
    PRODUCT_INSERT_ERROR = 100,
}

// MenuErrorEnum.cs
public enum MenuErrorEnum
{
    [Description("Bu Kısımda Kategori Mevcut Olduğu İçin Ürün Ekleyemezsiniz")]
    MENU_PRODUCT_INSERT_ERROR = 100,
    [Description("Bu Kısımda Ürün Mevcut Olduğu İçin Kategori Ekleyemezsiniz")]
    MENU_CATEGORY_INSERT_ERROR = 101,
    [Description("Bir Hata Meydana Geldi")]
    MENU_PRODUCT_OR_CATEGORY_INSERT_ERROR = 102,
}

// PlaceLanguageErrorEnum.cs — PLACE_LANGUAGE_INSERT_ERROR, PLACE_LANGUAGE_REMOVE_ERROR
// MediaFileErrorEnum.cs — MEDIA_FILE_INSERT_ERROR
```

### Framework Wrapper DTO'lar

```csharp
// Result<T> — tüm service metodları bunu döner
public class Result<T>
{
    public virtual ResultType ResultType { get; set; }
    public virtual int ErrorCode { get; set; }
    public virtual string ErrorMesssage { get; set; }
    public virtual T Data { get; set; }
}

public class SuccessResult<T> : Result<T>
{
    public SuccessResult(T data) : base()
    {
        ErrorCode = 0;
        ErrorMesssage = "";
    }
    public override ResultType ResultType => ResultType.Ok;
    public override T Data => _data;
}

public class ErrorResult<T> : Result<T>
{
    public ErrorResult(T? data, Enum error) : base()
    {
        ErrorCode = (int)((object)error);
        ErrorMesssage = error.ToDescriptionString(); // [Description] attribute değeri
    }
    public override ResultType ResultType => ResultType.Error;
}

// DataTable filtreleme ve sayfalama
public class DataTableFilterModel<T>
{
    public T data { get; set; }
    public string? orderDirective { get; set; }   // "asc" / "desc"
    public string? orderProperty { get; set; }    // "Name", "Price.Amount" gibi
    public int? pageNumber { get; set; }
    public int? recordPerPage { get; set; }
}

public class DataTableResponseWrapper<T>
{
    public List<T> data { get; set; }
    public int recordCount { get; set; }
    public DataTableResponseWrapper(int recordCount, List<T> data) { ... }
}
```

### Domain DTO'lar (önemli olanlar)

```csharp
// Place
public class PlaceInsertDto   { string Name; string AboutUs; string Instagram; string Phone; string Email; }
public class PlaceUpdateRequestDto { long Id; string Name; ...; List<PlacePictureFormInputDto> PlacePictureFormInput; }
public class PlaceDataTableDto { long Id; string Name; string Email; ... }
public class PlaceFilterDto    { string PlaceName; string PlaceEmail; }
public class PlacePictureFormInputDto { long Id; string code; long PlaceId; }
public class PlaceLanguageDto  { long Id; long PlaceId; long ContentLanguageId; bool IsDefault; }
public class PlaceSetDefaultLanguageDto { long id; long placeId; }

// Category
public class CategoryAddDto    { long PlaceId; List<CategoryContentDto> CategoryContents; List<CategoryPictureDto> CategoryPictures; }
public class CategoryUpdateRequestDto { long Id; List<CategoryUpdateFormInputDto> CategoryUpdateFormInputDtos; List<CategoryPictureUpdateFormDto> CategoryPictureUpdateFormDtos; List<CategoryContentUpdateDto> CategoryContents; List<CategoryPictureDto> CategoryPictures; List<CategoryPictureDto> CategoryPicturesDeleted; }
public class CategoryDataTableDto { long Id; string CategoryName; string PlaceName; long PlaceId; long LanguageId; }
public class CategoryUpdateFormInputDto { long LanguageId; long CategoryContentId; long CategoryId; string LanguageName; string CategoryContentName; string Prefix; }

// Product
public class ProductInsertDto  { long Id; decimal Price; long PlaceId; List<ProductContentDto> ProductContents; List<ProductPictureDto> ProductPictures; }
public class ProductUpdateRequestDto { ProductDto ProductUpdate; List<ProductContentDto> ProductContents; List<PrdouctContentFormInputDto> ProductContentFormInput; List<ProductPictureFormInputDto> ProductMainPictureFormInput; List<ProductPictureDto> ProductPictures; List<ProductPictureDto> ProductPicturesDeleted; }
public class ProductDataTableDto { long Id; long ProductContentId; string Name; decimal Price; string PictureCode; string PlaceName; long PlaceId; }

// Menu
public class MenuDto           { long Id; string MenuName; ActivePassive State; long PlaceId; }
public class MenuProductInsertDto { long MenuId; long? ProductId; long? CategoryId; long? ParentCategoryId; int Level; int Order; MenuProductTypeEnum Type; }
public class MenuProductListDto { long Id; string Name; long? ParentCategoryId; long? CategoryId; long? ProductId; int Level; long MenuId; int Order; MenuProductTypeEnum Type; string Code; string PlaceName; }
public class MenuProductListMobilDto { string Name; long ParentCategoryId; long CategoryId; long ProductId; int Order; MenuProductTypeEnum Type; string PictureCode; decimal Price; }
public class MenuDataTableDto  { long Id; string MenuName; ActivePassive State; string StateText; string PlaceName; long PlaceId; }
```

---

## 6. Application — Service'ler (TÜM, tam kod)

### PlaceService.cs

```csharp
public class PlaceService : BaseService, IPlaceService
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IPlaceLanguageRepository _placeLanguageRepository;
    private readonly IContentLanguageRepository _contentLanguageRepository;
    private readonly IPlacePictureRepository _placePictureRepository;

    public PlaceService(IMapper mapper, ILoggerService logger,
        IAuthorizeService authorizeService, IAuthonticationService authonticationService,
        IPlaceRepository placeRepository, IPlaceLanguageRepository placeLanguageRepository,
        IContentLanguageRepository contentLanguageRepository,
        IPlacePictureRepository placePictureRepository)
        : base(mapper, logger, authorizeService, authonticationService)
    {
        _placeRepository = placeRepository;
        _placeLanguageRepository = placeLanguageRepository;
        _contentLanguageRepository = contentLanguageRepository;
        _placePictureRepository = placePictureRepository;
    }

    public async Task<Result<bool>> CreateNewPlace(PlaceInsertDto placeUpdateRequestDto)
    {
        var placeToBeAdded = _mapper.Map<Place>(placeUpdateRequestDto);
        var insertedPlace = await _placeRepository.InsertAsync(placeToBeAdded);
        var effectedRowCount = await _placeRepository.SaveChangesAsync();
        if (effectedRowCount > 0)
            return new SuccessResult<bool>(true);
        else
            return new ErrorResult<bool>(false, PlaceErrorEnum.PLACE_INSERT_ERROR);
    }

    public async Task<Result<bool>> UpdatePlace(PlaceUpdateRequestDto placeUpdateRequestDto)
    {
        var placeEntity = _mapper.Map<Place>(placeUpdateRequestDto);
        _placeRepository.Update(placeEntity, UpdateStrategy.OnlyMain);
        await _placeRepository.SaveChangesAsync();
        return new SuccessResult<bool>(true);
    }

    public Result<DataTableResponseWrapper<PlaceDataTableDto>> GetPlacesForDataTable(
        DataTableFilterModel<PlaceFilterDto> placeFilterDto)
    {
        var recordPerPage = placeFilterDto.recordPerPage.GetValueOrDefault();
        var pageNumber = placeFilterDto.pageNumber.GetValueOrDefault();
        int ofset = pageNumber * recordPerPage;

        var placesEntityFilter = _placeRepository.GetPlaces(placeFilterDto.data);
        var productResult = placesEntityFilter
            .ApplySorting(placeFilterDto.orderProperty, placeFilterDto.orderDirective)
            .AsNoTracking();

        int toplamKayit = placesEntityFilter.Count(); // 1. DB sorgusu

        var placesEntity = placesEntityFilter.Skip(ofset).Take(recordPerPage).ToList(); // 2. DB sorgusu

        if (placesEntity != null)
        {
            var placesDto = _mapper.Map<List<PlaceDataTableDto>>(placesEntity);
            var result = new DataTableResponseWrapper<PlaceDataTableDto>(toplamKayit, placesDto);
            return new SuccessResult<DataTableResponseWrapper<PlaceDataTableDto>>(result);
        }
        return new SuccessResult<DataTableResponseWrapper<PlaceDataTableDto>>(null);
    }

    public async Task<Result<PlaceDto>> GetPlaceById(long id)
    {
        var placesEntity = await _placeRepository.GetPlaceById(id).FirstOrDefaultAsync();
        var placeDto = _mapper.Map<PlaceDto>(placesEntity);
        return new SuccessResult<PlaceDto>(placeDto);
    }

    public async Task<Result<PlaceUpdateRequestDto>> GetPlaceByIdForUpdateRequest(long id)
    {
        var placeEntity = await _placeRepository.GetPlaceById(id).FirstOrDefaultAsync();
        var placeDto = _mapper.Map<PlaceUpdateRequestDto>(placeEntity);
        placeDto.PlacePictureFormInput = await GetPlacePictureFormInput(
            new PlacePictureFilterDto() { PlaceId = id });
        return new SuccessResult<PlaceUpdateRequestDto>(placeDto);
    }

    private async Task<List<PlacePictureFormInputDto>> GetPlacePictureFormInput(
        PlacePictureFilterDto placePictureFilter)
    {
        var placePictureQuery = _placePictureRepository.GetPlacePictures(placePictureFilter);
        var placePictureQueryList = from placePicture in placePictureQuery
                                    select new PlacePictureFormInputDto
                                    {
                                        Id = placePicture.Id,
                                        code = placePicture.MediaGuiId,
                                        PlaceId = placePicture.PlaceId
                                    };
        return await placePictureQueryList.ToListAsync();
    }
}
```

### CategoryService.cs

```csharp
public class CategoryService : BaseService, ICategoryService
{
    // 6 repository injection: ICategoryRepository, ICategoryContentRepository,
    // ICategoryPictureRepository, IPlaceRepository, IContentLanguageRepository, IMediaFileRepository

    public async Task<Result<bool>> AddNewCategory(CategoryAddDto dto)
    {
        var categoryEntity = _mapper.Map<Category>(dto);
        _categoryRepository.Insert(categoryEntity, InsertStrategy.InsertAll);
        var result = await _categoryRepository.SaveChangesAsync();
        if (result < 0)
            return new ErrorResult<bool>(false, CategoryErrorEnum.CATEGORY_INSERT_ERROR);
        return new SuccessResult<bool>(true);
    }

    public Result<DataTableResponseWrapper<CategoryDataTableDto>> GetCategoryByDatatableDto(
        DataTableFilterModel<CategoryDataTableFilterPanelDto> dto)
    {
        var caregoryListFilterDto = _mapper.Map<CategoryListPanelFilterDto>(dto.data);
        var queryJoinedTables = GetListCategoryWithContent(caregoryListFilterDto);

        queryJoinedTables = queryJoinedTables
            .ApplySorting(dto.orderProperty, dto.orderDirective)
            .AsNoTracking();

        var totalRecordCount = queryJoinedTables.Count();
        var recordPerPage = dto.recordPerPage.GetValueOrDefault();
        var pageNumber = dto.pageNumber.GetValueOrDefault();
        int offset = pageNumber * recordPerPage;
        queryJoinedTables = queryJoinedTables.Skip(offset).Take(recordPerPage);

        var categoryListModelList = queryJoinedTables.ToList();
        var categoryWithContentDtos = _mapper.Map<List<CategoryDataTableDto>>(categoryListModelList);
        var result = new DataTableResponseWrapper<CategoryDataTableDto>(totalRecordCount, categoryWithContentDtos);
        return new SuccessResult<DataTableResponseWrapper<CategoryDataTableDto>>(result);
    }

    public async Task<Result<CategoryUpdateRequestDto>> GetCategoryById(long Id)
    {
        var categoryEntity = await _categoryRepository.FirstOrDefaultAsync(a => a.Id == Id);
        if (categoryEntity != null)
        {
            var categoryUpdateModel = new CategoryUpdateModel();
            categoryUpdateModel.CategoryPictures = _categoryPictureRepository
                .GetCategoryPictures(new CategoryPictureFilterDto { CategoryId = Id }).ToList();
            categoryUpdateModel.CategoryContents = _categoryContentRepository
                .GetContents(new CategoryContentFilterDto { CategoryId = Id }).ToList();

            var categoryUpdateRequestDto = _mapper.Map<CategoryUpdateRequestDto>(categoryUpdateModel);
            categoryUpdateRequestDto.CategoryPictureUpdateFormDtos =
                await GetCategoryPictureFormInput(new CategoryPictureFilterDto() { CategoryId = Id });
            categoryUpdateRequestDto.CategoryUpdateFormInputDtos =
                await GetContentLanguageWithCategoryContent(Id).ToListAsync();

            return new SuccessResult<CategoryUpdateRequestDto>(categoryUpdateRequestDto);
        }
        return new ErrorResult<CategoryUpdateRequestDto>(null, CategoryErrorEnum.CATEGORY_ID_ERROR);
    }

    public async Task<Result<bool>> DeleteCategoryById(long id)
    {
        _categoryRepository.Delete(id);
        var result = await _categoryRepository.SaveChangesAsync();
        if (result == 1)
            return new SuccessResult<bool>(true);
        else
            return new ErrorResult<bool>(false, CategoryErrorEnum.CATEGORY_DELETE_ERROR);
    }

    public async Task<Result<bool>> UpdateCategory(CategoryUpdateRequestDto dto)
    {
        // Id == 0 → Insert, Id != 0 → Update ayırımı
        var categoryContentEntitiesToInsert = _mapper.Map<List<CategoryContent>>(
            dto.CategoryContents.Where(x => x.Id == 0).ToList());
        var categoryContentEntitiesToUpdate = _mapper.Map<List<CategoryContent>>(
            dto.CategoryContents.Where(x => x.Id != 0)).ToList();
        var categoryPicturesToInsert = _mapper.Map<List<CategoryPicture>>(dto.CategoryPictures);
        var categoryPicturesToDelete = _mapper.Map<List<CategoryPicture>>(dto.CategoryPicturesDeleted);

        _categoryContentRepository.Insert(categoryContentEntitiesToInsert);
        _categoryPictureRepository.Insert(categoryPicturesToInsert);
        _categoryPictureRepository.Delete(categoryPicturesToDelete);
        _categoryContentRepository.UpdateWithProperties(
            categoryContentEntitiesToUpdate.ToArray(),
            new Expression<Func<CategoryContent, object>>[] { cc => cc.Name });

        await _categoryRepository.SaveChangesAsync();
        return new SuccessResult<bool>(true);
    }

    // Private: ContentLanguage LEFT JOIN CategoryContent (update formu için)
    private IQueryable<CategoryUpdateFormInputDto> GetContentLanguageWithCategoryContent(long categoryId)
    {
        var contentLanguageList = _contentLanguageRepository.GetContentLanguages(new ContentLanguageFilterDto());
        var categoryContentList = _categoryContentRepository.GetContents(
            new CategoryContentFilterDto() { CategoryId = categoryId });

        return from contentLanguage in contentLanguageList
               join categoryContent in categoryContentList
               on new { A = contentLanguage.Id, B = categoryId }
               equals new { A = categoryContent.LanguageId, B = categoryContent.CategoryId }
               into productContents
               from categoryContent in productContents.DefaultIfEmpty()
               select new CategoryUpdateFormInputDto
               {
                   LanguageId = contentLanguage.Id,
                   CategoryContentId = categoryContent.Id,
                   CategoryId = categoryContent.CategoryId,
                   LanguageName = contentLanguage.Name,
                   CategoryContentName = categoryContent.Name,
                   Prefix = contentLanguage.Prefix,
               };
    }

    // Private: 3-way JOIN (CategoryContent + CategoryPicture + Place)
    private IQueryable<CategoryListModel> GetListCategoryWithContent(CategoryListPanelFilterDto filter)
    {
        var queryCategoriesWithContent = _categoryContentRepository
            .GetContentWithCategory(_mapper.Map<CategoryWithContentFilterDto>(filter));
        var queryPlace = _placeRepository.GetPlaces(_mapper.Map<PlaceFilterDto>(filter));
        var queryCategoryPicture = _categoryPictureRepository
            .GetCategoryPictures(_mapper.Map<CategoryPictureFilterDto>(filter));

        return from queryCategoriesWithContentElement in queryCategoriesWithContent
               join categoryPicture in queryCategoryPicture
               on queryCategoriesWithContentElement.CategoryId equals categoryPicture.CategoryId
               into categoryPictures
               from categoryPicture in categoryPictures.DefaultIfEmpty()
               join place in queryPlace
               on queryCategoriesWithContentElement.Category.PlaceId equals place.Id
               into places
               from place in places.DefaultIfEmpty()
               select new CategoryListModel
               {
                   Id = queryCategoriesWithContentElement.Category.Id,
                   CategoryName = queryCategoriesWithContentElement.Name,
                   PlaceEmail = place.Email,
                   PlaceName = place.Name,
                   PlaceId = place.Id,
                   LanguageId = queryCategoriesWithContentElement.LanguageId
               };
    }
}
```

### ProductService.cs

```csharp
public class ProductService : BaseService, IProductService
{
    // 8 dependency: IProductRepository, IProductContentRepository, IProductPictureRepository,
    // IContentLanguageRepository, IPlaceRepository, IUtilService, IMediaFileRepository, IMediaFileService

    public async Task<Result<bool>> CreateNewProduct(ProductInsertDto productDto)
    {
        var productEntity = _mapper.Map<Product>(productDto);
        _productRepository.Insert(productEntity, InsertStrategy.InsertAll);
        await _productRepository.SaveChangesAsync();
        return new SuccessResult<bool>(true);
    }

    public async Task<Result<ExcelProductParseResponseModel>> CreateNewProductExcel(
        ExcelProductParseRequestModel excelProduct)
    {
        var productDto = _mapper.Map<List<ProductInsertDto>>(excelProduct.ExcelProducts);
        var productEntity = _mapper.Map<List<Product>>(productDto);
        await _productRepository.InsertAsync(productEntity);
        await _productRepository.SaveChangesAsync();
        return new SuccessResult<ExcelProductParseResponseModel>(
            new ExcelProductParseResponseModel { State = true });
    }

    public async Task<Result<bool>> UpdateProduct(ProductUpdateRequestDto productDto)
    {
        try
        {
            var productEntity = _mapper.Map<Product>(productDto.ProductUpdate);
            _productRepository.Update(productEntity, UpdateStrategy.OnlyMain);

            var productContentEntityUpdate = _mapper.Map<List<ProductContent>>(
                productDto.ProductContents.Where(x => x.Id != 0).ToList());
            var productContentEntityInsert = _mapper.Map<List<ProductContent>>(
                productDto.ProductContents.Where(x => x.Id == 0).ToList());
            var productPictureEntityInsert = _mapper.Map<List<ProductPicture>>(productDto.ProductPictures);
            var productPictureEntityDelete = _mapper.Map<List<ProductPicture>>(productDto.ProductPicturesDeleted);

            _productContentRepository.UpdateWithProperties(
                productContentEntityUpdate.ToArray(),
                new Expression<Func<ProductContent, object>>[] { s => s.Detail, s => s.Name });
            _productContentRepository.Insert(productContentEntityInsert);
            _productPictureRepository.Insert(productPictureEntityInsert);
            _productPictureRepository.DeleteWithStateRange(productPictureEntityDelete);

            await _productRepository.SaveChangesAsync();
        }
        catch (Exception ee)
        {
            // UYARI: Exception yutularak her zaman başarı döndürülüyor!
        }
        return new SuccessResult<bool>(true);
    }

    public async Task<Result<ProductUpdateRequestDto>> GetProductForUpdate(long id)
    {
        var productUpdateModel = new ProductUpdateModel();
        var productEntity = await _productRepository.GetProductById(id).AsNoTracking().FirstOrDefaultAsync();
        if (productEntity != null)
        {
            productUpdateModel.ProductUpdate = productEntity;
            productUpdateModel.ProductPictures = await _productPictureRepository
                .GetProductPictures(new ProductPictureFilterDto() { ProductId = id }).ToListAsync();
            productUpdateModel.ProductContents = await _productContentRepository
                .GetContents(new ProductContentFilterDto() { ProductId = id }).ToListAsync();
        }
        var productDto = _mapper.Map<ProductUpdateRequestDto>(productUpdateModel);
        productDto.ProductContentFormInput = await GetContentLanguageWithProductContent(id).ToListAsync();
        productDto.ProductMainPictureFormInput = await GetProductPictureFormInput(
            new ProductPictureFilterDto() { ProductId = id });
        return new SuccessResult<ProductUpdateRequestDto>(productDto);
    }

    public Result<DataTableResponseWrapper<ProductDataTableDto>> GetProductDataTablePanel(
        DataTableFilterModel<ProductDataTableFilterPanelDto> dataTableFilterModel)
    {
        var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
        var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
        int ofset = pageNumber * recordPerPage;
        var productListFilterDto = _mapper.Map<ProductListPanelFilterDto>(dataTableFilterModel.data);
        var productListFilter = GetProductListForPanel(productListFilterDto);
        var productResult = productListFilter
            .ApplySorting(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective)
            .AsNoTracking();
        int toplamKayit = productResult.Count();
        var productList = productResult.Skip(ofset).Take(recordPerPage).ToList();
        if (productList != null)
        {
            var productDto = _mapper.Map<List<ProductDataTableDto>>(productList);
            var result = new DataTableResponseWrapper<ProductDataTableDto>(toplamKayit, productDto);
            return new SuccessResult<DataTableResponseWrapper<ProductDataTableDto>>(result);
        }
        return new SuccessResult<DataTableResponseWrapper<ProductDataTableDto>>(null);
    }

    // Private: ContentLanguage LEFT JOIN ProductContent (update formu için)
    private IQueryable<PrdouctContentFormInputDto> GetContentLanguageWithProductContent(long productId)
    {
        var contentLanguageList = _contentLanguageRepository.GetContentLanguages(new ContentLanguageFilterDto());
        var productContentList = _productContentRepository.GetContents(
            new ProductContentFilterDto() { ProductId = productId });

        return from contentLanguage in contentLanguageList
               join productContent in productContentList
               on new { A = contentLanguage.Id, B = productId }
               equals new { A = productContent.LanguageId, B = productContent.ProductId }
               into productContents
               from productContent in productContents.DefaultIfEmpty()
               select new PrdouctContentFormInputDto
               {
                   ContentLanguageId = contentLanguage.Id,
                   ProductContentId = productContent.Id,
                   ProductId = productContent.ProductId,
                   LanguageName = contentLanguage.Name,
                   ProductDetail = productContent.Detail,
                   Prefix = contentLanguage.Prefix,
                   ProductName = productContent.Name,
                   IsDefault = contentLanguage.IsDefault,
                   Order = contentLanguage.Order
               };
    }

    // Private: 3-way JOIN (ProductContent + ProductPicture + Place) → panel liste
    private IQueryable<ProductListModel> GetProductListForPanel(ProductListPanelFilterDto filter)
    {
        var productNameDefault = _productContentRepository.GetProductsWithContent(
            _mapper.Map<ProductContentWithProductFilterDto>(filter));
        var productMainPicture = _productPictureRepository.GetProductPictures(
            _mapper.Map<ProductPictureFilterDto>(filter));
        var productPlace = _placeRepository.GetPlaces();

        return from productContent in productNameDefault
               join productPicture in productMainPicture
               on productContent.ProductId equals productPicture.ProductId
               into productPictures
               from productPicture in productPictures.DefaultIfEmpty()
               join place in productPlace
               on productContent.Product.PlaceId equals place.Id
               into places
               from place in places.DefaultIfEmpty()
               select new ProductListModel
               {
                   Id = productContent.ProductId,
                   ProductContentId = productContent.Id,
                   Name = productContent.Name,
                   Price = productContent.Product.Price,
                   PictureCode = productContent.Product.ProductPictures.FirstOrDefault() != null
                       ? productContent.Product.ProductPictures.FirstOrDefault().MediaGuiId : "",
                   PlaceName = place.Name,
                   PlaceId = place.Id
               };
    }
}
```

### MenuService.cs

```csharp
public class MenuService : BaseService, IMenuService
{
    // 11 dependency injection (IMenuRepository, IMenuProductRepository, IProductService, ICategoryService,
    // IProductContentRepository, ICategoryContentRepository, IProductPictureRepository,
    // ICategoryPictureRepository, IPlaceRepository, IProductRepository, ICategoryRepository)

    public Result<DataTableResponseWrapper<MenuDataTableDto>> GetMenuForDataTablePanel(
        DataTableFilterModel<MenuDataTableFilterDto> dataTableFilterModel)
    {
        var menu = _menuRepository.GetMenus(_mapper.Map<MenuFilterDto>(dataTableFilterModel.data));
        var placeList = _placeRepository.GetPlaces(_mapper.Map<PlaceFilterDto>(dataTableFilterModel.data));

        var menuQuery = from m in menu
                        join place in placeList on m.PlaceId equals place.Id
                        into places
                        from place in places.DefaultIfEmpty()
                        select new MenuDataTableDto
                        {
                            Id = m.Id,
                            MenuName = m.MenuName,
                            State = m.State,
                            StateText = m.State == ActivePassive.ACTIVE ? "Aktif" : "Pasif",
                            PlaceName = place.Name,
                            PlaceId = place.Id
                        };

        var menuList = menuQuery.Skip(ofset).Take(recordPerPage).ToList();
        return new SuccessResult<DataTableResponseWrapper<MenuDataTableDto>>(
            new DataTableResponseWrapper<MenuDataTableDto>(toplamKayit, menuList));
    }

    public async Task<Result<bool>> CreateNewMenuProduct(MenuProductInsertDto menuProductInsert)
    {
        if (menuProductInsert.Type == MenuProductTypeEnum.CATEGORY)
        {
            // İş kuralı: Aynı level'da PRODUCT varsa CATEGORY eklenemez
            var menuKontrol = _menuProductRepository.Where(x =>
                x.MenuId == menuProductInsert.MenuId &&
                x.Level == menuProductInsert.Level &&
                x.Type == MenuProductTypeEnum.PRODUCT && !x.Deleted).FirstOrDefault();

            if (menuKontrol == null)
            {
                var menuProductEntity = _mapper.Map<MenuProduct>(menuProductInsert);
                _menuProductRepository.Insert(menuProductEntity, InsertStrategy.OnlytMain);
                await _menuProductRepository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            }
            return new ErrorResult<bool>(false, MenuErrorEnum.MENU_CATEGORY_INSERT_ERROR);
        }
        else if (menuProductInsert.Type == MenuProductTypeEnum.PRODUCT)
        {
            // İş kuralı: Aynı level'da CATEGORY varsa PRODUCT eklenemez
            var menuKontrol = _menuProductRepository.Where(x =>
                x.MenuId == menuProductInsert.MenuId &&
                x.Level == menuProductInsert.Level &&
                x.Type == MenuProductTypeEnum.CATEGORY && !x.Deleted).FirstOrDefault();

            if (menuKontrol == null)
            {
                var menuProductEntity = _mapper.Map<MenuProduct>(menuProductInsert);
                _menuProductRepository.Insert(menuProductEntity, InsertStrategy.OnlytMain);
                await _menuProductRepository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            }
            return new ErrorResult<bool>(false, MenuErrorEnum.MENU_PRODUCT_INSERT_ERROR);
        }
        return new ErrorResult<bool>(false, MenuErrorEnum.MENU_PRODUCT_OR_CATEGORY_INSERT_ERROR);
    }

    public async Task<Result<MenuDto>> CreateNewMenu(MenuDto menu)
    {
        var menuEntity = _mapper.Map<Menu>(menu);
        var insertedMenu = _menuRepository.Insert(menuEntity, InsertStrategy.OnlytMain);
        await _menuRepository.SaveChangesAsync();
        return new SuccessResult<MenuDto>(_mapper.Map<MenuDto>(insertedMenu));
    }

    public Result<List<MenuProductListDto>> GetListMenuProduct(MenuProductFilterDto menuProductFilter)
    {
        // 6-way LINQ JOIN: MenuProduct + ProductContent + CategoryContent + ProductPicture + CategoryPicture + Menu + Place
        var menuProductQuery = from menuProduct in menuProducts
                               join productContent in productNameDefault
                               on menuProduct.ProductId equals productContent.ProductId
                               into productContents
                               from productContent in productContents.DefaultIfEmpty()
                               join categoryContent in categoryNameDefault
                               on menuProduct.CategoryId equals categoryContent.CategoryId
                               into categoryContents
                               from categoryContent in categoryContents.DefaultIfEmpty()
                               join productPicture in productMainPicture
                               on menuProduct.ProductId equals productPicture.ProductId
                               into productPictures
                               from productPicture in productPictures.DefaultIfEmpty()
                               join categoryPicture in categoryMainPicture
                               on menuProduct.CategoryId equals categoryPicture.CategoryId
                               into categoryPictures
                               from categoryPicture in categoryPictures.DefaultIfEmpty()
                               join menu in menuList on menuProduct.MenuId equals menu.Id
                               into menus
                               from menu in menus.DefaultIfEmpty()
                               join place in placeList on menu.PlaceId equals place.Id
                               into places
                               from place in places.DefaultIfEmpty()
                               select new MenuProductListDto
                               {
                                   Id = menuProduct.Id,
                                   Name = productContent.Name != null ? productContent.Name : categoryContent.Name,
                                   ParentCategoryId = menuProduct.ParentCategoryId,
                                   CategoryId = menuProduct.CategoryId,
                                   ProductId = menuProduct.ProductId,
                                   Level = menuProduct.Level,
                                   MenuId = menuProduct.MenuId,
                                   Order = menuProduct.Order,
                                   Type = menuProduct.Type,
                                   Code = productPicture != null ? productPicture.MediaGuiId : categoryPicture.MediaGuiId,
                                   PlaceName = place.Name,
                               };
        return new SuccessResult<List<MenuProductListDto>>(menuProductQuery.ToList());
    }
}
```

### PlaceLanguageService.cs

```csharp
public class PlaceLanguageService : BaseService, IPlaceLanguageService
{
    public Result<List<PlaceLanguageDto>> GetPlaceLanguageByPlaceId(long id)
    {
        var entities = _placeLanguageRepository.GetPlaceLanguagesByPlaceId(id).ToList();
        return new SuccessResult<List<PlaceLanguageDto>>(_mapper.Map<List<PlaceLanguageDto>>(entities));
    }

    public Result<List<PlaceLanguageDto>> GetPlaceLanguageByPlaceIdWithContentLanguage(long id)
    {
        var entities = _placeLanguageRepository.GetPlaceLanguagesByPlaceIdWithContentLanguage(id).ToList();
        return new SuccessResult<List<PlaceLanguageDto>>(_mapper.Map<List<PlaceLanguageDto>>(entities));
    }

    public async Task<Result<bool>> SetDefaultPlaceLanguage(PlaceSetDefaultLanguageDto dto)
    {
        // Eski default'u false yap (partial update)
        var defaultEntity = _placeLanguageRepository
            .Where(x => !x.Deleted && x.PlaceId == dto.placeId && x.IsDefault).FirstOrDefault();
        if (defaultEntity != null)
        {
            _placeLanguageRepository.UpdateWithProperties(defaultEntity,
                new Expression<Func<PlaceLanguage, object>>[] { s => s.IsDefault });
            defaultEntity.IsDefault = false;
        }
        // Yeni default'u true yap (partial update)
        var placeLanguageEntity = await _placeLanguageRepository.FindAsync(dto.id);
        if (placeLanguageEntity != null)
        {
            _placeLanguageRepository.UpdateWithProperties(placeLanguageEntity,
                new Expression<Func<PlaceLanguage, object>>[] { s => s.IsDefault });
            placeLanguageEntity.IsDefault = true;
        }
        await _placeLanguageRepository.SaveChangesAsync();
        return new SuccessResult<bool>(true);
    }

    public async Task<Result<bool>> createNewPlaceLanguage(PlaceLanguageDto placeLanguageDto)
    {
        // Duplicate kontrolü: aynı ContentLanguageId + PlaceId kombinasyonu
        var existing = _placeLanguageRepository
            .Where(x => !x.Deleted && x.ContentLanguageId == placeLanguageDto.ContentLanguageId
                        && x.PlaceId == placeLanguageDto.PlaceId)
            .FirstOrDefault();

        if (existing == null)
        {
            var entity = _mapper.Map<PlaceLanguage>(placeLanguageDto);
            await _placeLanguageRepository.InsertAsync(entity);
            var count = await _placeLanguageRepository.SaveChangesAsync();
            if (count > 0) return new SuccessResult<bool>(true);
            return new ErrorResult<bool>(false, PlaceLanguageErrorEnum.PLACE_LANGUAGE_INSERT_ERROR);
        }
        return new SuccessResult<bool>(true); // Zaten var, idempotent
    }

    public async Task<Result<bool>> deletePlaceLanguage(List<PlaceLanguageDto> placeLanguageDto)
    {
        var entities = _mapper.Map<List<PlaceLanguage>>(placeLanguageDto);
        _placeLanguageRepository.DeleteWithStateRange(entities); // soft delete
        var count = await _placeLanguageRepository.SaveChangesAsync();
        if (count > 0) return new SuccessResult<bool>(true);
        return new ErrorResult<bool>(false, PlaceLanguageErrorEnum.PLACE_LANGUAGE_REMOVE_ERROR);
    }
}
```

### ContentLanguageService.cs

```csharp
public class ContentLanguageService : BaseService, IContentLanguageService
{
    public Result<List<ContentLanguageDto>> GetAllContentLanguageDto()
    {
        var entities = _contentLanguageRepository.GetContentLanguage(null).ToList();
        return new SuccessResult<List<ContentLanguageDto>>(_mapper.Map<List<ContentLanguageDto>>(entities));
    }

    public Result<List<ContentLanguageDto>> GetContenLanguageNotInPlace(long placeId)
    {
        // Mekanda kayıtlı olmayan dilleri döndürür (UI'da yeni dil ekleme dropdown için)
        var placeLanguageIdList = _placeLanguageRepository
            .GetPlaceLanguagesByPlaceId(placeId)
            .Select(x => x.ContentLanguageId)
            .ToList();
        var entities = _contentLanguageRepository.GetContentLanguage(placeLanguageIdList);
        return new SuccessResult<List<ContentLanguageDto>>(_mapper.Map<List<ContentLanguageDto>>(entities));
    }
}
```

### MediaFileService.cs

```csharp
public class MediaFileService : BaseService, IMediaFileService
{
    public async Task<Result<CreateMediaFileResponseDto>> CreateNewMediaFile(MediaFileDto mediaFileDto)
    {
        var entity = _mapper.Map<MediaFile>(mediaFileDto);
        var inserted = await _mediaFileRepository.InsertAsync(entity);
        var count = await _mediaFileRepository.SaveChangesAsync();
        var dto = _mapper.Map<CreateMediaFileResponseDto>(inserted);
        if (count > 0) return new SuccessResult<CreateMediaFileResponseDto>(dto);
        return new ErrorResult<CreateMediaFileResponseDto>(dto, MediaFileErrorEnum.MEDIA_FILE_INSERT_ERROR);
    }
}
```

---

## 7. Application — Consumer'lar

### Ana API: Sadece Publisher

Ana CRM API'sinde consumer yoktur. Sadece `IPublishEndpoint` üzerinden event yayınlar:

```csharp
// Startup.cs — consumer kaydı YOK
services.AddRabbitMqServices(new RabbitMqConfigModel()
{
    HostAddress = GetAppSettingValue("EventBusSettings:HostAddress")
});
// AddMassTransit içinde config.AddConsumer<...> yok
```

### FileAPI: ExcelProductInsertConsumer

```csharp
// FileAPI/Startup.cs
services.AddMassTransit(config => {
    config.AddConsumer<ExcelProductInsertConsumer>(); // Consumer kayıtlı
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.ExcelProductInsertQueue, c =>
        {
            c.ConfigureConsumer<ExcelProductInsertConsumer>(ctx);
            c.PrefetchCount = 1; // Tek seferde 1 mesaj işle
        });
    });
});
```

### RabbitMQ Event Sınıfları

```csharp
public class IntegrationBaseEvent
{
    public Guid Id { get; private set; }
    public DateTime CreationDate { get; private set; }
    public IntegrationBaseEvent() { Id = Guid.NewGuid(); CreationDate = DateTime.UtcNow; }
}

public class ExcelProductInsertEvent { public string ProductName { get; set; } }

public class TestEvent { public string Name { get; set; } }

public class ExceptionEvent
{
    public string ExceptionMessage { get; set; }
    public string ActionName { get; set; }
    public string ControllerName { get; set; }
    public string ExceptionName { get; set; }
}

public class NotificationEvent { /* bildirim verisi */ }
```

---

## 8. Persistence — DbContext (tam kod)

```csharp
namespace Persistence.DbContext
{
    public class PixdinnCrmDbContext : UnitOfWork, IMultipleDbProjectBaseDbContext
    {
        public PixdinnCrmDbContext(DbContextOptions<PixdinnCrmDbContext> dbContextOptions)
           : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // LazyLoading kapalı (yorum satırında bırakılmış)
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cascade delete döngüsünü kırmak için NoAction
            // MenuProduct hem Product, Category, ParentCategory (self-ref), Menu'ya bağlı
            modelBuilder.Entity<MenuProduct>().HasOne(e => e.Product).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MenuProduct>().HasOne(e => e.Category).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MenuProduct>().HasOne(e => e.ParentCategory).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MenuProduct>().HasOne(e => e.Menu).WithMany().OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductContent> ProductContent { get; set; }
        public DbSet<ContentLanguage> ContentLanguage { get; set; }
        public DbSet<ProductPicture> ProductPicture { get; set; }
        // public DbSet<MediaFile> MediaFile { get; set; }  ← YORUM SATIRI (kullanılmıyor)
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryPicture> CategoryPicture { get; set; }
        public DbSet<CategoryContent> CategoryContent { get; set; }
        public DbSet<MenuProduct> MenuProduct { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Place> Place { get; set; }
        public DbSet<PlaceLanguage> PlaceLanguage { get; set; }
        public DbSet<PlacePicture> PlacePicture { get; set; }
    }
}
```

### UnitOfWork (FrameworkCore base)

```csharp
public abstract class UnitOfWork : DbContext, IUnitOfWork
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine); // TÜM SQL'leri console'a yazar — production'da kapatılmalı
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges()
    {
        BeforeSave(DateTime.UtcNow);
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        BeforeSave(DateTime.UtcNow);
        return await base.SaveChangesAsync();
    }

    private void BeforeSave(DateTime timestamp)
    {
        ChangeTracker.DetectChanges();
        // Potansiyel alan: CreatedAt/UpdatedAt otomatik set etme
    }
}
```

---

## 9. Persistence — Repository'ler (TÜM, tam kod)

### PlaceRepository.cs

```csharp
public class PlaceRepository : ConnectedRepository<Place>, IPlaceRepository
{
    private PixdinnCrmDbContext _appDbContext { get => _dbContext as PixdinnCrmDbContext; }

    public PlaceRepository(IUnitOfWork dbContext) : base(dbContext) { }

    public IQueryable<Place> GetPlaces()
    {
        return _appDbContext.Place.AsNoTracking();
    }

    public IQueryable<Place> GetPlaces(PlaceFilterDto placeFilterDto)
    {
        Expression<Func<Place, bool>> predicatePlace = s => s.Deleted == false;
        if (!String.IsNullOrEmpty(placeFilterDto.PlaceName))
            predicatePlace = predicatePlace.And(s => s.Name.Contains(placeFilterDto.PlaceName));
        if (!String.IsNullOrEmpty(placeFilterDto.PlaceEmail))
            predicatePlace = predicatePlace.And(s => s.Email.Contains(placeFilterDto.PlaceEmail));
        return _appDbContext.Place.Where(predicatePlace).AsNoTracking();
    }

    public IQueryable<Place> GetPlaceById(long placeId)
    {
        return _appDbContext.Place.Where(x => x.Id == placeId);
    }
}
```

### CategoryRepository.cs

```csharp
public class CategoryRepository : ConnectedRepository<Category>, ICategoryRepository
{
    private readonly ICategoryContentRepository _categoryContentRepository;

    public CategoryRepository(IUnitOfWork dbContext, ICategoryContentRepository categoryContentRepository)
        : base(dbContext)
    {
        _categoryContentRepository = categoryContentRepository;
    }

    public IQueryable<Category> GetCategorieById(long id)
    {
        return _appDbContext.Category.Where(x => x.Id == id)
            .Select(c => new Category { Id = c.Id, PlaceId = c.PlaceId });
    }

    public IQueryable<Category> GetCategories(long? placeId)
    {
        Expression<Func<Category, bool>> predicate = s => s.Deleted == false;
        if (placeId != null)
            predicate = predicate.And(s => s.PlaceId == placeId);
        return _appDbContext.Category.Where(predicate)
            .Select(c => new Category { Id = c.Id, PlaceId = c.PlaceId });
    }
}
```

### CategoryContentRepository.cs

```csharp
public class CategoryContentRepository : ConnectedRepository<CategoryContent>, ICategoryContentRepository
{
    public IQueryable<CategoryContent> GetContents(CategoryContentFilterDto dto)
    {
        Expression<Func<CategoryContent, bool>> predicate = cc => cc.Deleted == false;
        if (!String.IsNullOrEmpty(dto.CategoryName))
            predicate = predicate.And(cc => cc.Name.Contains(dto.CategoryName));
        if (dto.LanguageId != null)
            predicate = predicate.And(cc => cc.LanguageId == dto.LanguageId);
        if (dto.CategoryId != null)
            predicate = predicate.And(cc => cc.CategoryId == dto.CategoryId);
        return _appDbContext.CategoryContent.Where(predicate);
    }

    public IQueryable<CategoryContent> GetContentWithCategory(CategoryWithContentFilterDto dto)
    {
        Expression<Func<CategoryContent, bool>> predicate = cc => cc.Deleted == false && cc.Category != null;
        if (!String.IsNullOrEmpty(dto.CategoryName))
            predicate = predicate.And(cc => cc.Name.Contains(dto.CategoryName));
        if (dto.PlaceId != null)
            predicate = predicate.And(cc => cc.Category.PlaceId == dto.PlaceId);
        if (dto.LanguageId != null)
            predicate = predicate.And(cc => cc.LanguageId == dto.LanguageId);
        else
            predicate = predicate.And(cc => cc.Language.IsDefault); // Dil verilmezse varsayılan dil
        return _appDbContext.CategoryContent.Where(predicate);
    }
}
```

### ProductRepository.cs

```csharp
public class ProductRepository : ConnectedRepository<Product>, IProductRepository
{
    public IQueryable<Product> GetProductById(long id)
    {
        return _appDbContext.Product.Where(x => x.Id == id)
            .Select(p => new Product { Id = p.Id, Price = p.Price, PlaceId = p.PlaceId });
    }

    public IQueryable<Product> GetProducts()
    {
        return _appDbContext.Product.Where(x => !x.Deleted);
    }

    public IQueryable<Product> GetProducts(ProductFilterDto productFilter)
    {
        Expression<Func<Product, bool>> predicateProduct = p => !p.Deleted;
        if (productFilter.PlaceId != null) predicateProduct = predicateProduct.And(p => p.PlaceId == productFilter.PlaceId);
        if (productFilter.MaxPrice != null) predicateProduct = predicateProduct.And(p => p.Price <= productFilter.MaxPrice);
        if (productFilter.MinPrice != null) predicateProduct = predicateProduct.And(p => p.Price >= productFilter.MinPrice);
        return _appDbContext.Product.Where(predicateProduct);
    }
}
```

### ProductContentRepository.cs

```csharp
class ProductContentRepository : ConnectedRepository<ProductContent>, IProductContentRepository
{
    public IQueryable<ProductContent> GetContents(ProductContentFilterDto dto)
    {
        Expression<Func<ProductContent, bool>> predicate = cc => cc.Deleted == false;
        if (!String.IsNullOrEmpty(dto.ProductName))
            predicate = predicate.And(pc => pc.Name.Contains(dto.ProductName));
        if (dto.LanguageId != null)
            predicate = predicate.And(pc => pc.LanguageId == dto.LanguageId);
        if (dto.ProductId != null)
            predicate = predicate.And(pc => pc.ProductId == dto.ProductId);
        return _appDbContext.ProductContent.Where(predicate);
    }

    public IQueryable<ProductContent> GetProductsWithContent(ProductContentWithProductFilterDto dto)
    {
        Expression<Func<ProductContent, bool>> predicate = pc => !pc.Deleted && pc.Product != null;
        if (dto.LanguageId != null)
            predicate = predicate.And(pc => pc.LanguageId == dto.LanguageId);
        else
            predicate = predicate.And(pc => pc.Language.IsDefault);
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
}
```

### MenuRepository.cs

```csharp
public class MenuRepository : ConnectedRepository<Menu>, IMenuRepository
{
    public IQueryable<Menu> GetMenus(MenuFilterDto menuFilter)
    {
        Expression<Func<Menu, bool>> predicate = s => s.Deleted == false;
        if (menuFilter.PlaceId != null) predicate = predicate.And(s => s.PlaceId == menuFilter.PlaceId);
        if (!string.IsNullOrEmpty(menuFilter.MenuName)) predicate = predicate.And(s => s.MenuName.Contains(menuFilter.MenuName));
        return _appDbContext.Menu.Where(predicate).AsNoTracking();
    }

    public IQueryable<Menu> GetMenus()
    {
        return _appDbContext.Menu.Where(s => s.Deleted == false).AsNoTracking();
    }

    public IQueryable<Menu> GetMenuById(long menuId)
    {
        return _appDbContext.Menu.Where(s => s.Deleted == false && s.Id == menuId).AsNoTracking();
    }
}
```

### MenuProductRepository.cs

```csharp
public class MenuProductRepository : ConnectedRepository<MenuProduct>, IMenuProductRepository
{
    public IQueryable<MenuProduct> ListMenuProduct(MenuProductFilterDto menuProductFilter)
    {
        Expression<Func<MenuProduct, bool>> predicate = s => s.Deleted == false;
        // ParentCategoryId null ise kök seviye, null değilse alt seviye filtrelemesi
        if (menuProductFilter.ParentCategoryId != null)
            predicate = predicate.And(s => s.ParentCategoryId == menuProductFilter.ParentCategoryId);
        else
            predicate = predicate.And(s => s.ParentCategoryId == null);
        if (menuProductFilter.MenuId != null)
            predicate = predicate.And(s => s.MenuId == menuProductFilter.MenuId);
        return _appDbContext.MenuProduct.Where(predicate);
    }
}
```

### PlaceLanguageRepository.cs

```csharp
class PlaceLanguageRepository : ConnectedRepository<PlaceLanguage>, IPlaceLanguageRepository
{
    public IQueryable<PlaceLanguage> GetPlaceLanguagesByPlaceId(long placeId)
    {
        Expression<Func<PlaceLanguage, bool>> predicate = s => s.Deleted == false;
        predicate = predicate.And(s => s.PlaceId == placeId);
        return _appDbContext.PlaceLanguage.Where(predicate).AsNoTracking();
    }

    public IQueryable<PlaceLanguage> GetPlaceLanguagesByPlaceIdWithContentLanguage(long placeId)
    {
        Expression<Func<PlaceLanguage, bool>> predicate = s => s.Deleted == false;
        predicate = predicate.And(s => s.PlaceId == placeId);
        return _appDbContext.PlaceLanguage.Where(predicate)
            .Include(s => s.ContentLanguage) // Explicit Include
            .AsNoTracking();
    }
}
```

### ConnectedRepository.cs (FrameworkCore base)

```csharp
public class ConnectedRepository<TEntity> : BaseRepository<TEntity>, IRepository<TEntity>
    where TEntity : class, IEntity
{
    // Soft delete: fiziksel silme YOK, Deleted=true set edilir
    public override void DeleteWithStateRange(IEnumerable<TEntity> entities)
    {
        entities.ToList().ForEach(item => { item.Deleted = true; });
        Update(entities);
    }

    // Partial update: sadece belirtilen property'ler güncellenir
    public override void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties)
    {
        _dbContext.Entry(entity).State = EntityState.Unchanged;
        foreach (var property in properties)
            _dbContext.Entry(entity).Property(property).IsModified = true;
    }

    public override void UpdateWithProperties(TEntity[] entities, Expression<Func<TEntity, object>>[] properties)
    {
        foreach (var entity in entities)
        {
            _dbContext.Entry(entity).State = EntityState.Unchanged;
            foreach (var property in properties)
                _dbContext.Entry(entity).Property(property).IsModified = true;
        }
    }

    // Dinamik sıralama: string property adından Expression oluşturur
    public override IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities)
    {
        if (!string.IsNullOrEmpty(directive) && !string.IsNullOrEmpty(property))
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var body = property.Split('.').Aggregate((Expression)parameter, Expression.Property);
            if (body.Type.IsValueType) body = Expression.Convert(body, typeof(object));
            var selector = Expression.Lambda<Func<TEntity, object>>(body, parameter);
            return directive == "asc" ? entities.OrderBy(selector) : entities.OrderByDescending(selector);
        }
        return entities.OrderBy(i => i.Id);
    }
}
```

---

## 10. Persistence — Entity Config

Proje ayrı bir `EntityConfiguration` / `EntityTypeConfiguration<T>` sınıf yapısı kullanmıyor. Tüm Fluent API konfigürasyonu DbContext.OnModelCreating içindedir:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    // MenuProduct'ın 4 farklı FK'ı cascade delete döngüsüne yol açar.
    // EF Core MSSQL'de bu durumda migration hatası verir → NoAction ile çözülür.
    modelBuilder.Entity<MenuProduct>().HasOne(e => e.Product).WithMany().OnDelete(DeleteBehavior.NoAction);
    modelBuilder.Entity<MenuProduct>().HasOne(e => e.Category).WithMany().OnDelete(DeleteBehavior.NoAction);
    modelBuilder.Entity<MenuProduct>().HasOne(e => e.ParentCategory).WithMany().OnDelete(DeleteBehavior.NoAction);
    modelBuilder.Entity<MenuProduct>().HasOne(e => e.Menu).WithMany().OnDelete(DeleteBehavior.NoAction);
}
```

Diğer tüm ilişkiler ve tablo isimleri Data Annotation ile tanımlanır:
- `[Table("TableName", Schema = "Crm")]` — tüm tablolarda schema = "Crm"
- `[ForeignKey("FKConstraintName")]` — FK constraint isimleri explicit tanımlı

---

## 11. API — Controller'lar (TÜM, tam kod)

### PlaceController.cs

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class PlaceController : ControllerBase
{
    private readonly IPlaceService _placeService;
    private readonly IConfiguration _configuration;
    private readonly IFileApiWebRequest _fileApiWebRequest;
    private readonly IMediaFileService _mediaFileService;

    public PlaceController(IPlaceService placeService, IFileApiWebRequest fileApiWebRequest,
        IMediaFileService mediaFileService, IConfiguration configuration)
    {
        _placeService = placeService;
        _configuration = configuration;
        _mediaFileService = mediaFileService;
        _fileApiWebRequest = fileApiWebRequest;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "Add")]
    [ValidateFilter] // FluentValidation tetikler
    public async Task<IActionResult> Add(PlaceInsertDto placeDto)
    {
        var result = await _placeService.CreateNewPlace(placeDto);
        return this.FromResult(result); // Result<T> → IActionResult
    }

    [HttpPut]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "Update")]
    [ValidateFilter]
    public async Task<IActionResult> Update(PlaceUpdateRequestDto dto)
    {
        var result = await _placeService.UpdatePlace(dto);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "List")]
    public IActionResult List(DataTableFilterModel<PlaceFilterDto> placeFilterDto)
    {
        var result = _placeService.GetPlacesForDataTable(placeFilterDto);
        return this.FromResult(result);
    }

    // Dosya yükleme: Refit ile FileAPI'ye proxy eder
    [HttpPost]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "Update")]
    public async Task<IActionResult> AddPlacePicture([FromForm] IFormFile file)
    {
        var fileApiUrl = _configuration.GetSection("FileApi:Url").Value;
        IRefitService refitFile = RestService.For<IRefitService>(fileApiUrl); // DI yerine runtime oluşturuluyor
        UploadFileRequest uploadFileRequest = new UploadFileRequest();
        uploadFileRequest.GroupList = JsonConvert.DeserializeObject<List<GroupData>>(Request.Form["fileGroup"]);
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            uploadFileRequest.File = memoryStream.ToArray();
        }
        uploadFileRequest.FileName = file.FileName;
        var response = await refitFile.UploadPicture(uploadFileRequest);
        return this.FromResult(new SuccessResult<UploadFileResponse>(new UploadFileResponse { FileKey = response.FileKey }));
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "List")]
    public async Task<IActionResult> GetPlaceById(long id)
    {
        var result = await _placeService.GetPlaceByIdForUpdateRequest(id);
        return this.FromResult(result);
    }
}
```

### CategoryController.cs

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IConfiguration _configuration;

    [HttpPost] [ProducesResponseType(typeof(DataTableResponseWrapper<CategoryDataTableDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "GetCategories")]
    public IActionResult GetCategories(DataTableFilterModel<CategoryDataTableFilterPanelDto> dto)
    {
        var resp = _categoryService.GetCategoryByDatatableDto(dto);
        return this.FromResult(resp);
    }

    [HttpPost] [SwaggerOperation(OperationId = "AddCategory")]
    public async Task<ActionResult> AddCategory(CategoryAddDto categoryAddDto)
    {
        var resp = await _categoryService.AddNewCategory(categoryAddDto);
        return this.FromResult(resp);
    }

    [HttpPost] // Dosya yükleme: Refit ile FileAPI'ye proxy eder
    public async Task<IActionResult> AddCategoryPicture(IFormFile file) { /* Refit pattern */ }

    [HttpGet("{id:int}")] [SwaggerOperation(OperationId = "GetCategoryById")]
    public async Task<ActionResult> GetCategoryById(long id)
    {
        var resp = await _categoryService.GetCategoryById(id);
        return this.FromResult(resp);
    }

    [HttpDelete("{id:int}")] [SwaggerOperation(OperationId = "Delete")]
    public async Task<ActionResult> Delete(long id)
    {
        var resp = await _categoryService.DeleteCategoryById(id);
        return this.FromResult(resp);
    }

    [HttpPut] [SwaggerOperation(OperationId = "Update")]
    public async Task<IActionResult> Update(CategoryUpdateRequestDto dto)
    {
        var result = await _categoryService.UpdateCategory(dto);
        return this.FromResult(result);
    }

    [HttpPost] // Mobil için — aynı datatable metodu (ayrı API'ye taşınacak notu var)
    public IActionResult GetCategoriesMobil(DataTableFilterModel<CategoryDataTableFilterPanelDto> dto)
    {
        var resp = _categoryService.GetCategoryByDatatableDto(dto);
        return this.FromResult(resp);
    }
}
```

### ProductController.cs

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
public class ProductController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add(ProductInsertDto dto)
    {
        var result = await _productService.CreateNewProduct(dto);
        return this.FromResult(result);
    }

    [HttpPut] [ValidateFilter]
    public async Task<IActionResult> Update(ProductUpdateRequestDto dto)
    {
        var result = await _productService.UpdateProduct(dto);
        return this.FromResult(result);
    }

    [HttpPost] // Dosya yükleme: Refit ile FileAPI'ye proxy eder
    public async Task<IActionResult> AddProductPicture(IFormFile file) { /* Refit pattern */ }

    [HttpPost] // DataTable liste + sayfalama
    public IActionResult List(DataTableFilterModel<ProductDataTableFilterPanelDto> dataTableFilterModel)
    {
        var result = _productService.GetProductDataTablePanel(dataTableFilterModel);
        return this.FromResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductById(long id)
    {
        var result = await _productService.GetProductForUpdate(id);
        return this.FromResult(result);
    }

    [HttpPost] // Excel bulk insert
    public async Task<IActionResult> ProductExcelInsert(ExcelProductParseRequestModel excelProduct)
    {
        var result = await _productService.CreateNewProductExcel(excelProduct);
        return this.FromResult(new SuccessResult<ExcelProductParseResponseModel>(result.Data));
    }
}
```

### MenuController.cs

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
public class MenuController : ControllerBase
{
    [HttpPost]
    public IActionResult List(DataTableFilterModel<MenuDataTableFilterDto> menuFilterDto)
    {
        var result = _menuService.GetMenuForDataTablePanel(menuFilterDto);
        return this.FromResult(result);
    }

    [HttpGet] // Menü güncelleme formu verisi
    public async Task<IActionResult> MenuUpdateForm(long placeId, long menuId)
    {
        var result = await _menuService.GetMenuUpdateFormData(menuId);
        return this.FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddMenu(MenuDto dto)
    {
        var result = await _menuService.CreateNewMenu(dto);
        return this.FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddMenuProduct(MenuProductInsertDto dto)
    {
        var result = await _menuService.CreateNewMenuProduct(dto);
        return this.FromResult(result);
    }

    [HttpPost]
    public IActionResult GetListMenuProduct(MenuProductFilterDto menuProductFilterDto)
    {
        var result = _menuService.GetListMenuProduct(menuProductFilterDto);
        return this.FromResult(result);
    }
}
```

### PlaceLanguageController.cs

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
public class PlaceLanguageController : Controller // ControllerBase yerine Controller
{
    [HttpGet] public IActionResult GetPlaceLanguageList(long id)
    {
        var result = _placeLanguageService.GetPlaceLanguageByPlaceId(id);
        return this.FromResult(result);
    }

    [HttpGet] public IActionResult GetPlaceLanguageWithContentLanguageList(long id)
    {
        var result = _placeLanguageService.GetPlaceLanguageByPlaceIdWithContentLanguage(id);
        return this.FromResult(result);
    }

    [HttpPost] public async Task<IActionResult> SetDefaultPlaceLanguage(PlaceSetDefaultLanguageDto dto)
    {
        var result = await _placeLanguageService.SetDefaultPlaceLanguage(dto);
        return this.FromResult(result);
    }

    [HttpGet] public IActionResult ContenLanguageListNotInPlace(long id)
    {
        var result = _contentLanguageService.GetContenLanguageNotInPlace(id);
        return this.FromResult(result);
    }

    [HttpPost] [ValidateFilter]
    public async Task<IActionResult> AddPlaceLanguage(PlaceLanguageDto placeLanguageDto)
    {
        var result = await _placeLanguageService.createNewPlaceLanguage(placeLanguageDto);
        return this.FromResult(result);
    }

    [HttpPost] public async Task<IActionResult> RemovePlaceLanguage(List<PlaceLanguageDto> placeLanguageDto)
    {
        var result = await _placeLanguageService.deletePlaceLanguage(placeLanguageDto);
        return this.FromResult(result);
    }
}
```

### ContentLanguageController.cs

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
public class ContentLanguageController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
    [SwaggerOperation(OperationId = "List")]
    public IActionResult List()
    {
        var result = _contentLanguageService.GetAllContentLanguageDto();
        return this.FromResult(result);
    }
}
```

### TestEventPubllishController.cs

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
public class TestEventPubllishController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    [HttpGet]
    public async Task<IActionResult> RabbitMqTest(string testName)
    {
        await _publishEndpoint.Publish<TestEvent>(new TestEvent { Name = testName });
        return Ok(1);
    }

    [HttpGet]
    public async Task<IActionResult> RabbitMqTestException(string exceptionMessage,
        string exceptionName, string actionName, string controllerName)
    {
        await _publishEndpoint.Publish<TestEvent>(new TestEvent { Name = actionName });
        await _publishEndpoint.Publish<ExceptionEvent>(new ExceptionEvent
        {
            ExceptionMessage = exceptionMessage,
            ActionName = actionName,
            ControllerName = controllerName,
            ExceptionName = exceptionName
        });
        return Ok(1);
    }

    [HttpPost]
    public async Task<IActionResult> RabbitMqNotificationTest(NotificationEvent notificationEvent)
    {
        await _publishEndpoint.Publish<NotificationEvent>(notificationEvent);
        return Ok("success");
    }
}
```

---

## 12. API — Startup.cs (tam kod)

```csharp
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
        : base(configuration, env)
    {
        base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        // ProjectPrefix = "PixdinnCrm" → AutoMapper assembly taraması için kullanılır
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // 1. MSSQL DbContext + Migration Assembly
        var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
        dbcontextOptions.Add(GetDbContextOption(
            FrameworkCore.Enums.UsingDbType.MSSQL,
            GetAppSettingValue("ConnectionStrings:PixdinnConnectionStringTest"),
            GetAppSettingValue("StartupConfigs:MigrationAssembly")));
        services.AddPixdinnDbService<PixdinnCrmDbContext>(dbcontextOptions);

        // 2. API versiyonlama + Swagger + ProblemDetails + CORS + FluentValidation
        services.AddPixdinnApiService(Configuration, WebHostEnvironment,
            GetAppSettingValue("StartupConfigs:Policy"),
            GetAppSettingValue("StartupConfigs:ApiUrl"));

        // 3. AutoMapper (ProjectPrefix ile *.dll tarama)
        services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);

        // 4. Framework Singleton servisleri:
        // IAuthorizeService, IAuthonticationService, ILoggerService,
        // IUtilService, ICustomHttpUtilService, IFileApiWebRequest
        services.AddFrameworkServices();

        // 5. FluentValidation kayıtları
        services.AddFluentValidators();
        // Kayıtlılar: ProductInsertDto→ProductUpdateRequestDtoValidator,
        //             PlaceUpdateRequestDto→PlaceValidator,
        //             PlaceLanguageDto→PlaceLanguageValidator,
        //             CategoryAddDto→CategoryAddValidator

        // 6. MassTransit + RabbitMQ (sadece Publisher, Consumer YOK)
        services.AddRabbitMqServices(new RabbitMqConfigModel()
        {
            HostAddress = GetAppSettingValue("EventBusSettings:HostAddress")
        });

        // 7. Refit typed HTTP client → FileAPI
        services.AddRefitClient<IRefitService>(new RefitSettings { CollectionFormat = CollectionFormat.Csv })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(GetAppSettingValue("FileApi:Url")));

        // 8. JSON Enum değerleri string olarak serialize edilir
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    // Autofac container: Tüm repository ve service'ler assembly tarama ile kayıt
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureRepositories(ApiOptions); // Persistence assembly
        builder.ConfigureServices(ApiOptions);     // Application assembly
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env); // CORS + Routing + DevException
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseSignalRBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions); // ProblemDetails middleware
    }
}

// Program.cs
public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory()) // Autofac DI
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}
```

---

## 13. Infrastructure (FileAPI + TokenService)

### FileAPI — Dosya Depolama Microservice

```csharp
// FileAPI/Controllers/FileController.cs
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IPublishEndpoint _publishEndpoint;

    // 1. Resim yükleme: byte[] alır → dosya sistemine yazar → FileKey (GUID) döner
    [HttpPost]
    public async Task<IActionResult> UploadPicture([FromBody] UploadFileRequest uploadFileRequest)
    {
        var response = await _fileService.UploadPicture(uploadFileRequest);
        return this.FromResult(response);
    }

    // 2. Excel yükleme: IFormFile → parse
    [HttpPost]
    public async Task<IActionResult> LoadProductInsertExcel(IFormFile file)
    {
        var response = await _fileService.UploadExcel(file);
        return this.FromResult(response);
    }

    // 3. Excel parse → RabbitMQ publish → Consumer async işler
    [HttpPost]
    public IActionResult ParseProductInsertExcel(ParseProductInsertExcelRequestDto req)
    {
        req.FilePath = _fileService.GetExcelFilePath(req.GuiId).Remove(0, 1);
        _publishEndpoint.Publish<ParseProductInsertExcelRequestDto>(req);
        return this.FromResult(new SuccessResult<bool>(true));
    }
}

// FileAPI Startup özeti:
// - PixdinnCrmFileDbContext (ayrı DB)
// - ExcelProductInsertConsumer (RabbitMQ consumer) kayıtlı
// - StaticFiles → /Uploaded_Files path ile dosya sunumu
// - PrefetchCount = 1 (tek seferde 1 mesaj işleme)
```

### TokenService — JWT Auth Microservice

```csharp
// Domain/Entities/AppUser.cs
public class AppUser : IdentityUser<long>
{
    // ASP.NET Identity standard özellikleri
}

// Domain/Entities/UserRefreshToken.cs — refresh token entity
// Domain/Interfaces/Services/IAuthenticationService.cs
public interface IAuthenticationService
{
    Task<Result<TokenDto>> CreateTokenAsync(LoginDto loginDto);
    Task<Result<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
    Task<Result<bool>> RevokeRefreshToken(string refreshToken);
}

// Presentation/Controllers/AuthController.cs — login/refresh endpoint'leri
// Presentation/Controllers/UserController.cs — kullanıcı yönetimi
```

### IRefitFileService (HTTP Client Interface)

```csharp
public interface IRefitService
{
    [Post("/File/UploadPicture")]
    Task<UploadFileResponse> UploadPicture([Body] UploadFileRequest request);
}

public class UploadFileRequest
{
    public byte[] File { get; set; }
    public string FileName { get; set; }
    public List<GroupData> GroupList { get; set; }

    public class GroupData
    {
        public string GroupKey { get; set; }
        public string GroupValue { get; set; }
    }
}

public class UploadFileResponse
{
    public string FileKey { get; set; }  // GUID — entity'lerde MediaGuiId olarak saklanır
}
```

---

## 14. appsettings yapısı

```json
{
  "ConnectionStrings": {
    "PixdinnConnectionStringTest":    "Server=213.159.1.3;Database=PixdinnCrm;User Id=pixdinn_crm;Password=***;...",
    "PixdinnConnectionStringAliLocal": "Server=DESKTOP-QSS6NQF\\MSSQLSERVERALI;Database=PixdinnCrm;...",
    "PixdinnConnectionStringMuammerLocal": "Server=.;Database=Pix;User Id=sa;Password=1;..."
  },
  "FileApi": {
    "Url": "https://localhost:44386/v1/",
    "Upload": "File/UploadFile"
  },
  "EventBusSettings": {
    "HostAddress": "amqp://guest:guest@localhost:5672"
  },
  "StartupConfigs": {
    "ProjectPrefix": "PixdinnCrm",          // AutoMapper + Autofac assembly taraması için
    "Policy": "_myAllowSpecificOrigins",
    "ApiUrl": "https://localhost:44374",
    "MigrationAssembly": "PixdinnCrm.Persistence",
    "AllowAnonymous": false,
    "ApiName": "PixdinnCrm"
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft": "Warning" }
  },
  "AllowedHosts": "*"
}
```

**Kritik yapılandırmalar:**
- `ProjectPrefix`: `BaseStartup` içinde dll taraması için kullanılır (`PixdinnCrm*.dll`)
- `MigrationAssembly`: EF Core migration'ları ayrı Persistence projesinde
- `FileApi:Url`: Refit client base URL (versiyonlu: `/v1/`)
- `EventBusSettings:HostAddress`: RabbitMQ AMQP URL
- `AllowAnonymous: false`: auth aktif (ancak JWT middleware Startup'ta kayıtlı değil — geliştirilme aşamasında)

---

## 15. docker-compose

```yaml
# docker-compose.yml
version: '3.4'
services:
  pixdinncrm.api:
    build: { context: ., dockerfile: API/Dockerfile }

  pixdinncrmlogservice.api:
    build: { context: ., dockerfile: PixdinnCrmLogService.API/Dockerfile }

  pixdinncrmnotificationservice.api:
    build: { context: ., dockerfile: PixdinnCrmNotificationService.API/Dockerfile }

  pixdinncrmFile.api:
    build: { context: ., dockerfile: FileAPI/Dockerfile }

  rabbitmq:
    image: rabbitmq:3-management-alpine

  logdb:
    image: mongo

volumes:
  mongo_data:

# docker-compose.override.yml — port mappings ve env vars
# pixdinncrm.api              → 8001:80
# pixdinncrmlogservice.api    → 8002:80
# pixdinncrmnotificationservice.api → 8003:80
# pixdinncrmFile.api          → 8004:80
# rabbitmq                    → 5672:5672 + 15672:15672 (management)
# logdb (MongoDB)             → 27017:27017

# Her servis için env vars override.yml'de:
# - ASPNETCORE_ENVIRONMENT=Development
# - ConnectionStrings:PixdinnConnectionStringSaid=Server=213.159.1.3;...
# - EventBusSettings:HostAddress=amqp://guest:guest@rabbitmq:5672
# - StartupConfigs:ProjectPrefix=PixdinnCrm (servis adına göre değişir)
```

---

## 16. Mimari Kararlar

### 1. Clean Architecture (katmanlı mimari)
Domain → Application → Persistence → API katmanları. Domain'in hiçbir dışa bağımlılığı yoktur. Dependency Inversion Principle katı biçimde uygulanır.

### 2. Custom FrameworkCore (Shared Library)
Tüm projeler aynı FrameworkCore'u paylaşır: `BaseEntity`, `BaseRepository`, `BaseService`, `BaseStartup`, `UnitOfWork`, `IRepository<T>`, `Result<T>`, `DataTableFilterModel<T>`. Bu sayede tüm servisler aynı convention'ı izler.

### 3. Repository Pattern — IQueryable Döndürme
Repository'ler `IQueryable<T>` döndürür, `ToList()` / `FirstOrDefault()` çağrısı servis katmanında yapılır. Bu, JOIN ve filtre kompozisyonunu servis katmanında LINQ ile mümkün kılar.

### 4. Soft Delete Pattern
`BaseEntity.Deleted = false` varsayılan. `DeleteWithStateRange` fiziksel silme yapmaz, `Deleted = true` set eder ve `Update` çağırır.

### 5. Partial Update (UpdateWithProperties)
EF Core `Entry.State = Unchanged + Property.IsModified = true` pattern'i ile sadece belirtilen kolonlar güncellenir. Gereksiz kolon güncellemesini engeller.

### 6. Expression Predicate Chaining (And Extension)
```csharp
Expression<Func<Place, bool>> predicate = s => s.Deleted == false;
if (!string.IsNullOrEmpty(filter.PlaceName))
    predicate = predicate.And(s => s.Name.Contains(filter.PlaceName));
```
`LinqExpression.And()` extension metodu ile filtreler tip-güvenli zincir oluşturur.

### 7. DataTable Pattern
Tüm liste endpoint'leri `DataTableFilterModel<TFilter>` alır (sayfa no, kayıt/sayfa, sıralama yönü, sıralama alanı + domain filtresi) ve `DataTableResponseWrapper<TDto>` (toplam kayıt + sayfalı liste) döner.

### 8. Dinamik Sıralama (ApplySorting)
String property adından Expression.Lambda oluşturarak `OrderBy` / `OrderByDescending` uygular. Nokta notasyonu destekler (`"Product.Price"`).

### 9. GUID Tabanlı Medya Referansı
Entity'lerde `MediaGuiId` (string GUID) ile FileAPI'ye gevşek bağ. FK olmadığından join yapılamaz ama servis bağımsızlığı sağlar.

### 10. Otofac + Assembly Tarama
`ConfigureRepositories` ve `ConfigureServices` metodları Autofac'ta convention-based kayıt yapar. `ProjectPrefix` ile başlayan dll'ler taranır.

### 11. Event-Driven Excel Import
Excel parse → RabbitMQ publish → Consumer async işleme pipeline'ı. Büyük veri setleri için senkron bloklama olmaz.

### 12. Microservice Ayrımı
Ana API yalnızca domain logic içerir. Dosya işleme (FileAPI), loglama (LogService/MongoDB), bildirim (NotificationService), auth (TokenService) ayrı servisler.

---

## 17. Dikkat Noktaları

### KURAL İHLALLERİ

**1. Boş Exception Catch — Kritik Bug**
```csharp
// ProductService.UpdateProduct
try { ... }
catch (Exception ee)
{
    // HATA YUTULUYOR — her zaman başarı döner!
}
return new SuccessResult<bool>(true);
```

**2. Hatalı Validator Kuralı**
```csharp
// CategoryAddValidator.cs
RuleFor(i => i.PlaceId).Equal(0) // YanlIş! PlaceId sıfır olamaz
    .WithMessageNormal(CategoryErrorEnum.CATEGORY_ID_ERROR);
// Doğrusu: .NotEqual(0) olmalı
```

**3. Debug Kalıntıları (Production'da kaldırılmalı)**
```csharp
var query = queryJoinedTables.ToQueryString(); // Service metotlarında
var cc = menuProductQuery.ToQueryString();     // Debug için bırakılmış
optionsBuilder.LogTo(Console.WriteLine);       // UnitOfWork — tüm SQL'ler console'a
```

**4. Hardcoded Refit Client (DI Anti-Pattern)**
```csharp
// PlaceController, CategoryController, ProductController içinde:
IRefitService refitFile = RestService.For<IRefitService>(fileApiUrl); // DI yerine runtime oluşturuluyor
// IRefitService DI'ye kayıtlı olmasına rağmen RestService.For kullanılıyor
```

**5. Ölü Kod (MediaFile DbSet yorum satırında)**
```csharp
// PixdinnCrmDbContext.cs
//public DbSet<MediaFile> MediaFile { get; set; }
// MediaFile entity var ama DbContext'e kayıtlı değil — kullanılamaz
```

**6. Naming Convention İhlali**
```csharp
// IPlaceLanguageService metodları küçük harfle başlıyor:
Task<Result<bool>> createNewPlaceLanguage(PlaceLanguageDto placeLanguageDto); // C# PascalCase olmalı
Task<Result<bool>> deletePlaceLanguage(List<PlaceLanguageDto> placeLanguageDto);
```

**7. N+1 Sorgu Riski (Pagination)**
```csharp
// GetPlacesForDataTable:
int toplamKayit = placesEntityFilter.Count();   // 1. DB sorgusu (COUNT)
var placesEntity = placesEntityFilter.Skip(ofset).Take(recordPerPage).ToList(); // 2. DB sorgusu (SELECT)
// Total = 2 DB sorgusu — büyük tablolarda index kullanımına dikkat
```

**8. Nullable Uyumsuzluğu (Dosya Ortasında)**
```csharp
// MenuProduct.cs
#nullable enable
public long? ParentCategoryId { get; set; } // Nullable
public MenuProduct? ParentCategory { get; set; }
#nullable disable
public int Order { get; set; } // Nullable bağlamı kapatıldı
```

**9. Controller Türü Tutarsızlığı**
```csharp
// PlaceLanguageController Controller'dan türüyor (View desteği var)
public class PlaceLanguageController : Controller // Controller ← View için

// Diğerleri ControllerBase'den (sadece API)
public class PlaceController : ControllerBase
```

**10. MenuService Çift Bağımlılık**
`MenuService` içinde hem `IProductService` hem `ICategoryService` enjekte edilmiş. `MenuController` ise hem `IMenuService` hem `IProductService` enjekte alıyor. Service'ler arası bağımlılık döngüsü riski.

**11. Credential appsettings.json'da**
```json
"PixdinnConnectionStringTest": "Server=213.159.1.3;...Password=AliMuammer.!299;..."
// Şifreler kaynak kodunda — environment variable veya secret manager kullanılmalı
```

**12. .NET 5 (EOL)**
.NET 5, Mayıs 2022'de destek süresi doldu. .NET 8 LTS'ye geçiş gerekli.

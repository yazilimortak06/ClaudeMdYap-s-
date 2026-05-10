# yonetim_panel_angular — Geliştirme Kuralları

Bu dosya, gerçek projeden çıkarılan, yeni admin panel geliştirirken uygulanması gereken kuralları ve code pattern'lerini içerir.

---

## Kural 1: APP_INITIALIZER ile Oturum Geri Yükleme

Sayfa yenilemede oturumu korumak için `APP_INITIALIZER` kullan:

```typescript
// app.module.ts
function appInitializer(authService: AuthenticationService) {
  return () => {
    return new Promise<void>((resolve, reject) => {
      authService.getAdminByToken().subscribe().add(resolve);
    });
  };
}

providers: [
  {
    provide: APP_INITIALIZER,
    useFactory: appInitializer,
    multi: true,
    deps: [AuthenticationService],
  },
]
```

**Önemli:** `add(resolve)` ile subscription'ın tamamlandığında uygulama başlaması sağlanır. Observable hata verse bile uygulama başlamalı — bu yüzden `reject` kullanılmıyor.

---

## Kural 2: PrepareForm Pattern — Form Açılmadan Önce Backend'den Veri Al

Ekleme/güncelleme formlarını backend'den veriyle doldurmak için `PrepareForm` endpoint pattern'i kullan:

```typescript
// Login için:
prepareLoginForm() {
  this.srvProgressSpinner.show();
  this.srvAuthService.prepareLoginForm(this.prepareLoginFormRequestModel)
    .subscribe((response: PrepareLoginFormResponseModel) => {
      this.loginRequestModel.loginFormKey = response.loginFormKey;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorPrepareLoginForm(error);
    });
}

// Ürün ekleme için:
prepareInsertForm() {
  this.srvProgressSpinner.show();
  this.srvProduct.prepareProductInsertForm(this.prepareProductInsertFormRequestModel)
    .subscribe((response: PrepareProductInsertFormResponseModel) => {
      this.prepareProductInsertFormResponseModel = response;
      // Form için gerekli dropdown listelerini, varsayılan değerleri doldur
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.dialogRef.close();
      this.onErrorPrepareInsertForm(error);
    });
}
```

**Kural:** Her form bileşeni `ngOnInit()` içinde `prepareInsertForm()` veya `prepareUpdateForm()` çağırarak backend'den gerekli dropdown/select listelerini ve varsayılan değerleri alır.

---

## Kural 3: Datatable — DatatableRequestWrapper Generic Pattern

Tüm liste servis çağrıları aynı generic wrapper ile yapılır:

```typescript
// Request wrapper
searchCriterBusiness: DatatableRequestWrapper<BusinessFilterModel> = new DatatableRequestWrapper();

// İlk yüklemede:
ngOnInit() {
  this.searchCriterBusiness.data = new BusinessFilterModel();
  this.getBusinesss(false);
}

// Liste çekme:
getBusinesss(hasFilter: boolean) {
  this.businessList.data = [];
  this.srvProgressSpinner.show();

  if (hasFilter) { this.paginatorBusinessTable.pageIndex = 0; }

  // Sıralama ayarı
  if (this.sortBusinessTable && this.sortBusinessTable.direction != "" && this.sortBusinessTable.active != "") {
    this.searchCriterBusiness.orderDirective = this.sortBusinessTable.direction;
    this.searchCriterBusiness.orderProperty = this.sortBusinessTable.active;
  } else {
    this.searchCriterBusiness.orderDirective = "desc";
    this.searchCriterBusiness.orderProperty = "Id";
  }

  // Pagination ayarı
  if (this.paginatorBusinessTable) {
    this.searchCriterBusiness.recordPerPage = this.paginatorBusinessTable.pageSize;
    this.searchCriterBusiness.pageNumber = this.paginatorBusinessTable.pageIndex;
  } else {
    this.searchCriterBusiness.recordPerPage = 15;  // varsayılan
  }

  this.srvBusinessService.list(this.searchCriterBusiness)
    .subscribe((response: DatatableResponseWrapper<BusinessListModel[]>) => {
      this.businessList.data = response.data;
      this.totalRecordSizeBusinessTable = response.recordCount;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorBusinessList(error);
    });
}

// Sort + Paginator değişimlerini dinle:
ngAfterViewInit(): void {
  this.sortBusinessTable.sortChange.subscribe(() => {
    this.paginatorBusinessTable.pageIndex = 0;
  });
  merge(this.sortBusinessTable.sortChange, this.paginatorBusinessTable.page)
    .pipe(tap(() => { this.getBusinesss(false); }))
    .subscribe(() => { }, () => null);
}
```

---

## Kural 4: Dialog-Based CRUD Pattern

Ekleme ve güncelleme işlemleri route değil, `MatDialog` ile yapılır:

```typescript
// Ekleme dialogu aç:
onNewBtnClickedDialog(): void {
  const dialogRef = this.dialog.open(AddBusinessComponent, {
    width: '1200px',
    panelClass: 'form-modal',
    data: {},
  });
  // Dialog içinden EventEmitter ile listeyi yenile:
  const subAddEvent = dialogRef.componentInstance.onAdd.subscribe(() => {
    this.getBusinesss(false);
  });
  dialogRef.afterClosed().subscribe(result => {
    subAddEvent.unsubscribe();  // Memory leak önleme
  });
}

// Güncelleme dialogu aç:
onEditBtnClickedDialog(id): void {
  const dialogRef = this.dialog.open(UpdateBusinessComponent, {
    width: '1200px',
    panelClass: 'form-modal',
    data: { id: id },
  });
  const subUpdateEvent = dialogRef.componentInstance.onUpdate.subscribe(() => {
    this.getBusinesss(false);
  });
  dialogRef.afterClosed().subscribe(result => {
    subUpdateEvent.unsubscribe();
  });
}

// Dialog bileşeninde EventEmitter tanımla:
@Component({ ... })
export class AddBusinessComponent {
  onAdd = new EventEmitter();
  // ...
  save() {
    // ...
    this.srvBusinessService.add(request).subscribe((response) => {
      this.onAdd.emit();
      this.dialogRef.close();
    });
  }
}
```

---

## Kural 5: Onaylama Dialogu Pattern (EvetHayir)

Silme ve durum değiştirme işlemlerinde onaylama dialogu kullan:

```typescript
removeBusinessDialog(id) {
  const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
    width: '300px',
    data: {
      title: "",
      description: "İşletmeyi kaldırmak istediğinize emin misiniz?"
    },
  });
  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.removeBusiness(id);
    }
  });
}

// MatSlideToggle ile durum değiştirme — toggle geri al:
changeActiveStateDialog(id, isActive, ob: MatSlideToggleChange) {
  const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
    width: '300px',
    data: { description: "Durumu değiştirmek istediğinize emin misiniz?" },
  });
  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.changeActiveState(id, isActive, ob);
    } else {
      ob.source.checked = isActive;  // Kullanıcı iptal ederse toggle'ı geri al
    }
  });
}
```

---

## Kural 6: SubheaderService ile Breadcrumb

Her list component `ngOnInit()` içinde breadcrumb günceller:

```typescript
constructor(private subheader: SubHeaderService) {}

ngOnInit(): void {
  this.subheader.setBreadcrumbs([
    { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
    { title: 'İşletme Yönetimi', linkPath: null, linkText: '', isActive: false },
    { title: 'İşletmeler', linkPath: null, linkText: '', isActive: false },
  ]);
  // ...
}
```

**Kural:** Son eleman `isActive: false`, ilk eleman (Ana Sayfa) `isActive: true` ve `linkPath: ''`.

---

## Kural 7: BehaviorSubject Auth State Pattern

Auth state BehaviorSubject ile yönetilmeli:

```typescript
@Injectable()
export class AuthenticationService {
  public adminSubject: BehaviorSubject<AuthenticationModel> = new BehaviorSubject<AuthenticationModel>(null);
  isLoadingSubject: BehaviorSubject<boolean>;

  // Anlık değer okuma:
  public get adminValue(): AuthenticationModel {
    return this.adminSubject.value;
  }

  // Token ile geri yükleme:
  getAdminByToken(): Observable<AuthenticationModel> {
    this.isLoadingSubject.next(true);
    return this.getAuthFromLocalStorage().pipe(
      map((admin: AuthenticationModel) => {
        if (admin) {
          this.adminSubject = new BehaviorSubject<AuthenticationModel>(admin);
        } else {
          this.logout();
        }
        return admin;
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  // Login sonrası güncelleme:
  this.srvAuthService.adminSubject = new BehaviorSubject<AuthenticationModel>(authenticationModel);
}
```

---

## Kural 8: HTTP Interceptor Kayıt Biçimi

Interceptor `LayoutModule.providers[]`'a eklenmelidir (şu an bug var, providers'da yok):

```typescript
// layout.module.ts
import { httpEventInterceptorProvider } from './utils/interseptors/http-event-interseptor';

@NgModule({
  providers: [
    httpEventInterceptorProvider  // BU SATIR OLMALI — şu an eksik!
  ],
})
export class LayoutModule { }

// interseptor dosyasında export:
export const httpEventInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: HttpEventInterceptor,
  multi: true
};
```

---

## Kural 9: Servis URL Yapısı — Environment + Endpoint

```typescript
@Injectable()
export class BusinessService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "BusinessManagment/";
    // Sonuç: "http://localhost:40543/v1/BusinessManagment/"
  }

  list(request): Observable<...> {
    return this.http.post(this.srvUrl + "GetBusinessDatatablePanel", request);
    // Son URL: "http://localhost:40543/v1/BusinessManagment/GetBusinessDatatablePanel"
  }
}
```

**Pattern:** Tüm endpoint metot isimleri PascalCase, RESTful değil action-based (GetBusinessDatatablePanel, AddBusiness, UpdateBusiness, RemoveBusiness, ChangeStateBusiness).

---

## Kural 10: addProductRequestModel — Nested Array'leri Constructor'da İnit Et

```typescript
constructor(... @Inject(MAT_DIALOG_DATA) public data: any) {
  // Array alanları constructor'da başlatılmalı:
  this.addProductRequestModel.productCategory = [];
  this.addProductRequestModel.productContent = [];
  this.addProductRequestModel.productPrice = [];
  this.addProductRequestModel.allergenProduct = [];
  this.addProductRequestModel.tagProduct = [];
  this.addProductRequestModel.isActive = true;  // Varsayılan true
}
```

---

## Kural 11: UtilsService ile Hata Mesajı Gösterme

```typescript
onErrorBusinessList(error: any) {
  let errorData = this.srvUtils.getServerErrorRequest(error);
  this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
}

// Başarı için:
this.srvUtils.showActionNotification("Başarıyla Kaldırıldı", EnumMessageType.Success, 2000);
this.srvUtils.showActionNotification("Başarıyla Güncellendi", EnumMessageType.Success, 2000);
this.srvUtils.showActionNotification("Başarıyla Eklendi", EnumMessageType.Success, 8000);

// Validasyon hatası için:
this.srvUtils.showActionNotification("Gerekli Alanları Doldurun", EnumMessageType.Error, 4000);
```

**EnumMessageType değerleri:** `Error`, `Success`, `Warning`, `Info`

---

## Kural 12: PanelAdminUserType ile Koşullu Menü

AsideComponent ve diğer template'lerde kullanıcı tipine göre menü filtreleme:

```typescript
// Enum tanımı:
export enum PanelAdminUserType {
  ADMIN_USER = "ADMIN_USER",
  PLACE_ADMIN_USER = "PLACE_ADMIN_USER",
  ROOT_ADMIN_USER = "ROOT_ADMIN_USER"
}

// Aside component'de:
authenticationData: AuthenticationModel = new AuthenticationModel();
panelAdminUserType = PanelAdminUserType;

constructor(...) {
  this.authenticationData = JSON.parse(localStorage.getItem("auth_token"));
}
```

```html
<!-- Template'de koşullu menü: -->
<li *ngIf="authenticationData?.panelAdminUserType === panelAdminUserType.ROOT_ADMIN_USER">
  <!-- Sadece ROOT_ADMIN_USER görebilir -->
</li>
```

---

## Kural 13: FormGroup Validasyon Kontrolü

```typescript
isControlHasError(controlName: string, validationType: string): boolean {
  const control = this.loginForm.controls[controlName];
  if (!control) { return false; }
  const result = control.hasError(validationType) && (control.dirty || control.touched);
  return result;
}
```

```html
<!-- Template'de kullanımı: -->
<div *ngIf="isControlHasError('userName', 'required')" class="invalid-feedback">
  Kullanıcı adı zorunludur.
</div>
```

---

## Kural 14: Stepper Formlar (Çok Adımlı Form)

Ürün ekleme gibi karmaşık formlarda Angular Material Stepper kullan:

```typescript
@ViewChild('stepper') stepper;
isLinear = false;
firstFormGroup: FormGroup;
secondFormGroup: FormGroup;

ngOnInit() {
  this.firstFormGroup = this._formBuilder.group({
    isActiveCtrl: ['',],
    productCodeCtrl: ['', Validators.required],
    categoryCtrl: ['', Validators.required],
  });
  this.secondFormGroup = this._formBuilder.group({
    cookingTimeCtrl: ['',],
    calorieCtrl: ['',],
  });
}

formNext() { this.stepper.next(); }
formPrevious() { this.stepper.previous(); }
```

---

## Kural 15: Enum Mapping — Ekranda Görüntüleme

Enum değerlerini Türkçe etiketlerle göstermek için mapping objesi kullan:

```typescript
export const adminManagmentTypeMapping: Record<keyof typeof AdminManagmentType, string> = {
  MAIN_ADMIN: "Genel Admin",
  BUSINESS_ADMIN: "İşletme Admin",
  MENU_ADMIN: "Menü Admin"
};
```

```typescript
// Component'de:
adminManagmentTypeMapping = adminManagmentTypeMapping;
adminManagmentTypes = Object.values(AdminManagmentType).filter(value => typeof value === 'string');
```

```html
<!-- Template'de: -->
<span>{{ adminManagmentTypeMapping[item.type] }}</span>
<!-- veya select için: -->
<mat-option *ngFor="let type of adminManagmentTypes" [value]="type">
  {{ adminManagmentTypeMapping[type] }}
</mat-option>
```

---

## Özet — Yeni Admin Panel Projesi İçin Checklist

| Konu | Yapılacak Eylem |
|---|---|
| Auth State | BehaviorSubject + localStorage pattern |
| APP_INITIALIZER | Token geri yükleme için ekle |
| PrepareForm | Her form bileşeni ngOnInit'te backend'den form datasını al |
| Datatable | DatatableRequestWrapper generic pattern + MatTable + MatPaginator + MatSort |
| CRUD Dialog | MatDialog + EventEmitter ile liste yenileme |
| Onaylama | EvetHayirDialog (silme + durum değiştirme) |
| Hata Mesajı | UtilsService.showActionNotification() |
| Breadcrumb | SubheaderService.setBreadcrumbs() |
| Interceptor | providers[]'a mutlaka ekle |
| console.log | Production'a geçmeden sil |
| Auth Guard | Observable dönen methodları await ile çöz |
| Logout | userLogout() çağrı koşulunu düzelt |
| Enum Mapping | Türkçe label için Record<> mapping objesi |
| Koşullu Menü | panelAdminUserType ile ngIf filtrele |

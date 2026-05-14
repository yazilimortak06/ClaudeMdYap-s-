# sarj_pro_panel_angular — Kurallar (rules.md)

Bu dosya, `sarj_pro_panel_angular` kaynak kodunun tam analizi sonucunda cikartilan
tasarim, guvenlik ve kod kalitesi kurallarini icerir. Her kural, projeden alinmis
somut TypeScript/HTML kodu ornekleri ile desteklenmektedir.

---

## Kural 1: AuthGuard'da Observable'i Truthy Kontrol Olarak Kullanmayın

**Sorun:** `getAuthFromLocalStorage()` bir `Observable<T>` döner. Observable nesnesi
her zaman truthy'dir; `of(undefined)` döndürse bile `if (observable)` ifadesi true
degerlenir.

**Kötü (bu projeden):**
```typescript
// auth.guard.ts
canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const currentUser = this.authService.adminValue;
    // getAuthFromLocalStorage() Observable doner; bu kontrol hicbir ise yaramaz
    if (currentUser && this.authService.getAuthFromLocalStorage()) {
        return true;
    }
    this.authService.logout();
    return false;
}
```

**Dogru:**
```typescript
canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const currentUser = this.authService.adminValue;
    if (!currentUser) {
        this.authService.logout();
        return of(false);
    }
    return this.authService.getAuthFromLocalStorage().pipe(
        map(auth => {
            if (auth) { return true; }
            this.authService.logout();
            return false;
        })
    );
}
```

---

## Kural 2: Sabit String ile Kendi Değerini Karşılaştıran if Bloğu Yazma

**Sorun:** `authLocalStorageToken` bir `private readonly` sabit: her zaman `"auth_token"`.
Bu sabitin kendisiyle `!= "auth_token"` karsilastirilmasi daima `false` üretir.
`userLogout()` API cağrısı ve refresh token iptali hicbir zaman calismaz.

**Kötü (bu projeden):**
```typescript
// authentication-service.ts
private authLocalStorageToken = "auth_token"; // Sabit string literal

logout() {
    // Bu kosul ASLA true olmaz: "auth_token" != "auth_token" === false
    if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
        this.userLogout().subscribe(auth => { });
    }
    localStorage.removeItem(this.authLocalStorageToken);
    this.router.navigate(['/auth/login']);
}
```

**Dogru:**
```typescript
logout() {
    // API'ye cikis bildirimi her zaman yapilir
    this.userLogout().subscribe({
        error: () => { /* Hata olsa da devam et */ }
    });
    localStorage.removeItem(this.authLocalStorageToken);
    localStorage.removeItem(this.authUserInfo);
    this.adminSubject.next(null);
    this.router.navigate(['/auth/login']);
}
```

---

## Kural 3: Şifreleri Client Tarafında MD5 ile Hashlemeyin

**Sorun:** MD5 kriptografik olarak kirilmis bir algoritmadir. Rainbow table ve collision
saldirilarina karsi savunmasizdir. Client tarafinda hash yapmanin ek bir güvenlik
faydasi yoktur; saldirgan MD5 hash degerini dogrudan gönderebilir.

**Kötü (bu projeden):**
```typescript
// panelAdmin-add.component.ts
import { Md5 } from 'ts-md5';

md5 = new Md5();

save() {
    // HATALI: Client MD5 hash -> sunucuya MD5 hash gonderiliyor
    // Saldirgan MD5 degerini bilirse dogrudan kimlik dogrulamadan gecer
    this.panelAdminInsertModel.password = this.md5.appendStr(this.password).end() + "";
    this.panelAdminInsertModel.passwordAgain = this.md5.appendStr(this.passwordAgain).end() + "";
    this.srvPanelAdmin.add(this.panelAdminInsertModel).subscribe(...);
}
```

**Dogru:**
```typescript
// Sifreyi plain-text HTTPS uzerinden gonder
// Sunucu tarafinda bcrypt veya argon2 kullan
save() {
    this.panelAdminInsertModel.password = this.password;
    this.panelAdminInsertModel.passwordAgain = this.passwordAgain;
    this.srvPanelAdmin.add(this.panelAdminInsertModel).subscribe(...);
}

// C# sunucu tarafinda:
// passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
```

---

## Kural 4: HTTP Interceptor'da console.log Bırakmayın

**Sorun:** Her HTTP isteğinde ve yanıtında `console.log` çalışıyor. Bu, production
ortamında performans kaybına, tarayıcı konsolunda gizli veri ifşasına ve debug
bilgilerinin kullanıcıya görünmesine neden olur.

**Kötü (bu projeden):**
```typescript
// http-event-interseptor.ts
intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authenticationModel: AuthenticationModel = JSON.parse(
        localStorage.getItem(this.authLocalStorageToken)
    );
    console.log(authenticationModel); // Her istekte token loglanıyor!

    return next.handle(request).pipe(
        tap((event: HttpEvent<any>) => {
            if (event instanceof HttpResponse) {
                console.log(event); // Her başarili yanit loglanıyor!
                this.srvProgressSpinner.hide();
            }
        })
    );
}
```

**Dogru:**
```typescript
// environment.ts kullanarak sadece development'ta log bas
intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authenticationModel: AuthenticationModel = JSON.parse(
        localStorage.getItem(this.authLocalStorageToken)
    );

    if (!environment.production) {
        console.log('[Interceptor] Auth:', authenticationModel?.token?.token ? 'present' : 'absent');
    }

    if (authenticationModel) {
        request = this.addTokenHeader(request, authenticationModel);
    }
    return next.handle(request);
}
```

---

## Kural 5: UtilsService ve Paylaşılan Servisleri Singleton Olarak Tanımlayın

**Sorun:** `UtilsService` üzerinde `@Injectable()` var ama `providedIn: 'root'` yok.
Her feature modülü `providers: [UtilsService]` ekliyor. Bu, her modülün kendi
bağımsız `UtilsService` instance'ını alması anlamına gelir. Snackbar ve dialog
durumu modüller arasında paylaşılmaz.

**Kötü (bu projeden):**
```typescript
// utils.service.ts
@Injectable() // providedIn eksik!
export class UtilsService { ... }

// panelAdmin-module.ts
@NgModule({
    providers: [
        UtilsService,  // Yeni instance olusturuluyor
        PanelAdminService
    ]
})
export class PanelAdminModule { }

// authority-module.ts  
@NgModule({
    providers: [
        UtilsService,  // Baska bir instance! Ayni servis degil
    ]
})
export class AuthorityModule { }
```

**Dogru:**
```typescript
// utils.service.ts
@Injectable({
    providedIn: 'root' // Uygulama genelinde tek instance
})
export class UtilsService { ... }

// Feature modüllerinde providers listesinden kaldir:
@NgModule({
    providers: [
        // UtilsService burada OLMAMALI
        PanelAdminService
    ]
})
export class PanelAdminModule { }
```

---

## Kural 6: Observable'ı İki Kez Çağırmayın, Sonucu Paylaşın

**Sorun:** `getAdminByToken()` icinde `getAuthFromLocalStorage()` iki kez cagriliyor.
Birincisi sadece null kontrolü için, ikincisi gerçek veriyi almak için. Bu gereksiz
iki localStorage erişimi anlamına gelir.

**Kötü (bu projeden):**
```typescript
// authentication-service.ts
getAdminByToken(): Observable<AuthenticationModel> {
    const auth = this.getAuthFromLocalStorage(); // 1. cagri - sonuc kullanilmiyor
    if (!auth) { // Observable hicbir zaman null degildir, bu kontrol is yapmaz
        return of(undefined);
    }
    this.isLoadingSubject.next(true);
    return this.getAuthFromLocalStorage().pipe( // 2. cagri - gercek veri buradan
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
```

**Dogru:**
```typescript
getAdminByToken(): Observable<AuthenticationModel> {
    this.isLoadingSubject.next(true);
    return this.getAuthFromLocalStorage().pipe(
        tap((admin: AuthenticationModel) => {
            if (admin) {
                this.adminSubject.next(admin); // new BehaviorSubject yerine next() kullan
            } else {
                this.logout();
            }
        }),
        finalize(() => this.isLoadingSubject.next(false))
    );
}
```

---

## Kural 7: BehaviorSubject'i null/undefined ile Karşılaştırmayın

**Sorun:** `BehaviorSubject` nesnesi `undefined` olamaz. Bunu `undefined` ile
karsilastirmak anlamsiz bir kontroldür ve her zaman true döner.

**Kötü (bu projeden):**
```typescript
// http-event-interseptor.ts
private addTokenHeader(request: HttpRequest<any>, authenticationModel: AuthenticationModel) {
    // adminSubject bir BehaviorSubject'tir, hicbir zaman undefined olamaz
    // Bu kontrol hicbir zaman false olmaz; else bloku calismaz
    if (this.authService.adminSubject !== undefined) {
        try {
            const headers = new HttpHeaders()
                .set('Authorization', 'Bearer ' + authenticationModel.token.token);
            return request = request.clone({ headers });
        } catch {
            this.authService.logout();
        }
    } else {
        // Bu blok ASLA calismaz
        const headers = new HttpHeaders()
            .set('Authorization', 'Bearer ' + authenticationModel.token.token);
        return request = request.clone({ headers });
    }
}
```

**Dogru:**
```typescript
private addTokenHeader(request: HttpRequest<any>, authenticationModel: AuthenticationModel) {
    if (authenticationModel?.token?.token) {
        try {
            const headers = new HttpHeaders()
                .set('Authorization', 'Bearer ' + authenticationModel.token.token);
            return request.clone({ headers });
        } catch {
            this.authService.logout();
            return request; // Klonlanmamis request doner
        }
    }
    return request;
}
```

---

## Kural 8: Model Sınıfına Metot Eklemeyin

**Sorun:** `AuthenticationModel` icinde `pipe()` metodu tanimlanmis. Model siniflari
sadece veri tutmalidir (property'ler). Davranissal metodlar (özellikle Observable
operator metodlari) modele ait olmamalidir.

**Kötü (bu projeden):**
```typescript
// authentication-model.ts
export class AuthenticationModel {
    // BUG: Model icinde Observable metodu - yanlis tasarim
    pipe(arg0: OperatorFunction<AuthenticationModel, AuthenticationModel>,
         arg1: MonoTypeOperatorFunction<unknown>): Observable<AuthenticationModel> {
        throw new Error("Method not implemented."); // Her zaman exception atar!
    }
    token: AccessTokenModel;
    name: string;
    surname: string;
    // ...
}
```

**Dogru:**
```typescript
// authentication-model.ts - Sadece veri
export class AuthenticationModel {
    token: AccessTokenModel;
    name: string;
    surname: string;
    phone: string;
    mail: string;
    companyName: string;
    connectionId: string;
    companyId: number;
    panelAdminUserType: PanelAdminUserType;
}
```

---

## Kural 9: ngOnInit İçinde Asenkron Değerleri Subscribe Olmadan Okumayın

**Sorun:** `subscribe()` callback asenkron çalışır. Aynı satırda subscribe'dan hemen
sonra değeri okumaya çalışmak, değer henüz atanmamış olduğu için `undefined` üretir.

**Kötü (bu projeden):**
```typescript
// user-partial.component.ts
ngOnInit(): void {
    const sb = this.srvAuthService.adminSubject.asObservable().pipe(
        first(admin => !!admin)
    ).subscribe(admin => {
        this.admin = Object.assign({}, admin); // Asenkron atama
    });
    this.subscriptions.push(sb);
    console.log(this.admin); // BUG: undefined - subscribe henuz resolve olmadi
}
```

**Dogru:**
```typescript
ngOnInit(): void {
    const sb = this.srvAuthService.adminSubject.asObservable().pipe(
        first(admin => !!admin)
    ).subscribe(admin => {
        this.admin = Object.assign({}, admin);
        console.log('Admin yuklendi:', this.admin); // Subscribe icinde kullan
    });
    this.subscriptions.push(sb);
}
```

---

## Kural 10: Promise Zinciri Yerine Observable Zinciri Kullanın

**Sorun:** Angular uygulamalarinda `Promise` zinciri (`then().then().then()`) yerine
RxJS Observable operatorleri (`switchMap`, `forkJoin`, `concatMap`) kullanilmalidir.
Observable'lar iptal edilebilir, hata yonetimi daha tutarlidir ve Angular'in
degisim algilama mekanizmasiyla daha iyi çalişir.

**Kötü (bu projeden):**
```typescript
// authority-list.component.ts
initAuthFormContent() {
    this.srvProgressSpinner.show();
    this.getAdminUserType().then(res => {
        this.getAuthorityList().then(res => {
            this.getAdminUserTypeAuthList().then(res => {
                this.srvProgressSpinner.hide();
            }).catch(err => { this.srvProgressSpinner.hide(); });
        }).catch(err => { this.srvProgressSpinner.hide(); });
    }).catch(err => { this.srvProgressSpinner.hide(); });
}
```

**Dogru:**
```typescript
initAuthFormContent() {
    this.srvProgressSpinner.show();
    this.srvAdminUserTypeAuth.getAdminUserType(this.adminUserTypeAuthorityFilterModel.id)
        .pipe(
            switchMap(userType => {
                this.adminUserTypeId = userType.id;
                return this.srvAuthority.listAuthGroupWithAuth();
            }),
            switchMap(authorityList => {
                this.authorityList = authorityList;
                return this.srvAdminUserTypeAuth.list(this.adminUserTypeAuthorityFilterModel);
            }),
            finalize(() => this.srvProgressSpinner.hide())
        )
        .subscribe({
            next: (authList) => { this.mapAuthorityList(authList); },
            error: (err) => { this.onErrorAuthorityList(err); }
        });
}
```

---

## Kural 11: HTTP PUT ve POST İşlemlerini Doğru Semantic ile Kullanın

**Sorun:** `ParameterService.add()` metodu aslında `UpdateParameterValue` endpoint'ini
çağırıyor. Metot ismi ile davranış tutarsız.

**Kötü (bu projeden):**
```typescript
// parameter-service.ts
// "add" isimli metot PUT/update semantiğini kullaniyor
add(entity: ParameterListModel[]): Observable<any> {
    return this.http.post<any>(this.srvUrl + "UpdateParameterValue", entity);
}
```

**Dogru:**
```typescript
// parameter-service.ts
// Semantigi yansitin: update
updateParameterValues(entity: ParameterListModel[]): Observable<any> {
    return this.http.put<any>(this.srvUrl + "UpdateParameterValue", entity);
}
```

---

## Kural 12: deprecated JavaScript API Kullanmayın (escape/unescape)

**Sorun:** `escape()` ve `unescape()` global JavaScript fonksiyonlari deprecated'dir
ve modern tarayicilarda kaldirilabilir. UTF-8 güvenli alternatifleri vardir.

**Kötü (bu projeden):**
```typescript
// policy-managment.component.ts
// Base64 decode - deprecated escape() kullaniyor
this.policyHtmlEditor.editor.setContent(
    decodeURIComponent(escape(window.atob(this.policyDetailModel.policyContent)))
);

// Base64 encode - deprecated unescape() kullaniyor
this.policyUpdateRequestModel.policyContent =
    btoa(unescape(encodeURIComponent(this.policyHtmlEditor.editordoc)));
```

**Dogru:**
```typescript
// TextDecoder/TextEncoder kullanan guvenli yontem
// Base64 decode
const bytes = Uint8Array.from(atob(this.policyDetailModel.policyContent), c => c.charCodeAt(0));
const decoded = new TextDecoder('utf-8').decode(bytes);
this.policyHtmlEditor.editor.setContent(decoded);

// Base64 encode
const encoded = new TextEncoder().encode(this.policyHtmlEditor.editordoc);
const base64 = btoa(String.fromCharCode(...encoded));
this.policyUpdateRequestModel.policyContent = base64;
```

---

## Kural 13: logout() Sonrası Sayfa Yenileme Yapma

**Sorun:** `document.location.reload()` gereksizdir. `router.navigate(['/auth/login'])`
zaten Angular'in routing sistemini temizler. Tam sayfa yenileme uygulamanin tum
durumunu sıfırlar ve kotu bir kullanici deneyimi yaratir.

**Kötü (bu projeden):**
```typescript
// user-partial.component.ts
logout() {
    this.srvAuthService.logout(); // Bu zaten /auth/login'e navigate ediyor
    document.location.reload(); // Gereksiz tam sayfa yenileme
}
```

**Dogru:**
```typescript
logout() {
    this.srvAuthService.logout(); // Router navigate zaten icerde yapiliyor
    // document.location.reload() OLMAMALI
}
```

---

## Kural 14: HTML Şablonunda Aynı Directive'i İki Kez Yazmayın

**Sorun:** Angular `formControlName` directive'i bir element'e iki kez yazilmis.
Ikinci tanim birincinin üzerine yazar, ancak bu hataya acik ve okunaksiz koddur.

**Kötü (bu projeden):**
```html
<!-- login.component.html:53 -->
<!-- formControlName iki kez yazilmis: once 'UserName' (gecersiz key), sonra 'userName' -->
<input [(ngModel)]="loginRequestModel.userName"
       formControlName="UserName"
       formControlName="userName"
       placeholder="Kullanici Adi" />
```

**Dogru:**
```html
<!-- FormGroup kullaniliyorsa [(ngModel)] ve formControlName birlikte kullanilmamali -->
<input formControlName="userName"
       placeholder="Kullanici Adi"
       autocomplete="off" />
```

---

## Kural 15: APP_INITIALIZER ile Uygulama Başlangıcında Auth Doğrulaması Yapın

Bu projede dogru uygulanan bir pattern. `APP_INITIALIZER` ile token kontrolünü
uygulama bootstrap asamasinda yapin. Bu, component'lara ulasmadan önce oturum
durumunun belirlenmesini saglar.

**Bu projeden (dogru kullanim):**
```typescript
// app.module.ts
function appInitializer(authService: AuthenticationService) {
    return () => {
        return new Promise<void>((resolve, reject) => {
            authService.getAdminByToken().subscribe().add(resolve);
        });
    };
}

@NgModule({
    providers: [
        {
            provide: APP_INITIALIZER,
            useFactory: appInitializer,
            multi: true,
            deps: [AuthenticationService],
        },
    ]
})
export class AppModule { }
```

---

## Kural 16: Layout Module'u Feature Routing'den Bağımsız Tutun

Bu projede `LayoutModule` ve `PagesRoutingModule` arasinda dogru bir ayrilik var.
`LayoutModule` sablonu (header, footer, aside) yonetir; `PagesRoutingModule` icerik
route'larini tanimlar.

**Bu projeden (dogru yapilanma):**
```typescript
// layout.module.ts
@NgModule({
    declarations: [
        LayoutComponent, // Kabuk component
        AsideComponent,
        HeaderComponent,
        FooterComponent,
        // ... diger layout componentleri
    ],
    imports: [
        PagesRoutingModule, // Feature route'lari ayri module'de
        SharedModule,
        CoreModule,
    ]
})
export class LayoutModule { }

// pages-routing.module.ts (ayri dosya)
const routes: Routes = [{
    path: '',
    component: LayoutComponent,
    children: [
        { path: 'panelAdmin', loadChildren: () => import(...).then(m => m.PanelAdminModule) },
        { path: 'authority', loadChildren: () => import(...).then(m => m.AuthorityModule) },
        // ...
    ]
}];
```

---

## Kural 17: Feature Modülü providers Listesini Minimize Edin

**Sorun:** Her feature modulu kendi `providers` listesinde servis tanimliyor.
Eger servisin singleton olmasi gerekiyorsa `providedIn: 'root'` kullanin.
Eger module-scoped olmasi gerekiyorsa bilinçli bir tercih olarak aciklayin.

**Bu projeden (problemli):**
```typescript
// Her modülde tekrar eden servis kayitlari
// authority-module.ts
providers: [ UtilsService, AuthorityService, PanelAdminTypeService, AdminUserTypeAuthService ]

// language-module.ts  
providers: [ UtilsService, ContentLanguageService ] // Baska bir UtilsService instance'i

// paramaters-module.ts
providers: [ UtilsService, ParameterService, ... ] // Bir tane daha
```

**Dogru yaklasim:**
```typescript
// utils.service.ts - Root'ta singleton
@Injectable({ providedIn: 'root' })
export class UtilsService { ... }

// Feature servisleri - root'ta veya modul'de olabilir, tutarli olsun
@Injectable({ providedIn: 'root' })
export class PanelAdminService { ... }

// Modul taniminda providers listesi kuculmeli:
@NgModule({
    providers: [] // Servisler root'ta tanimli ise buraya gerek yok
})
export class PanelAdminModule { }
```

---

## Kural 18: Global Event Nesnesini Servislerde Kullanmayın

**Sorun:** `UtilsService.helperDialog()` icinde global `event` nesnesi kullaniliyor.
Bu, strict TypeScript modunda hata verir ve fonksiyonun disiinda bir mouse/keyboard
event olmasi gerektiğini ima eder. Servis metodlari context'ten bagimsiz olmalidir.

**Kötü (bu projeden):**
```typescript
// utils.service.ts
helperDialog(description: string = '') {
    event.stopPropagation(); // Global 'event' - hangi event? Her zaman var mi?
    return this.dialog.open(YardimDialogComponent, {
        data: { description }, width: '440px'
    });
}
```

**Dogru:**
```typescript
// Caller event'i parametre olarak gonderir
helperDialog(description: string = '', event?: MouseEvent) {
    if (event) {
        event.stopPropagation();
    }
    return this.dialog.open(YardimDialogComponent, {
        data: { description }, width: '440px'
    });
}

// HTML'de:
// (click)="srvUtils.helperDialog('Yardim metni', $event)"
```

---

## Kural 19: DatatableRequestWrapper Generic Pattern Kullanın

Bu projede dogru uygulanan bir pattern. `DatatableRequestWrapperCore<T>` generic
sinifi, farkli filtreleme kriterleriyle server-side pagination destekler. Tum
liste componentleri bu pattern'i kullanmalidir.

**Bu projeden (dogru generic pattern):**
```typescript
// datatable-request-core-model.ts
export class DatatableRequestWrapperCore<T> {
    constructor() {
        this.data = {} as T;
    }
    data: T;             // Filtreleme kriterleri (generic)
    orderDirective: string; // 'asc' | 'desc'
    orderProperty: string;  // Siralama kolonu
    pageNumber: number;     // 0-bazli sayfa numarasi
    recordPerPage: number;  // Sayfa basina kayit
    offset: number;
}

// Kullanim (panelAdmin-list.component.ts):
searchCriterPanelAdmin: DatatableRequestWrapper<PanelAdminFilterModel> =
    new DatatableRequestWrapper();

this.searchCriterPanelAdmin.data = new PanelAdminFilterModel();
this.searchCriterPanelAdmin.orderDirective = "desc";
this.searchCriterPanelAdmin.orderProperty = "Id";
this.searchCriterPanelAdmin.recordPerPage = this.paginatorPanelAdminTable.pageSize;
this.searchCriterPanelAdmin.pageNumber = this.paginatorPanelAdminTable.pageIndex;
```

---

## Kural 20: Routing'e Eklenmemiş Modülleri Temizleyin veya Ekleyin

**Sorun:** `system-parameter-list` componenti ve modülü olusturulmus ama
`pages-routing.module.ts`'e eklenmemis. Bu, ölü kod durumu yaratir.

**Tespit (bu projeden):**
```
src/app/sarjAllPro/components/system-parameter/  <- Klasor var
  system-parameter-module.ts                      <- Modül var
  system-parameter-list/                          <- Component var
    system-parameter-list.component.ts

src/app/sarjAllPro/pages-routing.module.ts:
  // 'system-parameter' route'u YOK
  children: [
      { path: 'log', ... },
      { path: 'paramaters', ... },
      { path: 'authority', ... },
      { path: 'language', ... },
      { path: 'panelAdmin', ... },
      { path: 'policy', ... },
      // system-parameter EKSIK
  ]
```

**Dogru:**
```typescript
// pages-routing.module.ts
children: [
    { path: 'log', loadChildren: () => import('./components/log/log-module').then(m => m.LogModule) },
    { path: 'paramaters', loadChildren: ... },
    { path: 'authority', loadChildren: ... },
    { path: 'language', loadChildren: ... },
    { path: 'panelAdmin', loadChildren: ... },
    { path: 'policy', loadChildren: ... },
    // Ekle:
    {
        path: 'system-parameter',
        loadChildren: () =>
            import('./components/system-parameter/system-parameter-module')
                .then(m => m.SystemParameterModule)
    },
    { path: '**', redirectTo: 'error/404' },
]
```

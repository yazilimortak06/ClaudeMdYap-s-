// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\auth\auth.module.ts
import { NgModule } from "@angular/core";
import { AuthenticationService } from "src/app/shared_admin/auth/authentication-service";
import { AuthGuard } from "./auth.guard";

@NgModule({
    imports: [],
    declarations: [],
    exports: [],
    providers: [
        AuthenticationService,
        AuthGuard
    ]
})
export class AuthModule { }

// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\announcement\announcement.module.ts
import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { DropzoneSharedModule } from "src/app/core/external-components/dropzone-shared.module";
import { SharedModule } from "src/app/shared_admin/shared.module";
import { UtilsService } from "src/app/shared_admin/utils/services/utils.service";
import { AnnouncementService } from "../../services/announcement/announcement-service";
import { ContentLanguageService } from "../../services/contentLanguage/contentLanguage-service";
import { AddAnnouncementComponent } from "./announcement-add/announcement-add.component";
import { AnnouncementListComponent } from "./announcement-list/announcement-list.component";
import { UpdateAnnouncementComponent } from "./announcement-update/announcement-update.component";

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        DropzoneSharedModule,
        RouterModule.forChild([
            { path: 'list', component: AnnouncementListComponent },
            { path: 'add', component: AddAnnouncementComponent },
            { path: 'update', component: UpdateAnnouncementComponent },
        ])
    ],
    providers: [
        UtilsService,
        AnnouncementService,
        ContentLanguageService,
    ],
    declarations: [
        AddAnnouncementComponent,
        AnnouncementListComponent,
        UpdateAnnouncementComponent,
    ]
})
export class AnnouncementModule { }

// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\sarjAllPro\components\panelAdmin\panelAdmin-update\panelAdmin-update.component.ts

import { GoogleMapsAPIWrapper, MapsAPILoader, MouseEvent } from '@agm/core';
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { Subscription } from 'rxjs';
import { PanelAdminModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-model';
import { PanelAdminUpdateModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-update-model';
import { PanelAdminTypeModel } from 'src/app/sarjAllPro/models/panelAdminType/panelAdminType-model';
import { PanelAdminService } from 'src/app/sarjAllPro/services/panelAdmin/panelAdmin-service';
import { PanelAdminTypeService } from 'src/app/sarjAllPro/services/panelAdminType/panelAdminType-service';
import { ProgressSpinnerService } from "src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service";
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from "src/app/shared_admin/utils/services/utils.service";
import { environment } from "src/environments/environment";
import { Md5 } from 'ts-md5';

@Component({
  selector: 'app-panelAdmin-update',
  templateUrl: './panelAdmin-update.component.html',
  styleUrls: ['./panelAdmin-update.component.scss']
})
export class UpdatePanelAdminComponent implements OnInit {
  idSubscription: Subscription;
  isLinear = false;
  firstFormGroup: FormGroup;
  panelAdminModel: PanelAdminUpdateModel = new PanelAdminUpdateModel();
  panelAdminTypes: PanelAdminTypeModel[] = [];
  @ViewChild('stepper') stepper;
  md5 = new Md5();

  constructor(
    private _formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvPanelAdmin: PanelAdminService,
    private srvPanelAdminType: PanelAdminTypeService,
    private srvUtils: UtilsService,
    private subheader: SubHeaderService
  ) {
    this.panelAdminModel = new PanelAdminUpdateModel();
  }

  password: string;
  passwordAgain: string;

  ngAfterViewInit(): void { }

  ngOnInit() {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'Admin Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'Adminler', linkPath: '/panelAdmin/list', linkText: '', isActive: true },
      { title: 'Admin Güncelle', linkPath: null, linkText: '', isActive: false },
    ]);
    this.idSubscription = this.route.params.subscribe(
      params => {
        let id = params['id'];
        this.panelAdminModel.id = id;
        this.firstFormGroup = this._formBuilder.group({
          userNameCtrl: ['', Validators.required],
          nameCtrl: ['', Validators.required],
          surNameCtrl: ['', Validators.required],
          adminTypeCtrl: ['', Validators.required],
        });

        this.getPanelAdminTypes();
        this.getPanelAdmin(this.panelAdminModel.id);
      });
  }

  getPanelAdminTypes() {
    this.panelAdminTypes = [];
    this.srvProgressSpinner.show();
    this.srvPanelAdminType.getPanelAdminTypes().subscribe((response: PanelAdminTypeModel[]) => {
      this.panelAdminTypes = response;
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  getPanelAdmin(id) {
    this.srvProgressSpinner.show();
    this.srvPanelAdmin.getPanelAdminTypeById(id).subscribe(async (response: PanelAdminModel) => {
      this.panelAdminModel.userName = response.userName;
      this.panelAdminModel.mail = response.mail;
      this.panelAdminModel.name = response.name;
      this.panelAdminModel.panelAdminTypeId = response.panelAdminTypeId;
      this.panelAdminModel.phone = response.phone;
      this.panelAdminModel.surname = response.surname;
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  update() {
    this.srvProgressSpinner.show();
    this.srvPanelAdmin.update(this.panelAdminModel).subscribe((response: any) => {
      this.srvUtils.showActionNotification("Başarıyla Güncellendi", EnumMessageType.Success, 8000);
      setTimeout(() => {
        this.router.navigate(['panelAdmin/list']);
      }, 2500);
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }
}

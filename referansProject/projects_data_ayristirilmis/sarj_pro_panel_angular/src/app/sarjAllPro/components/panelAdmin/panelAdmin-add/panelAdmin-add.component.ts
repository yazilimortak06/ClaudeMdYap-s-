// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\sarjAllPro\components\panelAdmin\panelAdmin-add\panelAdmin-add.component.ts

import { GoogleMapsAPIWrapper, MapsAPILoader, MouseEvent } from '@agm/core';
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { adminManagmentTypeMapping, AdminManagmentType } from 'src/app/sarjAllPro/enums/authentication/panel-admin-user-type-enum';
import { PanelAdminInsertModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-insert-model';
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
  selector: 'app-panelAdmin-add',
  templateUrl: './panelAdmin-add.component.html',
  styleUrls: ['./panelAdmin-add.component.scss']
})
export class AddPanelAdminComponent implements OnInit {
  isLinear = false;
  firstFormGroup: FormGroup;
  panelAdminInsertModel: PanelAdminInsertModel = new PanelAdminInsertModel();
  panelAdminTypes: PanelAdminTypeModel[] = [];
  @ViewChild('stepper') stepper;
  md5 = new Md5();

  // Admin tipi enum
  adminManagmentTypeMapping = adminManagmentTypeMapping;
  adminManagmentTypes = Object.values(AdminManagmentType).filter(value => typeof value === 'string');
  adminManagmentType = AdminManagmentType;

  constructor(
    private _formBuilder: FormBuilder,
    private router: Router,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvPanelAdmin: PanelAdminService,
    private srvPanelAdminType: PanelAdminTypeService,
    private srvUtils: UtilsService,
    private subheader: SubHeaderService
  ) { }

  password: string;
  passwordAgain: string;

  ngAfterViewInit(): void { }

  ngOnInit() {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'Admin Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'Adminler', linkPath: '/panelAdmin/list', linkText: '', isActive: true },
      { title: 'Admin Ekle', linkPath: null, linkText: '', isActive: false },
    ]);
    this.firstFormGroup = this._formBuilder.group({
      userNameCtrl: ['', Validators.required],
      nameCtrl: ['', Validators.required],
      surNameCtrl: ['', Validators.required],
      passwordCtrl: ['', Validators.required],
      passwordAgainCtrl: ['', Validators.required],
      adminTypeCtrl: ['', Validators.required],
      adminManagmentTypeCtrl: ['',],
      companyCtrl: ['',],
    });

    this.getPanelAdminTypes();
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

  save() {
    this.srvProgressSpinner.show();
    this.panelAdminInsertModel.isActive = true;
    this.panelAdminInsertModel.password = this.md5.appendStr(this.password).end() + "";
    this.panelAdminInsertModel.passwordAgain = this.md5.appendStr(this.passwordAgain).end() + "";
    this.srvPanelAdmin.add(this.panelAdminInsertModel).subscribe((response: any) => {
      this.srvUtils.showActionNotification("Başarıyla Eklendi", EnumMessageType.Success, 8000);
      setTimeout(() => {
        this.router.navigate(['panelAdmin/list']);
      }, 2500);
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  checkPassword() {
    if (this.password != this.passwordAgain) {
      this.srvUtils.showActionNotification("Şifreler Uyuşmamaktadır", EnumMessageType.Error, 3000);
      return false;
    }
    return true;
  }

  onNext() {
    if (this.checkPassword()) {
      this.stepper.next();
    }
  }
}

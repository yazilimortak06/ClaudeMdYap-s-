import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from '../../partials/layout/layout.component';
import { NotFoundComponent } from './not-found-page/not-found.component';
import { UnauthorizedComponent } from './unauthorized-page/unauthorized.component';

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forChild([
            {
                path: '',
                component: NotFoundComponent,
            },
            {
                path: 'error-unauthorized',
                component: UnauthorizedComponent,
            }
        ])
    ],
    providers: [
    ],
    declarations: [
        NotFoundComponent,
        UnauthorizedComponent
    ]
})
  export class ErrorPageModule { }

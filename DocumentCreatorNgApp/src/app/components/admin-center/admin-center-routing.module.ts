import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule }   from '@angular/common';

import { AdminCenterComponent } from './admin-center.component';
import { AdminCenterHomeComponent } from './admin-center-home/admin-center-home.component';
import { TemplateListComponent } from './template-list/template-list.component';
import { TemplateDetailComponent } from './template-detail/template-detail.component';

const adminCenterRoutes: Routes = [
    {
        path: 'admin',
        component: AdminCenterComponent,
        children: [
            
                { path: '', component: AdminCenterHomeComponent },
                { path: 'templates', component: TemplateListComponent },
                { path: 'templates/:id', component: TemplateDetailComponent }
        ]
    }
];

  @NgModule({
    declarations: [
      AdminCenterComponent,
      AdminCenterHomeComponent,
      TemplateListComponent,
      TemplateDetailComponent
    ],
    imports: [
      CommonModule,
      BrowserModule,
      RouterModule,
      RouterModule.forChild(adminCenterRoutes)
    ],
    exports: [
      //AdminCenterComponent
    ],
    providers: []
  })
  export class AdminCenterRoutingModule { }
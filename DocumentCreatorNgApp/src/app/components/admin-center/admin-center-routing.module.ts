import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminCenterComponent } from './admin-center.component';
import { AdminCenterHomeComponent } from './admin-center-home/admin-center-home.component';
import { TemplateListComponent } from './template-list/template-list.component';
import { TemplateDetailComponent } from './template-detail/template-detail.component';

const adminCenterRoutes: Routes = [
    {
        path: 'admin',
        component: AdminCenterComponent,
        children: [{
            path: '',
            component: AdminCenterHomeComponent,
            children: [
                { path: 'templates', component: TemplateListComponent },
                { path: 'templates/:id', component: TemplateDetailComponent }
            ]
        }]
    }
];

  @NgModule({
    imports: [
      RouterModule.forChild(adminCenterRoutes)
    ],
    exports: [
      RouterModule
    ]
  })
  export class AdminCenterRoutingModule { }
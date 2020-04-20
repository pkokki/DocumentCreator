import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule }   from '@angular/common';

import { MaterialModule } from '../../material.module';

import { AdminCenterComponent } from './admin-center.component';
import { SettingsComponent } from './settings/settings.component';
import { TemplateDetailComponent } from './template-detail/template-detail.component';
import { TemplatesTableComponent } from './templates-table/templates-table.component';

const adminCenterRoutes: Routes = [
    {
        path: 'admin',
        component: AdminCenterComponent,
        children: [
                { path: '', pathMatch: 'full', redirectTo: 'templates' },
                { path: 'settings', component: SettingsComponent },
                { path: 'templates', component: TemplatesTableComponent },
                { path: 'templates/:name', component: TemplateDetailComponent },
                { path: 'templates/:name/versions', component: TemplatesTableComponent },
                { path: 'templates/:name/versions/:version', component: TemplateDetailComponent }
        ], 
    }
];

  @NgModule({
    declarations: [
      AdminCenterComponent,
      SettingsComponent,
      TemplateDetailComponent,
      TemplatesTableComponent
    ],
    imports: [
      CommonModule,
      BrowserModule,
      RouterModule,
      MaterialModule,
      RouterModule.forChild(adminCenterRoutes)
    ],
    exports: [
      //AdminCenterComponent
    ],
    providers: []
  })
  export class AdminCenterRoutingModule { }
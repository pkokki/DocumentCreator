import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule }   from '@angular/common';

import { MaterialModule } from '../../material.module';

import { AdminCenterComponent } from './admin-center.component';
import { SettingsComponent } from './settings/settings.component';
import { TemplateDetailComponent } from './template-detail/template-detail.component';
import { TemplatesTableComponent } from './templates-table/templates-table.component';
import { MappingsTableComponent } from './mappings-table/mappings-table.component';
import { DocumentsTableComponent } from './documents-table/documents-table.component';
import { MappingsDetailComponent } from './mappings-detail/mappings-detail.component';

const adminCenterRoutes: Routes = [
    {
        path: 'admin',
        component: AdminCenterComponent,
        children: [
                { path: '', pathMatch: 'full', redirectTo: 'templates' },
                { path: 'settings', component: SettingsComponent },
                
                { path: 'templates', component: TemplatesTableComponent },
                { path: 'templates/:name/versions', component: TemplatesTableComponent },
                
                { path: 'templates/:name', component: TemplateDetailComponent },
                { path: 'templates/:name/versions/:version', component: TemplateDetailComponent },
                
                { path: 'mappings', component: MappingsTableComponent },

                { path: 'mappings/:name', component: MappingsDetailComponent },
                { path: 'mappings/:name/versions/:version', component: MappingsDetailComponent },
                
                { path: 'documents', component: DocumentsTableComponent },
        ], 
    }
];

  @NgModule({
    declarations: [
      AdminCenterComponent,
      SettingsComponent,
      TemplateDetailComponent,
      TemplatesTableComponent,
      MappingsTableComponent,
      MappingsDetailComponent,
      DocumentsTableComponent
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
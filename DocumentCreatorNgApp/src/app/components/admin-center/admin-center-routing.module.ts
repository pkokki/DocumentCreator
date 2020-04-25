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
                { path: 'templates/:templateName', component: TemplateDetailComponent },
                { path: 'templates/:templateName/versions', component: TemplatesTableComponent },
                { path: 'templates/:templateName/versions/:templateVersion', component: TemplateDetailComponent },
                { path: 'templates/:templateName/versions/:templateVersion/mappings', component: MappingsTableComponent },
                { path: 'templates/:templateName/versions/:templateVersion/mappings/:mappingName/documents', component: DocumentsTableComponent }, // 1, 2, 3
                // 1, 2, 3, 4 
                { path: 'templates/:templateName/versions/:templateVersion/documents', component: DocumentsTableComponent }, // 1, 2
                { path: 'templates/:templateName/mappings', component: MappingsTableComponent },
                { path: 'templates/:templateName/mappings/:mappingName/versions', component: MappingsTableComponent },
                { path: 'templates/:templateName/mappings/:mappingName/versions/:mappingVersion', component: MappingsDetailComponent },
                { path: 'templates/:templateName/mappings/:mappingName/versions/:mappingVersion/documents', component: DocumentsTableComponent }, // 1, 3, 4
                { path: 'templates/:templateName/mappings/:mappingName/documents', component: DocumentsTableComponent }, // 1, 3
                { path: 'templates/:templateName/documents', component: DocumentsTableComponent }, // 1
                
                { path: 'mappings', component: MappingsTableComponent },
                { path: 'mappings/:mappingName', component: MappingsDetailComponent },
                { path: 'mappings/:mappingName/versions', component: MappingsTableComponent },
                { path: 'mappings/:mappingName/versions/:mappingVersion', component: MappingsDetailComponent },
                { path: 'mappings/:mappingName/versions/:mappingVersion/documents', component: DocumentsTableComponent }, // 3, 4
                { path: 'mappings/:mappingName/documents', component: DocumentsTableComponent }, // 3
                
                { path: 'documents', component: DocumentsTableComponent },  // 0
                
                { path: 'mappings/:mappingName/templates', component: TemplatesTableComponent },
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
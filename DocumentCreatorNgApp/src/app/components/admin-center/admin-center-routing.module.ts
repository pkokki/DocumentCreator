import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule }   from '@angular/common';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatBadgeModule } from '@angular/material/badge';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';

import { AdminCenterComponent } from './admin-center.component';
import { SettingsComponent } from './settings/settings.component';
import { TemplateListComponent } from './template-list/template-list.component';
import { TemplateDetailComponent } from './template-detail/template-detail.component';
import { TemplateVersionsComponent } from './template-versions/template-versions.component';

const adminCenterRoutes: Routes = [
    {
        path: 'admin',
        component: AdminCenterComponent,
        children: [
                { path: '', pathMatch: 'full', redirectTo: 'templates' },
                { path: 'settings', component: SettingsComponent },
                { path: 'templates', component: TemplateListComponent },
                { path: 'templates/:name', component: TemplateDetailComponent },
                { path: 'templates/:name/versions', component: TemplateVersionsComponent },
                { path: 'templates/:name/versions/:version', component: TemplateDetailComponent }
        ], 
    }
];

  @NgModule({
    declarations: [
      AdminCenterComponent,
      SettingsComponent,
      TemplateListComponent,
      TemplateDetailComponent,
      TemplateVersionsComponent
    ],
    imports: [
      CommonModule,
      BrowserModule,
      RouterModule,

      BrowserAnimationsModule,
      MatCheckboxModule,
      MatCheckboxModule,
      MatButtonModule,
      MatInputModule,
      MatAutocompleteModule,
      MatDatepickerModule,
      MatFormFieldModule,
      MatRadioModule,
      MatSelectModule,
      MatSliderModule,
      MatSlideToggleModule,
      MatMenuModule,
      MatSidenavModule,
      MatBadgeModule,
      MatToolbarModule,
      MatListModule,
      MatGridListModule,
      MatCardModule,
      MatStepperModule,
      MatTabsModule,
      MatExpansionModule,
      MatButtonToggleModule,
      MatChipsModule,
      MatIconModule,
      MatProgressSpinnerModule,
      MatProgressBarModule,
      MatDialogModule,
      MatTooltipModule,
      MatSnackBarModule,
      MatTableModule,
      MatSortModule,
      MatPaginatorModule,

      RouterModule.forChild(adminCenterRoutes)
    ],
    exports: [
      //AdminCenterComponent
    ],
    providers: []
  })
  export class AdminCenterRoutingModule { }
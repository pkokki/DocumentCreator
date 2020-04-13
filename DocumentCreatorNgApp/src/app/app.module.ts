import { BrowserModule } from '@angular/platform-browser';
import { CommonModule }   from '@angular/common';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { QuickstartModule } from './components/quickstart/quickstart.module';
import { AdminCenterRoutingModule } from './components/admin-center/admin-center-routing.module'
import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './components/var/page-not-found/page-not-found.component';

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    QuickstartModule,
    AdminCenterRoutingModule,
    // AppRoutingModule should be last.
    // Most importantly, it comes after the modules with own routing.
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

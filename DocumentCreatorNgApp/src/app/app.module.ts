import { BrowserModule } from '@angular/platform-browser';
import { CommonModule }   from '@angular/common';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { httpErrorInterceptorProvider } from './services/http-error.interceptor';
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
    HttpClientModule,
    QuickstartModule,
    AdminCenterRoutingModule,
    // AppRoutingModule should be last.
    // Most importantly, it comes after the modules with own routing.
    AppRoutingModule
  ],
  providers: [
    httpErrorInterceptorProvider
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

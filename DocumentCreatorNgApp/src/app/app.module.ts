import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { QuickstartModule } from './components/quickstart/quickstart.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    QuickstartModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms'; 
import { HttpClientModule } from '@angular/common/http'; 

import { Step1Component } from './step1/step1.component';
import { Step2Component } from './step2/step2.component';
import { Step3Component } from './step3/step3.component';
import { Step4Component } from './step4/step4.component';
import { QuickstartComponent } from './quickstart.component';

@NgModule({
    declarations: [
      Step1Component,
      Step2Component,
      Step3Component,
      Step4Component,
      QuickstartComponent
    ],
    exports: [QuickstartComponent],
    imports: [
      BrowserModule,
      FormsModule,
      HttpClientModule
    ],
    providers: []
  })
  export class QuickstartModule { }
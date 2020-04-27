import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { QuickstartComponent } from './components/quickstart/quickstart.component';
import { ExpressionsComponent } from './components/expressions/expressions.component';
import { PageNotFoundComponent } from './components/var/page-not-found/page-not-found.component';

const appRoutes: Routes = [
  { path: '', component: QuickstartComponent },
  { path: 'quickstart', component: QuickstartComponent },
  { path: 'expressions', component: ExpressionsComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(
    appRoutes,
    { enableTracing: false, useHash: true } // <-- true for debugging
  )],
  exports: [RouterModule]
})
export class AppRoutingModule { }

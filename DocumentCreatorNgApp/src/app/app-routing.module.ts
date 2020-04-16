import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { QuickstartComponent } from './components/quickstart/quickstart.component';
import { PageNotFoundComponent } from './components/var/page-not-found/page-not-found.component';


const appRoutes: Routes = [
  { path: '', component: QuickstartComponent },
  { path: 'quickstart', component: QuickstartComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(
    appRoutes,
    { enableTracing: false } // <-- true for debugging
  )],
  exports: [RouterModule]
})
export class AppRoutingModule { }

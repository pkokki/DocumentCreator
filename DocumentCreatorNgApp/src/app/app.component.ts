import { Component } from '@angular/core';
import { State } from './services/state/state.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Document Creator';
  
  constructor(public state: State) { }

}

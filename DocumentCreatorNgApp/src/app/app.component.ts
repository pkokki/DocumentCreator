import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'DocumentCreatorNgApp';
  templateName: string;
  mappingName: string;
  apiUrl = "http://localhost:43456/api";
}

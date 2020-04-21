import { Component, OnInit } from '@angular/core';
import { State } from '../../../services/state/state.service';

@Component({
  selector: 'app-step2',
  templateUrl: './step2.component.html',
  styleUrls: ['../quickstart.component.css']
})
export class Step2Component implements OnInit {
  constructor(public state: State) { }

  ngOnInit(): void {
  }

  download() {
    const url = this.state.apiBaseUrl+'/templates/'+this.state.templateName+'/mappings/'+this.state.mappingName;
    window.open(url, '_blank');
  }
}

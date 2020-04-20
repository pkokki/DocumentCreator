import { Component, OnInit } from '@angular/core';
import { EnvService } from 'src/app/services/env/env.service';
import { MatRadioChange } from '@angular/material/radio';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit {
  availableEndpoints = [];
  selectedEndpointName: string;

  constructor(
    private envService: EnvService
  ) { }

  ngOnInit() {
    this.envService.getEnv().subscribe(env => {
      this.availableEndpoints = env.endpoints;
      this.selectedEndpointName = env.activeEndpoint.name;
    });
  }

  endpointSelectionChanged(event: MatRadioChange){
    this.envService.activate(event.value.name);
  }
}

import { Component, OnInit } from '@angular/core';
import { EnvService, Env } from 'src/app/services/env/env.service';

@Component({
  selector: 'app-admin-center-home',
  templateUrl: './admin-center-home.component.html'
})
export class AdminCenterHomeComponent implements OnInit {
  vm = {
    activeEndpoint: <{ url: any; name: string; }>{},
    endpoints: []
  };

  constructor(
    private envService: EnvService
  ) { }

  ngOnInit(): void {
    this.envService.active().subscribe(env => this.updateVM(env));
  }

  activate(endpointName: string) {
    this.envService.activate(endpointName).subscribe(env => this.updateVM(env));
  }

  private updateVM(env: Env) {
    this.vm.endpoints.splice(0, this.vm.endpoints.length, ... env.endpoints);
    this.vm.activeEndpoint = env.activeEndpoint;
  }

}

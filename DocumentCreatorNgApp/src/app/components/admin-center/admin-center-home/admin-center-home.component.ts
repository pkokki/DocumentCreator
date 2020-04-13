import { Component, OnInit } from '@angular/core';
import { EnvService, Env } from 'src/app/services/env/env.service';

@Component({
  selector: 'app-admin-center-home',
  templateUrl: './admin-center-home.component.html'
})
export class AdminCenterHomeComponent implements OnInit {
  endpoints = [];
  activeEnv = {
    name: 'None',
    url: 'N/A'
  };

  constructor(
    private envService: EnvService
  ) { }

  ngOnInit(): void {
    this.envService.get().subscribe(env => this.handleEnv(env));
  }

  private handleEnv(env: Env): void {
    if (env.endpoints && env.endpoints.length) {
      this.endpoints.push(...env.endpoints);
      Object.assign(this.activeEnv, env.endpoints[0]);
    }
  }

}

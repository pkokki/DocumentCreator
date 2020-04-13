import { Component, OnInit } from '@angular/core';
import { State } from 'src/app/services/state/state.service';

@Component({
  selector: 'app-quickstart',
  templateUrl: './quickstart.component.html',
  styleUrls: ['./quickstart.component.css']
})
export class QuickstartComponent implements OnInit {

  constructor(public state: State) { }

  ngOnInit(): void {
  }

}

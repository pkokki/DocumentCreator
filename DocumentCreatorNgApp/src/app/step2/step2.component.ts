import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-step2',
  templateUrl: './step2.component.html',
  styleUrls: ['./step2.component.css']
})
export class Step2Component implements OnInit {
  @Input() templateName: string;
  @Input() apiUrl: string;
  constructor() { }

  ngOnInit(): void {
  }
}

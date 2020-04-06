import { Component, OnInit, Input } from '@angular/core';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-step4',
  templateUrl: './step4.component.html',
  styleUrls: ['./step4.component.css']
})
export class Step4Component implements OnInit {
  @Input() apiUrl: string;
  @Input() templateName: string;
  @Input() mappingName: string;
  payload = {
    body: JSON.stringify(JSON.parse('{"LogHeader":{"RequestId":"123456", "Unit":"123"},"RequestData":{"ProductDescription":"Προθεσμιακή με Bonus 3 Μηνών - Από Ευρώ 10.000","AccountNumber":"923456789012345", "SEPA":0.17, "PS1014":3, "PS1015":1, "PS1016":"MONTH", "PS1053": "START", "PS1056":10000, "F1041":5000, "F1042":10000, "InterestTable":[{"Period":1,"Interest":0.2,"Points":500}, {"Period":3,"Interest":0.25,"Points":1000}]}}'), null, 2)
  };

  constructor() { }

  ngOnInit(): void {
  }

  submit() {

  }
}

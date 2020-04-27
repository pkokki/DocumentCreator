import { Component, OnInit } from '@angular/core';
import { EnvService } from 'src/app/services/env/env.service';

@Component({
  selector: 'app-expressions',
  templateUrl: './expressions.component.html',
  styleUrls: ['./expressions.component.css']
})
export class ExpressionsComponent implements OnInit {

  expressions = [
    '8 / 2 * (2 + 2)',
		'x1 * x2 - x1 / x2',
		'UPPER(CONCATENATE(x3," ","john"))',
		'SUM(x4) * 24%',
		'REPLACE(x5.y1, SEARCH(x5.y2, x5.y1), LEN(x5.y2), x5.y3)',
		'NOW() + x5.y4[1]'
  ];
  expressions2 = Array(this.expressions.length).fill(0);
  results = Array(this.expressions.length).fill({name: null, value: null, text: null, error: null});
  payload = `{
    "x1": 10,
    "x2": 2,
    "x3": "smith",
    "x4": [100, 200, 300, 400],
    "x5": {
      "y1": "A quick brown fox jumps over the lazy dog",
      "y2": "brown",
      "y3": "#red#",
      "y4": [ 1, 2, 3]
    }
  }`;

  constructor(
    private envService: EnvService
  ) { }

  ngOnInit(): void {
  }

  evaluate() {
    const request = {
      expressions: this.expressions,
      payload: JSON.parse(this.payload)
    };
    this.envService.post<{name: string, value: any, text: string, error: string}[]>('/expressions', request).subscribe(response => {
      console.log(response);
      this.results = response;
    })
  }
}

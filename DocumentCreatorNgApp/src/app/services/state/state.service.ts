import { Injectable } from '@angular/core';
import { EnvService } from '../env/env.service';

@Injectable({
    providedIn: 'root'
})
export class State {

    constructor(private env: EnvService) {
        var id = ("0000" + Math.floor((Math.random()*1000)+1)).slice(-4);
        env.getEnv().subscribe(env => this.apiBaseUrl = env.baseUrl);
        this.templateName = 'T' + id;
        this.mappingName = 'M' + id;
        this.testPayload = JSON.stringify(
            JSON.parse('{"LogHeader":{"RequestId":"123456", "Unit":"123"},"RequestData":{"ProductDescription":"Προθεσμιακή με Bonus 3 Μηνών - Από Ευρώ 10.000","AccountNumber":"923456789012345", "SEPA":0.17, "PS1014":3, "PS1015":1, "PS1016":"MONTH", "PS1053": "START", "PS1056":10000, "F1041":5000, "F1042":10000, "InterestTable":[{"Period":1,"Interest":0.2,"Points":500}, {"Period":3,"Interest":0.25,"Points":1000}]}}'), 
            null, 
            2);
    }

    public apiBaseUrl: string;
    public templateName: string;
    public mappingName: string;
    public testPayload: string;

}
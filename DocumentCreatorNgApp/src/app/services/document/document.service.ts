import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvService } from '../env/env.service';
import { tap } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class DocumentService {

    constructor(
        private envService: EnvService,
    ) { }

    getDocuments(filters?: string | string[], page?: number, pageSize?: number, orderBy?: string, isDesc?: boolean): Observable<Document[]> {
        const params = [];
        if (page) params.push(`page=${page}`);
        if (pageSize) params.push(`pageSize=${pageSize}`);
        if (orderBy) params.push(`orderBy=${orderBy}`);
        if (isDesc) params.push(`descending=${isDesc}`);
        if (filters) {
            params.push(Array.isArray(filters) ? [...filters] : filters);
        }

        const url = '/documents' + (params.length ? '?' +  params.join('&') : '');
        return this.envService.get<Document[]>(url).pipe(
            tap(ev => {
                console.log('getDocuments', ev);
            })
        );
    }
}

export interface Document {
    id: string;
    templateName: string;
    templateVersion: string;
    mappingName: string;
    mappingVersion: string;
    timestamp: Date;
    size: number;
}
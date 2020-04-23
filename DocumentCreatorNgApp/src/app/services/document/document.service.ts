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

    getDocuments(filters?: string | string[], page?: number, pageSize?: number, orderBy?: string, isDesc?: boolean): Observable<PagedResults<Document>> {
        const params = [];
        if (page) params.push(`page=${page}`);
        if (pageSize) params.push(`pageSize=${pageSize}`);
        if (orderBy) params.push(`orderBy=${orderBy}`);
        if (isDesc) params.push(`descending=${isDesc}`);
        if (filters) {
            params.push(Array.isArray(filters) ? [...filters] : filters);
        }

        const query = params.length ? '?' + params.join('&') : null;
        return this.envService.get<PagedResults<Document>>('/documents' + query).pipe(
            tap(ev => {
                console.log('getDocuments', query, ev);
            })
        );
    }
}

export interface PagedResults<T> {
    page: number;
    pageSize: number;
    totalPages: number;
    total: number;
    results: T[];
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
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { EnvService } from '../env/env.service';
import { tap } from 'rxjs/operators';
import { HttpResponse, HttpHeaders } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class DocumentService {

    constructor(
        private envService: EnvService,
    ) { }

    downloadDocument(documentId: string): void {
        if (!documentId)
            throw throwError("invalid documentId");
        this.envService.download('/documents/' + documentId);
    }

    getDocuments(filters?: string | string[], page?: number, pageSize?: number, orderBy?: string, isDesc?: boolean): Observable<PagedResults<Document>> {
        const params = [];
        if (page) params.push(`page=${page}`);
        if (pageSize) params.push(`pageSize=${pageSize}`);
        if (orderBy) params.push(`orderBy=${orderBy}`);
        if (isDesc) params.push(`descending=${isDesc}`);
        if (filters) {
            if (Array.isArray(filters))
                params.push(...filters);
            else
                params.push(filters);
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
    documentId: string;
    templateName: string;
    templateVersion: string;
    mappingName: string;
    mappingVersion: string;
    timestamp: Date;
    size: number;
    fileName: string;
    url: string;
}
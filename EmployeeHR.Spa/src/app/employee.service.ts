import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from './employee';

@Injectable({
    providedIn: 'root'
})
export class EmployeeService {

    constructor(private http: HttpClient) { }

    get(): Observable<Employee[]> {

        const url = `https://localhost:5001/api/employee`;

        return this.http.get<Employee[]>(url)
            .pipe(
                catchError((e) => {
                    console.error(e);
                    return throwError(e);
                })
            );

    }
}

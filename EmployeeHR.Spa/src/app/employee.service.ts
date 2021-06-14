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

    delete(item: Employee): Observable<number> {
        const url = `https://localhost:5001/api/employee/${item.id}`;

        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            }),
            body: item,
        };

        return this.http.delete<number>(url, httpOptions).pipe(
            catchError(e => {
                console.error(e);
                return throwError(e);
            })
        );
    }

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

    post(e: Employee): Observable<Employee> {
        const url = `https://localhost:5001/api/employee`;

        return this.http.post<Employee>(url, e)
            .pipe(
                catchError(e => {
                    console.error(e);
                    return throwError(e);
                })
            );
    }

    put(id: number | undefined, e: Employee): Observable<Employee> {
        const url = `https://localhost:5001/api/employee/${id}`;
        return this.http.put<Employee>(url, e).pipe(
            catchError(e => {
                console.error(e);
                return throwError(e);
            })
        );
    }
}

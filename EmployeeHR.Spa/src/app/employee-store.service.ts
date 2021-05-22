import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { Employee } from './employee';
import { EmployeeService } from './employee.service';

@Injectable({
    providedIn: 'root'
})
export class EmployeeStoreService implements OnDestroy {


    private subscription$ = new Subscription();
    private isLoadingValue = false;
    private _isLoading: Subject<boolean> = new BehaviorSubject(this.isLoadingValue);
    public readonly isLoading = this._isLoading.asObservable();

    private _dataValue: Employee[] | undefined;
    private _data: Subject<Employee[]> = new Subject<Employee[]>();


    constructor(private service: EmployeeService) { }

    ngOnDestroy(): void {
        if (this.subscription$) {
            this.subscription$.unsubscribe();
        }
    }

    get(): Observable<Employee[]> {
        this._isLoading.next(true);

        if (!this._dataValue) {
            this.subscription$.add(
                this.service.get().subscribe(
                    (data: Employee[]) => {
                        this._dataValue = data;

                        this._data.next(this._dataValue);

                        this._isLoading.next(false);
                    },
                    (err: any) => {
                        console.error(err);
                        this._isLoading.next(false);
                    }
                )
            );
        } else {

            this._data.next(this._dataValue);
            this._isLoading.next(false);
        }

        return this._data.asObservable();
    }

    save(e: Employee, isNew: boolean): void {
        if (isNew) {
            this.post(e);
        }
        else {
            this.put(e.id, e);
        }
    }

    private _add(item: Employee): number {

        if (!this._dataValue) {
            this._dataValue = [];
        }

        const n: number = this._dataValue.push(item);
        return n;
    }

    private put(id: number | undefined, e: Employee) {
        throw new Error('Method not implemented.');
    }

    private post(item: Employee): void {

        this._isLoading.next(true);

        const itemToAdd = {} as Employee;
        Object.assign(itemToAdd, item);

        itemToAdd.id = 0;

        this.subscription$.add(
            this.service.post(itemToAdd).subscribe(
                (employeeAdded: Employee) => {

                    const n: number = this._add(employeeAdded);

                    if (this._data) {
                        this._data.next(this._dataValue);
                    }

                    this._isLoading.next(false);
                },
                (err: any) => {
                    console.error(err);

                    this._isLoading.next(false);
                }
            )
        );
    }

}

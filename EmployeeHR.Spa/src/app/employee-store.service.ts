import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { Employee } from './employee';
import { EmployeeService } from './employee.service';
import { of } from 'rxjs';
@Injectable({
    providedIn: 'root'
})
export class EmployeeStoreService implements OnDestroy {


    private subscription$ = new Subscription();
    private _isLoading: Subject<boolean> = new BehaviorSubject<boolean>(false);
    public readonly isLoading: Observable<boolean> = this._isLoading.asObservable();

    private items: Employee[] | undefined;
    private _data: Subject<Employee[]> = new Subject<Employee[]>();
    private data: Observable<Employee[]> = this._data.asObservable();

    constructor(private service: EmployeeService) { }

    ngOnDestroy(): void {
        if (this.subscription$) {
            this.subscription$.unsubscribe();
        }
    }

    get(): Observable<Employee[]> {

        if (this.items) {
            this.data = of(this.items);
            return this.data;
        }

        this._isLoading.next(true);

        this.subscription$.add(
            this.service.get().subscribe(
                (value: Employee[]) => {
                    this.items = value;

                    this._data.next(this.items);

                    this._isLoading.next(false);
                },
                (err: any) => {
                    console.error(err);
                    this._isLoading.next(false);
                }
            )
        );

        this.data = this._data.asObservable();
        return this.data;
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

        if (!this.items) {
            this.items = [];
        }

        const n: number = this.items.push(item);
        return n;
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
                        this._data.next(this.items);
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

    private _update(item: Employee) {
        const e: Employee | undefined = this.items?.find((value, index, arr) => { return value.id === item.id; });
        if (e) {
            Object.assign(e, item);
        }

        return e;
    }

    private put(id: number | undefined, item: Employee): void {

        this._isLoading.next(true);

        this.subscription$.add(
            this.service.put(id, item)
                .subscribe(
                    (employeeUpdated: Employee) => {

                        this._update(employeeUpdated);

                        if (this._data) {
                            this._data.next(this.items); // send a copy of the array
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

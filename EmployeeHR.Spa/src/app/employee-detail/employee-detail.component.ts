import { Component, OnDestroy, OnInit } from '@angular/core';
import { Employee } from '../employee';
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription, throwError } from 'rxjs';
import { catchError, concatMap } from 'rxjs/operators';
import { EmployeeStoreService } from '../employee-store.service';
@Component({
    selector: 'app-employee-detail',
    templateUrl: './employee-detail.component.html',
    styleUrls: ['./employee-detail.component.scss'],
})
export class EmployeeDetailComponent implements OnInit, OnDestroy {
    submitted = false;

    onSubmit() {
        this.submitted = true;
        this.isNew = false;
        this.storeService.save(this.model, this.isNew);
    }

    private subscription$: Subscription = new Subscription();

    private id: number | typeof NaN = NaN;
    private isNew = false;
    model: Employee = new Employee();

    constructor(private route: ActivatedRoute, private storeService: EmployeeStoreService) { }

    ngOnDestroy(): void {
        if (this.subscription$) {
            this.subscription$.unsubscribe();
        }
    }

    ngOnInit(): void {
        this.subscription$.add(
            this.route.params
                .pipe(
                    concatMap((value: Params) => {
                        this.isNew = isNaN(value['id']);
                        this.id = +value['id'];

                        return this.storeService.get();
                    }),
                    catchError((err: any) => {
                        console.log(err);
                        return throwError(err);
                    })
                )
                .subscribe(
                    (employees: Employee[]) => {
                        this.model = this.isNew
                            ? new Employee()
                            : employees.find((e) => {
                                return e.id == this.id;
                            }) || new Employee();
                    },
                    (err: any) => {
                        console.error(err);
                    }
                )
        );
    }
}

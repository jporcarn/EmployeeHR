import { Component, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { process } from "@progress/kendo-data-query";
import { CompositeFilterDescriptor, GroupDescriptor, orderBy, SortDescriptor, State } from '@progress/kendo-data-query';
import { Subscription } from 'rxjs';
import { Employee } from '../employee';
import { EmployeeStoreService } from '../employee-store.service';

const createFormGroup = (dataItem: Employee) =>
    new FormGroup({
        id: new FormControl(dataItem.id),
        firstName: new FormControl(dataItem.firstName, Validators.required),
        lastName: new FormControl(dataItem.lastName, Validators.required),
        socialSecurityNumber: new FormControl(dataItem.socialSecurityNumber, Validators.required),
        phoneNumber: new FormControl(dataItem.phoneNumber),
        rowVersion: new FormControl(dataItem.rowVersion),
    });

const matches = (el: { matches: any; msMatchesSelector: any }, selector: string) => (el.matches || el.msMatchesSelector).call(el, selector);

@Component({
    selector: 'app-employee',
    templateUrl: './employee.component.html',
    styleUrls: ['./employee.component.scss'],
})
export class EmployeeComponent implements OnInit, OnDestroy {
    @ViewChild(GridComponent)
    private grid!: GridComponent;

    public gridCurrentState: {
        skip: number;
        take: number;
        sort: Array<SortDescriptor>;
        filter: CompositeFilterDescriptor;
        group: Array<GroupDescriptor>;
    } = {
            skip: 0,
            take: 20,
            sort: [
                {
                    field: 'id',
                    dir: 'desc',
                },
            ],
            filter: {
                logic: 'and',
                filters: [],
            },
            group: [],
        };

    public gridView!: GridDataResult;

    public keysSelected: number[] = [];

    public employees: Employee[] = [];

    public formGroup: FormGroup | undefined;

    private editedRowIndex: number | undefined;

    private isNew = false;

    private subscription$ = new Subscription();

    public loadingData = false;

    constructor(private storeService: EmployeeStoreService, private renderer: Renderer2) { }

    public ngOnInit(): void {

        this.subscription$.add(
            this.storeService.isLoading.subscribe(
                (loading: boolean) => {
                    this.loadingData = loading;
                },
                (err: any) => {
                    console.error(err);
                }
            )
        );

        this.subscription$.add(
            this.storeService.get()
                .subscribe(
                    (employees: Employee[]) => {
                        this.employees = employees;

                        this.loadItems();
                    },
                    (err: any) => {
                        console.error(err);
                    }
                )
        );

    }

    public ngOnDestroy(): void {

        if (this.subscription$) {
            this.subscription$.unsubscribe();
        }
    }

    private loadItems(): void {
        this.gridView = {
            data: orderBy(this.employees, this.gridCurrentState.sort),
            total: this.employees.length,
        };

        this.gridView = process(this.employees, this.gridCurrentState);

    }

    public dataStateChangeHandler(state: State): void {
        this.closeEditor();
        this.gridCurrentState = state as {
            skip: number;
            take: number;
            sort: Array<SortDescriptor>;
            filter: CompositeFilterDescriptor;
            group: Array<GroupDescriptor>;
        };

        this.loadItems();
    }

    public addHandler(): void {
        this.closeEditor();

        this.formGroup = createFormGroup(new Employee());

        this.isNew = true;

        this.grid.addRow(this.formGroup);
    }

    public saveRow() {
        if (this.formGroup && this.formGroup.valid) {
            this.saveCurrent();
        }
    }

    public editHandler({ sender, rowIndex, dataItem }: { sender: GridComponent, rowIndex: number, dataItem: Employee }) {
        this.closeEditor();

        this.formGroup = createFormGroup(dataItem);

        this.editedRowIndex = rowIndex;

        sender.editRow(rowIndex, this.formGroup);
    }

    public saveHandler({ sender, rowIndex, formGroup, isNew }: { sender: GridComponent, rowIndex: number, formGroup: FormGroup, isNew: boolean }) {

        this.saveCurrent();

        sender.closeRow(rowIndex);
    }

    public removeHandler({ dataItem }: { dataItem: Employee }) {
        this.storeService.delete(dataItem);
    }

    public cancelHandler(): void {
        this.closeEditor();
    }

    private closeEditor(): void {
        this.grid.closeRow(this.editedRowIndex);

        this.isNew = false;
        this.editedRowIndex = undefined;
        this.formGroup = undefined;
    }

    private saveCurrent(): void {
        if (this.formGroup) {
            this.storeService.save(this.formGroup.value, this.isNew);
            this.closeEditor();
        }
    }
}

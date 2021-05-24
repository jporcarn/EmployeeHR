import { Component, OnInit } from '@angular/core';
import { Employee } from '../employee';

@Component({
    selector: 'app-employee-detail',
    templateUrl: './employee-detail.component.html',
    styleUrls: ['./employee-detail.component.scss']
})
export class EmployeeDetailComponent implements OnInit {

    model: Employee = new Employee();

    submitted = false;

    onSubmit() { this.submitted = true; }

    constructor() { }

    ngOnInit(): void {
    }

}

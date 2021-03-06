import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { EmployeeComponent } from './employee/employee.component';
import { ErrorComponent } from './error/error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { HttpClientModule } from '@angular/common/http';
import { EmployeeDetailComponent } from './employee-detail/employee-detail.component';
import { FormsModule } from '@angular/forms';
import { IntlModule } from "@progress/kendo-angular-intl";
import { DateInputsModule } from "@progress/kendo-angular-dateinputs";
import { LabelModule } from "@progress/kendo-angular-label";


@NgModule({
    declarations: [
        AppComponent,
        EmployeeComponent,
        ErrorComponent,
        NotFoundComponent,
        LandingPageComponent,
        EmployeeDetailComponent
    ],
    imports: [
        BrowserModule,
        FormsModule,
        AppRoutingModule,
        GridModule,
        BrowserAnimationsModule,
        HttpClientModule,
        IntlModule,
        LabelModule,
        DateInputsModule,
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }

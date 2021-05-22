export class Employee {

    id: number | undefined;
    firstName: string;
    lastName: string;
    socialSecurityNumber: string;
    phoneNumber: string;
    rowVersion: Date;

    constructor() {
        this.id = undefined;
        this.firstName = "";
        this.lastName = "";
        this.socialSecurityNumber = "";
        this.phoneNumber = "";
        this.rowVersion = new Date();
    }

}

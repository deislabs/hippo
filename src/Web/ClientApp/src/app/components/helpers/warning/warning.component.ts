import { Component, Input, OnChanges, OnInit } from '@angular/core';

@Component({
    selector: 'app-warning',
    templateUrl: './warning.component.html',
    styleUrls: ['./warning.component.css']
})
export class WarningComponent implements OnInit, OnChanges {
    @Input() error: any = null;
    errors: Array<string> = [];

    ngOnInit(): void {
        this.errors = this.parseError(this.error);
    }

    ngOnChanges(): void {
        this.errors = this.parseError(this.error);
    }

    closeWarning() {
        this.errors = [];
    }

    parseError(error: any) {
        if (error) {
            if (error.error) {
                const err = error.error;
                if (err.errors) {
                    return this.parseValidationErrors(err.errors);
                } else {
                    return [
                        err.detail ? err.detail : `${err.status} - ${err.title}`
                    ];
                }
            }
            return [
                error.message
                    ? error.message
                    : error.status
                    ? `${error.status} - ${error.statusText}`
                    : 'Server error'
            ];
        } else {
            return [];
        }
    }

    parseValidationErrors(error: any) {
        const errors = [];
        for (const prop in error) {
            if (Object.prototype.hasOwnProperty.call(error, prop)) {
                errors.push(...error[prop]);
            }
        }
        return errors;
    }
}

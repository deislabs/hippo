import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// custom validator to check that two fields match
export function MustMatch(
    controlName: string,
    matchingControlName: string
): ValidatorFn {
    return (c: AbstractControl): ValidationErrors | null => {
        const control = c.get(controlName);
        const matchingControl = c.get(matchingControlName);

        if (matchingControl?.errors && !matchingControl?.errors['mustMatch']) {
            // return if another validator has already found an error on the matchingControl
            return null;
        }

        return control?.value !== matchingControl?.value
            ? { mustMatch: true }
            : null;
    };
}

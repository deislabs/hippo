import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { faLock, faUser } from '@fortawesome/free-solid-svg-icons';

import { AccountService } from 'src/app/core/api/v1';
import { MustMatch } from 'src/app/_helpers/must-match.validator';
import { Router } from '@angular/router';

@Component({
    selector: 'app-account-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
    error: any = null;
    registrationForm!: FormGroup;
    loading = false;
    submitted = false;
    faUser = faUser;
    faLock = faLock;

    constructor(
        private readonly accountService: AccountService,
        private readonly router: Router
    ) {}

    ngOnInit() {
        this.registrationForm = new FormGroup(
            {
                username: new FormControl('', [Validators.required]),
                password: new FormControl('', [
                    Validators.required,
                    Validators.minLength(6),
                ]),
                passwordConfirm: new FormControl('', [
                    Validators.required,
                    Validators.minLength(6),
                ]),
            },
            MustMatch('password', 'passwordConfirm')
        );
    }

    get f() {
        return this.registrationForm.controls;
    }

    onSubmit() {
        this.submitted = true;

        if (this.registrationForm.invalid) {
            return;
        }

        this.loading = true;
        this.accountService
            .apiAccountPost({
                userName: this.f['username'].value,
                password: this.f['password'].value,
            })
            .subscribe({
                // TODO: navigate to registration confirmation page
                next: () => this.router.navigate(['/']),
                error: (err) => {
                    console.log(err);
                    this.error = err;
                    this.loading = false;
                },
            });
    }
}

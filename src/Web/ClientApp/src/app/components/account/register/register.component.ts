import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { faLock, faUser } from '@fortawesome/free-solid-svg-icons';
import { AccountService } from 'src/app/core/api/v1';
import { MustMatch } from 'src/app/_helpers/must-match.validator';

@Component({
	selector: 'app-account-register',
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
	errors:Array<string> = [];
	registrationForm!: FormGroup;
	loading = false;
	submitted = false;
	faUser = faUser;
	faLock = faLock;

	constructor(
		private readonly accountService: AccountService,
		private readonly router: Router,
	) { }

	ngOnInit() {
		this.registrationForm = new FormGroup(
			{
				username: new FormControl('', [
					Validators.required
				]),
				password: new FormControl('', [
					Validators.required,
					Validators.minLength(8),
				]),
				passwordConfirm: new FormControl('', [
					Validators.required,
					Validators.minLength(8)
				])
			},
			MustMatch('password', 'passwordConfirm'),
		);
	}

	get f() { return this.registrationForm.controls; }

	onSubmit() {
		this.submitted = true;

		if (this.registrationForm.invalid) {
			return;
		}

		this.loading = true;
		this.accountService.apiAccountPost({ userName: this.f['username'].value, password: this.f['password'].value })
			.subscribe(
				{
					// TODO: navigate to registration confirmation page
					next: () => this.router.navigate(['/']),
					error: (err) => {
						console.log(err.error.errors);
						this.errors = this.parseError(err.error);
						this.loading = false;
					}
				}
			);
	}

	parseError(error: any) {
		if (error.errors) {
			return this.parseValidationErrors(error.errors);
		} else {
			return [error.title];
		}
	}

	parseValidationErrors(error: any) {
		let errors = [];
		for (var prop in error) {
			if (error.hasOwnProperty(prop)) {
				errors.push(...error[prop]);
			}
		}
		return errors;
	}
}

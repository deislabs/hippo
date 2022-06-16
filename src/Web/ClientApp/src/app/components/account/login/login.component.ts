import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faLock, faUser } from '@fortawesome/free-solid-svg-icons';
import { SessionService } from 'src/app/_services/session.service';

@Component({
	selector: 'app-account-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
	errors:Array<string> = [];
	loginForm!: FormGroup;
	loading = false;
	submitted = false;
	returnUrl = '/';
	faUser = faUser;
	faLock = faLock;

	constructor(
		private readonly sessionService: SessionService,
		private route: ActivatedRoute,
		private readonly router: Router,
	) {
		if (this.sessionService.isLoggedIn()) {
			this.router.navigate([this.returnUrl]);
		}
	}

	ngOnInit() {
		this.loginForm = new FormGroup({
			username: new FormControl('', [
				Validators.required
			]),
			password: new FormControl('', [
				Validators.required,
				Validators.minLength(8),
			])
		});

		// get return url from route parameters or default to '/'
		this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
	}

	get f() { return this.loginForm.controls; }

	onSubmit() {
		this.submitted = true;

		if (this.loginForm.invalid) {
			return;
		}

		this.loading = true;
		this.sessionService.login(this.f['username'].value, this.f['password'].value)
			.subscribe(
				{
					next: () => this.router.navigate([this.returnUrl]),
					error: (error) => {
						console.log(error);
						this.errors = this.parseError(error);
						this.loading = false;
					}
				}
			);
	}

	parseError(error: any) {
		if (error.error)
		{
			var err = error.error;
			if (err.errors) {
				return this.parseValidationErrors(err.errors);
			} else {
				return [(err.detail) ? err.detail : `${err.status} - ${err.title}`]
			}
		}
		return [(error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error'];
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

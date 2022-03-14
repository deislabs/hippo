import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { faPlus, faTrash } from '@fortawesome/free-solid-svg-icons';
import { EnvironmentVariableDto, EnvironmentVariableService } from 'src/app/core/api/v1';

@Component({
	selector: 'app-environment-variable-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	@Input() channelId = '';

	envvars: EnvironmentVariableDto[] = [];

	error = '';
	envvarForm!: FormGroup;
	loading = false;
	submitted = false;
	faPlus = faPlus;
	faTrash = faTrash;

	constructor(private readonly envvarService: EnvironmentVariableService) { }

	ngOnInit(): void {
		this.refreshData();

		this.envvarForm = new FormGroup({
			key: new FormControl('', [
				Validators.required
			]),
			value: new FormControl('', [
				Validators.required
			])
		});
	}

	get f() { return this.envvarForm.controls; }

	refreshData() {
		this.envvarService.apiEnvironmentvariableGet().subscribe(vm => {
			this.envvars = vm.environmentVariables.filter(element => element.channelId == this.channelId);
		});
	}

	addEnvironmentVariable(channelId: string) {
		this.envvarService.apiEnvironmentvariablePost({ channelId: channelId, key: this.f['key'].value, value: this.f['value'].value, })
		.subscribe({
			next: () => {
				this.envvarForm.reset();
				this.refreshData();
			},
			error: (error) => {
				console.log(error);
				this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
				this.loading = false;
			}
		});
	}

	deleteEnvironmentVariable(id: string) {
		this.envvarService.apiEnvironmentvariableIdDelete(id)
		.subscribe({
			next: () => this.refreshData(),
			error: (error) => {
				console.log(error);
				this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
			}
		});
	}
}

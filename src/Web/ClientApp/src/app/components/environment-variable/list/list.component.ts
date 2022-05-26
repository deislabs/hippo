import { Component, Input, OnInit } from '@angular/core';
import { faBackward, faTrash, faSave } from '@fortawesome/free-solid-svg-icons';
import { EnvironmentVariableService } from 'src/app/core/api/v1';

@Component({
	selector: 'app-environment-variable-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	@Input() channelId = '';

	envvars: any = [];
	originalEnvVars: any = [];

	errors:Array<string> = [];
	loading = false;
	submitted = false;
	faBackward = faBackward;
	faTrash = faTrash;
	faSave = faSave;

	constructor(private readonly envvarService: EnvironmentVariableService) { }

	ngOnInit(): void {
		this.refreshData();
	}

	addNewVariable() {
		this.envvars.push({
			channelId: this.channelId,
			key: "",
			value: ""
		})
	}

	inputChanges(changedVar: any) {
		if (!changedVar.id) {
			return;
		}
		
		let originalVar = this.originalEnvVars.filter((v: any) => v.id === changedVar.id)[0];

		if (originalVar.key !== changedVar.key ||
			originalVar.value !== changedVar.value) {
			changedVar.isChanged = true;
		} else {
			changedVar.isChanged = false;
		}
	}

	undoChange(changedVar: any) {
		let originalVar = this.originalEnvVars.filter((v: any) => v.id === changedVar.id)[0];

		changedVar.key = originalVar.key;
		changedVar.value = originalVar.value;
		changedVar.isChanged = false;
	}

	removeVariable(envvar: any) {
		this.envvars = this.envvars.filter((v: any) => v !== envvar);
	}

	save() {
		this.envvarService.apiEnvironmentvariableRangePost({
			channelId: this.channelId,
			environmentVariables: this.envvars
		}).subscribe({
			next: () => {
				this.refreshData();
				this.submitted = true;
				this.errors = [];
			},
			error: (err) => {
				console.log(err.error.errors);
				this.errors = this.parseError(err.error);
				this.submitted = false;
				this.loading = false;
			}
		});
	}

	refreshData() {
		this.envvarService.apiEnvironmentvariableGet().subscribe(
			{
				next: (vm) => {
					this.errors = [];
					this.envvars = vm.environmentVariables.filter(element => element.channelId == this.channelId);
					this.originalEnvVars = this.envvars.map((v: any) => {
						return {
							id: v.id,
							channelId: v.channelId,
							key: v.key,
							value: v.value
						}
					});
				},
				error: (err) => {
					console.log(err.error.errors);
					this.errors = this.parseError(err.error);
					this.submitted = false;
					this.loading = false;
				}
			});
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

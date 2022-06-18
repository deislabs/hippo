import { Component, Input, OnChanges } from '@angular/core';
import { faBackward, faTrash, faSave } from '@fortawesome/free-solid-svg-icons';
import { ChannelService, EnvironmentVariableService } from 'src/app/core/api/v1';

@Component({
	selector: 'app-environment-variable-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnChanges {
	@Input() channelId = '';

	envvars: any = [];
	originalEnvVars: any = [];

	error: any = null;
	loading: boolean = false;
	submitted = false;
	faBackward = faBackward;
	faTrash = faTrash;
	faSave = faSave;

	constructor(private readonly channelService: ChannelService, private readonly envVarService: EnvironmentVariableService) { }

	ngOnChanges(): void {
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
		if (!this.validateEnvVars()) {
			return;
		}

		this.channelService.apiChannelChannelIdEnvironmentVariablesPut(this.channelId, {
			environmentVariables: this.envvars
		}).subscribe({
			next: () => {
				this.refreshData();
				this.submitted = true;
				this.error = null;
			},
			error: (err) => {
				console.log(err.error.errors);
				this.error = err;
				this.submitted = false;
			}
		});
	}

	validateEnvVars() {
		let isValid = true;
		this.envvars.forEach((envvar: any) => {
			envvar.errors = [];

			if (envvar.key === '') {
				envvar.errors.push('Must specify key');
				isValid = false;
			}

			if (envvar.value === '') {
				envvar.errors.push('Must specify value');
				isValid = false;
			}
		});

		return isValid;
	}

	refreshData() {
		this.loading = true;
		this.envVarService.apiEnvironmentvariableGet().subscribe(
			{
				next: (vm) => {
					this.error = null;
					this.envvars = vm.environmentVariables.filter(element => element.channelId == this.channelId);
					this.originalEnvVars = this.envvars.map((v: any) => {
						return {
							id: v.id,
							channelId: v.channelId,
							key: v.key,
							value: v.value
						}
					});
					this.loading = false;
				},
				error: (err) => {
					console.log(err.error.errors);
					this.error = err;
					this.submitted = false;
					this.loading = false;
				}
			});
	}
}

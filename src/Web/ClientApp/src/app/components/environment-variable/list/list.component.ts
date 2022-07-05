import {
    ChannelService,
} from 'src/app/core/api/v1';
import { Component, EventEmitter, Input, Output, OnChanges, SimpleChange, ViewChild } from '@angular/core';
import { faBackward, faSave, faTrash } from '@fortawesome/free-solid-svg-icons';

import { SuccessComponent } from '../../helpers/success/success.component';

@Component({
    selector: 'app-environment-variable-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css'],
})
export class ListComponent implements OnChanges {
	@Input() channelId = '';
	@Input() originalEnvVars: any = [];
	@ViewChild(SuccessComponent) success: SuccessComponent = new SuccessComponent;

	@Output()
  	updated: EventEmitter<Array<any>> = new EventEmitter<Array<any>>();

	envvars: any = [];

	error: any = null;
	faBackward = faBackward;
	faTrash = faTrash;
	faSave = faSave;

	constructor(private readonly channelService: ChannelService) { }

	ngOnChanges(changes: { [propName: string]: SimpleChange }): void {
		if(changes['channelId'] && changes['channelId'].previousValue != changes['channelId'].currentValue) {
			this.success.hide();
		}
		this.refreshData();
	}

    addNewVariable() {
        this.envvars.push({
            channelId: this.channelId,
            key: '',
            value: '',
        });
    }

    inputChanges(changedVar: any) {
        if (!changedVar.id) {
            return;
        }

        const originalVar = this.originalEnvVars.filter(
            (v: any) => v.id === changedVar.id
        )[0];

        if (
            originalVar.key !== changedVar.key ||
            originalVar.value !== changedVar.value
        ) {
            changedVar.isChanged = true;
        } else {
            changedVar.isChanged = false;
        }
    }

    undoChange(changedVar: any) {
		let originalVar = this.originalEnvVars.filter((v: any) => v.id === changedVar.id)[0];
        if (
            originalVar.key !== changedVar.key ||
            originalVar.value !== changedVar.value
        ) {
            changedVar.isChanged = true;
        } else {
            changedVar.isChanged = false;
        }
    }

	save() {
		if (!this.validateEnvVars()) {
			return;
		}

		this.channelService.apiChannelIdPatch(this.channelId, {
			environmentVariables: this.envvars,
		}).subscribe({
			next: () => {
				this.emitUpdated(this.envvars);
				this.refreshData();
				this.success.show();
				this.error = null;
			},
			error: (err) => {
				this.error = err;
			}
		});
	}

    removeVariable(envvar: any) {
        this.envvars = this.envvars.filter((v: any) => v !== envvar);
    }

	emitUpdated(envvars: any) {
		this.updated.emit(envvars);
	}

    validateEnvVars(): boolean {
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
		this.envvars = [];
		this.originalEnvVars.forEach((originalEnvVar: any) => {
			this.envvars.push({
				id: originalEnvVar.id,
				channelId: originalEnvVar.channelId,
				key: originalEnvVar.key,
				value: originalEnvVar.value,
			});
		});
	}
}

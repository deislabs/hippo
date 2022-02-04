import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faPlus, faTrash } from '@fortawesome/free-solid-svg-icons';
import { ChannelDto, ChannelService, EnvironmentVariableService } from 'src/app/core/api/v1';

@Component({
	selector: 'app-channel-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	@Input() appId = '';
	channels: ChannelDto[] = [];
	envvarForm!: FormGroup;
	error = '';
	loading = false;
	submitted = false;
	showChannelForm = false;

	faPlus = faPlus;
	faTrash = faTrash;

	constructor(private readonly channelService: ChannelService) { }

	ngOnInit() {
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

	refreshData() {
		this.channelService.apiChannelGet().subscribe(vm => {
			this.channels = vm.channels.filter(element => element.appId == this.appId);
		});
	}

	get ef() { return this.envvarForm.controls; }

	onAddChannel() {
		this.showChannelForm = true;
	}
}

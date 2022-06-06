import { Component, OnInit } from '@angular/core';
import { AppDto, AppService, JobStatusService } from 'src/app/core/api/v1';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { interval, startWith, Subscription, switchMap } from 'rxjs';
import { JobStatus } from 'src/app/_helpers/constants';

@Component({
	selector: 'app-application-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	apps: any[] = [];
	error = '';
	faPlus = faPlus;
	faCircle = faCircle;

	jobStatus = JobStatus;

	timeInterval!: Subscription;

	constructor(private readonly appService: AppService,
		private readonly jobStatusService: JobStatusService) { }

	ngOnInit(): void {
		this.refreshData();

		this.timeInterval = interval(5000)
		.pipe(
			startWith(0),
			switchMap(() => this.jobStatusService.apiJobstatusGet())
		).subscribe(res => {
			this.updateChannelStatuses(res);
		})
	}

	updateChannelStatuses(channelStatusList: any) {
		let channels = this.getAllChannels();

		channelStatusList.forEach((channelStatus: any) => {
			let channel = channels.filter((channel: any) => channel.id === channelStatus.channelId)[0];
			if (channel) {
				channel.status = channelStatus.status;
			}
		});
	}

	getAllChannels() {
		let allChannels: any = [];

		this.apps.forEach(app => {
			app.channels.forEach((channel: any) => {
				allChannels.push(channel);
			})
		})

		return allChannels;
	}

	ngOnDestroy(): void {
		this
	}

	refreshData() {
		this.appService.apiAppGet().subscribe(vm => (this.apps = vm.apps));
	}
}

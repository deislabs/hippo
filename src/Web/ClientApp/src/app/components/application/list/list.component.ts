import { Component, OnInit } from '@angular/core';
import { AppChannelListItem, AppItem, AppItemPage, AppService, ChannelJobStatusItem, ChannelJobStatusItemPage, JobStatus, JobStatusService } from 'src/app/core/api/v1';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { interval, startWith, Subscription, switchMap } from 'rxjs';

@Component({
	selector: 'app-application-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	apps: AppItem[] | null | undefined = [];
	statuses: ChannelJobStatusItem[] | null | undefined = [];
	error: any = null;
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
		).subscribe((res: ChannelJobStatusItemPage) => {
			this.statuses = res.items;
		})
	}

	getChannelStatus(channel: AppChannelListItem): JobStatus | undefined {
		let channelStatus = this.statuses?.filter((status: ChannelJobStatusItem) => channel.id === status.channelId)[0];
		return channelStatus?.status;
	}

	getStatusColor(status: JobStatus | undefined) {
		switch(status){
			case JobStatus.Unknown:
				return 'gray';
			case JobStatus.Pending:
				return 'yellow';
			case JobStatus.Running:
				return 'green';
			case JobStatus.Dead:
				return 'red';
			default:
				return 'gray';
		}
	}

	getAllChannels() {
		let allChannels: AppChannelListItem[] = [];

		this.apps?.forEach((app: AppItem) => {
			allChannels = allChannels.concat(app.channels);
		})

		return allChannels;
	}

	ngOnDestroy(): void {
		this
	}

	refreshData() {
		this.appService.apiAppGet().subscribe(
			{
				next: vm => (this.apps = vm.items),
				error: (error) => {
					console.log(error);
					this.error = error;
				}
			}
		);
	}
}

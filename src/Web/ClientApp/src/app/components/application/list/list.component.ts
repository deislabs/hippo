import {
    AppChannelListItem,
    AppItem,
    AppsService,
    ChannelJobStatusItem,
    ChannelStatusesService,
    JobStatus,
} from 'src/app/core/api/v1';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { faCircle, faPlus } from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-application-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css'],
})
export class ListComponent implements OnInit, OnDestroy {
    apps: AppItem[] | null | undefined = [];
    statuses: ChannelJobStatusItem[] | null | undefined = [];
    error: any = null;
    faPlus = faPlus;
    faCircle = faCircle;

    jobStatus = JobStatus;

    interval: any = null;
    timeInterval = 5000;

    constructor(
        private readonly appsService: AppsService,
        private readonly channelStatusesService: ChannelStatusesService
    ) {}

    ngOnInit(): void {
        this.refreshData();
        this.getJobStatus();

        this.interval = setInterval(() => {
            this.getJobStatus();
        }, this.timeInterval);
    }

    getJobStatus(): void {
        this.channelStatusesService
            .apiChannelStatusesGet()
            .subscribe((res) => (this.statuses = res.items));
    }

    getChannelStatus(channel: AppChannelListItem): JobStatus | undefined {
        const channelStatus = this.statuses?.filter(
            (status: ChannelJobStatusItem) => channel.id === status.channelId
        )[0];
        return channelStatus?.status;
    }

    getStatusColor(status: JobStatus | undefined) {
        switch (status) {
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
        });

        return allChannels;
    }

    ngOnDestroy(): void {
        clearInterval(this.interval);
        this;
    }

    refreshData() {
        this.appsService.apiAppsGet().subscribe({
            next: (vm) => (this.apps = vm.items),
            error: (error) => {
                console.log(error);
                this.error = error;
            },
        });
    }
}

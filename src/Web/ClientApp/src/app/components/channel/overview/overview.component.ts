import {
    ChannelItem,
    ChannelStatusesService,
    ChannelsService,
    JobStatus,
    RevisionItem,
} from 'src/app/core/api/v1';
import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import {
    faCircle,
    faNetworkWired,
    faTimesCircle,
} from '@fortawesome/free-solid-svg-icons';

import { ComponentTypes } from 'src/app/_helpers/constants';
import { Router } from '@angular/router';
import TimeAgo from 'javascript-time-ago';
import en from 'javascript-time-ago/locale/en';

@Component({
    selector: 'app-channel-overview',
    templateUrl: './overview.component.html',
    styleUrls: ['./overview.component.css'],
})
export class OverviewComponent implements OnChanges, OnInit, OnDestroy {
    @Input() channelId = '';
    channel!: ChannelItem;
    channelStatus!: JobStatus;
    activeRevision!: RevisionItem | undefined;
    publishedAt: string | null | undefined;
    icons = { faCircle, faTimesCircle, faNetworkWired };
    types = ComponentTypes;
    protocol = window.location.protocol;
    loading = false;
    timeAgo: any;

    interval: any = null;
    timeInterval = 5000;

    constructor(
        private readonly channelsService: ChannelsService,
        private readonly channelStatusesService: ChannelStatusesService,
        private router: Router
    ) {
        TimeAgo.addDefaultLocale(en);
        this.timeAgo = new TimeAgo('en-US');
    }

    ngOnInit(): void {
        this.getJobStatus();

        this.interval = setInterval(() => {
            this.getJobStatus();
        }, this.timeInterval);
    }

    ngOnChanges(): void {
        this.refreshData();
    }

    ngOnDestroy(): void {
        clearInterval(this.interval);
    }

    getJobStatus(): void {
        this.channelStatusesService
            .apiChannelStatusesGet(undefined, undefined, this.channelId)
            .subscribe((res) => (this.channelStatus = res.items[0].status));
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

    refreshData() {
        this.loading = true;
        this.channelsService.apiChannelsIdGet(this.channelId).subscribe({
            next: (channel) => {
                !channel
                    ? this.router.navigate(['/404'])
                    : (this.channel = channel);
                this.activeRevision = channel.activeRevision;
                if (channel.lastPublishAt) {
                    const date = new Date(channel.lastPublishAt);
                    this.publishedAt = this.timeAgo.format(date);
                }
                this.loading = false;
            },
            error: (error) => {
                console.log(error);
                this.loading = false;
            },
        });
    }

    getRedirectRoute(route: string): string {
        if (route) {
            if (route.slice(-3) === '...') {
                return route.slice(0, -3);
            } else {
                return route;
            }
        } else return '';
    }
}

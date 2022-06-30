import { ChannelItem, ChannelService, RevisionItem } from 'src/app/core/api/v1';
import { Component, Input, OnChanges } from '@angular/core';
import {
    faCheckCircle,
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
export class OverviewComponent implements OnChanges {
    @Input() channelId = '';
    channel!: ChannelItem;
    activeRevision!: RevisionItem | undefined;
    publishedAt: string | null | undefined;
    icons = { faCheckCircle, faTimesCircle, faNetworkWired };
    types = ComponentTypes;
    protocol = window.location.protocol;
    loading = false;
    timeAgo: any;

    constructor(
        private readonly channelService: ChannelService,
        private router: Router
    ) {
        TimeAgo.addDefaultLocale(en);
        this.timeAgo = new TimeAgo('en-US');
    }

    ngOnChanges(): void {
        this.refreshData();
    }

    refreshData() {
        this.loading = true;
        this.channelService.apiChannelIdGet(this.channelId).subscribe({
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
}

import { ChannelItem, ChannelService, RevisionItem } from 'src/app/core/api/v1';
import { Component, Input, OnChanges } from '@angular/core';
import {
    faCheckCircle,
    faNetworkWired,
    faTimesCircle,
} from '@fortawesome/free-solid-svg-icons';

import { ComponentTypes } from 'src/app/_helpers/constants';
import { Router } from '@angular/router';

@Component({
    selector: 'app-channel-overview',
    templateUrl: './overview.component.html',
    styleUrls: ['./overview.component.css'],
})
export class OverviewComponent implements OnChanges {
    @Input() channelId = '';
    channel!: ChannelItem;
    activeRevision!: RevisionItem | undefined;
    icons = { faCheckCircle, faTimesCircle, faNetworkWired };
    types = ComponentTypes;
    protocol = window.location.protocol;
    loading = false;

    constructor(
        private readonly channelService: ChannelService,
        private router: Router
    ) {}

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
                this.loading = false;
            },
            error: (error) => {
                console.log(error);
                this.loading = false;
            },
        });
    }
}

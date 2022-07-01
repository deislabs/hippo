import { ActivatedRoute, Router } from '@angular/router';
import {
    AppChannelListItem,
    ChannelItem,
    ChannelService,
} from 'src/app/core/api/v1';
import { ApplicationTabs, ComponentTypes } from 'src/app/_helpers/constants';
import { Component, OnInit } from '@angular/core';
import {
    faAngleDown,
    faChartBar,
    faCog,
    faFilter,
    faStream,
} from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-channel',
    templateUrl: './channel.component.html',
    styleUrls: ['./channel.component.css'],
})
export class ChannelComponent implements OnInit {
    icons = { faCog, faStream, faFilter, faChartBar, faAngleDown };
    channel!: ChannelItem;
    channelId!: string;
    selectedChannel!: AppChannelListItem;
    isSelectClicked = false;
    tabs = ApplicationTabs;
    types = ComponentTypes;
    activeTab = ApplicationTabs.Overview;
    protocol = window.location.protocol;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private readonly channelService: ChannelService
    ) {}

    ngOnInit(): void {
        this.route.params.subscribe((params) => {
            this.channelId = params['id'];
            this.refreshData();
        });
    }

    changeTab(tab: string) {
        this.activeTab = tab;
    }

    toggleIsSelectClicked() {
        this.isSelectClicked = !this.isSelectClicked;
    }

    refreshData() {
        this.channelService
            .apiChannelIdGet(this.channelId)
            .subscribe((channel) => {
                !channel
                    ? this.router.navigate(['/404'])
                    : (this.channel = channel);
                this.selectedChannel = <AppChannelListItem>(
                    channel?.appSummary?.channels.filter(
                        (channel) => channel.id === this.channelId
                    )[0]
                );
            });
    }
}

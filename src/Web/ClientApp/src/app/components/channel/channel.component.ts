import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChannelService, ChannelDto, AppChannelSummary } from 'src/app/core/api/v1';
import { ApplicationTabs } from 'src/app/_helpers/constants';
import { faCog, faStream, faFilter, faChartBar, faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
	selector: 'app-channel',
	templateUrl: './channel.component.html',
	styleUrls: ['./channel.component.css']
})
export class ChannelComponent implements OnInit {
	icons = { faCog, faStream, faFilter, faChartBar, faAngleDown };
	channel!: ChannelDto;
	channelId!: string;
	selectedChannel!: AppChannelSummary;
	isSelectClicked: boolean = false;
	tabs = ApplicationTabs;
	activeTab = ApplicationTabs.Overview;
	protocol = window.location.protocol;

	constructor(
		private route: ActivatedRoute,
		private router: Router,
		private readonly channelService: ChannelService) { }

	ngOnInit(): void {
		this.route.params.subscribe(params => {
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
		this.channelService.apiChannelChannelIdGet(this.channelId).subscribe(channel => {
			!channel ? this.router.navigate(['/404']) : this.channel = channel;
			this.selectedChannel = <AppChannelSummary>(channel?.appSummary?.channels.filter((channel) => channel.id === this.channelId)[0]);
		});
	}
}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppDto, ChannelSummaryDto, AppService } from 'src/app/core/api/v1';
import { ApplicationTabs } from 'src/app/_helpers/constants';
import { faCog, faStream, faFilter, faChartBar, faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
	selector: 'app-channel',
	templateUrl: './channel.component.html',
	styleUrls: ['./channel.component.css']
})
export class ChannelComponent implements OnInit {
	icons = { faCog, faStream, faFilter, faChartBar, faAngleDown };
	channelId!: string;
	app!: AppDto;
	selectedChannel!: ChannelSummaryDto;
	isSelectClicked: boolean = false;
	tabs = ApplicationTabs;
	activeTab = ApplicationTabs.Overview;

	constructor(
		private route: ActivatedRoute,
		private router: Router,
		private readonly appService: AppService) { }

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
		this.appService.apiAppChannelIdGet(this.channelId).subscribe(app => {
			!app ? this.router.navigate(['/404']) : this.app = app;
			this.selectedChannel = <ChannelSummaryDto>(app?.channels.filter((channel) => channel.id === this.channelId)[0]);
		});
	}
}

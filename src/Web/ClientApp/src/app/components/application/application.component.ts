import { toBase64String } from '@angular/compiler/src/output/source_map';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppDto, ChannelSummaryDto, AppService } from 'src/app/core/api/v1';
import { ApplicationTabs } from 'src/app/_helpers/constants';
import { faCog, faStream, faFilter, faChartBar, faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
	selector: 'app-application',
	templateUrl: './application.component.html',
	styleUrls: ['./application.component.css']
})
export class ApplicationComponent implements OnInit {
	icons = { faCog, faStream, faFilter, faChartBar, faAngleDown };
	id!: string;
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
		this.route.queryParams.subscribe(params => {
			this.id = params['id'];
			this.refreshData();
		});
	}

	changeTab(tab: string) {
		this.activeTab = tab;
	}

	changeSelectedChannel(channel: ChannelSummaryDto) {
		this.selectedChannel = channel;
	}

	toggleIsSelectClicked() {
		this.isSelectClicked = !this.isSelectClicked;
	}

	refreshData() {
		this.appService.apiAppGet().subscribe(vm => {
			let app = vm.apps.find(element => element.id == this.id);
			app === undefined ? this.router.navigate(['/404']) : this.app = app;
			this.selectedChannel = <ChannelSummaryDto>(app?.channels[0]);
		});
	}
}

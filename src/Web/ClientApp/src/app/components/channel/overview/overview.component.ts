import { Component, OnChanges, Input } from '@angular/core';
import { ChannelDto, ChannelService, RevisionDto } from 'src/app/core/api/v1';
import { ComponentTypes } from 'src/app/_helpers/constants';
import { faCheckCircle, faTimesCircle, faNetworkWired } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';

@Component({
	selector: 'channel-overview',
	templateUrl: './overview.component.html',
	styleUrls: ['./overview.component.css']
})
export class OverviewComponent implements OnChanges {
	@Input() channelId = '';
	channel!: ChannelDto;
	activeRevision!: RevisionDto | undefined;
	icons = { faCheckCircle, faTimesCircle, faNetworkWired };
	types = ComponentTypes;
	protocol = window.location.protocol;
	loading: boolean = false;

	constructor(
		private readonly channelService: ChannelService,
		private router: Router) { }

	ngOnChanges(): void {
		this.refreshData();
	}

	refreshData() {
		this.loading = true;
		this.channelService.apiChannelChannelIdGet(this.channelId).subscribe({
			next: (channel) => {
				!channel ? this.router.navigate(['/404']) : this.channel = channel;
				this.activeRevision = channel.activeRevision;
				this.loading = false;
			},
			error: (error) => {
				console.log(error);
				this.loading = false;
			}
		});
	}
}

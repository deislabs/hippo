import { Component, OnInit, Input } from '@angular/core';
import { ChannelService, RevisionDto } from 'src/app/core/api/v1';
import { ComponentTypes } from 'src/app/_helpers/constants';
import { faCheckCircle, faTimesCircle, faNetworkWired } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'channel-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class OverviewComponent implements OnInit {
  @Input() channelId = '';
  activeRevision!: RevisionDto | undefined;
  icons = { faCheckCircle, faTimesCircle, faNetworkWired };
  types = ComponentTypes;

  constructor(private readonly channelService: ChannelService) { }

  ngOnInit(): void {
    this.refreshData();
  }

  refreshData() {
		this.channelService.apiChannelChannelIdOverviewGet(this.channelId).subscribe(vm => (this.activeRevision = vm.activeRevision));
	}
}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppDto, AppService, ChannelDto, ChannelService } from 'src/app/core/api/v1';

@Component({
  selector: 'app-application-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
	id!: string;
	channels!: ChannelDto[];

  constructor(
		private route: ActivatedRoute,
		private readonly channelService: ChannelService,
	) { }

	ngOnInit(): void {
		this.route.queryParams.subscribe(params => {
			this.id = params['id'];
			this.channelService.apiChannelGet().subscribe(vm => {
				this.channels = vm.channels.filter(element => element.appId == this.id);
			});
		});
	}

}

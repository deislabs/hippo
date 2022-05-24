import { Component, OnInit, Input } from '@angular/core';
import { ChannelService } from 'src/app/core/api/v1';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.css']
})
export class LogsComponent implements OnInit {
  @Input() appId = '';
  @Input() channelId = '';

  logs: Array<string> = [];

  constructor(private readonly channelService: ChannelService) { }

  ngOnInit(): void {
    this.channelService.apiChannelLogsChannelIdGet(this.channelId).subscribe(vm => {
			this.logs = vm.logs;
		});
  }

}

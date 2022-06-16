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
  loading: boolean = false;

  constructor(private readonly channelService: ChannelService) { }

  ngOnInit(): void {
    this.refreshData();
  }

  refreshData(): void {
    this.loading = true;
    
    this.channelService.apiChannelLogsChannelIdGet(this.channelId).subscribe({
      next: (vm) => {
        this.logs = vm.logs;
        this.loading = false;
      },
      error: (error) => {
        console.log(error);
        this.loading = false;
      }
    });
  }

}

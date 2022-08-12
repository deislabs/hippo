import { Component, Input, OnChanges } from '@angular/core';
import { ChannelsService } from 'src/app/core/api/v1';

@Component({
    selector: 'app-logs',
    templateUrl: './logs.component.html',
    styleUrls: ['./logs.component.css'],
})
export class LogsComponent implements OnChanges {
    @Input() appId = '';
    @Input() channelId = '';

    logs: Array<string> = [];
    loading = false;

    constructor(private readonly channelsService: ChannelsService) {}

    ngOnChanges(): void {
        this.refreshData();
    }

    refreshData(): void {
        this.loading = true;

        this.channelsService.apiChannelsIdLogsGet(this.channelId).subscribe({
            next: (vm) => {
                this.logs = vm.logs;
                this.loading = false;
            },
            error: (error) => {
                console.log(error);
                this.loading = false;
            },
        });
    }
}

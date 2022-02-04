import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { ChannelDto, ChannelService } from 'src/app/core/api/v1';

@Component({
	selector: 'app-channel',
	templateUrl: './channel.component.html',
	styleUrls: ['./channel.component.css']
})
export class ChannelComponent implements OnInit {
	@Input() id = '';
	channel!: ChannelDto;
	faTrash = faTrash;

	constructor(private readonly channelService: ChannelService, private router: Router) { }

	ngOnInit(): void {
		this.refreshData();
	}

	refreshData() {
		this.channelService.apiChannelGet().subscribe(vm => {
			let channel = vm.channels.find(element => element.id == this.id);
			channel === undefined ? this.router.navigate(['/404']) : this.channel = channel;
		});
	}

	deleteChannel(id: string) {
		this.channelService.apiChannelIdDelete(id)
		.subscribe({
			next: () => this.refreshData(),
			error: (error) => console.log(error)
		});
	}
}

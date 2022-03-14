import { Component, OnInit } from '@angular/core';
import { AppDto, AppService } from 'src/app/core/api/v1';

@Component({
	selector: 'app-application-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	apps: AppDto[] = [];
	error = '';

	constructor(private readonly appService: AppService) { }

	ngOnInit(): void {
		this.refreshData();
	}

	refreshData() {
		this.appService.apiAppGet().subscribe(vm => (this.apps = vm.apps));
	}

	deleteApp(id:string) {
		this.appService.apiAppIdDelete(id)
		.subscribe({
			next: () => this.refreshData(),
			error: (error) => {
				console.log(error);
				this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
			}
		});
	}

	editApp(id:string) { }
}

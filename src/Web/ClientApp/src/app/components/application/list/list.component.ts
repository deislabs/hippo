import { Component, OnInit } from '@angular/core';
import { AppDto, AppService } from 'src/app/core/api/v1';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
	selector: 'app-application-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
	apps: AppDto[] = [];
	error = '';
	faPlus = faPlus;
	faCircle = faCircle;
	constanta = "ceva";

	constructor(private readonly appService: AppService) { }

	ngOnInit(): void {
		this.refreshData();
	}

	refreshData() {
		this.appService.apiAppGet().subscribe(vm => (this.apps = vm.apps));
	}
}

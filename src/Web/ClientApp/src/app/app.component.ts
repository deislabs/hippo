import { Component } from '@angular/core';
import { AppConfigService } from './_services/app-config.service';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss']
})
export class AppComponent {
	constructor(private readonly appConfigService: AppConfigService) { }

	title = this.appConfigService.title;
}

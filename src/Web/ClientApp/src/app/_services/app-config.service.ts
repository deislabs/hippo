import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AppConfig } from "../_interfaces/app-config";

@Injectable({ providedIn: 'root' })
export class AppConfigService {
	private config: any;

	constructor(private readonly http: HttpClient) { }

	load(defaults?: AppConfig): Promise<AppConfig> {
		return new Promise<AppConfig>(resolve => {
			this.http.get('/assets/config.json').subscribe(
				{
					next: (response) => {
						console.log('using server-side configuration')
						this.config = Object.assign({}, defaults || {}, response || {});
						resolve(this.config);
					},
					error: () => {
						console.log('using default configuration');
						this.config = Object.assign({}, defaults || {});
						resolve(this.config);
					}
				}
			);
		});
	}

	get title() : string {
		return this.config.title;
	}
}

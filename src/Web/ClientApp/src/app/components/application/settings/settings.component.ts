import { Component, OnInit, Input } from '@angular/core';
import { AppService } from 'src/app/core/api/v1';
import { Router } from '@angular/router';
import { faPlus, faTrash } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  @Input() channel: any = {};

  showChannelForm = false;

  faPlus = faPlus;
	faTrash = faTrash;

  constructor(private router: Router,
    private readonly appService: AppService) { }

  ngOnInit(): void {
  }

  deleteApp(id:string) {
		this.appService.apiAppIdDelete(id)
		.subscribe({
			next: () => this.router.navigate(['/']),
			error: (error) => {
				console.log(error);
			}
		});
	}

  addNewChannel() {
		this.showChannelForm = true;
	}

}

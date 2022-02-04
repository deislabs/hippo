import { Component, OnInit } from '@angular/core';
import { faHippo, faUser } from '@fortawesome/free-solid-svg-icons';
import { SessionService } from 'src/app/_services/session.service';

@Component({
	selector: 'app-navbar',
	templateUrl: './navbar.component.html',
	styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
	faHippo = faHippo;
	faUser = faUser;

	constructor(
		private readonly sessionService: SessionService	) { }

	ngOnInit(): void {
	}

	isLoggedIn(): boolean {
		return this.sessionService.isLoggedIn();
	}

	logout(): void {
		this.sessionService.logout();
		location.reload();
	}

}

import { Component, OnInit } from '@angular/core';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { SessionService } from 'src/app/_services/session.service';

@Component({
	selector: 'app-navbar',
	templateUrl: './navbar.component.html',
	styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
	faUser = faUser;
	menuActive = '';

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

	onBurger(): void {
		this.menuActive = (this.menuActive == '' ? 'is-active' : '');
	}
}

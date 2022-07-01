import { Component } from '@angular/core';
import { SessionService } from 'src/app/_services/session.service';
import { faUser } from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
    faUser = faUser;
    menuActive = '';

    constructor(private readonly sessionService: SessionService) {}

    isLoggedIn(): boolean {
        return this.sessionService.isLoggedIn();
    }

    logout(): void {
        this.sessionService.logout();
        location.reload();
    }
    onBurger(): void {
        this.menuActive = this.menuActive == '' ? 'is-active' : '';
    }
}

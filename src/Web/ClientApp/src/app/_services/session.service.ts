import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { AccountService, TokenInfo } from 'src/app/core/api/v1';

@Injectable({
	providedIn: 'root'
})
export class SessionService {
	tokenSubject$: BehaviorSubject<TokenInfo>;
	token$: Observable<TokenInfo>;

	constructor(private readonly accountService: AccountService) {
		this.tokenSubject$ = new BehaviorSubject<TokenInfo>(JSON.parse(localStorage.getItem('token') || '{}'));
		this.token$ = this.tokenSubject$.asObservable();
	}

	// log into the system with the provided credentials
	login(username: string, password: string) {
		return this.accountService.apiAccountCreatetokenPost({ userName: username, password })
			.pipe(map(tokenInfo => {
				localStorage.setItem('token', JSON.stringify(tokenInfo));
				this.tokenSubject$.next(tokenInfo);
				return tokenInfo;
			}));
	}

	logout(): void {
		localStorage.removeItem('token');
		this.tokenSubject$.next({});
	}

	isLoggedIn(): boolean {
		return this.tokenValue.token != null && !this.isExpired();
	}

	isExpired(): boolean {
		return this.getExpiration() <= new Date(Date.now());
	}

	private getExpiration(): Date {
		return new Date(this.tokenValue.expiration || Date.now());
	}

	public get tokenValue(): TokenInfo {
		return this.tokenSubject$.value;
	}
}

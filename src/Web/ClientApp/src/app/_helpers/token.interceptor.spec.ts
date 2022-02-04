import { TestBed } from '@angular/core/testing';

import { TokenInterceptor } from './token.interceptor';

describe('AuthInterceptor', () => {
	beforeEach(() => TestBed.configureTestingModule({
		providers: [
			TokenInterceptor
			]
	}));

	it('should be created', () => {
		const interceptor: TokenInterceptor = TestBed.inject(TokenInterceptor);
		expect(interceptor).toBeTruthy();
	});
});

import {
    HttpErrorResponse,
    HttpEvent,
    HttpHandler,
    HttpInterceptor,
    HttpRequest
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private router: Router) {}

    intercept(
        request: HttpRequest<unknown>,
        next: HttpHandler
    ): Observable<HttpEvent<unknown>> {
        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status === 401) {
                    // 401 errors are most likely going to be because we have an expired token that we need to refresh.
                    //
                    // For now, just redirect to the login page. Hippo needs to support token refresh requests.
                    this.router.navigate(['/login'], {
                        queryParams: {
                            returnUrl: this.router.routerState.snapshot.url
                        }
                    });
                }
                return throwError(() => error);
            })
        );
    }
}

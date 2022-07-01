import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-not-found',
    templateUrl: './not-found.component.html',
    styleUrls: ['./not-found.component.css'],
})
export class NotFoundComponent implements OnInit {
    returnUrl = '/';

    constructor(
        private route: ActivatedRoute,
        private readonly router: Router
    ) {}

    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    goBack(): void {
        this.router.navigate([this.returnUrl]);
    }
}

import { Component, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-success',
    templateUrl: './success.component.html',
    styleUrls: ['./success.component.css'],
})
export class SuccessComponent {
    visible = false;
    counter = 0;
    interval: any;
    intervalResetTime = 3000;
    incrementInterval = 10;

    show(): void {
        clearInterval(this.interval);
        this.visible = true;
        this.startTimer();
    }

    hide(): void {
        clearInterval(this.interval);
        this.visible = false;
        this.counter = 0;
    }

    startTimer(): void {
        this.counter = 0;

        this.interval = setInterval(() => {
            if (this.counter === this.intervalResetTime) {
                clearInterval(this.interval);
                this.visible = false;
                this.counter = 0;
            } else {
                this.counter += this.incrementInterval;
            }
        }, this.incrementInterval);
    }
}

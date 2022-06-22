import {
    Component,
    Injectable,
} from '@angular/core';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-success',
    templateUrl: './success.component.html',
    styleUrls: ['./success.component.css']
})
export class SuccessComponent {
    visible: boolean = false;
    counter: number = 0;
    interval: any = null;

    constructor() {}

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
            if (this.counter === 3) {
                clearInterval(this.interval);
                this.visible = false;
                this.counter = 0;
            } else {
                this.counter++;
            }
        }, 1000);
    }
}

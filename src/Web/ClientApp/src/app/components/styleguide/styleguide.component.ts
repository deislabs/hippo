import {
    faBold,
    faCheck,
    faEdit,
    faItalic,
    faPlus,
    faTimes,
    faTrash,
    faUnderline,
} from '@fortawesome/free-solid-svg-icons';

import { Component } from '@angular/core';
import { faGithub } from '@fortawesome/free-brands-svg-icons';

@Component({
    // eslint-disable-next-line @angular-eslint/component-selector
    selector: 'styleguide',
    templateUrl: './styleguide.component.html',
    styleUrls: ['./styleguide.component.css'],
})
export class StyleguideComponent {
    faGithub = faGithub;
    faPlus = faPlus;
    faEdit = faEdit;
    faTrash = faTrash;
    faTimes = faTimes;
    faCheck = faCheck;
    faUnderline = faUnderline;
    faItalic = faItalic;
    faBold = faBold;
}

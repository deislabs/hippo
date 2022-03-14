import { Component, OnInit } from '@angular/core';
import { faGithub } from '@fortawesome/free-brands-svg-icons';
import { faBold, faCheck, faEdit, faItalic, faPlus, faTimes, faTrash, faUnderline } from '@fortawesome/free-solid-svg-icons';

@Component({
	selector: 'styleguide',
	templateUrl: './styleguide.component.html',
	styleUrls: ['./styleguide.component.css']
})
export class StyleguideComponent implements OnInit {
	faGithub = faGithub;
	faPlus = faPlus;
	faEdit = faEdit;
	faTrash = faTrash;
	faTimes = faTimes;
	faCheck = faCheck;
	faUnderline = faUnderline;
	faItalic = faItalic;
	faBold = faBold;

	constructor() { }

	ngOnInit(): void { }

}

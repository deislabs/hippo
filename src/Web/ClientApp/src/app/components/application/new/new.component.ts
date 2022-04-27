import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faLock, faUser } from '@fortawesome/free-solid-svg-icons';
import { AppService, StorageService } from 'src/app/core/api/v1';

@Component({
  selector: 'app-application-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.css']
})
export class NewComponent implements OnInit {
  error = '';
  appForm!: FormGroup;
  storageList: Array<string> = [];
  loading = false;
  submitted = false;
  returnUrl = '/';
  faUser = faUser;
  faLock = faLock;

  constructor(private readonly appService: AppService, private readonly storageService: StorageService, private route: ActivatedRoute, private readonly router: Router) { }

  ngOnInit() {
    this.appForm = new FormGroup({
      name: new FormControl('', [
        Validators.required
      ]),
      storageId: new FormControl('', [
        Validators.required,
      ])
    });
    this.storageList = ['first_storage', 'second_storage'];

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get f() { return this.appForm.controls; }

  onSubmit() {
    this.submitted = true;

    if (this.appForm.invalid) {
      return;
    }

    this.loading = true;
    this.appService.apiAppPost({ name: this.f['name'].value, storageId: this.f['storageId'].value })
    .subscribe({
      // TODO: navigate to registration confirmation page
      next: () => this.router.navigate(['/app']),
      error: (error) => {
        console.log(error);
        this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        this.loading = false;
      }
    });
  }
}

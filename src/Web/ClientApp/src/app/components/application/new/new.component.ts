import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faDatabase, faSearch, faTable } from '@fortawesome/free-solid-svg-icons';
import { AppService, StorageService } from 'src/app/core/api/v1';
import { debounceTime, distinctUntilChanged, mergeMap, catchError } from 'rxjs/operators';
import { Subject, of } from 'rxjs';

@Component({
  selector: 'app-application-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.css']
})
export class NewComponent implements OnInit {
  faDatabase = faDatabase;
  error = '';
  appForm!: FormGroup;
  storageList: Array<any> = [];
  loading = false;
  storageListLoading = false;
  submitted = false;
  returnUrl = '/';
  faTable = faTable;
  faSearch = faSearch;
  selectedStorage: string|null = null;
  storageQuery: string = '';

  storageQueryChanged = new Subject<string>();

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

    this.registerDebouncedStorageQuery();
    
    this.storageListLoading = true;
    this.storageQueryChanged.next('');

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  registerDebouncedStorageQuery() {
    this.storageQueryChanged.pipe(
      debounceTime(500),
      distinctUntilChanged())
      .subscribe((query: any) => {
        this.queryStorages(query).subscribe(
        {
          next: (response) => {
            this.storageList = response.storages;
            this.storageListLoading = false;
          },
          error: (error) => {
            console.log(error);
            this.storageList = [];
            this.storageListLoading = false;
          }
        });
      });
  }

  searchStorages(newQuery: any) {
    this.storageQueryChanged.next(newQuery);
  }

  queryStorages(newQuery: any) {
    this.storageListLoading = true;
    return this.storageService.apiStorageExportGet(newQuery, 0, 5);
  }

  selectStorage(storage: string) {
    this.selectedStorage = storage;
    this.appForm.patchValue({
      storageId: storage
   });
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

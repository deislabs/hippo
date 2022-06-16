import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faDatabase, faSearch, faTable } from '@fortawesome/free-solid-svg-icons';
import { AppService, ChannelRevisionSelectionStrategy, ChannelService, StorageService } from 'src/app/core/api/v1';
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

  constructor(private readonly appService: AppService, private readonly channelService: ChannelService, private readonly storageService: StorageService, private route: ActivatedRoute, private readonly router: Router) { }

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
    
    this.searchStorages('');

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
    this.storageListLoading = true;
    this.storageQueryChanged.next(newQuery);
  }

  queryStorages(newQuery: any) {
    return this.storageService.apiStorageGet(newQuery, 0, 5);
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
      next: (appId) => {
        this.channelService.apiChannelPost({ appId: appId, name: 'Production', revisionSelectionStrategy: ChannelRevisionSelectionStrategy.UseRangeRule, rangeRule: "*", 
            domain: `${this.f['name'].value}.${window.location.hostname}`.replace('_', '-').toLowerCase()})
          .subscribe({
            next: () => this.router.navigate(['/']),
            error: (error) => {
              console.log(error);
              this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'An error occurred while creating the channel';
              // we can still continue at this point; the user will just have to add a channel to the app.
              this.router.navigate(['/']);
            }
          })
      },
      error: (error) => {
        console.log(error);
        this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'An error occurred while creating the application';
        this.loading = false;
      }
    });
  }
}

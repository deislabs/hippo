import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ChannelRevisionSelectionStrategy, ChannelService } from 'src/app/core/api/v1';

@Component({
  selector: 'app-channel-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.css']
})
export class NewComponent implements OnInit {
  appId = '';
  error = '';
  channelForm!: FormGroup;
  loading = false;
  submitted = false;
  returnUrl = '/';

  constructor(private readonly channelService: ChannelService, private route: ActivatedRoute, private readonly router: Router) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
			this.appId = params['id'];
		});

    this.channelForm = new FormGroup({
      name: new FormControl('', [
        Validators.required
      ])
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get f() { return this.channelForm.controls; }

  onSubmit() {
    this.submitted = true;

    if (this.channelForm.invalid) {
      return;
    }

    this.loading = true;
    this.channelService.apiChannelPost({ appId: this.appId, name: this.f['name'].value, revisionSelectionStrategy: ChannelRevisionSelectionStrategy.NUMBER_0, rangeRule: '*' })
    .subscribe({
      next: () => location.reload(),
      error: (error) => {
        console.log(error);
        this.error = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        this.loading = false;
      }
    });
  }
}

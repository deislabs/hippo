import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ChannelRevisionSelectionStrategy, ChannelService } from 'src/app/core/api/v1';

@Component({
  selector: 'app-channel-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.css']
})
export class NewComponent implements OnInit {
  @Input() appId: any = {};
  error: any = null;
  channelForm!: FormGroup;
  loading = false;
  submitted = false;
  returnUrl = '/';

  @Output()
  cancelled: EventEmitter<string> = new EventEmitter<string>();

  @Output()
  created: EventEmitter<Object> = new EventEmitter<Object>();

  constructor(private readonly channelService: ChannelService, private route: ActivatedRoute, private readonly router: Router) { }

  ngOnInit() {
    this.channelForm = new FormGroup({
      name: new FormControl('', [
        Validators.required
      ])
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get f() { return this.channelForm.controls; }

  cancel() {
    this.cancelled.emit('')
  }

  emitCreated(channelId: string, name: string) {
    this.created.emit({channelId: channelId, name: name})
  }

  onSubmit() {
    this.submitted = true;

    if (this.channelForm.invalid) {
      return;
    }

    this.loading = true;
    this.channelService.apiChannelPost({ appId: this.appId, name: this.f['name'].value, revisionSelectionStrategy: ChannelRevisionSelectionStrategy.UseRangeRule, rangeRule: '*' })
    .subscribe({
      next: (channelId) => {
        this.emitCreated(channelId, this.f['name'].value);
      },
      error: (error) => {
        console.log(error);
        this.error = error;
        this.loading = false;
      }
    });
  }
}

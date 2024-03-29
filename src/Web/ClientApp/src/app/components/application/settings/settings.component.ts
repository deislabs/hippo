import { AppsService, ChannelsService } from 'src/app/core/api/v1';
import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faPlus, faTrash } from '@fortawesome/free-solid-svg-icons';

import { Router } from '@angular/router';

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.css'],
})
export class SettingsComponent implements OnInit {
    @Input() channel: any = {};

    showChannelForm = false;
    isEditingAppInfo = false;

    editAppName = '';
    editChannelName = '';

    editChannelId = '';

    faPlus = faPlus;
    faTrash = faTrash;
    faEdit = faEdit;

    loading = false;

    constructor(
        private router: Router,
        private readonly appsService: AppsService,
        private readonly channelsService: ChannelsService
    ) {}

    ngOnInit(): void {
        this.editAppName = this.channel.appSummary.name;
    }

    deleteApp(id: string) {
        this.appsService.apiAppsIdDelete(id).subscribe({
            next: () => this.router.navigate(['/']),
            error: (error) => {
                console.log(error);
            },
        });
    }

    editAppInfo() {
        if (this.editAppName !== this.channel.appSummary.name) {
            this.appsService
                .apiAppsIdPut(this.channel.appSummary.id, {
                    id: this.channel.appSummary.id,
                    name: this.editAppName,
                })
                .subscribe({
                    next: () => {
                        this.channel.appSummary.name = this.editAppName;
                        this.isEditingAppInfo = false;
                    },
                    error: (error) => {
                        console.log(error);
                    },
                });
        } else {
            this.cancelEditingAppInfo();
        }
    }

    startEditingAppInfo() {
        this.isEditingAppInfo = true;
        this.editAppName = this.channel.appSummary.name;
    }

    cancelEditingAppInfo() {
        this.isEditingAppInfo = false;
    }

    addNewChannel() {
        this.showChannelForm = true;
    }

    deleteChannel(channelId: string) {
        if (this.channel.appSummary.channels.length > 1) {
            this.channelsService.apiChannelsIdDelete(channelId).subscribe({
                next: () => {
                    this.channel.appSummary.channels =
                        this.channel.appSummary.channels.filter(
                            (channel: any) => channel.id !== channelId
                        );

                    if (this.channel.id === channelId) {
                        this.router.navigate([
                            `/channel/${this.channel.appSummary.channels[0].id}`,
                        ]);
                    }
                },
                error: (error) => {
                    console.log(error);
                },
            });
        }
    }

    startEditingChannelInfo(channel: any) {
        this.editChannelName = channel.name;
        this.editChannelId = channel.id;
    }

    cancelEditingChannelInfo() {
        this.editChannelId = '';
    }

    editChannelInfo(channel: any) {
        if (this.editChannelName !== this.channel.name) {
            this.channelsService.apiChannelsIdGet(channel.id).subscribe({
                next: (channelsDetails) => {
                    this.channelsService
                        .apiChannelsIdPut(channel.id, {
                            id: channelsDetails.id,
                            name: this.editChannelName,
                            revisionSelectionStrategy:
                                channelsDetails.revisionSelectionStrategy,
                            rangeRule: channelsDetails.rangeRule,
                            domain: channelsDetails.domain,
                            activeRevisionId:
                                channelsDetails.activeRevision?.id,
                        })
                        .subscribe({
                            next: () => {
                                channel.name = this.editChannelName;
                                this.editChannelId = '';
                            },
                            error: (error) => {
                                console.log(error);
                            },
                        });
                },
                error: (error) => {
                    console.log(error);
                },
            });
        } else {
            this.cancelEditingChannelInfo();
        }
    }

    cancelNewChannelAddition() {
        this.showChannelForm = false;
    }

    newChannelCreated(channel: any) {
        this.showChannelForm = false;
        this.channel.appSummary.channels.push({
            id: channel.channelId,
            name: channel.name,
        });
    }
}

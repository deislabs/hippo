import {
    ModuleWithProviders,
    NgModule,
    Optional,
    SkipSelf
} from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';

import { AccountService } from './api/account.service';
import { AppService } from './api/app.service';
import { CertificateService } from './api/certificate.service';
import { ChannelService } from './api/channel.service';
import { EnvironmentVariableService } from './api/environmentVariable.service';
import { JobStatusService } from './api/jobStatus.service';
import { RevisionService } from './api/revision.service';
import { StorageService } from './api/storage.service';

@NgModule({
    imports: [],
    declarations: [],
    exports: [],
    providers: []
})
export class ApiModule {
    public static forRoot(
        configurationFactory: () => Configuration
    ): ModuleWithProviders<ApiModule> {
        return {
            ngModule: ApiModule,
            providers: [
                { provide: Configuration, useFactory: configurationFactory }
            ]
        };
    }

    constructor(
        @Optional() @SkipSelf() parentModule: ApiModule,
        @Optional() http: HttpClient
    ) {
        if (parentModule) {
            throw new Error(
                'ApiModule is already loaded. Import in your base AppModule only.'
            );
        }
        if (!http) {
            throw new Error(
                'You need to import the HttpClientModule in your AppModule! \n' +
                    'See also https://github.com/angular/angular/issues/20575'
            );
        }
    }
}

import { APP_INITIALIZER, NgModule } from '@angular/core';
import {
    ApiModule,
    Configuration,
    ConfigurationParameters
} from './core/api/v1';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { JDENTICON_CONFIG, NgxJdenticonModule } from 'ngx-jdenticon';

import { AppComponent } from './app.component';
import { AppConfigService } from './_services/app-config.service';
import { AppRoutingModule } from './app-routing.module';
import { AuthInterceptor } from './_helpers/auth.interceptor';
import { BrowserModule } from '@angular/platform-browser';
import { ChannelComponent } from './components/channel/channel.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { HealthCheckComponent } from './components/health-check/health-check.component';
import { ListComponent } from './components/application/list/list.component';
import { ListComponent as ListEnvironmentVariableComponent } from './components/environment-variable/list/list.component';
import { LoginComponent } from './components/account/login/login.component';
import { LogsComponent } from './components/channel/logs/logs.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { NewComponent as NewChannelComponent } from './components/channel/new/new.component';
import { NewComponent } from './components/application/new/new.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { OverviewComponent } from './components/channel/overview/overview.component';
import { RegisterComponent } from './components/account/register/register.component';
import { SessionService } from './_services/session.service';
import { SettingsComponent } from './components/application/settings/settings.component';
import { StyleguideComponent } from './components/styleguide/styleguide.component';
import { SuccessComponent } from './components/helpers/success/success.component';
import { TokenInterceptor } from './_helpers/token.interceptor';
import { WarningComponent } from './components/helpers/warning/warning.component';

export function apiConfigFactory(): Configuration {
    const params: ConfigurationParameters = {
        basePath: `${window.location.protocol}//${window.location.host}`
    };
    return new Configuration(params);
}

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        RegisterComponent,
        ListComponent,
        ListEnvironmentVariableComponent,
        NavbarComponent,
        NotFoundComponent,
        StyleguideComponent,
        NewComponent,
        NewChannelComponent,
        ChannelComponent,
        HealthCheckComponent,
        SettingsComponent,
        LogsComponent,
        OverviewComponent,
        WarningComponent,
        SuccessComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ApiModule.forRoot(apiConfigFactory),
        HttpClientModule,
        ReactiveFormsModule,
        FormsModule,
        FontAwesomeModule,
        NgxJdenticonModule
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
            deps: [SessionService]
        },
        {
            provide: JDENTICON_CONFIG,
            useValue: {
                replaceMode: 'observe'
            }
        },
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [AppConfigService],
            useFactory: (appConfigService: AppConfigService) => () =>
                appConfigService.load()
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}

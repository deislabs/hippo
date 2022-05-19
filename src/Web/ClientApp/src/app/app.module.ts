import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppRoutingModule } from './app-routing.module';
import { ApiModule, Configuration, ConfigurationParameters } from './core/api/v1';
import { AppComponent } from './app.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TokenInterceptor } from './_helpers/token.interceptor';
import { NavbarComponent } from './components/navbar/navbar.component';
import { StyleguideComponent } from './components/styleguide/styleguide.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { AuthInterceptor } from './_helpers/auth.interceptor';
import { SessionService } from './_services/session.service';
import { ListComponent } from './components/application/list/list.component';
import { ListComponent as ListEnvironmentVariableComponent } from './components/environment-variable/list/list.component';
import { LoginComponent } from './components/account/login/login.component';
import { RegisterComponent } from './components/account/register/register.component';
import { NewComponent } from './components/application/new/new.component';
import { NewComponent as NewChannelComponent } from './components/channel/new/new.component';
import { ChannelComponent } from './components/channel/channel.component';
import { HealthCheckComponent } from './components/health-check/health-check.component';
import { environment } from './../environments/environment';
import { SettingsComponent } from './components/application/settings/settings.component';
import { NgxJdenticonModule, JDENTICON_CONFIG } from 'ngx-jdenticon';

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
	],
	imports: [
		BrowserModule,
		AppRoutingModule,
		ApiModule.forRoot(apiConfigFactory),
		HttpClientModule,
		ReactiveFormsModule,
		FormsModule,
		FontAwesomeModule,
		NgxJdenticonModule,
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
				replaceMode: "observe"
			}
		}
	],
	bootstrap: [AppComponent]
})
export class AppModule { }

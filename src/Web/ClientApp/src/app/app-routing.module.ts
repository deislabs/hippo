import { Injectable, NgModule } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { RouterModule, RouterStateSnapshot, Routes, TitleStrategy } from "@angular/router";
import { environment } from "src/environments/environment";
import { LoginComponent } from "./components/account/login/login.component";
import { RegisterComponent } from "./components/account/register/register.component";
import { ListComponent } from "./components/application/list/list.component";
import { NewComponent } from "./components/application/new/new.component";
import { ChannelComponent } from "./components/channel/channel.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";
import { StyleguideComponent } from "./components/styleguide/styleguide.component";
import { AuthGuard } from "./_helpers/auth.guard";
import { AppConfigService } from "./_services/app-config.service";

const routes: Routes = [
	{ path: '404', component: NotFoundComponent, title: 'Not Found' },
	{ path: 'channel/:id', component: ChannelComponent, canActivate: [AuthGuard], title: 'Overview' },
	{ path: 'app/new', component: NewComponent, canActivate: [AuthGuard], title: 'Create a new App' },
	{ path: 'login', component: LoginComponent, title: 'Log in' },
	{ path: 'register', component: RegisterComponent , title: 'Sign up' },
	{ path: 'styleguide', component: StyleguideComponent, title: 'Style Guide' },
	{ path: '', component: ListComponent, canActivate: [AuthGuard], title: 'Dashboard' },
	{ path: '**', redirectTo: '404' }
];

@Injectable()
export class TemplatePageTitleStrategy extends TitleStrategy {
	constructor(private readonly title: Title, private readonly appConfigService: AppConfigService) {
		super();
	}

	override updateTitle(routerState: RouterStateSnapshot) {
		const title = this.buildTitle(routerState);
		if (title !== undefined) {
			this.title.setTitle(`${this.appConfigService.title} | ${title}`);
		}
	}
}

@NgModule({
	imports: [RouterModule.forRoot(routes, { useHash: true })],
	exports: [RouterModule],
	providers: [
		{provide: TitleStrategy, useClass: TemplatePageTitleStrategy},
	]
})

export class AppRoutingModule { }

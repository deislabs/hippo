import { Injectable, NgModule } from '@angular/core';
import {
    RouterModule,
    RouterStateSnapshot,
    Routes,
    TitleStrategy,
} from '@angular/router';

import { AppConfigService } from './_services/app-config.service';
import { AuthGuard } from './_helpers/auth.guard';
import { ChannelComponent } from './components/channel/channel.component';
import { ListComponent } from './components/application/list/list.component';
import { LoginComponent } from './components/account/login/login.component';
import { NewComponent } from './components/application/new/new.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { RegisterComponent } from './components/account/register/register.component';
import { StyleguideComponent } from './components/styleguide/styleguide.component';
import { Title } from '@angular/platform-browser';

const routes: Routes = [
    { path: '404', component: NotFoundComponent, title: 'Not Found' },
    {
        path: 'channel/:id',
        component: ChannelComponent,
        canActivate: [AuthGuard],
        title: 'Overview',
    },
    {
        path: 'app/new',
        component: NewComponent,
        canActivate: [AuthGuard],
        title: 'Create a new App',
    },
    { path: 'login', component: LoginComponent, title: 'Log in' },
    { path: 'register', component: RegisterComponent, title: 'Sign up' },
    {
        path: 'styleguide',
        component: StyleguideComponent,
        title: 'Style Guide',
    },
    {
        path: '',
        component: ListComponent,
        canActivate: [AuthGuard],
        title: 'Dashboard',
    },
    { path: '**', redirectTo: '404' },
];

@Injectable()
export class TemplatePageTitleStrategy extends TitleStrategy {
    constructor(
        private readonly title: Title,
        private readonly appConfigService: AppConfigService
    ) {
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
        { provide: TitleStrategy, useClass: TemplatePageTitleStrategy },
    ],
})
export class AppRoutingModule {}

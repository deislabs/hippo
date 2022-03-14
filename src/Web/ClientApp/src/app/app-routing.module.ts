import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { LoginComponent } from "./components/account/login/login.component";
import { RegisterComponent } from "./components/account/register/register.component";
import { ApplicationComponent } from "./components/application/application.component";
import { ListComponent } from "./components/application/list/list.component";
import { NewComponent } from "./components/application/new/new.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";
import { StyleguideComponent } from "./components/styleguide/styleguide.component";
import { AuthGuard } from "./_helpers/auth.guard";

const routes: Routes = [
	{ path: '404', component: NotFoundComponent },
	{ path: 'app', component: ApplicationComponent, canActivate: [AuthGuard] },
	{ path: 'app/new', component: NewComponent, canActivate: [AuthGuard] },
	{ path: 'login', component: LoginComponent },
	{ path: 'register', component: RegisterComponent },
	{ path: 'styleguide', component: StyleguideComponent },
	{ path: '', component: ListComponent, canActivate: [AuthGuard] },
    { path: '**', redirectTo: '404' }
];

@NgModule({
	imports: [RouterModule.forRoot(routes, { useHash: true })],
	exports: [RouterModule]
})

export class AppRoutingModule { }

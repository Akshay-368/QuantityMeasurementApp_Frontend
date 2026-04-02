import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';

import { DashboardComponent } from './components/dashboard/dashboard';

import { authGuard } from './guards/auth-guard';

export const routes: Routes = [

{
path:'',
component:LoginComponent
},

{
path:'home',
component:DashboardComponent,
canActivate:[authGuard]
},

{
path:'dashboard',
redirectTo:'home'
},

{
path:'**',
redirectTo:''
}

];
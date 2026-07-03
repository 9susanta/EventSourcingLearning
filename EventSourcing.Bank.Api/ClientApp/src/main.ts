import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, withEnabledBlockingInitialNavigation } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { ErrorInterceptor } from './app/interceptors/error.interceptor';
import { AccountListComponent } from './app/account-list.component';
import { AccountDetailComponent } from './app/account-detail.component';
import { API_BASE_URL } from './app/services/api.service';
import { environment } from './environments/environment';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter([
      { path: '', component: AccountListComponent },
      { path: 'accounts/create', loadComponent: () => import('./app/account-create.component').then(m => m.AccountCreateComponent) },
      { path: 'accounts/:id', component: AccountDetailComponent },
      { path: 'accounts/:id/edit', loadComponent: () => import('./app/account-edit.component').then(m => m.AccountEditComponent) },
      { path: 'accounts/:id/history', loadComponent: () => import('./app/account-history.component').then(m => m.AccountHistoryComponent) }
    ], withEnabledBlockingInitialNavigation()),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ]
}).catch(err => console.error(err));

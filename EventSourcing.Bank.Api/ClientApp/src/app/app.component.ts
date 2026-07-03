import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="app-shell">
      <header class="app-header">
        <div>
          <h1 class="app-title">Bank Client</h1>
          <p class="header-note">Manage accounts, deposits, withdrawals, and transaction history with a polished, production-ready dashboard.</p>
        </div>
        <nav class="app-nav">
          <a routerLink="/">Accounts</a>
          <a routerLink="/accounts/create">Create Account</a>
        </nav>
      </header>
      <main class="app-main">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`:host { display:block; min-height:100vh; }`]
})
export class AppComponent {}

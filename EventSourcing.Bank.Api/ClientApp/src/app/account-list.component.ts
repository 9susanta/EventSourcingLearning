import { Component, inject, signal } from '@angular/core';
import { NgForOf, NgIf, CurrencyPipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService, Account } from './services/api.service';

@Component({
  selector: 'account-list',
  standalone: true,
  imports: [NgIf, NgForOf, RouterModule, CurrencyPipe],
  template: `
    <section class="card">
      <div class="card-header">
        <div>
          <h2>Accounts</h2>
          <p class="header-note">Browse all accounts and quickly navigate to details for deposits, withdrawals, and transaction history.</p>
        </div>
        <a class="button" routerLink="/accounts/create">New Account</a>
      </div>

      <div *ngIf="loading()" class="status-info">Loading accounts…</div>

      <table *ngIf="!loading() && accounts().length" class="table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Balance</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let account of accounts()">
            <td>{{ account.name }}</td>
            <td>{{ (account.balance || 0) | currency:'USD':'symbol':'1.2-2' }}</td>
            <td><a class="button secondary" [routerLink]="['/accounts', account.id]">View</a></td>
          </tr>
        </tbody>
      </table>

      <div *ngIf="!loading() && !accounts().length" class="status-info">No accounts available yet. Create one to get started.</div>
    </section>
  `
})
export class AccountListComponent {
  private readonly api = inject(ApiService);
  readonly accounts = signal<Account[]>([]);
  readonly loading = signal(false);

  constructor() {
    this.loadAccounts();
  }

  loadAccounts(): void {
    this.loading.set(true);
    this.api.getAccounts().subscribe({
      next: data => {
        this.accounts.set(data || []);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }
}

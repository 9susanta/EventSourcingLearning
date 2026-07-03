import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf, CurrencyPipe } from '@angular/common';
import { ApiService, Account } from './services/api.service';
import { TransactionHistoryComponent } from './transaction-history.component';

@Component({
  selector: 'account-detail',
  standalone: true,
  imports: [NgIf, CurrencyPipe, FormsModule, RouterModule, TransactionHistoryComponent],
  template: `
    <section class="card" *ngIf="account(); else loadingOrNotFound">
      <div class="card-header">
        <div>
          <h2>{{ account()?.name }}</h2>
          <p class="header-note">Account ID {{ account()?.id }}</p>
        </div>
        <div class="button-group">
          <button class="button secondary" (click)="edit()">Edit</button>
          <a class="button secondary" [routerLink]="['/accounts', account()?.id, 'history']">History</a>
        </div>
      </div>

      <div class="grid">
        <div>
          <strong>Balance</strong>
          <p>{{ (account()?.balance || 0) | currency:'USD':'symbol':'1.2-2' }}</p>
        </div>
        <div>
          <strong>Status</strong>
          <p>{{ account()?.balance != null && account()?.balance >= 0 ? 'Active' : 'Review Required' }}</p>
        </div>
      </div>

      <div class="form-row">
        <label class="form-field">
          <span>Transaction amount</span>
          <input class="input" type="number" [(ngModel)]="amount" />
        </label>
      </div>

      <div class="button-group">
        <button class="button" (click)="doDeposit()">Deposit</button>
        <button class="button secondary" (click)="doWithdraw()">Withdraw</button>
      </div>

      <p *ngIf="message()" class="status-success">{{ message() }}</p>
      <p *ngIf="error()" class="status-error">{{ error() }}</p>

      <transaction-history [accountId]="account()?.id"></transaction-history>
    </section>

    <ng-template #loadingOrNotFound>
      <div *ngIf="loading()" class="status-info">Loading account information…</div>
      <div *ngIf="!loading() && !account()" class="status-info">Account not found.</div>
    </ng-template>
  `
})
export class AccountDetailComponent {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly account = signal<Account | null>(null);
  readonly amount = signal(0);
  readonly message = signal('');
  readonly error = signal('');
  readonly loading = signal(false);

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.load(id);
    }
  }

  load(id: string) {
    this.message.set('');
    this.error.set('');
    this.loading.set(true);
    this.api.getAccount(id).subscribe({
      next: account => {
        this.account.set(account);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load account.');
        this.loading.set(false);
      }
    });
  }

  doDeposit() {
    const account = this.account();
    if (!account) return;

    this.message.set('');
    this.error.set('');
    this.loading.set(true);
    this.api.deposit(account.id, this.amount()).subscribe({
      next: (updated: Account) => {
        this.account.set(updated);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Deposit failed.');
        this.loading.set(false);
      }
    });
  }

  doWithdraw() {
    const account = this.account();
    if (!account) return;

    this.message.set('');
    this.error.set('');
    this.loading.set(true);
    this.api.withdraw(account.id, this.amount()).subscribe({
      next: (updated: Account) => {
        this.account.set(updated);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Withdraw failed.');
        this.loading.set(false);
      }
    });
  }

  edit() {
    const account = this.account();
    if (!account) return;
    this.router.navigate(['/accounts', account.id, 'edit']);
  }
}

import { Component, inject, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService, Account } from './services/api.service';

@Component({
  selector: 'account-edit',
  standalone: true,
  imports: [NgIf, FormsModule],
  template: `
    <section class="card" *ngIf="account(); else loading">
      <div class="card-header">
        <div>
          <h2>Edit Account</h2>
          <p class="header-note">Update account details and balance information.</p>
        </div>
      </div>

      <form (ngSubmit)="save()">
        <div class="form-field">
          <label>Name</label>
          <input class="input" [(ngModel)]="account().name" name="name" required />
        </div>

        <div class="form-field">
          <label>Balance</label>
          <input class="input" type="number" [(ngModel)]="account().balance" name="balance" />
        </div>

        <div class="button-group">
          <button class="button" type="submit">Save</button>
          <button class="button secondary" type="button" (click)="cancel()">Cancel</button>
        </div>
      </form>

      <p *ngIf="message()" class="status-success">{{ message() }}</p>
      <p *ngIf="error()" class="status-error">{{ error() }}</p>
    </section>

    <ng-template #loading>
      <div class="status-info">Loading account details…</div>
    </ng-template>
  `
})
export class AccountEditComponent {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly account = signal<Account | null>(null);
  readonly message = signal('');
  readonly error = signal('');

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.load(id);
    }
  }

  load(id: string) {
    this.api.getAccount(id).subscribe({
      next: account => this.account.set(account),
      error: () => this.error.set('Failed to load')
    });
  }

  save() {
    const account = this.account();
    if (!account) return;

    this.message.set('');
    this.error.set('');

    this.api.updateAccount(account.id, { name: account.name, balance: account.balance }).subscribe({
      next: acc => this.router.navigate(['/accounts', acc.id]),
      error: () => this.error.set('Save failed')
    });
  }

  cancel() {
    const account = this.account();
    if (account) {
      this.router.navigate(['/accounts', account.id]);
    } else {
      this.router.navigate(['/']);
    }
  }
}

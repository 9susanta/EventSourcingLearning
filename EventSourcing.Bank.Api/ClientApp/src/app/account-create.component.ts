import { Component, inject, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from './services/api.service';

@Component({
  selector: 'account-create',
  standalone: true,
  imports: [NgIf, FormsModule],
  template: `
    <section class="card">
      <div class="card-header">
        <div>
          <h2>Create Account</h2>
          <p class="header-note">Add a new account to the system and set an initial balance.</p>
        </div>
      </div>

      <form (ngSubmit)="create()">
        <div class="form-field">
          <label>Name</label>
          <input class="input" [(ngModel)]="name" name="name" required />
        </div>

        <div class="form-field">
          <label>Initial Balance</label>
          <input class="input" type="number" [(ngModel)]="balance" name="balance" />
        </div>

        <div class="button-group">
          <button class="button" type="submit">Create</button>
          <button class="button secondary" type="button" (click)="cancel()">Cancel</button>
        </div>
      </form>

      @if (message()) {
        <p class="status-success">{{ message() }}</p>
      }
      @if (error()) {
        <p class="status-error">{{ error() }}</p>
      }
    </section>
  `
})
export class AccountCreateComponent {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  readonly name = signal('');
  readonly balance = signal<number | null>(null);
  readonly message = signal('');
  readonly error = signal('');

  create() {
    this.message.set('');
    this.error.set('');
    const body: any = { name: this.name() };
    if (this.balance() != null) {
      body.balance = this.balance();
    }

    this.api.createAccount(body).subscribe({
      next: acc => this.router.navigate(['/accounts', acc.id]),
      error: () => this.error.set('Create failed.')
    });
  }

  cancel() {
    this.router.navigate(['/']);
  }
}

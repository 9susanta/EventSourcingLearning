import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from './services/api.service';

@Component({
  selector: 'account-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section>
      <h2>Create Account</h2>
      <form (ngSubmit)="create()">
        <div>
          <label>Name: <input [(ngModel)]="name" name="name" required /></label>
        </div>
        <div>
          <label>Initial Balance: <input type="number" [(ngModel)]="balance" name="balance" /></label>
        </div>
        <div style="margin-top:0.5rem">
          <button type="submit">Create</button>
          <button type="button" (click)="cancel()">Cancel</button>
        </div>
      </form>
      <div *ngIf="message" style="color:green">{{message}}</div>
      <div *ngIf="error" style="color:red">{{error}}</div>
    </section>
  `
})
export class AccountCreateComponent {
  name = '';
  balance: number | null = null;
  message = '';
  error = '';

  constructor(private api: ApiService, private router: Router) {}

  create() {
    this.message = '';
    this.error = '';
    const body: any = { name: this.name };
    if (this.balance !== null && this.balance !== undefined) body.balance = this.balance;
    this.api.createAccount(body).subscribe({
      next: acc => { this.message = 'Created'; this.router.navigate(['/accounts', acc.id]); },
      error: () => this.error = 'Create failed'
    });
  }

  cancel() { this.router.navigate(['/']); }
}

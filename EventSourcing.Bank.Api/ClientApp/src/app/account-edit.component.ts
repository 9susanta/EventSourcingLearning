import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService, Account } from './services/api.service';

@Component({
  selector: 'account-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section *ngIf="account">
      <h2>Edit Account</h2>
      <form (ngSubmit)="save()">
        <div>
          <label>Name: <input [(ngModel)]="account.name" name="name" required /></label>
        </div>
        <div>
          <label>Balance: <input type="number" [(ngModel)]="account.balance" name="balance" /></label>
        </div>
        <div style="margin-top:0.5rem">
          <button type="submit">Save</button>
          <button type="button" (click)="cancel()">Cancel</button>
        </div>
      </form>
      <div *ngIf="message" style="color:green">{{message}}</div>
      <div *ngIf="error" style="color:red">{{error}}</div>
    </section>
    <div *ngIf="!account">Loading...</div>
  `
})
export class AccountEditComponent implements OnInit {
  account: Account | null = null;
  message = '';
  error = '';

  constructor(private route: ActivatedRoute, private router: Router, private api: ApiService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.load(id);
  }

  load(id: string) {
    this.api.getAccount(id).subscribe({ next: a => this.account = a, error: () => this.error = 'Failed to load' });
  }

  save() {
    if (!this.account) return;
    this.message = '';
    this.error = '';
    this.api.updateAccount(this.account.id, { name: this.account.name, balance: this.account.balance }).subscribe({ next: acc => { this.message = 'Saved'; this.router.navigate(['/accounts', acc.id]); }, error: () => this.error = 'Save failed' });
  }

  cancel() { if (this.account) this.router.navigate(['/accounts', this.account.id]); else this.router.navigate(['/']); }
}

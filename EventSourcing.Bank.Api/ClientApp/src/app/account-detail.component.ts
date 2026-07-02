import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService, Account } from './services/api.service';
import { TransactionHistoryComponent } from './transaction-history.component';


@Component({
  selector: 'account-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, TransactionHistoryComponent],
  template: `
    <section *ngIf="account">
      <h2>{{ account.name }}</h2>
      <p><strong>Id:</strong> {{ account.id }}</p>
      <p><strong>Balance:</strong> {{ account.balance || 0 }}</p>

      <div>
        <label>Amount: <input type="number" [(ngModel)]="amount" /></label>
        <button (click)="doDeposit()">Deposit</button>
        <button (click)="doWithdraw()">Withdraw</button>
        <button (click)="edit()">Edit Account</button>
      </div>

      <div *ngIf="message" style="margin-top:0.5rem;color:green">{{message}}</div>
      <div *ngIf="error" style="margin-top:0.5rem;color:red">{{error}}</div>

      <transaction-history [accountId]="account.id"></transaction-history>
    </section>
    <div *ngIf="!account">Loading...</div>
  `
})
export class AccountDetailComponent implements OnInit {
  account: Account | null = null;
  amount = 0;
  message = '';
  error = '';

  constructor(private route: ActivatedRoute, private api: ApiService, private router: Router) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.load(id);
    }
  }

  load(id: string) {
    this.message = '';
    this.error = '';
    this.api.getAccount(id).subscribe({ next: a => this.account = a, error: e => this.error = 'Failed to load account' });
  }

  doDeposit() {
    if (!this.account) return;
    this.message = '';
    this.error = '';
    this.api.deposit(this.account.id, this.amount).subscribe({ next: () => { this.message = 'Deposit successful'; this.load(this.account!.id); }, error: () => this.error = 'Deposit failed' });
  }

  doWithdraw() {
    if (!this.account) return;
    this.message = '';
    this.error = '';
    this.api.withdraw(this.account.id, this.amount).subscribe({ next: () => { this.message = 'Withdraw successful'; this.load(this.account!.id); }, error: () => this.error = 'Withdraw failed' });
  }

  edit() {
    if (!this.account) return;
    this.router.navigate(['/accounts', this.account.id, 'edit']);
  }
}

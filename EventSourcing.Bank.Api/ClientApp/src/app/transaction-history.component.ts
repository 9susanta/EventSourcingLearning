import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './services/api.service';

@Component({
  selector: 'transaction-history',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section *ngIf="accountId">
      <h3>Transaction History</h3>
      <div *ngIf="loading">Loading...</div>
      <ul *ngIf="!loading && transactions.length">
        <li *ngFor="let t of transactions">{{t.date || t.timestamp}}: {{t.type}} {{t.amount}}</li>
      </ul>
      <div *ngIf="!loading && !transactions.length">No transactions</div>
    </section>
  `
})
export class TransactionHistoryComponent implements OnChanges {
  @Input() accountId?: string;
  transactions: any[] = [];
  loading = false;

  constructor(private api: ApiService) {}

  ngOnChanges(): void {
    if (this.accountId) this.load(this.accountId);
  }

  load(id: string) {
    this.loading = true;
    this.api.getTransactions(id).subscribe({ next: t => { this.transactions = t || []; this.loading = false }, error: () => { this.loading = false } });
  }
}

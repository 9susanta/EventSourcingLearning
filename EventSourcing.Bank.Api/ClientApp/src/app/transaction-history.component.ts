import { Component, Input, OnChanges, inject, signal } from '@angular/core';
import { NgForOf, NgIf } from '@angular/common';
import { ApiService, EventDto } from './services/api.service';

interface TransactionHistoryEntry {
  type: string;
  description: string;
  amount: string;
  occurredAt: string;
}

@Component({
  selector: 'transaction-history',
  standalone: true,
  imports: [NgIf, NgForOf],
  template: `
    <section *ngIf="accountId">
      <h3>Transaction History</h3>
      <div *ngIf="loading()">Loading...</div>

      <ul *ngIf="!loading() && transactions().length" class="transaction-list">
        <li *ngFor="let t of transactions()">
          <div>
            <span class="transaction-type">{{ t.type }}</span>
            <span class="transaction-meta">{{ t.occurredAt }}</span>
          </div>
          <div>{{ t.description }}</div>
          <div class="transaction-amount" *ngIf="t.amount">{{ t.amount }}</div>
        </li>
      </ul>

      <div *ngIf="!loading() && !transactions().length">No transactions</div>
    </section>
  `
})
export class TransactionHistoryComponent implements OnChanges {
  @Input() accountId?: string;
  transactions = signal<TransactionHistoryEntry[]>([]);
  loading = signal(false);

  private readonly api = inject(ApiService);

  ngOnChanges(): void {
    if (this.accountId) {
      this.load(this.accountId);
    }
  }

  load(id: string) {
    this.loading.set(true);
    this.api.getTransactions(id).subscribe({
      next: transactions => {
        this.transactions.set((transactions || []).map(evt => this.formatEvent(evt)));
        this.loading.set(false);
      },
      error: () => {
        this.transactions.set([]);
        this.loading.set(false);
      }
    });
  }

  private formatEvent(event: EventDto): TransactionHistoryEntry {
    const eventData = this.parseEventData(event.data);
    const amount = this.extractNumber(eventData, 'amount');
    const occurredAt = new Date(event.occurredAt).toLocaleString();

    switch (event.type) {
      case 'AccountCreatedEvent':
        return {
          type: 'Account opened',
          description: eventData?.accountHolder
            ? `Account opened for ${eventData.accountHolder}`
            : 'Account opened',
          amount: this.formatAmount(this.extractNumber(eventData, 'initialBalance') ?? amount),
          occurredAt
        };
      case 'MoneyDepositedEvent':
        return {
          type: 'Deposit',
          description: amount != null ? `Deposited ${this.formatAmount(amount)}` : 'Deposit completed',
          amount: this.formatAmount(amount),
          occurredAt
        };
      case 'MoneyWithdrawnEvent':
        return {
          type: 'Withdrawal',
          description: amount != null ? `Withdrew ${this.formatAmount(amount)}` : 'Withdrawal completed',
          amount: this.formatAmount(amount),
          occurredAt
        };
      default:
        return {
          type: event.type,
          description: event.data,
          amount: amount != null ? this.formatAmount(amount) : '',
          occurredAt
        };
    }
  }

  private parseEventData(data: string): any {
    try {
      return JSON.parse(data);
    } catch {
      return undefined;
    }
  }

  private extractNumber(data: any, key: string): number | undefined {
    if (!data) return undefined;
    const raw = data[key] ?? data[key.toLowerCase()] ?? data[key.charAt(0).toUpperCase() + key.slice(1)];
    return typeof raw === 'number' ? raw : undefined;
  }

  private formatAmount(amount?: number): string {
    return amount != null ? `$${amount.toFixed(2)}` : '';
  }
}

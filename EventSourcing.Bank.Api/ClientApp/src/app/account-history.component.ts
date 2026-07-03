import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgIf } from '@angular/common';
import { TransactionHistoryComponent } from './transaction-history.component';

@Component({
  selector: 'account-history',
  standalone: true,
  imports: [NgIf, TransactionHistoryComponent],
  template: `
    <section *ngIf="accountId(); else missingId">
      <transaction-history [accountId]="accountId()"></transaction-history>
    </section>
    <ng-template #missingId>
      <p>Account ID is missing from the route.</p>
    </ng-template>
  `
})
export class AccountHistoryComponent {
  private readonly route = inject(ActivatedRoute);
  readonly accountId = signal(this.route.snapshot.paramMap.get('id') ?? undefined);
}

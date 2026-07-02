import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService, Account } from './services/api.service';

@Component({
  selector: 'account-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <section>
      <h2>Accounts</h2>
      <div style="margin-bottom:0.5rem"><a routerLink="/accounts/create">Create Account</a></div>
      <div *ngIf="loading">Loading...</div>
      <ul *ngIf="!loading && accounts.length">
        <li *ngFor="let a of accounts"><a [routerLink]="['/accounts', a.id]">{{a.name}} ({{a.balance || 0}})</a></li>
      </ul>
      <div *ngIf="!loading && !accounts.length">No accounts yet</div>
    </section>
  `
})
export class AccountListComponent implements OnInit {
  accounts: Account[] = [];
  loading = false;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loading = true;
    this.api.getAccounts()
      .subscribe({ next: data => { this.accounts = data || []; this.loading = false }, error: () => { this.loading = false } });
  }
}

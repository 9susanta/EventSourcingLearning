import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Account { id: string; name: string; balance?: number }

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  getAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>('/api/accounts');
  }

  getAccount(id: string): Observable<Account> {
    return this.http.get<Account>(`/api/accounts/${id}`);
  }

  deposit(accountId: string, amount: number): Observable<any> {
    return this.http.post(`/api/accounts/${accountId}/deposit?amount=${amount}`, null);
  }

  withdraw(accountId: string, amount: number): Observable<any> {
    return this.http.post(`/api/accounts/${accountId}/withdraw?amount=${amount}`, null);
  }

  // Create a new account. Body shape depends on API; use { name, balance? }
  createAccount(body: { name: string; balance?: number }): Observable<Account> {
    return this.http.post<Account>(`/api/accounts`, body);
  }

  // Update existing account
  updateAccount(id: string, body: { name?: string; balance?: number }): Observable<Account> {
    return this.http.put<Account>(`/api/accounts/${id}`, body);
  }

  // Get transaction history for an account
  getTransactions(accountId: string): Observable<any[]> {
    return this.http.get<any[]>(`/api/accounts/${accountId}/transactions`);
  }

  get<T>(url: string): Observable<T> { return this.http.get<T>(url); }
  post<T>(url: string, body: any): Observable<T> { return this.http.post<T>(url, body); }
  put<T>(url: string, body: any): Observable<T> { return this.http.put<T>(url, body); }
  delete<T>(url: string): Observable<T> { return this.http.delete<T>(url); }
}

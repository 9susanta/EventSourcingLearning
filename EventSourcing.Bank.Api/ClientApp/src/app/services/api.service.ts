import { Injectable, inject, InjectionToken } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');
export interface Account { id: string; name: string; balance?: number }
export interface EventDto { type: string; data: string; version: number; occurredAt: string }

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  getAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.baseUrl}/api/accounts`);
  }

  getAccount(id: string): Observable<Account> {
    return this.http.get<Account>(`${this.baseUrl}/api/accounts/${id}`);
  }

  deposit(accountId: string, amount: number): Observable<Account> {
    // send amount in JSON body (matches Postman collection)
    return this.http.post<Account>(`${this.baseUrl}/api/accounts/${accountId}/deposit`, { amount });
  }

  withdraw(accountId: string, amount: number): Observable<Account> {
    // send amount in JSON body (matches Postman collection)
    return this.http.post<Account>(`${this.baseUrl}/api/accounts/${accountId}/withdraw`, { amount });
  }

  createAccount(body: { name: string; balance?: number }): Observable<Account> {
    return this.http.post<Account>(`${this.baseUrl}/api/accounts`, body);
  }

  updateAccount(id: string, body: { name?: string; balance?: number }): Observable<Account> {
    return this.http.put<Account>(`${this.baseUrl}/api/accounts/${id}`, body);
  }

  getTransactions(accountId: string): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(`${this.baseUrl}/api/accounts/${accountId}/events`);
  }

  get<T>(url: string): Observable<T> { return this.http.get<T>(url); }
  post<T>(url: string, body: any): Observable<T> { return this.http.post<T>(url, body); }
  put<T>(url: string, body: any): Observable<T> { return this.http.put<T>(url, body); }
  delete<T>(url: string): Observable<T> { return this.http.delete<T>(url); }
}

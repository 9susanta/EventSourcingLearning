import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule],
  template: `
    <header style="padding:0.5rem;background:#f4f4f4">
      <a routerLink="/">Home</a>
    </header>
    <main style="padding:1rem">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`:host { display:block; }`]
})
export class AppComponent {}

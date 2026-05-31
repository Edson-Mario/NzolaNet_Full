import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="footer">
      <p>&copy; 2024 NzolaNet. Todos os direitos reservados.</p>
    </footer>
  `,
  styles: [`
    .footer {
      text-align: center;
      padding: 20px;
      color: var(--text-secondary);
      font-size: 13px;
      border-top: 1px solid var(--border);
      background: var(--surface);
    }
  `]
})
export class FooterComponent {}

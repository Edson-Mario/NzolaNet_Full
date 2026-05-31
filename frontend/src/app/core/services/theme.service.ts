import { Injectable, signal, effect } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly THEME_KEY = 'nzola_theme';
  isDark = signal<boolean>(false);

  constructor() {
    const saved = localStorage.getItem(this.THEME_KEY);
    this.isDark.set(saved === 'dark');
    this.applyTheme();

    effect(() => {
      this.isDark();
      this.applyTheme();
    });
  }

  toggle(): void {
    this.isDark.update(v => !v);
    localStorage.setItem(this.THEME_KEY, this.isDark() ? 'dark' : 'light');
  }

  private applyTheme(): void {
    document.body.classList.toggle('dark-theme', this.isDark());
  }
}

import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';
import { ApiService } from '../../core/services/api.service';
import { SignalRService } from '../../core/services/signalr.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy {
  auth = inject(AuthService);
  theme = inject(ThemeService);
  private api = inject(ApiService);
  private signalr = inject(SignalRService);
  private subs: Subscription[] = [];

  unreadCount = signal(0);
  showNotifDropdown = false;
  showUserMenu = false;

  ngOnInit(): void {
    if (this.auth.isLoggedIn() && !this.auth.isAdmin()) {
      this.loadUnreadCount();
      this.signalr.startConnection();

      this.subs.push(
        this.signalr.newNotification$.subscribe(({ notification, unreadCount }) => {
          if (notification.utilizadorId === this.auth.currentUser()?.id) {
            this.unreadCount.set(unreadCount);
          }
        })
      );
    }
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  refreshFeed(): void {
    this.signalr.refreshFeed$.next();
  }

  private loadUnreadCount(): void {
    this.api.getUnreadCount().subscribe(count => this.unreadCount.set(count));
  }

  toggleTheme(): void {
    this.theme.toggle();
  }

  logout(): void {
    this.auth.logout();
  }
}

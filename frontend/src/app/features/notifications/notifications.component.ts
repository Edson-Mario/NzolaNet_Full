import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { SignalRService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { Notification } from '../../core/models/notification.model';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, TimeAgoPipe],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss'
})
export class NotificationsComponent implements OnInit, OnDestroy {
  private api = inject(ApiService);
  private signalr = inject(SignalRService);
  private auth = inject(AuthService);
  private subs: Subscription[] = [];

  notifications: Notification[] = [];
  loading = true;

  ngOnInit(): void {
    this.loadNotifications();
    this.signalr.startConnection();

    this.subs.push(
      this.signalr.newNotification$.subscribe(({ notification }) => {
        if (notification.utilizadorId === this.auth.currentUser()?.id) {
          this.notifications.unshift(notification);
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  private loadNotifications(): void {
    this.api.getNotifications().subscribe({
      next: (notifs) => {
        this.notifications = notifs;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  markAsRead(id: number): void {
    this.api.markAsRead(id).subscribe(() => {
      const notif = this.notifications.find(n => n.id === id);
      if (notif) notif.lida = true;
    });
  }

  deleteNotification(id: number): void {
    this.api.deleteNotification(id).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== id);
      }
    });
  }

  markAllAsRead(): void {
    this.api.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => n.lida = true);
    });
  }

  trackByNotificationId(index: number, n: Notification): number {
    return n.id;
  }

  private readonly iconMap: Record<string, string> = {
    baze: '&#128077;',
    comentario: '&#128172;',
    seguidor: '&#128100;'
  };

  getIcon(tipo: string): string {
    return this.iconMap[tipo] || '&#128276;';
  }
}

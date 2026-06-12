import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
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

  notifications = signal<Notification[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.loadNotifications();
    this.signalr.startConnection();

    this.subs.push(
      this.signalr.newNotification$.subscribe(({ notification }) => {
        if (notification.utilizadorId === this.auth.currentUser()?.id) {
          this.notifications.update(n => [notification, ...n]);
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
        this.notifications.set(notifs);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  markAsRead(id: number): void {
    this.api.markAsRead(id).subscribe(() => {
      this.notifications.update(n =>
        n.map(x => x.id === id ? { ...x, lida: true } : x)
      );
    });
  }

  deleteNotification(id: number): void {
    this.api.deleteNotification(id).subscribe({
      next: () => {
        this.notifications.update(n => n.filter(x => x.id !== id));
      }
    });
  }

  markAllAsRead(): void {
    this.api.markAllAsRead().subscribe(() => {
      this.notifications.update(n => n.map(x => ({ ...x, lida: true })));
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

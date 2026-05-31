import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Post } from '../models/post.model';
import { Comment } from '../models/comment.model';
import { Notification } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  newPost$ = new Subject<Post>();
  postDeleted$ = new Subject<number>();
  newComment$ = new Subject<Comment>();
  bazeToggled$ = new Subject<{ post: number; bazed: boolean; count: number }>();
  postUpdated$ = new Subject<Post>();
  newNotification$ = new Subject<{ notification: Notification; unreadCount: number }>();
  refreshFeed$ = new Subject<void>();

  startConnection(): void {
    if (this.hubConnection) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5227/hub/nzolanet', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR error:', err));

    this.registerEvents();
  }

  private registerEvents(): void {
    this.hubConnection.on('NewPost', (post: Post) => {
      this.newPost$.next(post);
    });

    this.hubConnection.on('PostDeleted', (id: number) => {
      this.postDeleted$.next(id);
    });

    this.hubConnection.on('NewComment', (comment: Comment) => {
      this.newComment$.next(comment);
    });

    this.hubConnection.on('BazeToggled', (postId: number, bazed: boolean, count: number) => {
      this.bazeToggled$.next({ post: postId, bazed, count });
    });

    this.hubConnection.on('PostUpdated', (post: Post) => {
      this.postUpdated$.next(post);
    });

    this.hubConnection.on('NewNotification', (notification: Notification, unreadCount: number) => {
      this.newNotification$.next({ notification, unreadCount });
    });
  }

  stopConnection(): void {
    this.hubConnection?.stop();
  }
}

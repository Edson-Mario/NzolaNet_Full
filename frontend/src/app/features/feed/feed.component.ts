import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { SignalRService } from '../../core/services/signalr.service';
import { Post } from '../../core/models/post.model';
import { PostCardComponent } from '../posts/post-card/post-card.component';
import { CreatePostComponent } from '../posts/create-post/create-post.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CommonModule, PostCardComponent, CreatePostComponent],
  templateUrl: './feed.component.html',
  styleUrl: './feed.component.scss'
})
export class FeedComponent implements OnInit, OnDestroy {
  private api = inject(ApiService);
  private signalr = inject(SignalRService);
  private subs: Subscription[] = [];

  posts = signal<Post[]>([]);
  loading = signal(true);
  page = 1;
  feedType: 'following' | 'all' = 'following';

  ngOnInit(): void {
    this.loadPosts();
    this.signalr.startConnection();

    this.subs.push(
      this.signalr.newPost$.subscribe(post => {
        this.posts.update(p => {
          if (p.find(x => x.id === post.id)) return p;
          return [post, ...p];
        });
      }),
      this.signalr.postDeleted$.subscribe(id => {
        this.posts.update(p => p.filter(x => x.id !== id));
      }),
      this.signalr.bazeToggled$.subscribe(({ post: postId, bazed, count }) => {
        this.posts.update(p =>
          p.map(x => x.id === postId ? { ...x, bazesCount: count, isBazed: bazed } : x)
        );
      }),
      this.signalr.postUpdated$.subscribe(updated => {
        this.posts.update(p =>
          p.map(x => x.id === updated.id ? updated : x)
        );
      }),
      this.signalr.refreshFeed$.subscribe(() => this.refreshPosts())
    );
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  switchFeed(type: 'following' | 'all'): void {
    this.feedType = type;
    this.page = 1;
    this.loading.set(true);
    this.loadPosts();
  }

  loadPosts(): void {
    const request$ = this.feedType === 'following'
      ? this.api.getFollowingFeed(this.page)
      : this.api.getFeed(this.page);

    request$.subscribe({
      next: (posts) => {
        this.posts.set(posts);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private refreshPosts(): void {
    const request$ = this.feedType === 'following'
      ? this.api.getFollowingFeed(this.page)
      : this.api.getFeed(this.page);

    request$.subscribe({
      next: (posts) => {
        this.posts.set(posts);
      }
    });
  }

  trackByPostId(index: number, post: Post): number {
    return post.id;
  }

  onPostCreated(post: Post): void {
    this.posts.update(p => {
      if (p.find(x => x.id === post.id)) return p;
      return [post, ...p];
    });
  }

  onPostDeleted(id: number): void {
    this.posts.update(p => p.filter(x => x.id !== id));
  }
}

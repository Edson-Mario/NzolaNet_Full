import { Component, inject, OnInit, OnDestroy } from '@angular/core';
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

  posts: Post[] = [];
  loading = true;
  page = 1;
  feedType: 'following' | 'all' = 'following';

  ngOnInit(): void {
    this.loadPosts();
    this.signalr.startConnection();

    this.subs.push(
      this.signalr.newPost$.subscribe(post => {
        if (!this.posts.find(p => p.id === post.id)) {
          this.posts.unshift(post);
        }
      }),
      this.signalr.postDeleted$.subscribe(id => {
        this.posts = this.posts.filter(p => p.id !== id);
      }),
      this.signalr.bazeToggled$.subscribe(({ post: postId, bazed, count }) => {
        const post = this.posts.find(p => p.id === postId);
        if (post) {
          post.bazesCount = count;
          post.isBazed = bazed;
        }
      }),
      this.signalr.postUpdated$.subscribe(updated => {
        const index = this.posts.findIndex(p => p.id === updated.id);
        if (index !== -1) {
          this.posts[index] = updated;
        }
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
    this.loading = true;
    this.loadPosts();
  }

  loadPosts(): void {
    const request$ = this.feedType === 'following'
      ? this.api.getFollowingFeed(this.page)
      : this.api.getFeed(this.page);

    request$.subscribe({
      next: (posts) => {
        this.posts = posts;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  private refreshPosts(): void {
    const request$ = this.feedType === 'following'
      ? this.api.getFollowingFeed(this.page)
      : this.api.getFeed(this.page);

    request$.subscribe({
      next: (posts) => {
        this.posts = posts;
      }
    });
  }

  trackByPostId(index: number, post: Post): number {
    return post.id;
  }

  onPostCreated(post: Post): void {
    if (!this.posts.find(p => p.id === post.id)) {
      this.posts.unshift(post);
    }
  }

  onPostDeleted(id: number): void {
    this.posts = this.posts.filter(p => p.id !== id);
  }
}

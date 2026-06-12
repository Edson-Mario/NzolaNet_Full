import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { Post } from '../../../core/models/post.model';
import { PostCardComponent } from '../../posts/post-card/post-card.component';
import { EditProfileComponent } from '../edit-profile/edit-profile.component';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, RouterModule, PostCardComponent, EditProfileComponent],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss'
})
export class ProfilePageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private api = inject(ApiService);
  auth = inject(AuthService);

  user = signal<User | null>(null);
  posts = signal<Post[]>([]);
  loading = signal(true);
  isOwnProfile = false;
  showEditModal = false;

  seguidores = signal<User[]>([]);
  seguindo = signal<User[]>([]);
  showFollowersList = false;
  showFollowingList = false;

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const userId = +params['id'];
      this.isOwnProfile = userId === this.auth.currentUser()?.id;
      this.loading.set(true);
      forkJoin({
        user: this.api.getUser(userId),
        posts: this.api.getUserPosts(userId)
      }).subscribe({
        next: ({ user, posts }) => {
          this.user.set(user);
          this.posts.set(posts);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    });
  }

  toggleFollow(): void {
    const u = this.user();
    if (!u) return;

    if (u.isFollowing) {
      this.api.unfollowUser(u.id).subscribe(() => {
        this.user.update(x =>
          x ? { ...x, isFollowing: false, seguidoresCount: x.seguidoresCount! - 1 } : x
        );
      });
    } else {
      this.api.followUser(u.id).subscribe(() => {
        this.user.update(x =>
          x ? { ...x, isFollowing: true, seguidoresCount: x.seguidoresCount! + 1 } : x
        );
      });
    }
  }

  loadSeguidores(): void {
    const u = this.user();
    if (!u) return;
    this.api.getSeguidores(u.id).subscribe({
      next: (list) => {
        this.seguidores.set(list);
        this.showFollowersList = true;
      }
    });
  }

  loadSeguindo(): void {
    const u = this.user();
    if (!u) return;
    this.api.getSeguindo(u.id).subscribe({
      next: (list) => {
        this.seguindo.set(list);
        this.showFollowingList = true;
      }
    });
  }

  closeFollowersList(): void {
    this.showFollowersList = false;
  }

  closeFollowingList(): void {
    this.showFollowingList = false;
  }

  toggleFollowUser(u: User): void {
    if (u.isFollowing) {
      this.api.unfollowUser(u.id).subscribe(() => {
        u.isFollowing = false;
      });
    } else {
      this.api.followUser(u.id).subscribe(() => {
        u.isFollowing = true;
      });
    }
  }

  deactivateAccount(): void {
    this.api.deactivateAccount().subscribe({
      next: () => this.auth.logout()
    });
  }

  activateAccount(): void {
    const u = this.user();
    if (!u) return;
    this.api.activateAccount().subscribe({
      next: () => {
        this.user.update(x => x ? { ...x, isActive: true } : x);
      }
    });
  }

  trackByPostId(index: number, post: Post): number {
    return post.id;
  }

  trackByUserId(index: number, u: User): number {
    return u.id;
  }

  openEditModal(): void {
    this.showEditModal = true;
  }

  closeEditModal(): void {
    this.showEditModal = false;
  }

  onProfileUpdated(updated: User): void {
    this.user.update(u => ({ ...u, ...updated }) as User);
    this.showEditModal = false;
  }

  onPostDeleted(id: number): void {
    this.posts.update(p => p.filter(x => x.id !== id));
  }
}

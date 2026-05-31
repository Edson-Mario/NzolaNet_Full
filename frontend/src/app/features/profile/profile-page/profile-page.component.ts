import { Component, inject, OnInit } from '@angular/core';
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

  user: User | null = null;
  posts: Post[] = [];
  loading = true;
  isOwnProfile = false;
  showEditModal = false;

  seguidores: User[] = [];
  seguindo: User[] = [];
  showFollowersList = false;
  showFollowingList = false;

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const userId = +params['id'];
      this.isOwnProfile = userId === this.auth.currentUser()?.id;
      forkJoin({
        user: this.api.getUser(userId),
        posts: this.api.getUserPosts(userId)
      }).subscribe({
        next: ({ user, posts }) => {
          this.user = user;
          this.posts = posts;
          this.loading = false;
        },
        error: () => this.loading = false
      });
    });
  }

  toggleFollow(): void {
    if (!this.user) return;

    if (this.user.isFollowing) {
      this.api.unfollowUser(this.user.id).subscribe(() => {
        if (this.user) {
          this.user.isFollowing = false;
          this.user.seguidoresCount!--;
        }
      });
    } else {
      this.api.followUser(this.user.id).subscribe(() => {
        if (this.user) {
          this.user.isFollowing = true;
          this.user.seguidoresCount!++;
        }
      });
    }
  }

  loadSeguidores(): void {
    if (!this.user) return;
    this.api.getSeguidores(this.user.id).subscribe({
      next: (list) => {
        this.seguidores = list;
        this.showFollowersList = true;
      }
    });
  }

  loadSeguindo(): void {
    if (!this.user) return;
    this.api.getSeguindo(this.user.id).subscribe({
      next: (list) => {
        this.seguindo = list;
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
    if (!this.user) return;
    this.api.activateAccount().subscribe({
      next: () => {
        if (this.user) {
          this.user.isActive = true;
        }
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
    this.user = { ...this.user, ...updated } as User;
    this.showEditModal = false;
  }

  onPostDeleted(id: number): void {
    this.posts = this.posts.filter(p => p.id !== id);
  }
}

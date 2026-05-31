import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import {
  DashboardStats, AdminUser, TopBazesUser,
  PostListItem, CommentListItem, CreateAdminRequest,
  ChangeEmailRequest, ChangePasswordRequest
} from '../../../core/models/admin.model';
import { Comment } from '../../../core/models/comment.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})

export class DashboardComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);

  stats: DashboardStats | null = null;
  activeView = 'dashboard';
  loading = false;

  // Users
  users: AdminUser[] = [];
  userSearch = '';

  // Posts
  posts: PostListItem[] = [];
  postFilter = '';
  expandedPostComments: { [key: number]: Comment[] } = {};

  // Comments
  comments: CommentListItem[] = [];
  commentFilter = '';

  // Top Bazes
  topBazes: TopBazesUser[] = [];

  // Create Admin
  newAdmin: CreateAdminRequest = { nome: '', email: '', senha: '' };
  adminMessage = '';

  // Change Email/Password
  newEmail = '';
  changePassword: ChangePasswordRequest = { senhaAtual: '', novaSenha: '', confirmarSenha: '' };
  accountMessage = '';

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.api.getDashboardStats().subscribe(stats => this.stats = stats);
  }

  showView(view: string): void {
    this.activeView = view;
    this.loading = true;

    switch (view) {
      case 'users':
        this.loadUsers();
        break;
      case 'active-users':
        this.api.getActiveUsers().subscribe(u => { this.users = u; this.loading = false; });
        break;
      case 'inactive-users':
        this.api.getInactiveUsers().subscribe(u => { this.users = u; this.loading = false; });
        break;
      case 'posts':
        this.loadPosts();
        break;
      case 'comments':
        this.loadComments();
        break;
      case 'top-bazes':
        this.api.getTopBazes().subscribe(u => { this.topBazes = u; this.loading = false; });
        break;
      default:
        this.loading = false;
    }
  }

  // Users
  loadUsers(): void {
    this.api.getAdminUsers(this.userSearch || undefined).subscribe(u => {
      this.users = u;
      this.loading = false;
    });
  }

  searchUsers(): void {
    this.loadUsers();
  }

  deactivateUser(id: number): void {
    this.api.deactivateUser(id).subscribe(() => {
      const u = this.users.find(x => x.id === id);
      if (u) u.isActive = false;
    });
  }

  activateUser(id: number): void {
    this.api.activateUser(id).subscribe(() => {
      const u = this.users.find(x => x.id === id);
      if (u) u.isActive = true;
    });
  }

  deleteUser(id: number): void {
    if (confirm('Tem certeza que deseja eliminar este utilizador?')) {
      this.api.deleteAdminUser(id).subscribe(() => {
        this.users = this.users.filter(u => u.id !== id);
        this.loadStats();
      });
    }
  }

  removeInactive(): void {
    if (confirm('Tem certeza que deseja remover TODOS os utilizadores desativados?')) {
      this.api.removeInactiveUsers().subscribe(() => {
        this.loadUsers();
        this.loadStats();
      });
    }
  }

  // Posts
  loadPosts(): void {
    this.api.getAdminPosts(this.postFilter || undefined).subscribe(p => {
      this.posts = p;
      this.loading = false;
    });
  }

  filterPosts(filter: string): void {
    this.postFilter = filter;
    this.loadPosts();
  }

  toggleComments(postId: number): void {
    if (this.expandedPostComments[postId]) {
      delete this.expandedPostComments[postId];
    } else {
      this.api.getPostComments(postId).subscribe(c => {
        this.expandedPostComments[postId] = c;
      });
    }
  }

  deletePost(id: number): void {
    if (confirm('Tem certeza que deseja eliminar esta publicacao?')) {
      this.api.deleteAdminPost(id).subscribe(() => {
        this.posts = this.posts.filter(p => p.id !== id);
        this.loadStats();
      });
    }
  }

  // Comments
  loadComments(): void {
    this.api.getAdminComments(this.commentFilter || undefined).subscribe(c => {
      this.comments = c;
      this.loading = false;
    });
  }

  filterComments(filter: string): void {
    this.commentFilter = filter;
    this.loadComments();
  }

  deleteComment(id: number): void {
    if (confirm('Tem certeza que deseja eliminar este comentario?')) {
      this.api.deleteAdminComment(id).subscribe(() => {
        this.comments = this.comments.filter(c => c.id !== id);
        this.loadStats();
      });
    }
  }

  // Create Admin
  createAdmin(): void {
    this.api.createAdmin(this.newAdmin).subscribe({
      next: () => {
        this.adminMessage = 'Admin criado com sucesso!';
        this.newAdmin = { nome: '', email: '', senha: '' };
      },
      error: (err) => this.adminMessage = err.error?.message || 'Erro ao criar admin.'
    });
  }

  // Change Email
  changeAdminEmail(): void {
    this.api.changeEmail({ novoEmail: this.newEmail }).subscribe({
      next: () => {
        this.accountMessage = 'Email alterado com sucesso!';
        this.newEmail = '';
      },
      error: (err) => this.accountMessage = err.error?.message || 'Erro ao alterar email.'
    });
  }

  // Change Password
  changeAdminPassword(): void {
    this.api.changePassword(this.changePassword).subscribe({
      next: () => {
        this.accountMessage = 'Senha alterada com sucesso!';
        this.changePassword = { senhaAtual: '', novaSenha: '', confirmarSenha: '' };
      },
      error: (err) => this.accountMessage = err.error?.message || 'Erro ao alterar senha.'
    });
  }

  trackByAdminUserId(index: number, user: AdminUser): number { return user.id; }
  trackByPostListItemId(index: number, post: PostListItem): number { return post.id; }
  trackByCommentListItemId(index: number, c: CommentListItem | Comment): number { return c.id; }
  trackByTopBazesUserId(index: number, user: TopBazesUser): number { return user.id; }

  get userName(): string {
    return this.auth.currentUser()?.nome || '';
  }
}

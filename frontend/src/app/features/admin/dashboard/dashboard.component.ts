import { Component, inject, signal, OnInit } from '@angular/core';
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

  stats = signal<DashboardStats | null>(null);
  activeView = 'dashboard';
  loading = signal(false);

  // Users
  users = signal<AdminUser[]>([]);
  userSearch = '';

  // Posts
  posts = signal<PostListItem[]>([]);
  postFilter = '';
  expandedPostComments = signal<{ [key: number]: Comment[] }>({});

  // Comments
  comments = signal<CommentListItem[]>([]);
  commentFilter = '';

  // Top Bazes
  topBazes = signal<TopBazesUser[]>([]);

  // Create Admin
  newAdmin: CreateAdminRequest = { nome: '', email: '', senha: '' };
  adminMessage = signal('');

  // Change Email/Password
  newEmail = '';
  changePassword: ChangePasswordRequest = { senhaAtual: '', novaSenha: '', confirmarSenha: '' };
  accountMessage = signal('');

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.api.getDashboardStats().subscribe(stats => this.stats.set(stats));
  }

  showView(view: string): void {
    this.activeView = view;
    this.loading.set(true);

    switch (view) {
      case 'users':
        this.loadUsers();
        break;
      case 'active-users':
        this.api.getActiveUsers().subscribe(u => { this.users.set(u); this.loading.set(false); });
        break;
      case 'inactive-users':
        this.api.getInactiveUsers().subscribe(u => { this.users.set(u); this.loading.set(false); });
        break;
      case 'posts':
        this.loadPosts();
        break;
      case 'comments':
        this.loadComments();
        break;
      case 'top-bazes':
        this.api.getTopBazes().subscribe(u => { this.topBazes.set(u); this.loading.set(false); });
        break;
      default:
        this.loading.set(false);
    }
  }

  // Users
  loadUsers(): void {
    this.api.getAdminUsers(this.userSearch || undefined).subscribe(u => {
      this.users.set(u);
      this.loading.set(false);
    });
  }

  searchUsers(): void {
    this.loadUsers();
  }

  deactivateUser(id: number): void {
    this.api.deactivateUser(id).subscribe(() => {
      this.users.update(arr =>
        arr.map(u => u.id === id ? { ...u, isActive: false } : u)
      );
    });
  }

  activateUser(id: number): void {
    this.api.activateUser(id).subscribe(() => {
      this.users.update(arr =>
        arr.map(u => u.id === id ? { ...u, isActive: true } : u)
      );
    });
  }

  deleteUser(id: number): void {
    if (confirm('Tem certeza que deseja eliminar este utilizador?')) {
      this.api.deleteAdminUser(id).subscribe(() => {
        this.users.update(arr => arr.filter(u => u.id !== id));
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
      this.posts.set(p);
      this.loading.set(false);
    });
  }

  filterPosts(filter: string): void {
    this.postFilter = filter;
    this.loadPosts();
  }

  toggleComments(postId: number): void {
    const current = this.expandedPostComments();
    if (current[postId]) {
      this.expandedPostComments.update(map => {
        const { [postId]: _, ...rest } = map;
        return rest;
      });
    } else {
      this.api.getPostComments(postId).subscribe(c => {
        this.expandedPostComments.update(map => ({ ...map, [postId]: c }));
      });
    }
  }

  deletePost(id: number): void {
    if (confirm('Tem certeza que deseja eliminar esta publicacao?')) {
      this.api.deleteAdminPost(id).subscribe(() => {
        this.posts.update(arr => arr.filter(p => p.id !== id));
        this.loadStats();
      });
    }
  }

  // Comments
  loadComments(): void {
    this.api.getAdminComments(this.commentFilter || undefined).subscribe(c => {
      this.comments.set(c);
      this.loading.set(false);
    });
  }

  filterComments(filter: string): void {
    this.commentFilter = filter;
    this.loadComments();
  }

  deleteComment(id: number): void {
    if (confirm('Tem certeza que deseja eliminar este comentario?')) {
      this.api.deleteAdminComment(id).subscribe(() => {
        this.comments.update(arr => arr.filter(c => c.id !== id));
        this.loadStats();
      });
    }
  }

  // Create Admin
  createAdmin(): void {
    this.api.createAdmin(this.newAdmin).subscribe({
      next: () => {
        this.adminMessage.set('Admin criado com sucesso!');
        this.newAdmin = { nome: '', email: '', senha: '' };
      },
      error: (err) => this.adminMessage.set(err.error?.message || 'Erro ao criar admin.')
    });
  }

  // Change Email
  changeAdminEmail(): void {
    this.api.changeEmail({ novoEmail: this.newEmail }).subscribe({
      next: () => {
        this.accountMessage.set('Email alterado com sucesso!');
        this.newEmail = '';
      },
      error: (err) => this.accountMessage.set(err.error?.message || 'Erro ao alterar email.')
    });
  }

  // Change Password
  changeAdminPassword(): void {
    this.api.changePassword(this.changePassword).subscribe({
      next: () => {
        this.accountMessage.set('Senha alterada com sucesso!');
        this.changePassword = { senhaAtual: '', novaSenha: '', confirmarSenha: '' };
      },
      error: (err) => this.accountMessage.set(err.error?.message || 'Erro ao alterar senha.')
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

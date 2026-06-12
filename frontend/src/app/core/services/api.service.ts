import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Post, CreatePostRequest } from '../models/post.model';
import { Comment, CreateCommentRequest } from '../models/comment.model';
import { Notification } from '../models/notification.model';
import { User, UpdateProfileRequest } from '../models/user.model';
import {
  DashboardStats, AdminUser, TopBazesUser,
  PostListItem, CommentListItem, CreateAdminRequest,
  ChangeEmailRequest, ChangePasswordRequest
} from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly BASE_URL = '/api';

  constructor(private http: HttpClient) {}

  // Feed
  getFeed(page = 1, pageSize = 20): Observable<Post[]> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<Post[]>(`${this.BASE_URL}/Feed`, { params });
  }

  getFollowingFeed(page = 1, pageSize = 20): Observable<Post[]> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<Post[]>(`${this.BASE_URL}/Feed/following`, { params });
  }

  // Posts
  getPost(id: number): Observable<Post> {
    return this.http.get<Post>(`${this.BASE_URL}/Publicacao/${id}`);
  }

  createPost(dto: CreatePostRequest): Observable<Post> {
    return this.http.post<Post>(`${this.BASE_URL}/Publicacao`, dto);
  }

  updatePost(id: number, dto: Partial<CreatePostRequest>): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/Publicacao/${id}`, dto);
  }

  deletePost(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/Publicacao/${id}`);
  }

  getUserPosts(userId: number, page = 1, pageSize = 20): Observable<Post[]> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<Post[]>(`${this.BASE_URL}/Publicacao/user/${userId}`, { params });
  }

  // Comments
  getComments(publicacaoId: number): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.BASE_URL}/Comentario/publicacao/${publicacaoId}`);
  }

  createComment(dto: CreateCommentRequest): Observable<Comment> {
    return this.http.post<Comment>(`${this.BASE_URL}/Comentario`, dto);
  }

  deleteComment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/Comentario/${id}`);
  }

  updateComment(id: number, texto: string): Observable<Comment> {
    return this.http.put<Comment>(`${this.BASE_URL}/Comentario/${id}`, { texto });
  }

  // Bazes
  toggleBaze(publicacaoId: number): Observable<{ bazed: boolean }> {
    return this.http.post<{ bazed: boolean }>(`${this.BASE_URL}/Baze/${publicacaoId}`, {});
  }

  // Users
  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.BASE_URL}/Utilizador/${id}`);
  }

  getMe(): Observable<User> {
    return this.http.get<User>(`${this.BASE_URL}/Utilizador/me`);
  }

  updateProfile(dto: any): Observable<User> {
    return this.http.put<User>(`${this.BASE_URL}/Utilizador/profile`, dto);
  }

  followUser(id: number): Observable<any> {
    return this.http.post(`${this.BASE_URL}/Utilizador/${id}/follow`, {});
  }

  unfollowUser(id: number): Observable<any> {
    return this.http.delete(`${this.BASE_URL}/Utilizador/${id}/unfollow`);
  }

  deactivateAccount(): Observable<any> {
    return this.http.put(`${this.BASE_URL}/Utilizador/deactivate`, {});
  }

  activateAccount(): Observable<any> {
    return this.http.put(`${this.BASE_URL}/Utilizador/activate`, {});
  }

  getSeguidores(id: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.BASE_URL}/Utilizador/${id}/seguidores`);
  }

  getSeguindo(id: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.BASE_URL}/Utilizador/${id}/seguindo`);
  }

  // Notifications
  getNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.BASE_URL}/Notificacao`);
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<number>(`${this.BASE_URL}/Notificacao/unread-count`);
  }

  markAsRead(id: number): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/Notificacao/${id}/read`, {});
  }

  markAllAsRead(): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/Notificacao/read-all`, {});
  }

  deleteNotification(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/Notificacao/${id}`);
  }

  // Profile Photo
  removeProfilePhoto(): Observable<User> {
    return this.http.delete<User>(`${this.BASE_URL}/Utilizador/profile/photo`);
  }

  // File Upload
  uploadFile(file: File, folder = 'images'): Observable<{ url: string; path: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string; path: string }>(`${this.BASE_URL}/File/upload?folder=${folder}`, formData);
  }

  // Admin - Dashboard
  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.BASE_URL}/Admin/dashboard`);
  }

  // Admin - Users
  getAdminUsers(search?: string): Observable<AdminUser[]> {
    const params = search ? new HttpParams().set('search', search) : undefined;
    return this.http.get<AdminUser[]>(`${this.BASE_URL}/Admin/users`, { params });
  }

  getActiveUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.BASE_URL}/Admin/users/active`);
  }

  getInactiveUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.BASE_URL}/Admin/users/inactive`);
  }

  updateUserRole(id: number, role: string): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/Admin/users/${id}/role`, role);
  }

  deactivateUser(id: number): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/Admin/users/${id}/deactivate`, {});
  }

  activateUser(id: number): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/Admin/users/${id}/activate`, {});
  }

  deleteAdminUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/Admin/users/${id}`);
  }

  removeInactiveUsers(): Observable<{ removed: number }> {
    return this.http.delete<{ removed: number }>(`${this.BASE_URL}/Admin/users/inactive`);
  }

  // Admin - Posts
  getAdminPosts(filter?: string): Observable<PostListItem[]> {
    const params = filter ? new HttpParams().set('filter', filter) : undefined;
    return this.http.get<PostListItem[]>(`${this.BASE_URL}/Admin/posts`, { params });
  }

  deleteAdminPost(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/Admin/publicacoes/${id}`);
  }

  // Admin - Comments
  getAdminComments(filter?: string): Observable<CommentListItem[]> {
    const params = filter ? new HttpParams().set('filter', filter) : undefined;
    return this.http.get<CommentListItem[]>(`${this.BASE_URL}/Admin/comments`, { params });
  }

  deleteAdminComment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/Admin/comentarios/${id}`);
  }

  // Admin - Top Bazes
  getTopBazes(): Observable<TopBazesUser[]> {
    return this.http.get<TopBazesUser[]>(`${this.BASE_URL}/Admin/top-bazes`);
  }

  // Admin - Management
  createAdmin(dto: CreateAdminRequest): Observable<any> {
    return this.http.post(`${this.BASE_URL}/Admin/create-admin`, dto);
  }

  changeEmail(dto: ChangeEmailRequest): Observable<any> {
    return this.http.put(`${this.BASE_URL}/Admin/change-email`, dto);
  }

  changePassword(dto: ChangePasswordRequest): Observable<any> {
    return this.http.put(`${this.BASE_URL}/Admin/change-password`, dto);
  }

  // Admin - Posts with comments (for dropdown)
  getPostComments(postId: number): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.BASE_URL}/Comentario/publicacao/${postId}`);
  }
}

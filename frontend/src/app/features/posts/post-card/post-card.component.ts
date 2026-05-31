import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Post } from '../../../core/models/post.model';
import { Comment } from '../../../core/models/comment.model';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';

@Component({
  selector: 'app-post-card',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, TimeAgoPipe],
  templateUrl: './post-card.component.html',
  styleUrl: './post-card.component.scss'
})
export class PostCardComponent {
  @Input() post!: Post;
  @Output() deleted = new EventEmitter<number>();

  private api = inject(ApiService);
  auth = inject(AuthService);

  showComments = false;
  comments: Comment[] = [];
  newComment = '';
  commentLoading = false;
  editingCommentId: number | null = null;
  editCommentText = '';

  toggleBaze(): void {
    this.api.toggleBaze(this.post.id).subscribe(res => {
      this.post.isBazed = res.bazed;
      this.post.bazesCount += res.bazed ? 1 : -1;
    });
  }

  toggleComments(): void {
    this.showComments = !this.showComments;
    if (this.showComments && this.comments.length === 0) {
      this.api.getComments(this.post.id).subscribe(c => this.comments = c);
    }
  }

  addComment(): void {
    if (!this.newComment.trim()) return;
    this.commentLoading = true;

    this.api.createComment({
      publicacaoId: this.post.id,
      texto: this.newComment
    }).subscribe({
      next: (comment) => {
        this.comments.unshift(comment);
        this.post.comentariosCount++;
        this.newComment = '';
        this.commentLoading = false;
      },
      error: () => this.commentLoading = false
    });
  }

  deletePost(): void {
    if (confirm('Tem certeza que deseja eliminar esta publicação?')) {
      this.api.deletePost(this.post.id).subscribe(() => {
        this.deleted.emit(this.post.id);
      });
    }
  }

  deleteComment(id: number): void {
    if (confirm('Tem certeza que deseja eliminar este comentario?')) {
      this.api.deleteComment(id).subscribe(() => {
        this.comments = this.comments.filter(c => c.id !== id);
        this.post.comentariosCount--;
      });
    }
  }

  startEditComment(comment: Comment): void {
    this.editingCommentId = comment.id;
    this.editCommentText = comment.texto;
  }

  cancelEditComment(): void {
    this.editingCommentId = null;
    this.editCommentText = '';
  }

  saveEditComment(commentId: number): void {
    if (!this.editCommentText.trim()) return;
    this.api.updateComment(commentId, this.editCommentText).subscribe({
      next: (updated) => {
        const idx = this.comments.findIndex(c => c.id === commentId);
        if (idx !== -1) {
          this.comments[idx].texto = updated.texto;
        }
        this.editingCommentId = null;
        this.editCommentText = '';
      }
    });
  }

  trackByCommentId(index: number, c: Comment): number {
    return c.id;
  }

  isOwner(): boolean {
    return this.auth.currentUser()?.id === this.post.utilizadorId;
  }
}

import { Component, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { Post } from '../../../core/models/post.model';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-post.component.html',
  styleUrl: './create-post.component.scss'
})
export class CreatePostComponent {
  @Output() postCreated = new EventEmitter<Post>();

  private api = inject(ApiService);
  auth = inject(AuthService);

  texto = '';
  loading = false;
  expanded = false;

  submit(): void {
    if (!this.texto.trim()) return;
    this.loading = true;

    this.api.createPost({ texto: this.texto }).subscribe({
      next: (post) => {
        this.postCreated.emit(post);
        this.texto = '';
        this.expanded = false;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}

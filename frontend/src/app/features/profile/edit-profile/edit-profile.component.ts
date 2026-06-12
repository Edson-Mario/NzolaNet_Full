import { Component, inject, Input, Output, EventEmitter, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { User, UpdateProfileRequest } from '../../../core/models/user.model';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.scss'
})
export class EditProfileComponent implements OnInit {
  @Input() user!: User;
  @Output() saved = new EventEmitter<User>();
  @Output() cancelled = new EventEmitter<void>();

  private api = inject(ApiService);
  private auth = inject(AuthService);

  nome = '';
  privacidade = 'publico';
  saving = signal(false);

  selectedFile: File | null = null;
  previewUrl = signal<string | null>(null);

  ngOnInit(): void {
    this.nome = this.user.nome;
    this.privacidade = this.user.privacidade;
  }

  onFileSelect(event: any): void {
    const file = event.target.files[0];
    if (!file) return;
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  removePhoto(): void {
    this.api.removeProfilePhoto().subscribe({
      next: (updated) => {
        this.user.fotoPerfil = undefined;
        this.previewUrl.set(null);
        this.selectedFile = null;
        this.auth.updateCurrentUser(updated);
      }
    });
  }

  uploadPhoto(): void {
    if (!this.selectedFile) return;
    this.api.uploadFile(this.selectedFile, 'fotos_de_perfil').subscribe({
      next: (res) => {
        this.api.updateProfile({ fotoPerfil: res.url }).subscribe({
          next: (updated) => {
            this.user.fotoPerfil = res.url;
            this.auth.updateCurrentUser(updated);
            this.selectedFile = null;
          }
        });
      }
    });
  }

  save(): void {
    if (!this.nome.trim()) return;
    this.saving.set(true);

    const dto: UpdateProfileRequest = {
      nome: this.nome.trim(),
      privacidade: this.privacidade
    };

    this.api.updateProfile(dto).subscribe({
      next: (updated) => {
        this.auth.updateCurrentUser(updated);
        this.saved.emit(updated);
        this.saving.set(false);
      },
      error: () => this.saving.set(false)
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}

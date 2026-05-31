import { Component, inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
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
  bio = '';
  privacidade = 'publico';
  dataNascimento = '';
  endereco = '';
  nacionalidade = '';
  sexo = '';
  saving = false;

  selectedFile: File | null = null;
  previewUrl: string | null = null;

  ngOnInit(): void {
    this.nome = this.user.nome;
    this.bio = this.user.bio || '';
    this.privacidade = this.user.privacidade;
    this.dataNascimento = this.user.dataNascimento || '';
    this.endereco = this.user.endereco || '';
    this.nacionalidade = this.user.nacionalidade || '';
    this.sexo = this.user.sexo || '';
  }

  onFileSelect(event: any): void {
    const file = event.target.files[0];
    if (!file) return;
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
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
    this.saving = true;

    const dto: UpdateProfileRequest = {
      nome: this.nome.trim(),
      bio: this.bio.trim(),
      privacidade: this.privacidade,
      dataNascimento: this.dataNascimento || undefined,
      endereco: this.endereco.trim() || undefined,
      nacionalidade: this.nacionalidade.trim() || undefined,
      sexo: this.sexo || undefined
    };

    this.api.updateProfile(dto).subscribe({
      next: (updated) => {
        this.auth.updateCurrentUser(updated);
        this.saved.emit(updated);
        this.saving = false;
      },
      error: () => this.saving = false
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}

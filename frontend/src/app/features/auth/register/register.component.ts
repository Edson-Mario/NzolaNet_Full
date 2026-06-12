import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private api = inject(ApiService);
  private router = inject(Router);

  nome = '';
  email = '';
  senha = '';
  confirmSenha = '';

  selectedFile: File | null = null;

  error = signal('');
  loading = signal(false);

  onFileSelect(event: any): void {
    const file = event.target.files[0];
    this.selectedFile = file || null;
  }

  onSubmit(): void {
    if (!this.nome.trim()) {
      this.error.set('O nome é obrigatório');
      return;
    }
    if (this.senha !== this.confirmSenha) {
      this.error.set('As senhas não coincidem');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const register$ = this.auth.register({
      nome: this.nome,
      email: this.email,
      senha: this.senha,
      confirmarSenha: this.confirmSenha
    });

    if (this.selectedFile) {
      register$.subscribe({
        next: () => {
          this.api.uploadFile(this.selectedFile!, 'fotos_de_perfil').subscribe({
            next: (res) => {
              this.api.updateProfile({ fotoPerfil: res.url }).subscribe({
                next: () => {
                  this.auth.updateCurrentUser({ fotoPerfil: res.url });
                  this.router.navigate(['/']);
                }
              });
            },
            error: () => this.router.navigate(['/'])
          });
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Erro ao registar');
          this.loading.set(false);
        }
      });
    } else {
      register$.subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => {
          this.error.set(err.error?.message || 'Erro ao registar');
          this.loading.set(false);
        }
      });
    }
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  step = 1;

  // Step 1 - Personal data
  nome = '';
  dataNascimento = '';
  endereco = '';
  nacionalidade = '';
  sexo = '';

  // Step 2 - Account credentials
  email = '';
  senha = '';
  confirmSenha = '';

  error = '';
  loading = false;

  nextStep(): void {
    this.error = '';

    if (!this.nome.trim()) {
      this.error = 'O nome é obrigatório';
      return;
    }
    if (!this.dataNascimento) {
      this.error = 'A data de nascimento é obrigatória';
      return;
    }
    if (!this.endereco.trim()) {
      this.error = 'O endereço é obrigatório';
      return;
    }
    if (!this.nacionalidade.trim()) {
      this.error = 'A nacionalidade é obrigatória';
      return;
    }
    if (!this.sexo) {
      this.error = 'O sexo é obrigatório';
      return;
    }

    this.step = 2;
  }

  prevStep(): void {
    this.error = '';
    this.step = 1;
  }

  onSubmit(): void {
    if (this.senha !== this.confirmSenha) {
      this.error = 'As senhas não coincidem';
      return;
    }

    this.loading = true;
    this.error = '';

    this.auth.register({
      nome: this.nome,
      email: this.email,
      senha: this.senha,
      confirmarSenha: this.confirmSenha,
      dataNascimento: this.dataNascimento,
      endereco: this.endereco,
      nacionalidade: this.nacionalidade,
      sexo: this.sexo
    }).subscribe({
      next: () => this.router.navigate(['/']),
      error: (err) => {
        this.error = err.error?.message || 'Erro ao registar';
        this.loading = false;
      }
    });
  }
}

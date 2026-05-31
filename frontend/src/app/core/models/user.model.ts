export interface User {
  id: number;
  nome: string;
  email: string;
  fotoPerfil?: string;
  bio?: string;
  privacidade: string;
  role: string;
  isActive?: boolean;
  dataNascimento?: string;
  endereco?: string;
  nacionalidade?: string;
  sexo?: string;
  publicacoesCount?: number;
  seguidoresCount?: number;
  seguindoCount?: number;
  isFollowing?: boolean;
}

export interface AuthResponse {
  id: number;
  nome: string;
  email: string;
  fotoPerfil?: string;
  role: string;
  token: string;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface RegisterRequest {
  nome: string;
  email: string;
  senha: string;
  confirmarSenha: string;
  fotoPerfil?: string;
  bio?: string;
  dataNascimento?: string;
  endereco?: string;
  nacionalidade?: string;
  sexo?: string;
}

export interface UpdateProfileRequest {
  nome?: string;
  bio?: string;
  fotoPerfil?: string;
  privacidade?: string;
  dataNascimento?: string;
  endereco?: string;
  nacionalidade?: string;
  sexo?: string;
}

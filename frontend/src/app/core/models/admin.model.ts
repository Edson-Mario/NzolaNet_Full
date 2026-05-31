export interface DashboardStats {
  totalUtilizadores: number;
  totalPublicacoes: number;
  totalComentarios: number;
  totalBazes: number;
  utilizadoresAtivos: number;
  utilizadoresDesativados: number;
}

export interface AdminUser {
  id: number;
  nome: string;
  email: string;
  role: string;
  isActive: boolean;
  dataNascimento?: string;
  endereco?: string;
  nacionalidade?: string;
  sexo?: string;
  createdAt: string;
  publicacoesCount: number;
  bazesCount: number;
  seguidoresCount: number;
  seguindoCount: number;
}

export interface TopBazesUser {
  id: number;
  nome: string;
  fotoPerfil?: string;
  bazesCount: number;
  publicacoesCount: number;
}

export interface PostListItem {
  id: number;
  autorNome: string;
  autorFoto?: string;
  texto: string;
  bazesCount: number;
  comentariosCount: number;
  createdAt: string;
}

export interface CommentListItem {
  id: number;
  publicacaoId: number;
  autorNome: string;
  texto: string;
  createdAt: string;
}

export interface CreateAdminRequest {
  nome: string;
  email: string;
  senha: string;
}

export interface ChangeEmailRequest {
  novoEmail: string;
}

export interface ChangePasswordRequest {
  senhaAtual: string;
  novaSenha: string;
  confirmarSenha: string;
}

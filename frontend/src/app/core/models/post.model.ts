export interface Post {
  id: number;
  utilizadorId: number;
  autorNome: string;
  autorFoto?: string;
  texto: string;
  imagem?: string;
  video?: string;
  bazesCount: number;
  comentariosCount: number;
  isBazed: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePostRequest {
  texto: string;
  imagem?: string;
  video?: string;
}

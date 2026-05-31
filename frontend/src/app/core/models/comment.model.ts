export interface Comment {
  id: number;
  publicacaoId: number;
  utilizadorId: number;
  autorNome: string;
  autorFoto?: string;
  texto: string;
  createdAt: string;
}

export interface CreateCommentRequest {
  publicacaoId: number;
  texto: string;
}

export interface Notification {
  id: number;
  utilizadorId: number;
  tipo: string;
  mensagem: string;
  lida: boolean;
  createdAt: string;
}

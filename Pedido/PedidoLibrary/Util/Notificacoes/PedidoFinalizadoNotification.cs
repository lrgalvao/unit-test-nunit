using PedidoLibrary.Entidades;

namespace PedidoLibrary.Util.Notificacoes
{
	public class PedidoFinalizadoNotification : PedidoNotification
	{
		public PedidoFinalizadoNotification(Pedido pedido) : base(pedido)
		{
		}
	}
}

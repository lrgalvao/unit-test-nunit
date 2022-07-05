using PedidoLibrary.Entidades;

namespace PedidoLibrary.Util.Notificacoes
{
	public class PedidoCriadoNotification : PedidoNotification
	{
		public PedidoCriadoNotification(Pedido pedido) : base(pedido)
		{
		}
	}
}

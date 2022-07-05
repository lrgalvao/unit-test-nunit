using PedidoLibrary.Entidades;

namespace PedidoLibrary.Util.Notificacoes
{
	public class PedidoCanceladoNotification : PedidoNotification
	{
		public PedidoCanceladoNotification(Pedido pedido) : base(pedido)
		{
		}
	}
}

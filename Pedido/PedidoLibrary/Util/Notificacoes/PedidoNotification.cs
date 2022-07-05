using MediatR;
using PedidoLibrary.Entidades;

namespace PedidoLibrary.Util.Notificacoes
{
	public class PedidoNotification : INotification
	{
		public Pedido Pedido { get; private set; }

		public PedidoNotification(Pedido pedido)
		{
			Pedido = pedido;
		}
	}
}

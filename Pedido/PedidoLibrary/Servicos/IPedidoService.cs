using PedidoLibrary.Entidades;
using PedidoLibrary.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Servicos
{
	public interface IPedidoService
	{
		Pedido CriarPedido(Cliente cliente, bool ehExpress);
		void AdicionarProduto(Guid idPedido, Produto produto, int quantidade);
		void RemoverProduto(Guid idPedido, Produto produto, int? quantidade = null);
		void FinalizarPedido(Guid idPedido);
		void CancelarPedido(Guid idPedido);
		Pedido ObterPedido(Guid idPedido);
		Task<IList<Pedido>> ObterPedidosAsync(PedidoFilter filtro = null);
		IList<Pedido> ObterPedidos(PedidoFilter filtro = null);
	}
}

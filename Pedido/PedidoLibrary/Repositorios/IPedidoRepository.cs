using PedidoLibrary.Entidades;
using System;
using System.Collections.Generic;

namespace PedidoLibrary.Repositorios
{
	public interface IPedidoRepository
	{
		void Inserir(Pedido pedido);
		void Atualizar(Pedido pedido);
		void Remover(Guid idPedido);
		Pedido ObterPorId(Guid idPedido);
		IList<Pedido> ObterTodos();
	}
}

using PedidoLibrary.Servicos;
using PedidoLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PedidoLibrary.Entidades
{
	public class Pedido
	{
		public Guid Id { get; set; }
		public Cliente Cliente { get; set; }
		public IDictionary<Produto, int> ProdutosSelecionados { get; set; }
		public EstadoPedidoEnum Estado { get; set; }
		public bool EhExpress { get; set; }

		public bool EhValido()
		{
			var clienteValido = Cliente != null && Cliente.EhValido();
			
			var produtosValidos 
				= ProdutosSelecionados != null 
				&& (!ProdutosSelecionados.Any() 
					|| (ProdutosSelecionados.Keys.All(p => p.EhValido()) 
						&& ProdutosSelecionados.Values.All(valor => valor > 0)));

			return clienteValido && produtosValidos;
		}

		public decimal ValorTotal()
		{
			if (!EhValido())
			{
				throw new NegocioException(Mensagens.M09_PEDIDO_INVALIDO);
			}

			if (ProdutosSelecionados != null && ProdutosSelecionados.Any())
			{
				return ProdutosSelecionados.Sum(p => p.Key.Valor * p.Value);
			}

			return 0;
		}

		public override bool Equals(object obj)
		{
			return obj is Pedido pedido &&
				   Id.Equals(pedido.Id);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id);
		}
	}
}

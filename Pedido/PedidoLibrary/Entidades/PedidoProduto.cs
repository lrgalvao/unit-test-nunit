using PedidoLibrary.Util;
using System;
using System.ComponentModel.DataAnnotations;

namespace PedidoLibrary.Entidades
{
	public class PedidoProduto : IEntity
	{
		[Key]
		public Guid Id { get; set; }

		public Pedido Pedido { get; set; }
		
		public Produto Produto { get; set; }

		public int Quantidade { get; set; }
	}
}

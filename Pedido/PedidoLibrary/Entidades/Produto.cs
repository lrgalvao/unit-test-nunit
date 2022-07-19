using PedidoLibrary.Util;
using System;
using System.ComponentModel.DataAnnotations;

namespace PedidoLibrary.Entidades
{
	public class Produto : IEntity
	{
		[Key]
		public Guid Id { get; set; }
		public string Nome { get; set; }
		public decimal Valor { get; set; }
		public bool DisponivelExpress { get; set; }

		public bool EhValido()
		{
			return !string.IsNullOrEmpty(Nome) && Valor > 0;
		}

		public override bool Equals(object obj)
		{
			return obj is Produto produto &&
				   Id.Equals(produto.Id);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id);
		}
	}
}

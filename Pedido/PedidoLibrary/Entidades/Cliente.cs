using PedidoLibrary.Util;
using System;

namespace PedidoLibrary.Entidades
{
	public class Cliente
	{
		public Guid Id { get; set; }
		public string Nome { get; set; }
		public string Sobrenome { get; set; }
		public string Email { get; set; }
		public string Cpf { get; set; }

		public bool EhValido()
		{
			return !string.IsNullOrEmpty(Nome) 
				&& !string.IsNullOrEmpty(Sobrenome)
				// PONTO DE ATENÇÃO: E se estes métodos de validação de Email e CPF estivessem nesta classe?
				&& StringUtilities.ValidarEmail(Email) 
				&& StringUtilities.ValidarCpf(Cpf);
		}

		public override bool Equals(object obj)
		{
			return obj is Cliente cliente &&
				   Id.Equals(cliente.Id);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id);
		}
	}
}

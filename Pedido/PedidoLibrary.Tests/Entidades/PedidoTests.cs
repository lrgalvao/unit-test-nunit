using NUnit.Framework;
using PedidoLibrary.Entidades;
using PedidoLibrary.Servicos;
using PedidoLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PedidoLibrary.Tests.Entidades
{
	public class PedidoTests
	{
		[Test]
		public void Pedido_EhValido_PedidoValido()
		{
			// Arrange
			Pedido pedido = ConstruirPedido();

			// Act
			var ehValido = pedido.EhValido();

			// Assert
			Assert.IsTrue(ehValido);
		}

		[Test]
		public void Pedido_ValorTotal_SomaValoresPedidoValido()
		{
			// Arrange
			Pedido pedido = ConstruirPedido();

			// Act
			var valor = pedido.ValorTotal();

			// Assert
			Assert.AreEqual(200, valor);
		}

		[Test]
		public void Pedido_ValorTotal_SomaValoresPedidoInvalido()
		{
			// Arrange
			Pedido pedido = ConstruirPedido();
			pedido.Cliente.Cpf = "";

			// Act
			var excecao = Assert.Throws<NegocioException>(() => pedido.ValorTotal());

			// Assert
			Assert.AreEqual(Mensagens.M09_PEDIDO_INVALIDO, excecao.Message);
		}

		[Test]
		public void Pedido_EhValido_ClienteInvalido()
		{
			// Arrange
			var pedido = ConstruirPedido();
			pedido.Cliente.Cpf = "";

			// Act
			var ehValido = pedido.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Pedido_EhValido_ProdutoInvalido()
		{
			// Arrange
			var pedido = ConstruirPedido();
			pedido.ProdutosSelecionados.Keys.First().Valor = 0;

			// Act
			var ehValido = pedido.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Pedido_EhValido_ProdutoSemQuantidade()
		{
			// Arrange
			var pedido = ConstruirPedido();
			var produto = pedido.ProdutosSelecionados.Keys.First();
			pedido.ProdutosSelecionados[produto] = 0;

			// Act
			var ehValido = pedido.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		private static Pedido ConstruirPedido()
		{
			return new Pedido
			{
				Id = Guid.NewGuid(),
				Cliente = new Cliente
				{
					Nome = "Antônio",
					Sobrenome = "Jobim",
					Cpf = "580.276.580-12",
					Email = "antonio.jobim@pitang.com"
				},
				ProdutosSelecionados = new Dictionary<Produto, int>
				{
					{ new Produto { Id = Guid.NewGuid(), Nome = "Produto 1", Valor = 30, DisponivelExpress = false }, 2 },
					{ new Produto { Id = Guid.NewGuid(), Nome = "Produto 2", Valor = 20, DisponivelExpress = false }, 1 },
					{ new Produto { Id = Guid.NewGuid(), Nome = "Produto 3", Valor = 40, DisponivelExpress = false }, 3 }
				},
				EhExpress = false,
				Estado = EstadoPedidoEnum.Aberto
			};
		}
	}
}

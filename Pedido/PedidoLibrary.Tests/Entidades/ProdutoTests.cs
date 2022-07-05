using NUnit.Framework;
using PedidoLibrary.Entidades;
using System;

namespace PedidoLibrary.Tests.Entidades
{
	public class ProdutoTests
	{
		[Test]
		public void Produto_EhValido_ProdutoValido()
		{
			// Arrange
			var produto = new Produto
			{
				Nome = "Produto 1",
				Valor = 30,
				DisponivelExpress = false
			};

			// Act
			var ehValido = produto.EhValido();

			// Assert
			Assert.IsTrue(ehValido);
		}

		[Test]
		public void Produto_EhValido_NomeInvalido()
		{
			// Arrange
			var produto = new Produto
			{
				Nome = "",
				Valor = 30,
				DisponivelExpress = true
			};

			// Act
			var ehValido = produto.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Produto_EhValido_ValorInvalido()
		{
			// Arrange
			var produto = new Produto
			{
				Nome = "Produto 1",
				DisponivelExpress = true
			};

			// Act
			var ehValido = produto.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Produto_Equals_ProdutosIguais()
		{
			// Arrange
			var id = Guid.NewGuid();

			var produto1 = new Produto
			{
				Id = id,
				Nome = "Produto 1",
				Valor = 30,
				DisponivelExpress = true
			};

			var produto2 = new Produto
			{
				Id = id,
				Nome = "Produto 1",
				Valor = 30,
				DisponivelExpress = true
			};

			// Assert
			Assert.AreEqual(produto1, produto2);
		}

		[Test]
		public void Produto_Equals_ProdutosDiferentes()
		{
			// Arrange
			var produto1 = new Produto
			{
				Id = Guid.NewGuid(),
				Nome = "Produto 1",
				Valor = 30,
				DisponivelExpress = true
			};

			var produto2 = new Produto
			{
				Id = Guid.NewGuid(),
				Nome = "Produto 1",
				Valor = 30,
				DisponivelExpress = true
			};

			// Assert
			Assert.AreNotEqual(produto1, produto2);
		}
	}
}

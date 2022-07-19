using NUnit.Framework;
using PedidoLibrary.Entidades;
using System;

namespace PedidoLibrary.Tests.Entidades
{
	public class ClienteTests
	{
		[Test]
		public void Cliente_EhValido_ClienteValido()
		{
			// Arrange
			var cliente = new Cliente
			{
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "antonio.jobim@pitang.com"
			};

			// Act
			var ehValido = cliente.EhValido();

			// Assert
			Assert.IsTrue(ehValido);
		}

		[Test]
		public void Cliente_EhValido_NomeInvalido()
		{
			// Arrange
			var cliente = new Cliente
			{
				Nome = "",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "antonio.jobim@pitang.com"
			};

			// Act
			var ehValido = cliente.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Cliente_EhValido_SobrenomeInvalido()
		{
			// Arrange
			var cliente = new Cliente
			{
				Nome = "Antônio",
				Sobrenome = null,
				Cpf = "580.276.580-12",
				Email = "antonio.jobim@pitang.com"
			};

			// Act
			var ehValido = cliente.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Cliente_EhValido_ClienteCpfInvalido()
		{
			// Arrange
			var cliente = new Cliente
			{
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "111111111111",
				Email = "antonio.jobim@pitang.com"
			};

			// Act
			var ehValido = cliente.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Cliente_EhValido_ClienteEmailInvalido()
		{
			// Arrange
			var cliente = new Cliente
			{
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "@pitang.com"
			};

			// Act
			var ehValido = cliente.EhValido();

			// Assert
			Assert.IsFalse(ehValido);
		}

		[Test]
		public void Cliente_Equals_ClientesIguais()
		{
			// Arrange
			var id = Guid.NewGuid();

			var cliente1 = new Cliente
			{
				Id = id,
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "@pitang.com"
			};

			var cliente2 = new Cliente
			{
				Id = id,
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "@pitang.com"
			};

			// Assert
			Assert.AreEqual(cliente1, cliente2);
		}

		[Test]
		public void Cliente_Equals_ClientesDiferentes()
		{
			// Arrange
			var cliente1 = new Cliente
			{
				Id = Guid.NewGuid(),
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "@pitang.com"
			};

			var cliente2 = new Cliente
			{
				Id = Guid.NewGuid(),
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "@pitang.com"
			};

			// Assert
			Assert.AreNotEqual(cliente1, cliente2);
		}
	}
}

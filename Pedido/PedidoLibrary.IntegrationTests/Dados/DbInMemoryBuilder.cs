using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.EntityFrameworkCore;
using PedidoLibrary.Contexto;
using PedidoLibrary.Entidades;
using System;

namespace PedidoLibrary.IntegrationTests.Dados
{
	public class DbInMemoryBuilder
	{
		private const string FAKER_LOCALE = "pt_BR";
		private const string NOME_BANCO_MEMORIA = "PedidoDb";

		public static PedidoDbContext CarregarBancoEmMemoria()
		{
			var context 
				= new PedidoDbContext(
					new DbContextOptionsBuilder<PedidoDbContext>()
						.UseInMemoryDatabase(databaseName: NOME_BANCO_MEMORIA)
						.Options);

			context.Database.EnsureDeleted();

			var clienteFaker = new Faker<Cliente>(FAKER_LOCALE)
				.CustomInstantiator(f => 
					new Cliente
					{
						Id = Guid.NewGuid(),
						Nome = f.Name.FirstName(),
						Sobrenome = f.Name.LastName(),
						Cpf = f.Person.Cpf()
					})
				.RuleFor(c => c.Email, (f, c) =>
					  f.Internet.Email(c.Nome.ToLower(), c.Sobrenome.ToLower()));

			
			var produtoFaker = new Faker<Produto>(FAKER_LOCALE)
				.CustomInstantiator(f =>
					new Produto
					{
						Id = Guid.NewGuid(),
						Nome = f.Commerce.ProductName(),
						Valor = f.Random.Decimal(20, 2000),
						DisponivelExpress = f.Random.Bool()
					});

			var clientes = clienteFaker.Generate(20);
			var produtos = produtoFaker.Generate(200);

			context.AddRange(clientes);
			context.AddRange(produtos);

			context.SaveChanges();

			return context;
		}
	}
}

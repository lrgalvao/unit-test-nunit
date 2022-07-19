using FizzWare.NBuilder;
using FluentAssertions;
using FluentAssertions.Extensions;
using MediatR;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using PedidoLibrary.Contexto;
using PedidoLibrary.Entidades;
using PedidoLibrary.IntegrationTests.Dados;
using PedidoLibrary.Repositorios.Impl;
using PedidoLibrary.Servicos;
using PedidoLibrary.Servicos.Impl;
using PedidoLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidoLibrary.Tests.Servicos
{
	public class PedidoServiceIntegrationTests
	{
		private readonly Mock<IEmailService> mockEmailService = new();
		private readonly Mock<IMediator> mockMediator = new();

		private IClienteService _clienteService;
		private IProdutoService _produtoService;
		private IPedidoService _pedidoService;

		[SetUp]
		public void ConfigurarTeste()
		{
			PedidoDbContext context = DbInMemoryBuilder.CarregarBancoEmMemoria();

			_clienteService = new ClienteService(new ClienteRepository(context));
			_produtoService = new ProdutoService(new ProdutoRepository(context));
			_pedidoService = new PedidoService(new PedidoRepository(context), mockEmailService.Object, mockMediator.Object);
		}

		[Test]
		public async Task PedidoService_CriarPedido_Sucesso()
		{
			// Arrange
			var cliente = (await _clienteService.ObterTodosAsync()).First();

			// Act
			var pedido = _pedidoService.CriarPedido(cliente, false);

			// Assert
			var pedidoCriado = _pedidoService.ObterPedido(pedido.Id);

			pedidoCriado.Cliente.Should().BeEquivalentTo(cliente);
			pedido.ProdutosSelecionados.Should().BeEmpty();
		}

		[Test]
		[Category("Testes de Integração")]
		public async Task PedidoService_AdicionarProduto_Sucesso()
		{
			// Arrange
			var quantidadeProdutos = 10;
			var cliente = (await _clienteService.ObterTodosAsync()).First();
			var produto = (await _produtoService.ObterTodosAsync()).First();

			var pedido = _pedidoService.CriarPedido(cliente, false);

			// Act
			_pedidoService.AdicionarProduto(pedido.Id, produto, quantidadeProdutos);

			// Assert
			var pedidoCriado = _pedidoService.ObterPedido(pedido.Id);

			pedidoCriado.ProdutosSelecionados
				.Where(p => p.Produto == produto && p.Quantidade == quantidadeProdutos)
				.Should()
				.HaveCount(1, "foi adicionado o produto {0} com {1} itens", produto.Nome, quantidadeProdutos);
		}

		[Test]
		public async Task PedidoService_RemoverProduto_Sucesso()
		{
			// Arrange
			var cliente = (await _clienteService.ObterTodosAsync()).First();
			var produtos = (await _produtoService.ObterTodosAsync()).Take(3).ToList();
			
			var pedido = _pedidoService.CriarPedido(cliente, false);
			
			_pedidoService.AdicionarProduto(pedido.Id, produtos[0], 2);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[1], 5);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 8);

			// Act
			_pedidoService.RemoverProduto(pedido.Id, produtos[1]);
			_pedidoService.RemoverProduto(pedido.Id, produtos[2], 3);

			// Assert
			var pedidoCriado = _pedidoService.ObterPedido(pedido.Id);

			pedidoCriado.ProdutosSelecionados.FirstOrDefault(p => p.Produto == produtos[1]).Should().BeNull();
			pedidoCriado.ProdutosSelecionados.Sum(p => p.Quantidade).Should().Be(7);
		}

		[Test]
		public async Task PedidoService_CancelarPedido_Sucesso()
		{
			// Arrange
			var cliente = (await _clienteService.ObterTodosAsync()).First();

			var pedido = _pedidoService.CriarPedido(cliente, false);

			// Act
			_pedidoService.CancelarPedido(pedido.Id);

			// Assert
			var pedidoCriado = _pedidoService.ObterPedido(pedido.Id);

			pedidoCriado.Estado.Should().Be(EstadoPedidoEnum.Cancelado, "o pedido foi cancelado");
		}

		[Test]
		public async Task PedidoService_FinalizarProduto_Sucesso()
		{
			// Arrange
			var cliente = (await _clienteService.ObterTodosAsync()).First();
			var produto = (await _produtoService.ObterTodosAsync()).First();

			var pedido = _pedidoService.CriarPedido(cliente, false);

			_pedidoService.AdicionarProduto(pedido.Id, produto, 2);
			
			// Act
			_pedidoService.FinalizarPedido(pedido.Id);

			// Assert
			var pedidoCriado = _pedidoService.ObterPedido(pedido.Id);

			pedidoCriado.Estado.Should().Be(EstadoPedidoEnum.Finalizado);
		}

		[Test]
		public async Task PedidoService_ObterPedidosAsync_FiltroPorTermo()
		{
			// Arrange
			var clientes = (await _clienteService.ObterTodosAsync()).Take(2).ToList();
			var produtos = (await _produtoService.ObterTodosAsync()).Take(3).ToList();

			// pedido com produtos 0, 1 e 2
			var pedido = _pedidoService.CriarPedido(clientes[0], false);

			_pedidoService.AdicionarProduto(pedido.Id, produtos[0], 2);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[1], 5);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 8);

			// pedido com produtos 1 e 2
			pedido = _pedidoService.CriarPedido(clientes[1], false);

			_pedidoService.AdicionarProduto(pedido.Id, produtos[1], 4);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 10);

			// pedido com produtos 0 e 2
			pedido = _pedidoService.CriarPedido(clientes[1], false);

			_pedidoService.AdicionarProduto(pedido.Id, produtos[0], 6);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 3);


			var filtro = new PedidoFilter
			{
				Termo = produtos[1].Nome
			};

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync(filtro);

			// Assert
			pedidos.Should().HaveCount(2);
			pedidos.Should().AllSatisfy(p => p.ProdutosSelecionados.Any(p => p.Produto == produtos[1]));
		}

		[Test]
		//[Ignore("performance", Until = "2022-07-20 12:00:00")]
		public async Task PedidoService_ObterPedidosAsync_PerformanceSemFiltro()
		{
			// Arrange
			var cliente = (await _clienteService.ObterTodosAsync()).First();
			var produtos = (await _produtoService.ObterTodosAsync()).Take(3).ToList();
			var qtdePedidos = 1_000;
			var tempoLimite = 10;

			// pedido com produtos 0, 1 e 2
			for (var i = 0; i <= qtdePedidos; i++) 
			{
				var pedido = _pedidoService.CriarPedido(cliente, false);
				_pedidoService.AdicionarProduto(pedido.Id, produtos[0], 2);
			}

			// Act / Assert
			_pedidoService.ExecutionTimeOf(p => p.ObterPedidos(new PedidoFilter()))
				.Should()
				.BeLessThanOrEqualTo(tempoLimite.Milliseconds(),
				"deve suportar a uma consulta de {0} pedidos em {1} milissegundos", qtdePedidos, tempoLimite);
		}

		[Test]
		public async Task PedidoService_ObterPedidosAsync_FiltroPorEstado()
		{
			// Arrange
			var clientes = (await _clienteService.ObterTodosAsync()).Take(2).ToList();
			var produtos = (await _produtoService.ObterTodosAsync()).Take(3).ToList();

			var todosPedidos = await _pedidoService.ObterPedidosAsync();
			// pedido com produtos 0, 1 e 2
			var pedido = _pedidoService.CriarPedido(clientes[0], false);

			_pedidoService.AdicionarProduto(pedido.Id, produtos[0], 2);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[1], 5);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 8);

			_pedidoService.CancelarPedido(pedido.Id);

			// pedido com produtos 1 e 2
			pedido = _pedidoService.CriarPedido(clientes[1], false);

			_pedidoService.AdicionarProduto(pedido.Id, produtos[1], 4);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 10);

			_pedidoService.FinalizarPedido(pedido.Id);

			// pedido com produtos 0 e 2
			pedido = _pedidoService.CriarPedido(clientes[1], false);

			_pedidoService.AdicionarProduto(pedido.Id, produtos[0], 6);
			_pedidoService.AdicionarProduto(pedido.Id, produtos[2], 3);			

			var filtro = new PedidoFilter
			{
				Estados = new List<EstadoPedidoEnum>
				{
					EstadoPedidoEnum.Aberto
				}
			};

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync(filtro);

			// Assert
			pedidos.Should().HaveCount(1);
			pedidos.Should().AllSatisfy(p => p.Estado.Equals(EstadoPedidoEnum.Aberto));
		}
	}
}

using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using PedidoLibrary.Entidades;
using PedidoLibrary.Repositorios;
using PedidoLibrary.Servicos;
using PedidoLibrary.Servicos.Impl;
using PedidoLibrary.Util;
using PedidoLibrary.Util.Notificacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PedidoLibrary.Tests.Servicos
{
	public class PedidoServiceTestsV2
	{
		private readonly AutoMocker _mocker = new();
		private IPedidoService _pedidoService;

		[SetUp]
		public void ConfigurarTeste()
		{
			_pedidoService = _mocker.CreateInstance<PedidoService>();
		}

		[Test]
		public void PedidoService_CriarPedido_Sucesso()
		{
			// Arrange
			var cliente = new Cliente
			{
				Id = Guid.NewGuid(),
				Nome = "Antônio",
				Sobrenome = "Jobim",
				Cpf = "580.276.580-12",
				Email = "antonio.jobim@email.com"
			};

			// Act
			var pedido = _pedidoService.CriarPedido(cliente, false);

			// Assert
			pedido.Id.Should().NotBeEmpty();
			pedido.ProdutosSelecionados.Should().BeEmpty();
			pedido.Cliente.Should().BeEquivalentTo(cliente);
		}

		[Test]
		public void PedidoService_CriarPedido_SucessoComBuilder()
		{
			// Arrange
			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.Build();

			// Act
			var pedido = _pedidoService.CriarPedido(cliente, false);

			// Assert
			pedido.Id.Should().NotBeEmpty();
			pedido.ProdutosSelecionados.Should().BeEmpty();
			pedido.Cliente.Should().BeEquivalentTo(cliente);
		}

		[Test]
		public void PedidoService_CriarPedido_ClienteInvalido()
		{
			// Arrange
			var cliente = Builder<Cliente>.CreateNew().Build();

			// Act, Assert
			_pedidoService
				.Invoking(p => p.CriarPedido(cliente, true))
				.Should()
				.Throw<NegocioException>()
				.WithMessage(Mensagens.M04_CLIENTE_INVALIDO);
		}

		[Test]
		public void PedidoService_AdicionarProduto_Sucesso()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente 
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto 
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.With(pedido => pedido.ProdutosSelecionados = new List<PedidoProduto>())
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.AdicionarProduto(idPedido, produto, quantidade);

			// Assert
			_mocker.GetMock<IPedidoRepository>().Verify(mock => mock.Atualizar(pedido), Times.Once());
			pedido.ValorTotal().Should().Be(quantidade * valorProduto);
		}

		[Test]
		public void PedidoService_AdicionarProduto_ProdutoInvalido()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = 0)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.With(pedido => pedido.ProdutosSelecionados = new List<PedidoProduto>())
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act, Assert
			_pedidoService
				.Invoking(p => p.AdicionarProduto(idPedido, produto, quantidade))
				.Should()
				.Throw<NegocioException>()
				.WithMessage(Mensagens.M01_PRODUTO_INVALIDO);
		}

		[Test]
		public void PedidoService_AdicionarProduto_QuantidadeProdutosInvalida()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 0;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.With(pedido => pedido.ProdutosSelecionados = new List<PedidoProduto>())
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act, Assert
			_pedidoService
				.Invoking(p => p.AdicionarProduto(idPedido, produto, quantidade))
				.Should()
				.Throw<NegocioException>()
				.WithMessage(Mensagens.M02_QTDE_PRODUTOS_INVALIDA);
		}

		[Test]
		public void PedidoService_AdicionarProduto_PedidoInvalido()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			// Act, Assert
			_pedidoService
				.Invoking(p => p.AdicionarProduto(idPedido, produto, quantidade))
				.Should()
				.Throw<NegocioException>()
				.WithMessage(Mensagens.M03_PEDIDO_NAO_EXISTE);
		}

		[Test]
		public void PedidoService_AdicionarProduto_ProdutoNaoDisponivelExpress()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.With(pedido => pedido.ProdutosSelecionados = new List<PedidoProduto>())
					.With(pedido => pedido.EhExpress = true)
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act, Assert
			_pedidoService
				.Invoking(p => p.AdicionarProduto(idPedido, produto, quantidade))
				.Should()
				.Throw<NegocioException>()
				.WithMessage(Mensagens.M10_PRODUTO_NAO_DISPONIVEL_EXPRESS);
		}

		[Test]
		public void PedidoService_RemoverProduto_RemoverPorQuantidade()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.Build();

			pedido.ProdutosSelecionados
				= Builder<PedidoProduto>.CreateListOfSize(1)
					.TheFirst(1)
						.With(pedidoProduto => pedidoProduto.Pedido = pedido)
						.With(pedidoProduto => pedidoProduto.Produto = produto)
						.With(pedidoProduto => pedidoProduto.Quantidade = quantidade)
					.Build();


			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.RemoverProduto(idPedido, produto, 1);

			// Assert
			_mocker.GetMock<IPedidoRepository>().Verify(mock => mock.Atualizar(pedido), Times.Once());
			pedido.ValorTotal().Should().Be((quantidade - 1) * valorProduto);
		}

		[Test]
		public void PedidoService_RemoverProduto_RemoverProdutoCompleto()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)					
					.Build();

			pedido.ProdutosSelecionados
				= Builder<PedidoProduto>.CreateListOfSize(1)
					.TheFirst(1)
						.With(pedidoProduto => pedidoProduto.Pedido = pedido)
						.With(pedidoProduto => pedidoProduto.Produto = produto)
						.With(pedidoProduto => pedidoProduto.Quantidade = quantidade)
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.RemoverProduto(idPedido, produto);

			// Assert
			_mocker.GetMock<IPedidoRepository>().Verify(mock => mock.Atualizar(pedido), Times.Once());
			pedido.ValorTotal().Should().Be(0);
		}

		[Test]
		public void PedidoService_RemoverProduto_QuantidadeInvalida()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.Build();

			pedido.ProdutosSelecionados
				= Builder<PedidoProduto>.CreateListOfSize(1)
					.TheFirst(1)
						.With(pedidoProduto => pedidoProduto.Pedido = pedido)
						.With(pedidoProduto => pedidoProduto.Produto = produto)
						.With(pedidoProduto => pedidoProduto.Quantidade = quantidade)
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act, Assert
			_pedidoService
				.Invoking(p => p.RemoverProduto(idPedido, produto, -1))
				.Should()
				.Throw<NegocioException>()
				.WithMessage(Mensagens.M02_QTDE_PRODUTOS_INVALIDA);
		}

		[Test]
		public void PedidoService_FinalizarPedido_Sucesso()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.With(pedido => pedido.Estado = EstadoPedidoEnum.Aberto)
					.Build();

			pedido.ProdutosSelecionados
				= Builder<PedidoProduto>.CreateListOfSize(1)
					.TheFirst(1)
						.With(pedidoProduto => pedidoProduto.Pedido = pedido)
						.With(pedidoProduto => pedidoProduto.Produto = produto)
						.With(pedidoProduto => pedidoProduto.Quantidade = quantidade)
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.FinalizarPedido(idPedido);

			// Assert
			_mocker.GetMock<IPedidoRepository>().Verify(mock => 
				mock.Atualizar(pedido), 
				Times.Once());

			_mocker.GetMock<IMediator>().Verify(mock => 
				mock.Publish(It.IsAny<PedidoFinalizadoNotification>(), It.IsAny<CancellationToken>()), 
				Times.Once());

			_mocker.GetMock<IEmailService>().Verify(mock => 
				mock.EnviarEmail(cliente.Email, It.IsAny<string>(), It.IsAny<string>()), 
				Times.Once());

			pedido.Estado.Should().Be(EstadoPedidoEnum.Finalizado);
		}

		[Test]
		public void PedidoService_CancelarPedido_Sucesso()
		{
			// Arrange
			var idPedido = Guid.NewGuid();
			var quantidade = 2;
			decimal valorProduto = 145;

			var cliente
				= Builder<Cliente>.CreateNew()
					.With(cliente => cliente.Email = "antonio.jobim@email.com")
					.With(cliente => cliente.Cpf = "580.276.580-12")
					.Build();

			var produto
				= Builder<Produto>.CreateNew()
					.With(produto => produto.Valor = valorProduto)
					.Build();

			var pedido
				= Builder<Pedido>.CreateNew()
					.With(pedido => pedido.Id = idPedido)
					.With(pedido => pedido.Cliente = cliente)
					.With(pedido => pedido.Estado = EstadoPedidoEnum.Aberto)
					.Build();

			pedido.ProdutosSelecionados
				= Builder<PedidoProduto>.CreateListOfSize(1)
					.TheFirst(1)
						.With(pedidoProduto => pedidoProduto.Pedido = pedido)
						.With(pedidoProduto => pedidoProduto.Produto = produto)
						.With(pedidoProduto => pedidoProduto.Quantidade = quantidade)
					.Build();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.CancelarPedido(idPedido);

			// Assert
			_mocker.GetMock<IPedidoRepository>().Verify(mock =>
				mock.Atualizar(pedido),
				Times.Once());

			_mocker.GetMock<IMediator>().Verify(mock =>
				mock.Publish(It.IsAny<PedidoCanceladoNotification>(), It.IsAny<CancellationToken>()),
				Times.Once());

			pedido.Estado.Should().Be(EstadoPedidoEnum.Cancelado);
		}

		[Test]
		public async Task PedidoService_ObterPedidos_SemFiltroAsync()
		{
			// Arrange
			IList<Pedido> todosPedidos = ConstruirPedidos();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterTodosAsync()).Returns(() => Task.FromResult(todosPedidos.ToList()));

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync();

			// Assert
			pedidos.Should().HaveCount(5);
		}

		[TestCase("Antonio", 2)]
		[TestCase("chico.buarque", 1)]
		[TestCase("Monitor", 2)]
		[TestCase("Cadeira", 5)]
		[TestCase("414.718.530-47", 2)]
		[TestCase("Maria", 0)]
		[TestCase(" ", 5)]
		[TestCase("", 5)]
		[TestCase(null, 5)]
		public async Task PedidoService_ObterPedidos_FiltroPorTermoAsync(string termo, int qtdePedidosObtidos)
		{
			// Arrange
			IList<Pedido> todosPedidos = ConstruirPedidos();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterTodosAsync()).Returns(() => Task.FromResult(todosPedidos.ToList()));

			var filtro = new PedidoFilter { Termo = termo };

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync(filtro);

			// Assert
			pedidos.Should().HaveCount(qtdePedidosObtidos);
		}

		[TestCase(new [] { EstadoPedidoEnum.Aberto }, 3)]
		[TestCase(new [] { EstadoPedidoEnum.Finalizado }, 1)]
		[TestCase(new [] { EstadoPedidoEnum.Cancelado }, 1)]
		[TestCase(new [] { EstadoPedidoEnum.Aberto, EstadoPedidoEnum.Finalizado }, 4)]
		[TestCase(new EstadoPedidoEnum[] { }, 5)]
		[TestCase(null, 5)]
		public async Task PedidoService_ObterPedidos_FiltroPorEstadosAsync(EstadoPedidoEnum[] estados, int qtdePedidosObtidos)
		{
			// Arrange
			IList<Pedido> todosPedidos = ConstruirPedidos();

			_mocker.GetMock<IPedidoRepository>().Setup(mock => mock.ObterTodosAsync()).Returns(() => Task.FromResult(todosPedidos.ToList()));

			var filtro
				= new PedidoFilter
				{
					Estados = estados != null ? new List<EstadoPedidoEnum>(estados) : null
				};

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync(filtro);

			// Assert
			pedidos.Should().HaveCount(qtdePedidosObtidos);
		}

		private static IList<Produto> ConstruirProdutos()
		{
			return Builder<Produto>.CreateListOfSize(3)
					.TheFirst(1)
						.With(produto => produto.Nome = "Monitor")
						.With(produto => produto.Valor = 1500)
					.TheNext(1)
						.With(produto => produto.Nome = "Cadeira")
						.With(produto => produto.Valor = 1000)
					.TheNext(1)
						.With(produto => produto.Nome = "Mesa")
						.With(produto => produto.Valor = 500)
					.Build();
		}

		private static IList<Cliente> ConstruirClientes()
		{
			return Builder<Cliente>.CreateListOfSize(3)
					.TheFirst(1)
						.With(cliente => cliente.Nome = "Antonio")
						.With(cliente => cliente.Sobrenome = "Jobim")
						.With(cliente => cliente.Email = "antonio.jobim@email.com")
						.With(cliente => cliente.Cpf = "580.276.580-12")
					.TheNext(1)
						.With(cliente => cliente.Nome = "Chico")
						.With(cliente => cliente.Sobrenome = "Buarque")
						.With(cliente => cliente.Email = "chico.buarque@email.com")
						.With(cliente => cliente.Cpf = "960.956.010-53")
					.TheNext(1)
						.With(cliente => cliente.Nome = "Elis")
						.With(cliente => cliente.Sobrenome = "Regina")
						.With(cliente => cliente.Email = "elis.regina@email.com")
						.With(cliente => cliente.Cpf = "414.718.530-47")
					.Build();
		}

		private static IList<Pedido> ConstruirPedidos()
		{
			IList<Cliente> clientes = ConstruirClientes();

			IList<Produto> produtos = ConstruirProdutos();

			return Builder<Pedido>.CreateListOfSize(5)
					.All()
						.With(pedido => pedido.Id = Guid.NewGuid())
					// Pedido 1: Antonio Jobim, Monitor: 1500 (2) / Cadeira: 1000 (1), Aberto
					.TheFirst(1)
						.With(pedido => pedido.Cliente = clientes[0])
						.With(pedido => pedido.Estado = EstadoPedidoEnum.Aberto)
						.With(pedido => pedido.ProdutosSelecionados
							= Builder<PedidoProduto>.CreateListOfSize(2)
								.All().With(pedidoProduto => pedidoProduto.Pedido = pedido)
								.TheFirst(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[0])
									.With(pedidoProduto => pedidoProduto.Quantidade = 2)
								.TheNext(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[1])
									.With(pedidoProduto => pedidoProduto.Quantidade = 1)
								.Build())
					// Pedido 2: Antonio Jobim, Cadeira: 1000 (2) / Mesa: 500 (3), Finalizado
					.TheNext(1)
						.With(pedido => pedido.Cliente = clientes[0])
						.With(pedido => pedido.Estado = EstadoPedidoEnum.Finalizado)
						.With(pedido => pedido.ProdutosSelecionados
							= Builder<PedidoProduto>.CreateListOfSize(2)
								.All().With(pedidoProduto => pedidoProduto.Pedido = pedido)
								.TheFirst(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[1])
									.With(pedidoProduto => pedidoProduto.Quantidade = 2)
								.TheNext(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[2])
									.With(pedidoProduto => pedidoProduto.Quantidade = 3)
								.Build())
					// Pedido 3: Chico Buarque, Monitor: 1500 (1) / Cadeira: 1000 (2) / Mesa: 500 (2), Aberto
					.TheNext(1)
						.With(pedido => pedido.Cliente = clientes[1])
						.With(pedido => pedido.Estado = EstadoPedidoEnum.Aberto)
						.With(pedido => pedido.ProdutosSelecionados
							= Builder<PedidoProduto>.CreateListOfSize(3)
								.All().With(pedidoProduto => pedidoProduto.Pedido = pedido)
								.TheFirst(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[0])
									.With(pedidoProduto => pedidoProduto.Quantidade = 1)
								.TheNext(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[1])
									.With(pedidoProduto => pedidoProduto.Quantidade = 2)
								.TheNext(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[2])
									.With(pedidoProduto => pedidoProduto.Quantidade = 2)
								.Build())
					// Pedido 4: Elis Regina, Cadeira: 1000 (2) / Mesa: 500 (4), Aberto
					.TheNext(1)
						.With(pedido => pedido.Cliente = clientes[2])
						.With(pedido => pedido.Estado = EstadoPedidoEnum.Aberto)
						.With(pedido => pedido.ProdutosSelecionados
							= Builder<PedidoProduto>.CreateListOfSize(2)
								.All().With(pedidoProduto => pedidoProduto.Pedido = pedido)
								.TheFirst(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[1])
									.With(pedidoProduto => pedidoProduto.Quantidade = 2)
								.TheNext(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[2])
									.With(pedidoProduto => pedidoProduto.Quantidade = 4)
								.Build())
					// Pedido 5: Elis Regina, Cadeira: 1000 (1) / Mesa: 500 (3), Cancelado
					.TheNext(1)
						.With(pedido => pedido.Cliente = clientes[2])
						.With(pedido => pedido.Estado = EstadoPedidoEnum.Cancelado)
						.With(pedido => pedido.ProdutosSelecionados
							= Builder<PedidoProduto>.CreateListOfSize(2)
								.All().With(pedidoProduto => pedidoProduto.Pedido = pedido)
								.TheFirst(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[1])
									.With(pedidoProduto => pedidoProduto.Quantidade = 1)
								.TheNext(1)
									.With(pedidoProduto => pedidoProduto.Produto = produtos[2])
									.With(pedidoProduto => pedidoProduto.Quantidade = 3)
								.Build())
					.Build();
		}
	}
}

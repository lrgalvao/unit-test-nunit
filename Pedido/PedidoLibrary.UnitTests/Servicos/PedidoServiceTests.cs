using FizzWare.NBuilder;
using MediatR;
using Moq;
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
	public class PedidoServiceTests
	{
		private readonly Mock<IPedidoRepository> mockPedidoRepository = new();
		private readonly Mock<IEmailService> mockEmailService = new();
		private readonly Mock<IMediator> mockMediator = new();

		private IPedidoService _pedidoService;

		[SetUp]
		public void ConfigurarTeste()
		{
			_pedidoService = new PedidoService(mockPedidoRepository.Object, mockEmailService.Object, mockMediator.Object);
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
			Assert.IsNotNull(pedido.Id);
			Assert.IsEmpty(pedido.ProdutosSelecionados);
			Assert.AreEqual(cliente, pedido.Cliente);
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
			Assert.IsNotNull(pedido.Id);
			Assert.IsEmpty(pedido.ProdutosSelecionados);
			Assert.AreEqual(cliente, pedido.Cliente);
		}

		[Test]
		public void PedidoService_CriarPedido_ClienteInvalido()
		{
			// Arrange
			var cliente = Builder<Cliente>.CreateNew().Build();

			// Act
			var excecao = Assert.Throws<NegocioException>(() => _pedidoService.CriarPedido(cliente, true));

			// Assert
			Assert.AreEqual(Mensagens.M04_CLIENTE_INVALIDO, excecao.Message);
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(It.IsAny<Guid>())).Returns(() => pedido);

			// Act
			_pedidoService.AdicionarProduto(idPedido, produto, quantidade);

			// Assert
			mockPedidoRepository.Verify(mock => mock.Atualizar(pedido), Times.Once());
			Assert.AreEqual(quantidade * valorProduto, pedido.ValorTotal());
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			var excecao = Assert.Throws<NegocioException>(() => _pedidoService.AdicionarProduto(idPedido, produto, quantidade));

			// Assert
			Assert.AreEqual(Mensagens.M01_PRODUTO_INVALIDO, excecao.Message);
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			var excecao = Assert.Throws<NegocioException>(() => _pedidoService.AdicionarProduto(idPedido, produto, quantidade));

			// Assert
			Assert.AreEqual(Mensagens.M02_QTDE_PRODUTOS_INVALIDA, excecao.Message);
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			// Act
			var excecao = Assert.Throws<NegocioException>(() => _pedidoService.AdicionarProduto(idPedido, produto, quantidade));

			// Assert
			Assert.AreEqual(Mensagens.M03_PEDIDO_NAO_EXISTE, excecao.Message);
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			var excecao = Assert.Throws<NegocioException>(() => _pedidoService.AdicionarProduto(idPedido, produto, quantidade));

			// Assert
			Assert.AreEqual(Mensagens.M10_PRODUTO_NAO_DISPONIVEL_EXPRESS, excecao.Message);
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


			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.RemoverProduto(idPedido, produto, 1);

			// Assert
			mockPedidoRepository.Verify(mock => mock.Atualizar(pedido), Times.Once());
			Assert.AreEqual((quantidade - 1) * valorProduto, pedido.ValorTotal());
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.RemoverProduto(idPedido, produto);

			// Assert
			mockPedidoRepository.Verify(mock => mock.Atualizar(pedido), Times.Once());
			Assert.AreEqual(0, pedido.ValorTotal());
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			var excecao = Assert.Throws<NegocioException>(() => _pedidoService.RemoverProduto(idPedido, produto, -1));

			// Assert
			Assert.AreEqual(Mensagens.M02_QTDE_PRODUTOS_INVALIDA, excecao.Message);
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.FinalizarPedido(idPedido);

			// Assert
			mockPedidoRepository.Verify(mock => 
				mock.Atualizar(pedido), 
				Times.Once());

			mockMediator.Verify(mock => 
				mock.Publish(It.IsAny<PedidoFinalizadoNotification>(), It.IsAny<CancellationToken>()), 
				Times.Once());

			mockEmailService.Verify(mock => 
				mock.EnviarEmail(cliente.Email, It.IsAny<string>(), It.IsAny<string>()), 
				Times.Once());

			Assert.AreEqual(EstadoPedidoEnum.Finalizado, pedido.Estado);
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

			mockPedidoRepository.Setup(mock => mock.ObterPorId(idPedido)).Returns(() => pedido);

			// Act
			_pedidoService.CancelarPedido(idPedido);

			// Assert
			mockPedidoRepository.Verify(mock =>
				mock.Atualizar(pedido),
				Times.Once());

			mockMediator.Verify(mock =>
				mock.Publish(It.IsAny<PedidoCanceladoNotification>(), It.IsAny<CancellationToken>()),
				Times.Once());

			Assert.AreEqual(EstadoPedidoEnum.Cancelado, pedido.Estado);
		}

		[Test]
		public async Task PedidoService_ObterPedidos_SemFiltroAsync()
		{
			// Arrange
			IList<Pedido> todosPedidos = ConstruirPedidos();

			mockPedidoRepository.Setup(mock => mock.ObterTodosAsync()).Returns(() => Task.FromResult(todosPedidos.ToList()));

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync();

			// Assert
			Assert.AreEqual(5, pedidos.Count);
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

			mockPedidoRepository.Setup(mock => mock.ObterTodosAsync()).Returns(() => Task.FromResult(todosPedidos.ToList()));

			var filtro = new PedidoFilter { Termo = termo };

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync(filtro);

			// Assert
			Assert.AreEqual(qtdePedidosObtidos, pedidos.Count);
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

			mockPedidoRepository.Setup(mock => mock.ObterTodosAsync()).Returns(() => Task.FromResult(todosPedidos.ToList()));

			var filtro
				= new PedidoFilter
				{
					Estados = estados != null ? new List<EstadoPedidoEnum>(estados) : null
				};

			// Act
			var pedidos = await _pedidoService.ObterPedidosAsync(filtro);

			// Assert
			Assert.AreEqual(qtdePedidosObtidos, pedidos.Count);
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

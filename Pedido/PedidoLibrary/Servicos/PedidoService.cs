using MediatR;
using PedidoLibrary.Entidades;
using PedidoLibrary.Repositorios;
using PedidoLibrary.Util;
using PedidoLibrary.Util.Notificacoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PedidoLibrary.Servicos
{
	public class PedidoService : IPedidoService
	{
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IEmailService _emailService;
		private IMediator _mediator;

		public PedidoService(IPedidoRepository pedidoRepository, IEmailService emailService, IMediator mediator)
		{
			_pedidoRepository = pedidoRepository;
			_emailService = emailService;
			_mediator = mediator;
		}

		public void AdicionarProduto(Guid idPedido, Produto produto, int quantidade)
		{
			if (produto == null || !produto.EhValido())
				throw new NegocioException(Mensagens.M01_PRODUTO_INVALIDO);

			if (quantidade <= 0)
				throw new NegocioException(Mensagens.M02_QTDE_PRODUTOS_INVALIDA);

			var pedido = _pedidoRepository.ObterPorId(idPedido);

			if (pedido == null)
				throw new NegocioException(Mensagens.M03_PEDIDO_NAO_EXISTE);

			if (pedido.EhExpress && !produto.DisponivelExpress)
				throw new NegocioException(Mensagens.M10_PRODUTO_NAO_DISPONIVEL_EXPRESS);

			// PONTO DE ATENÇÃO: E se levássemos este método para a classe Pedido?
			AdicionarProdutoAoPedido(pedido, produto, quantidade);

			_pedidoRepository.Atualizar(pedido);
		}

		private static void AdicionarProdutoAoPedido(Pedido pedido, Produto produto, int quantidade)
		{
			if (pedido.ProdutosSelecionados.ContainsKey(produto))
			{
				var quantidadeAtual = pedido.ProdutosSelecionados[produto];
				pedido.ProdutosSelecionados[produto] = quantidadeAtual + quantidade;
			}
			else
			{
				pedido.ProdutosSelecionados[produto] = quantidade;
			}
		}

		public void CancelarPedido(Guid idPedido)
		{
			var pedidoCancelado = AtualizarEstadoPedido(idPedido, EstadoPedidoEnum.Cancelado);

			_mediator.Publish(new PedidoCanceladoNotification(pedidoCancelado));
		}

		private Pedido AtualizarEstadoPedido(Guid idPedido, EstadoPedidoEnum novoEstado)
		{
			var pedido = _pedidoRepository.ObterPorId(idPedido);

			if (pedido == null)
				throw new NegocioException(Mensagens.M03_PEDIDO_NAO_EXISTE);

			pedido.Estado = novoEstado;

			_pedidoRepository.Atualizar(pedido);

			return pedido;
		}

		public Pedido CriarPedido(Cliente cliente, bool ehExpress)
		{
			if (cliente == null || !cliente.EhValido())
				throw new NegocioException(Mensagens.M04_CLIENTE_INVALIDO);

			var pedido = new Pedido
			{
				Id = Guid.NewGuid(),
				Cliente = cliente,
				EhExpress = ehExpress,
				Estado = EstadoPedidoEnum.Aberto,
				ProdutosSelecionados = new Dictionary<Produto, int>()
			};

			_pedidoRepository.Inserir(pedido);

			_mediator.Publish(new PedidoCriadoNotification(pedido));

			return pedido;
		}

		public void FinalizarPedido(Guid idPedido)
		{
			var pedidoFinalizado = AtualizarEstadoPedido(idPedido, EstadoPedidoEnum.Finalizado);

			_mediator.Publish(new PedidoFinalizadoNotification(pedidoFinalizado));

			EnviarEmailPedidoFinalizado(pedidoFinalizado);
		}

		private void EnviarEmailPedidoFinalizado(Pedido pedidoFinalizado)
		{
			try
			{
				_emailService.EnviarEmail(
					pedidoFinalizado.Cliente.Email,
					Mensagens.M05_TITULO_EMAIL_PEDIDO_FINALIZADO,
					Mensagens.M06_CORPO_EMAIL_PEDIDO_FINALIZADO);
			}
			catch
			{
				throw new NegocioException(Mensagens.M07_ERRO_ENVIO_EMAIL);
			}
		}

		public Pedido ObterPedido(Guid idPedido)
		{
			return _pedidoRepository.ObterPorId(idPedido);
		}

		public IList<Pedido> ObterPedidos(PedidoFilter filtro = null)
		{
			var pedidos = _pedidoRepository.ObterTodos().AsQueryable();

			pedidos = AplicarFiltroTermo(filtro, pedidos);
			pedidos = AplicarFiltroEstado(filtro, pedidos);

			return pedidos.ToList();
		}

		private static IQueryable<Pedido> AplicarFiltroEstado(PedidoFilter filtro, IQueryable<Pedido> pedidos)
		{
			if (filtro?.Estados != null && filtro.Estados.Any())
			{
				pedidos = pedidos.Where(p => filtro.Estados.Any(estado => estado == p.Estado));
			}

			return pedidos;
		}

		private static IQueryable<Pedido> AplicarFiltroTermo(PedidoFilter filtro, IQueryable<Pedido> pedidos)
		{
			if (!string.IsNullOrEmpty(filtro?.Termo))
			{
				filtro.Termo = filtro.Termo.Trim();

				pedidos = pedidos.Where(p =>
					p.Cliente.Nome.Contains(filtro.Termo)
					|| p.Cliente.Cpf.Contains(filtro.Termo)
					|| p.Cliente.Email.Contains(filtro.Termo)
					|| p.ProdutosSelecionados.Keys.Any(produto => produto.Nome.Contains(filtro.Termo)));
			}

			return pedidos;
		}

		public void RemoverProduto(Guid idPedido, Produto produto, int? quantidade = null)
		{
			if (produto == null || !produto.EhValido())
				throw new NegocioException(Mensagens.M01_PRODUTO_INVALIDO);

			if (quantidade.HasValue && quantidade.Value < 0)
				throw new NegocioException(Mensagens.M02_QTDE_PRODUTOS_INVALIDA);

			var pedido = _pedidoRepository.ObterPorId(idPedido);

			if (pedido == null)
				throw new NegocioException(Mensagens.M03_PEDIDO_NAO_EXISTE);

			if (!pedido.ProdutosSelecionados.ContainsKey(produto))
				throw new NegocioException(Mensagens.M08_PRODUTO_NAO_EXISTE_NO_PEDIDO);

			if (!quantidade.HasValue)
			{
				pedido.ProdutosSelecionados.Remove(produto);
			}
			else
			{
				RemoverProdutoPorQuantidade(produto, quantidade, pedido);
			}

			_pedidoRepository.Atualizar(pedido);
		}

		private static void RemoverProdutoPorQuantidade(Produto produto, int? quantidade, Pedido pedido)
		{
			var quantidadeAtual = pedido.ProdutosSelecionados[produto];

			var novaQuantidade = quantidadeAtual - quantidade.Value;

			if (novaQuantidade < 1)
			{
				pedido.ProdutosSelecionados.Remove(produto);
			}
			else
			{
				pedido.ProdutosSelecionados[produto] = novaQuantidade;
			}
		}
	}
}

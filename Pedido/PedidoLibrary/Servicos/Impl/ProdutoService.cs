using PedidoLibrary.Entidades;
using PedidoLibrary.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Servicos.Impl
{
	public class ProdutoService : IProdutoService
	{
		private readonly IProdutoRepository _produtoRepository;

		public ProdutoService(IProdutoRepository produtoRepository)
		{
			_produtoRepository = produtoRepository;
		}

		public void Atualizar(Produto produto)
		{
			_produtoRepository.Atualizar(produto);
		}

		public void Excluir(Guid idProduto)
		{
			_produtoRepository.Remover(idProduto);
		}

		public Produto Inserir(Produto produto)
		{
			_produtoRepository.Inserir(produto);
			return produto;
		}

		public Produto ObterProduto(Guid idProduto)
		{
			return _produtoRepository.ObterPorId(idProduto);
		}

		public Task<List<Produto>> ObterTodosAsync()
		{
			return _produtoRepository.ObterTodosAsync();
		}
	}
}

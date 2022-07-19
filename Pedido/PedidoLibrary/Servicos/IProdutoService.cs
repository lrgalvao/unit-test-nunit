using PedidoLibrary.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Servicos
{
	public interface IProdutoService
	{
		Produto Inserir(Produto produto);
		void Excluir(Guid idProduto);
		void Atualizar(Produto produto);
		Produto ObterProduto(Guid idProduto);
		Task<List<Produto>> ObterTodosAsync();
	}
}

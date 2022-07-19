using PedidoLibrary.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Servicos
{
	public interface IClienteService
	{
		Cliente Inserir(Cliente cliente);
		void Excluir(Guid idCliente);
		void Atualizar(Cliente cliente);
		Cliente ObterCliente(Guid idCliente);
		Task<List<Cliente>> ObterTodosAsync();
	}
}

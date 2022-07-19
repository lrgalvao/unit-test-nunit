using PedidoLibrary.Entidades;
using PedidoLibrary.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Servicos.Impl
{
	public class ClienteService : IClienteService
	{
		private readonly IClienteRepository _clienteRepository;

		public ClienteService(IClienteRepository clienteRepository)
		{
			_clienteRepository = clienteRepository;
		}

		public void Atualizar(Cliente cliente)
		{
			_clienteRepository.Atualizar(cliente);
		}

		public void Excluir(Guid idCliente)
		{
			_clienteRepository.Remover(idCliente);
		}

		public Cliente Inserir(Cliente cliente)
		{
			_clienteRepository.Inserir(cliente);
			return cliente;
		}

		public Cliente ObterCliente(Guid idCliente)
		{
			return _clienteRepository.ObterPorId(idCliente);
		}

		public Task<List<Cliente>> ObterTodosAsync()
		{
			return _clienteRepository.ObterTodosAsync();
		}
	}
}

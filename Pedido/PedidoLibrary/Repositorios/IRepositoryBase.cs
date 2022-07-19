using PedidoLibrary.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Repositorios
{
	public interface IRepositoryBase<TEntity> where TEntity : class, IEntity
	{
		void Inserir(TEntity entidade);
		void Atualizar(TEntity entidade);
		void Remover(Guid id);
		TEntity ObterPorId(Guid id);
		Task<List<TEntity>> ObterTodosAsync();
	}
}

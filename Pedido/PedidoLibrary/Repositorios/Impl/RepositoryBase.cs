using Microsoft.EntityFrameworkCore;
using PedidoLibrary.Contexto;
using PedidoLibrary.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidoLibrary.Repositorios.Impl
{
	public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>, IDisposable where TEntity : class, IEntity
	{
		protected readonly PedidoDbContext _context;
		protected virtual DbSet<TEntity> EntitySet { get; }

		protected RepositoryBase(PedidoDbContext context)
		{
			_context = context;
			EntitySet = _context.Set<TEntity>();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public void Inserir(TEntity entidade)
		{
			_context.Add(entidade);
			_context.SaveChanges();
		}

		public void Atualizar(TEntity entidade)
		{
			_context.Update(entidade);
			_context.SaveChanges();
		}

		public void Remover(Guid id)
		{
			var entidade = ObterPorId(id);
			if (entidade != null) 
			{
				_context.Remove(entidade);
				_context.SaveChanges();
			}			
		}

		public TEntity ObterPorId(Guid id)
		{
			return EntitySet.Find(id);
		}

		public Task<List<TEntity>> ObterTodosAsync()
		{
			return EntitySet.ToListAsync();
		}
	}
}

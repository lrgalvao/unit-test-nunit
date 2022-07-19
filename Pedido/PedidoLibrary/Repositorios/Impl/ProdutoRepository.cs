using PedidoLibrary.Contexto;
using PedidoLibrary.Entidades;

namespace PedidoLibrary.Repositorios.Impl
{
	public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
	{
		public ProdutoRepository(PedidoDbContext context) : base(context)
		{
		}
	}
}

using PedidoLibrary.Contexto;
using PedidoLibrary.Entidades;

namespace PedidoLibrary.Repositorios.Impl
{
	public class PedidoRepository : RepositoryBase<Pedido>, IPedidoRepository
	{
		public PedidoRepository(PedidoDbContext context) : base(context)
		{
		}
	}
}

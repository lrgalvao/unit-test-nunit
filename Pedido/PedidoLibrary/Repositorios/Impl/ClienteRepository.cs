using PedidoLibrary.Contexto;
using PedidoLibrary.Entidades;

namespace PedidoLibrary.Repositorios.Impl
{
	public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
	{
		public ClienteRepository(PedidoDbContext context) : base(context)
		{
		}
	}
}

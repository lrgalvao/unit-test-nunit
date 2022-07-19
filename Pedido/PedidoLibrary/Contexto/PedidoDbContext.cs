using Microsoft.EntityFrameworkCore;
using PedidoLibrary.Entidades;

namespace PedidoLibrary.Contexto
{
	public class PedidoDbContext : DbContext
	{
		public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options)
		{

		}

		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<Produto> Produtos { get; set; }
		public DbSet<Pedido> Pedidos { get; set; }

		public override void Dispose()
		{
		}
	}
}

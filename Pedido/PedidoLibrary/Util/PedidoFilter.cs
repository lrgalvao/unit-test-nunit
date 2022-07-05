using System.Collections.Generic;

namespace PedidoLibrary.Util
{
	public class PedidoFilter
	{
		public string Termo { get; set; }
		public IList<EstadoPedidoEnum> Estados { get; set; }
	}
}

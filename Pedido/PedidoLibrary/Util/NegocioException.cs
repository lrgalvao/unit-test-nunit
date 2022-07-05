using System;

namespace PedidoLibrary.Util
{
	public class NegocioException : Exception
	{
		public NegocioException(string message) : base(message)
		{
		}

		public NegocioException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

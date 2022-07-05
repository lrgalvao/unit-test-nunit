namespace PedidoLibrary.Servicos
{
	public interface IEmailService
	{
		bool EnviarEmail(string destinatario, string titulo, string mensagem);
	}
}

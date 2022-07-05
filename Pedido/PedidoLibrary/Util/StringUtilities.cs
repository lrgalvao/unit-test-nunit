using System.Net.Mail;

namespace PedidoLibrary.Util
{
	public static class StringUtilities
	{
		public static bool ValidarEmail(string email)
		{
			try
			{
				var mailAddress = new MailAddress(email);
				return mailAddress.Address == email;
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarCpf(string cpf)
		{
			int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
			int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

			string tempCpf;
			string digito;
			int soma;
			int resto;

			// PONTO DE ATENÇÃO: Imagina que você não se preocupou em validar se o parâmetro foi informado
			if (string.IsNullOrEmpty(cpf))
				return false;

			cpf = cpf.Trim();
			cpf = cpf.Replace(".", "").Replace("-", "");

			if (cpf.Length != 11)
				return false;

			tempCpf = cpf.Substring(0, 9);
			soma = 0;

			for (int i = 0; i < 9; i++)
				soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

			resto = soma % 11;

			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;

			digito = resto.ToString();
			tempCpf = tempCpf + digito;
			soma = 0;

			for (int i = 0; i < 10; i++)
				soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

			resto = soma % 11;

			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;

			digito = digito + resto.ToString();

			return cpf.EndsWith(digito);
		}
	}
}

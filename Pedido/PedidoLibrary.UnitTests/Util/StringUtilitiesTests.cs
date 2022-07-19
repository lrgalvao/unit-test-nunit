using NUnit.Framework;
using PedidoLibrary.Util;

namespace PedidoLibrary.Tests.Util
{
	public class StringUtilitiesTests
	{
		[TestCase(null)]
		[TestCase("")]
		[TestCase("000.000.111.11")]
		[TestCase("101010101")]
		public void StringUtilities_ValidarCpf_CpfInvalido(string cpf)
		{
			// Act
			var cpfValido = StringUtilities.ValidarCpf(cpf);

			// Assert
			Assert.IsFalse(cpfValido);
		}

		[TestCase("580.276.580-12")]
		[TestCase("824.533.640-81")]
		[TestCase("220.271.670-05")]
		[TestCase("960.956.010-53")]
		public void StringUtilities_ValidarCpf_CpfValido(string cpf)
		{
			// Act
			var cpfValido = StringUtilities.ValidarCpf(cpf);

			// Assert
			Assert.IsTrue(cpfValido);
		}

		[TestCase("580.276.580-12", true)]
		[TestCase("824.533.640-81", true)]
		[TestCase("220.271.670-05", true)]
		[TestCase("960.956.010-53", true)]
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase("000.000.111.11", false)]
		[TestCase("101010101", false)]
		public void StringUtilities_ValidarCpf(string cpf, bool resultadoEsperado)
		{
			// Act
			var cpfValido = StringUtilities.ValidarCpf(cpf);

			// Assert
			Assert.AreEqual(cpfValido, resultadoEsperado);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("@email")]
		[TestCase("email")]
		[TestCase("email da pessoa")]
		[TestCase("email@")]
		[TestCase("email@ email.com")]
		public void StringUtilities_ValidarEmail_EmailInvalido(string email)
		{
			// Act
			var emailValido = StringUtilities.ValidarEmail(email);

			// Assert
			Assert.IsFalse(emailValido);
		}

		[TestCase("email@email.com")]
		[TestCase("email@email.com.br")]
		[TestCase("email@email.br")]
		[TestCase("email.pessoa@email.com")]
		[TestCase("email_pessoa@email.br")]
		[TestCase("pessoa@email")]
		public void StringUtilities_ValidarEmail_EmailValido(string email)
		{
			// Act
			var emailValido = StringUtilities.ValidarEmail(email);

			// Assert
			Assert.IsTrue(emailValido);
		}
	}
}

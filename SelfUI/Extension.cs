using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SelfUI
{
	public partial class Program
	{
		private const string Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 Edg/133.0.0.0";
		private const string SourceUrl = "https://ids.hit.edu.cn";

		private const string LoginUrl = $"{SourceUrl}/authserver/login?service=http%3A%2F%2Fjw.hitsz.edu.cn%2FcasLogin";
		private const string CaptchaUrl = $"{SourceUrl}/authserver/checkNeedCaptcha.htl";

		protected static string ExtractExecution(string html)
		{
			var match = ExecutionRegex().Match(html);
			return match.Success ? match.Groups[1].Value : null;
		}

		public static string GetTimeStamp() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();

		public static void PauseExit()
		{
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		[GeneratedRegex(@"name=""execution"" value=""(.*?)""")]
		private static partial Regex ExecutionRegex();

		[GeneratedRegex(@"<input type=""hidden"" id=""pwdEncryptSalt"" value=""(.*?)""")]
		private static partial Regex SaltRegex();

		private static string GetCookie(IEnumerable<string> list)
		{
			string Cookie = string.Empty;
			foreach (string cookie in list)
			{
				int StartIndex = cookie.IndexOf("JSESSIONID=");
				if (StartIndex != -1)
					Cookie = cookie[StartIndex..cookie.IndexOf(';', StartIndex)] + "; ";
				else
				{
					StartIndex = cookie.IndexOf("route=");
					if (StartIndex != -1)
						Cookie += cookie[StartIndex..cookie.IndexOf(';', StartIndex)];
				}
			}
			return Cookie;
		}
	}

    public static class EncryptAES
	{
		private const string AesChars = "ABCDEFGHJKMNPQRSTWXYZabcdefhijkmnprstwxyz2345678";

		private static string RandomString(int n)
		{
			string f = string.Empty;
			for (int i = 0; i < n; i++)
				f += AesChars[Convert.ToInt32(Math.Floor(new Random().NextDouble() * AesChars.Length))];

			return f;
		}

        public static string EncryptString(string psw, string salt)
		{
			if (salt.Length != 16)
				throw new ArgumentException("Salt length must be 16.");

			byte[] plainText = Encoding.UTF8.GetBytes(RandomString(64) + psw);

			using Aes aesAlg = Aes.Create();

			aesAlg.Key = Encoding.UTF8.GetBytes(salt);
			aesAlg.IV = Encoding.UTF8.GetBytes(RandomString(16));

			aesAlg.Mode = CipherMode.CBC;
			aesAlg.Padding = PaddingMode.PKCS7;

			return Convert.ToBase64String(aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV).TransformFinalBlock(plainText, 0, plainText.Length));
		}
	}
}
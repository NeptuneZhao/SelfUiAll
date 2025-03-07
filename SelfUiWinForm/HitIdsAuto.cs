namespace HitIdsAuto
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Security.Cryptography;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.IO;
	using System.Threading;
	using System.Net;

	using Request = System.Collections.Generic.KeyValuePair<string, string>;
	using SelfUiWinForm;

	[SupportedOSPlatform("Windows")]
	public partial class IdsLogin : IDisposable
	{
		public IdsLogin()
		{
			HandlerIds = HandlerJiaowu = new()
			{
				UseCookies = true,
				CookieContainer = new(),
				AllowAutoRedirect = false
			};

			IdsClient = new(HandlerIds);
			JiaowuClient = new(HandlerJiaowu);

			Username = "2023111656";
			Password = "zhaochenrui233";

		}
		public IdsLogin(string username, string password)
		{
			HandlerIds = HandlerJiaowu = new()
			{
				UseCookies = true,
				CookieContainer = new(),
				AllowAutoRedirect = false
			};

			IdsClient = new(HandlerIds);
			JiaowuClient = new(HandlerJiaowu);

			Username = username ?? "2023111656";
			Password = password ?? "zhaochenrui233";

		}
		
		public async Task Start()
		{
			await Login();
		}

		private async Task Login()
		{
			async Task Restore()
			{
				await Task.Delay(Timeout);
				await Login();
			}

			string ExecutionPage = await IdsClient.GetStringAsync(LoginUrl);

			if ((await IdsClient.GetStringAsync($"{CaptchaUrl}?username={Username}&_={GetTimeStamp()}")).Contains("true"))
			{
				LogStatus("Captcha: failed");
				await Restore();
				return;
			}

			LogStatus("Captcha: OK");

			IdsClient.DefaultRequestHeaders.Add("Referer", LoginUrl);
			IdsClient.DefaultRequestHeaders.Add("Origin", SourceUrl);
			IdsClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			IdsClient.DefaultRequestHeaders.Add("User-Agent", Agent);

			FormUrlEncodedContent postData = new([
				new Request("username", Username),
				new Request("password", EncryptAES.EncryptString(Password, SaltRegex().Match(ExecutionPage).Groups[1].Value)),
				new Request("dllt", "generalLogin"),
				new Request("cllt", "userNameLogin"),
				new Request("_eventId", "submit"),
				new Request("captcha", ""),
				new Request("execution", ExtractExecution(ExecutionPage))
			]);

			HttpResponseMessage WaitToCasResponse = await IdsClient.PostAsync(LoginUrl, postData);

			if (WaitToCasResponse.StatusCode != HttpStatusCode.Found) // 302
			{
				LogStatus("CAS: failed");
				await Restore();
				return;
			}

			LogStatus("CAS: OK");

			HttpResponseMessage WaitToTicketResponse = await IdsClient.GetAsync(WaitToCasResponse.Headers.Location!.ToString());

			if (WaitToTicketResponse.StatusCode != HttpStatusCode.Found)
			{
				LogStatus("Ticket: failed");
				await Restore();
				return;
			}

			LogStatus("Ticket: OK");

			JiaowuClient.DefaultRequestHeaders.Add("Cookie", GetCookie(WaitToTicketResponse.Headers.GetValues("Set-Cookie")));
			JiaowuClient.DefaultRequestHeaders.Add("User-Agent", Agent);
			JiaowuClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			JiaowuClient.DefaultRequestHeaders.Add("Host", "jw.hitsz.edu.cn");

			HttpResponseMessage aacResponse = await JiaowuClient.GetAsync("http://jw.hitsz.edu.cn/authentication/main");

			LogStatus("Jiaowu: OK");

			JiaowuClient.DefaultRequestHeaders.Add("origin", "http://jw.hitsz.edu.cn");
			JiaowuClient.DefaultRequestHeaders.Add("referer", "http://jw.hitsz.edu.cn/authentication/main");

			// Keep online
			if (aacResponse.StatusCode == HttpStatusCode.OK)
				new Thread(new ThreadStart(async () => await KeepOnline())).Start();

			// Fetch lessons
			FormUrlEncodedContent lessonContent = new([
				new Request("xn", "2024-2025"),
				new Request("xq", "2")
			]);

			HttpResponseMessage lessonResponse = await JiaowuClient.PostAsync("http://jw.hitsz.edu.cn/xszykb/queryxszykbzong", lessonContent);

			LogStatus("Lessons: OK");

			if (lessonResponse.StatusCode == HttpStatusCode.OK)
			{
				string lessonHtml = await lessonResponse.Content.ReadAsStringAsync();
				// Save in json file
				File.WriteAllText("lessons.json", lessonHtml);

			}
		}

		private static void LogStatus(string msg)
		{
			Program.mainForm.StatusLabel.Text += $"{msg} at {DateTime.Now:F}";
			Program.mainForm.StatusLabel.Refresh();
		}

		private async Task KeepOnline()
		{
			async Task Restore()
			{
				await Task.Delay(Timeout);
				await KeepOnline();
			}

			StringContent onlineContent = new("{\"code\":0,\"msg\":null,\"msg_en\":null,\"content\":null}", Encoding.UTF8, "application/json");

			HttpResponseMessage response = await JiaowuClient.PostAsync("http://jw.hitsz.edu.cn/component/online", onlineContent);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				await Restore();
				return;
			}

			await Task.Delay(60 * 1000);
			Program.mainForm.Text = $"SelfUI - Online at {DateTime.Now:F}\n";
			await KeepOnline();
		}

		private static string GetTimeStamp() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();

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

		private static string ExtractExecution(string html)
		{
			var match = ExecutionRegex().Match(html);
			return match.Success ? match.Groups[1].Value : string.Empty;
		}

		private const string Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 Edg/133.0.0.0";
		private const string SourceUrl = "https://ids.hit.edu.cn";

		private const string LoginUrl = $"{SourceUrl}/authserver/login?service=http%3A%2F%2Fjw.hitsz.edu.cn%2FcasLogin";
		private const string CaptchaUrl = $"{SourceUrl}/authserver/checkNeedCaptcha.htl";

		private const int Timeout = 10 * 1000;

		private readonly HttpClientHandler HandlerIds, HandlerJiaowu;
		private readonly HttpClient IdsClient, JiaowuClient;

		private readonly string Username, Password;

		void IDisposable.Dispose()
		{
			IdsClient.Dispose();
			JiaowuClient.Dispose();

			HandlerIds.Dispose();
			HandlerJiaowu.Dispose();

			GC.SuppressFinalize(this);
		}

	}

	internal static class EncryptAES
	{
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

		private const string AesChars = "ABCDEFGHJKMNPQRSTWXYZabcdefhijkmnprstwxyz2345678";
		private static string RandomString(int n)
		{
			string f = string.Empty;
			for (int i = 0; i < n; i++)
				f += AesChars[Convert.ToInt32(Math.Floor(new Random().NextDouble() * AesChars.Length))];

			return f;
		}
	}

	internal class Logger
	{
		public Logger()
		{
			if (!Directory.Exists("Logs"))
				Directory.CreateDirectory("Logs");
			LogPath = Path.Combine("Logs", $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}.log");
		}

		public static void Log(string message)
		{
			if (!Directory.Exists("Logs"))
				Directory.CreateDirectory("Logs");
			using StreamWriter sw = new(LogPath, true);
			sw.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
		}

		private static string LogPath;

	}
}

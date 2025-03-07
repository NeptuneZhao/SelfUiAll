global using System;
global using System.Net.Http;
global using System.Collections.Generic;

using System.IO;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SelfUI
{
	using Request = KeyValuePair<string, string>;

	public partial class Program
	{
		public static async Task Main()
		{
			string Username = "2023111656", Password = "zhaochenrui233";

			HttpClientHandler HandlerIds = new()
			{
				UseCookies = true,
				CookieContainer = new(),
				AllowAutoRedirect = false,
			}, HandlerJiaowu = HandlerIds;

			HttpClient IdsClient = new(HandlerIds);

			// Find the Salt while encrypting the password
			string ExecutionPage = await IdsClient.GetStringAsync(LoginUrl);

			// Byd, bypass the captcha
			if ((await IdsClient.GetStringAsync($"{CaptchaUrl}?username={Username}&_={GetTimeStamp()}")).Contains("true"))
			{
				Console.WriteLine("Captcha failed.");
				Process.Start(Assembly.GetExecutingAssembly().Location);
				Environment.Exit(0);
			}

			// Post the form
			FormUrlEncodedContent postData = new(
			[
				new Request("username", Username),
				new Request("password", EncryptAES.EncryptString(Password, SaltRegex().Match(ExecutionPage).Groups[1].Value)),
				new Request("dllt", "generalLogin"),
				new Request("cllt", "userNameLogin"),
				new Request("_eventId", "submit"),
				new Request("captcha", ""),
				new Request("execution", ExtractExecution(ExecutionPage))
			]);

			IdsClient.DefaultRequestHeaders.Add("Referer", LoginUrl);
			IdsClient.DefaultRequestHeaders.Add("Origin", SourceUrl);
			IdsClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			IdsClient.DefaultRequestHeaders.Add("User-Agent", Agent);

			HttpResponseMessage WaitToCasResponse = await IdsClient.PostAsync(LoginUrl, postData);

			// First, redirect from IDS to CAS(ticket)

			if (WaitToCasResponse.StatusCode != HttpStatusCode.Found) // 302
			{
				Console.WriteLine("Login failed at IDS/CAS sso.");
				PauseExit();
			}

			Console.WriteLine("Login successfully. Redirecting to CAS ticket...");

			// Second, redirect from CAS to Jiaowu

			HttpResponseMessage WaitToTicketResponse = await IdsClient.GetAsync(WaitToCasResponse.Headers.Location.ToString());

			if (WaitToTicketResponse.StatusCode != HttpStatusCode.Found)
			{
				Console.WriteLine("Login failed at Jiaowuxitong-ticket recognizing.");
				PauseExit();
			}

			// Last, go into /authentication/main using cookies provided by Step II.
			HttpClient JiaowuClient = new(HandlerJiaowu);

			// Add headers
			JiaowuClient.DefaultRequestHeaders.Add("Cookie", GetCookie(WaitToTicketResponse.Headers.GetValues("Set-Cookie")));
			JiaowuClient.DefaultRequestHeaders.Add("User-Agent", Agent);
			JiaowuClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			JiaowuClient.DefaultRequestHeaders.Add("Host", "jw.hitsz.edu.cn");

			HttpResponseMessage aacResponse = await JiaowuClient.GetAsync("http://jw.hitsz.edu.cn/authentication/main");

			string AcademicAffairsContent = await aacResponse.Content.ReadAsStringAsync();

			// 把重定向到的网址的内容输出到文件
			await File.WriteAllTextAsync("AcademicAffairs.html", AcademicAffairsContent);
			Console.WriteLine("AcademicAffairs.html has been saved.");

			// Proceed
			Console.ReadKey();
		}
	}
}
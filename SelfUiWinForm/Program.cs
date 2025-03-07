global using System;
global using System.Runtime.Versioning;
global using System.Windows.Forms;
using HitIdsAuto;
using System.Net.Http;

namespace SelfUiWinForm
{
	[SupportedOSPlatform("Windows")]
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			HitszKebiao keBiao = new();
			// 从文件中读取json
			keBiao.ParseLessonJson(System.IO.File.ReadAllText("lessons.json"));
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(mainForm);

		}

		public static MainForm mainForm = new();
		public static HttpClient JiaowuClient;
		public static string LessonJson;
	}
}

namespace HitIdsAuto
{
	using System.Collections.Generic;
	using System.Text.Json;
	public class HitszKebiao
	{
		private IList<string> LessonCollection;

		public void ParseLessonJson(string Json)
		{
			LessonCollection = Json.Split("},{");
			for (int i = 0; i < LessonCollection.Count; i++)
			{
				string var = LessonCollection[i];
				int
					Front = var.IndexOf("\"SKSJ\":"),
					Back = var.IndexOf("\"XB\"");
				LessonCollection[i] = var[(Front + 7)..Back];
			}
            // 写入
            System.IO.File.WriteAllLines("lessons.txt", LessonCollection);
        }
	}
}

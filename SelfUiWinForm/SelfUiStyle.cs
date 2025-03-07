
namespace SelfUI
{
	[SupportedOSPlatform("windows")]
	public class SelfUiStyle
	{

		internal AnchorStyles StopAnchor = AnchorStyles.None;

		public void MsStopAnchor(Form form)
		{
			if (form.Top <= 0 && form.Left <= 0)
				StopAnchor = AnchorStyles.None;
			else if (form.Top <= 0)
                StopAnchor = AnchorStyles.Top;
            else if (form.Left <= 0)
                StopAnchor = AnchorStyles.Left;
            else if (form.Left >= Screen.PrimaryScreen.Bounds.Width - form.Width)
                StopAnchor = AnchorStyles.Right;
            else if (form.Top >= Screen.PrimaryScreen.Bounds.Height - form.Height)
                StopAnchor = AnchorStyles.Bottom;
			else
                StopAnchor = AnchorStyles.None;
        }
	}
}

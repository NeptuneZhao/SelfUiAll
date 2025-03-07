
using HitIdsAuto;
using SelfUI;
using System.Drawing;

namespace SelfUiWinForm
{
	[SupportedOSPlatform("windows")]
	public partial class MainForm : Form
	{
		private readonly SelfUiStyle Style = new();
		private readonly IdsLogin Login = new();

		public MainForm()
		{
			InitializeComponent();

		}

		private async void MainForm_Load(object sender, EventArgs e)
		{
			Text = "SelfUI - Login";
			StatusLabel.Text = "!";
			await Login.Start();
		}

		private void MainForm_LocationChanged(object sender, EventArgs e)
		{
			Style.MsStopAnchor(this);
		}

		private void HiddenTimer_Tick(object sender, EventArgs e)
		{
			int HiddenPixel = 60;
			if (this.Bounds.Contains(Cursor.Position))
			{
				switch (Style.StopAnchor)
				{
					case AnchorStyles.Top:
						this.Location = new Point(this.Location.X, 0);
						break;
					case AnchorStyles.Left:
						this.Location = new Point(0, this.Location.Y);
						break;
					case AnchorStyles.Right:
						this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, this.Location.Y);
						break;
				}
			}
			else
			{
				switch (Style.StopAnchor)
				{
					case AnchorStyles.Top:
						this.Location = new Point(this.Location.X, HiddenPixel - this.Height);
						break;
					case AnchorStyles.Left:
						this.Location = new Point(HiddenPixel - this.Width, this.Location.Y);
						break;
					case AnchorStyles.Right:
						this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - HiddenPixel, this.Location.Y);
						break;
				}
			}

		}

		private void StatusLabel_Click(object sender, EventArgs e)
		{

		}
	}
}

using ELFVoiceChanger.Core;

namespace ELFVoiceChanger.View.Forms
{
	public partial class StartupForm : Form
	{
		public StartupForm()
		{
			InitializeComponent();
		}

		private void StartupForm_Shown(object sender, EventArgs e)
		{
			StartupManager.Startup(this);
		}
	}
}
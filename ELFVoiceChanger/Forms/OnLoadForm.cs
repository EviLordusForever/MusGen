using ELFVoiceChanger.Forms;

namespace ELFVoiceChanger
{
	public partial class OnLoadForm : Form
	{
		public OnLoadForm()
		{
			InitializeComponent();
			FormsManager.mainForm = new MainForm();
			FormsManager.mainForm.Show();
		}

		private void OnLoadForm_Shown(object sender, EventArgs e)
		{
			this.Visible = false;
		}
	}
}
using MusGen.Forms;
using MusGen.Core;

namespace MusGen
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

		private void OnLoadForm_Load(object sender, EventArgs e)
		{
			//Tests.Test1();
			//Tests.Test2();
		}
	}
}
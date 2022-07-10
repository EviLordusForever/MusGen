using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELFVoiceChanger.Forms
{
	public partial class AddLetterPatternForm : Form
	{
		public AddLetterPatternForm()
		{
			InitializeComponent();
		}

		private void AddLetterPatternForm_Load(object sender, EventArgs e)
		{
			voiceModelNameBox.Text = FormsManager.manageVoiceModelsForm.voiceModelsBox.SelectedItem as string;
		}
	}
}

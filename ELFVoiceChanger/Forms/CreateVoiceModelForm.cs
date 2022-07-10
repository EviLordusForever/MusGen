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
	public partial class CreateVoiceModelForm : Form
	{
		public CreateVoiceModelForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string name = textBox1.Text.Trim();

			if (name.Length > 0 && !VoiceModelsManager.voiceModelsNames.Contains(name))
			{
				VoiceModelsManager.AddVoiceModel(name);
				FormsManager.manageVoiceModelsForm.voiceModelsBox.Items.Add(name);
				FormsManager.manageVoiceModelsForm.voiceModelsBox.SelectedIndex = FormsManager.manageVoiceModelsForm.voiceModelsBox.Items.Count - 1;
				FormsManager.manageVoiceModelsForm.voiceModelNameBox.Enabled = true;
				this.Close();
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}
}

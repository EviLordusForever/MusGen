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
	public partial class ManageVoiceModelsForm : Form
	{
		public ManageVoiceModelsForm()
		{
			InitializeComponent();
		}

		private void ManageVoiceModelsForm_Load(object sender, EventArgs e)
		{
			if (VoiceModelsManager.voiceModelsNames.Count() > 0)
			{
				foreach (string str in VoiceModelsManager.voiceModelsNames)
					voiceModelsBox.Items.Add(str);
				voiceModelsBox.SelectedIndex = 0;
			}
			else
				voiceModelNameBox.Enabled = false;
		}

		private void voiceModelsBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			voiceModelNameBox.Text = voiceModelsBox.SelectedItem as string;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (voiceModelsBox.Items.Count > 0)
			{
				VoiceModelsManager.DeleteVoiceModel(voiceModelNameBox.Text);
				int selectedIndex = voiceModelsBox.SelectedIndex;
				voiceModelsBox.Items.Remove(voiceModelNameBox.Text);

				if (selectedIndex > 0)
					voiceModelsBox.SelectedIndex = selectedIndex - 1;
				else if (voiceModelsBox.Items.Count > 0)
					voiceModelsBox.SelectedIndex = selectedIndex;
				else
				{
					voiceModelsBox.SelectedIndex = -1;
					voiceModelNameBox.Enabled = false;
				}
			}				
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FormsManager.OpenCreateVoiceModelForm();
		}

		private void voiceModelNameBox_TextChanged(object sender, EventArgs e)
		{
			string name = voiceModelNameBox.Text.Trim();
			string oldName = voiceModelsBox.SelectedItem as string;

			if (name == oldName || Directory.Exists(Disk.programFiles + "\\VoiceModels\\" + name) || name.Trim().Length < 0)
			{
				voiceModelNameBox.Text = oldName;
				return;
			}

			int id = voiceModelsBox.Items.IndexOf(oldName);
			voiceModelsBox.Items.RemoveAt(id);
			voiceModelsBox.Items.Insert(id, name);
			voiceModelsBox.SelectedIndex = id;

			VoiceModelsManager.Rename(oldName, name);
		}
	}
}

using ELFVoiceChanger.Core;
using ELFVoiceChanger.Voice;
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

				IfVoiceModelsExist();
				LoadLetterPatternsList();
			}
			else
			{
				IfNoVoiceModels();
				IfNoLetterPatterns();
			}
		}

		private void voiceModelsBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (voiceModelsBox.SelectedIndex > -1)
			{
				voiceModelNameBox.Text = voiceModelsBox.SelectedItem as string;
				if (FormsManager.addLetterPatternForm != null && !FormsManager.addLetterPatternForm.IsDisposed)
					FormsManager.addLetterPatternForm.voiceModelNameBox.Text = voiceModelsBox.SelectedItem as string;
				letterPatternsBox.Enabled = true;
				letterPatternNameBox.Enabled = true;
				LoadLetterPatternsList();
			}
			else
				IfNoVoiceModels();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string name = voiceModelNameBox.Text;

			if (voiceModelsBox.Items.Count > 0)
			{
				if (UserAsker.Ask("A you really want to delete " + name + "?"))
				{
					VoiceModelsManager.DeleteVoiceModel(name);
					int selectedIndex = voiceModelsBox.SelectedIndex;
					voiceModelsBox.Items.Remove(name);

					if (selectedIndex > 0)
						voiceModelsBox.SelectedIndex = selectedIndex - 1;
					else if (voiceModelsBox.Items.Count > 0)
						voiceModelsBox.SelectedIndex = selectedIndex;
					else
						IfNoVoiceModels();
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

			VoiceModelsManager.Rename(oldName, name);

			voiceModelsBox.SelectedIndex = id;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (voiceModelsBox.Items.Count > 0)
				FormsManager.OpenAddLetterPatternForm();
		}

		public void IfNoVoiceModels()
		{
			voiceModelsBox.Enabled = false;
			voiceModelNameBox.Enabled = false;
			letterPatternsBox.Enabled = false;
			letterPatternNameBox.Enabled = false;
			button4.Enabled = false;
			button2.Enabled = false;
			selectAudioFileButton.Enabled = false;

			voiceModelNameBox.Text = "";
			letterPatternNameBox.Text = "";

			FormsManager.CloseAddLetterPatternForm();

			IfNoLetterPatterns();
		}

		public void IfNoLetterPatterns()
		{
			letterPatternNameBox.Enabled = false;
			letterPatternsBox.Enabled = false;
			deleteLetterPatternButton.Enabled = false;
			selectAudioFileButton.Enabled = false;

			letterPatternNameBox.Text = "";
			selectAudioFileButton.Text = "Select audio file";
		}

		public void IfLetterPatternsExist()
		{
			letterPatternNameBox.Enabled = true;
			letterPatternsBox.Enabled = true;
			deleteLetterPatternButton.Enabled = true;
			selectAudioFileButton.Enabled = true;

			letterPatternNameBox.Text = letterPatternsBox.SelectedItem as string;
			selectAudioFileButton.Text = letterPatternsBox.SelectedItem as string + ".wav";

		}

		public void IfVoiceModelsExist()
		{
			voiceModelsBox.Enabled = true;
			voiceModelNameBox.Enabled = true;
			button4.Enabled = true;
			button2.Enabled = true;
		}

		public void LoadLetterPatternsList()
		{
			int previousCount = letterPatternsBox.Items.Count;

			letterPatternsBox.Items.Clear();
			string[] patterns = Directory.GetFiles(Disk.programFiles + "\\VoiceModels\\" + voiceModelNameBox.Text + "\\LetterPatterns\\");

			if (patterns.Count() > 0)
			{
				foreach (string pattern in patterns)
					letterPatternsBox.Items.Add(TextMethods.StringBeforeLast(TextMethods.StringAfterLast(pattern, "\\"), "."));

				letterPatternsBox.SelectedIndex = 0;

				if (previousCount == 0)
					IfLetterPatternsExist();
			}
			else
				IfNoLetterPatterns();
		}

		private void ManageVoiceModelsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormsManager.CloseAddLetterPatternForm();
			FormsManager.CloseCreateVoiceModelForm();
		}

		private void letterPatternBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (letterPatternsBox.SelectedIndex > -1)
			{
				letterPatternNameBox.Text = letterPatternsBox.SelectedItem as string;

				selectAudioFileButton.Text = letterPatternsBox.SelectedItem as string + ".wav";
			}
			else
				IfNoLetterPatterns();
		}

		private void deleteLetterPatternButton_Click(object sender, EventArgs e)
		{
			string voiceModelName = voiceModelNameBox.Text;
			string name = letterPatternNameBox.Text;

			if (letterPatternsBox.Items.Count > 0)
			{
				if (UserAsker.Ask("A you really want to delete " + name + "?"))
				{
					VoiceModelsManager.DeleteLetterPattern(voiceModelName, name);
					
					int selectedIndex = letterPatternsBox.SelectedIndex;
					letterPatternsBox.Items.Remove(name);

					if (selectedIndex > 0)
						letterPatternsBox.SelectedIndex = selectedIndex - 1;
					else if (letterPatternsBox.Items.Count > 0)
						letterPatternsBox.SelectedIndex = selectedIndex;
					else
						IfNoLetterPatterns();
				}
			}
		}

		private void selectAudioFileButton_Click(object sender, EventArgs e)
		{
			if (UserAsker.Ask("Are you shure you want change audio file? (If you have no origin or copy for previous audio it will be replaced)"))
				openFileDialog1.ShowDialog();
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			string newPath = openFileDialog1.FileName;
			bool isGood = Wav.CheckWav(newPath);

			if (isGood)
				VoiceModelsManager.ChangeLetterPattern(voiceModelNameBox.Text, letterPatternNameBox.Text, newPath);
		}
	}
}

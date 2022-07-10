using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ELFVoiceChanger.Voice;
using ELFVoiceChanger.Core;

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

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			textBox1.Text = textBox1.Text.Trim();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog();
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			bool isGood = Wav.CheckWav(openFileDialog1.FileName);

			if (isGood)
			{
				button3.Text = TextMethods.StringAfterLast(openFileDialog1.FileName, "\\");
				button3.ForeColor = Color.Green;
				if (textBox1.Text.Length <= 0)
					textBox1.Text = TextMethods.StringBeforeLast(TextMethods.StringAfterLast(openFileDialog1.FileName, "\\"), ".");
			}
			else
			{
				button3.Text = "Select audio file";
				button3.ForeColor = Color.Red;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string name = textBox1.Text;
			if (button3.ForeColor == Color.Green && name.Length > 0)
			{
				try
				{
					VoiceModelsManager.AddLetterPattern(voiceModelNameBox.Text, name, openFileDialog1.FileName);
					FormsManager.manageVoiceModelsForm.letterPatternsBox.Items.Add(name);
					FormsManager.manageVoiceModelsForm.letterPatternNameBox.Text = name;
					FormsManager.manageVoiceModelsForm.letterPatternsBox.SelectedIndex = FormsManager.manageVoiceModelsForm.letterPatternsBox.Items.Count - 1;
					if (FormsManager.manageVoiceModelsForm.letterPatternsBox.Items.Count == 1)
						FormsManager.manageVoiceModelsForm.IfLetterPatternsExist();
					this.Close();
				}
				catch
				{
				}
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusGen.Voice;
using MusGen.Core;

namespace MusGen.Forms
{
	public partial class VoiceChangingForm : Form
	{
		public VoiceChangingForm()
		{
			InitializeComponent();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void selectAudioFileButton_Click(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog();
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			string path = openFileDialog1.FileName;
			bool isGood = Wav.CheckWav(path);

			if (isGood)
			{
				selectAudioFileButton.Text = TextMethods.StringAfterLast(openFileDialog1.FileName, "\\");
				selectAudioFileButton.ForeColor = Color.Green;
			}
			else
			{
				selectAudioFileButton.Text = "Select .wav file";
				selectAudioFileButton.ForeColor = Color.Red;
			}
		}

		private void VoiceChangingForm_Load(object sender, EventArgs e)
		{
			if (VoiceModelsManager.voiceModelsNames.Count > 0)
			{
				foreach (string vm in VoiceModelsManager.voiceModelsNames)
				{
					voiceModelsOrig.Items.Add(vm);
					voiceModelsExport.Items.Add(vm);
				}
				voiceModelsOrig.SelectedIndex = 0;
				voiceModelsExport.SelectedIndex = 0;
			}
			else
				this.Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				//EffectsCreator.Effect7(openFileDialog1.FileName, exportFileName.Text, 10000);
				Nad nad = new Nad();
				Wav wav = new Wav();
				wav.Read(openFileDialog1.FileName);
				nad.MakeNad(wav, 1);

				Wav wavOut = new Wav();
				wavOut.Read(openFileDialog1.FileName);
				wavOut = nad.MakeWav(wavOut);
				wavOut.Export(exportFileName.Text);
			}
		}
	}
}

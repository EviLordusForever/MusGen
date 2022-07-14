using System;
using System.Collections.Generic;
using ELFVoiceChanger.Core;

namespace ELFVoiceChanger.Forms
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			pictureBox1.ImageLocation = Disk.programFiles + "Images\\img.png";
			pictureBox1.Load();

			//Wav wav = new Wav();
			//wav.ReadWav(Disk.programFiles + "Amongus.wav");
			//wav.SaveWav(Disk.programFiles + "Amongus2.wav");


		}

		private void button1_Click(object sender, EventArgs e)
		{
			FormsManager.OpenManageVoiceModelForm();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			FormsManager.OpenVoiceChangingForm();
		}
	}
}

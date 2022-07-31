using System;
using System.Collections.Generic;
using ELFVoiceChanger.Core;
using ELFVoiceChanger.View;
using ELFVoiceChanger.Voice;

namespace ELFVoiceChanger.View.Forms
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FormsManager.OpenManageVoiceModelForm();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			FormsManager.OpenVoiceChangingForm();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			FormsManager.OpenEffectsForm();
		}
	}
}

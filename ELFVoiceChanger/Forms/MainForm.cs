using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ELFVoiceChanger.Forms.FormsManager;

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

			Wav wav = new Wav();
			wav.ReadWav(Disk.programFiles + "Amongus.wav");
			wav.SaveWav(Disk.programFiles + "Amongus2.wav");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FormsManager.OpenManageVoiceModelForm();
		}
	}
}

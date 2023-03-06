using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.View;
using ELFVoiceChanger.Voice;
using ELFVoiceChanger.View.Forms;

namespace ELFVoiceChanger.Core
{
	public static class StartupManager
	{
		public static void Startup(Form sender)
		{
			sender.Visible = false;

			Disk.currentDirectory = Environment.CurrentDirectory;
			Disk.programFiles = Disk.currentDirectory + "\\ProgramFiles\\";

			FormsManager.mainForm = new MainForm();
			FormsManager.mainForm.Show();

			FormsManager.mainForm.pictureBox1.ImageLocation = Disk.programFiles + "Images\\img.png";
			FormsManager.mainForm.pictureBox1.Load();

			//PeriodFinder.DFT();

			Wav wav = new Wav();
			wav.InitializeFrom("Colorful");
			for (int i = 0; i < wav.Length; i++)
			{
				wav.L[i] = (float)(Math.Sin(i * Math.PI / 180) * 0.99);
				wav.R[i] = (float)(Math.Sin(i * Math.PI / 180) * 0.99);
			}

			wav.Export("SIN");
		}
	}
}

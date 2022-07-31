using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.View;
using ELFVoiceChanger.Voice;

namespace ELFVoiceChanger.Core
{
	public static class StartupManager
	{
		public static void Startup()
		{
			FormsManager.mainForm.pictureBox1.ImageLocation = Disk.programFiles + "Images\\img.png";
			FormsManager.mainForm.pictureBox1.Load();

			//Wav wav = new Wav();
			//wav.ReadWav(Disk.programFiles + "Amongus.wav");
			//wav.SaveWav(Disk.programFiles + "Amongus2.wav");

			//PeriodFinder.DFT();

			Wav wav = new Wav();
			wav.InitializeFrom("Amongus");
			for (int i = 0; i < wav.Length; i++)
				wav.L[i] = (float)Math.Sin(i * Math.PI / 180);

			wav.SaveWav("SIN");
		}
	}
}

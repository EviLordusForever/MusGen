using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.Core;

namespace ELFVoiceChanger.Voice
{
	public static class AudioProcessor
	{
		public static void ProcessAudio(string originPath, string name)
		{
			Wav wav1 = new Wav();
			Wav wav2 = new Wav();
			wav1.ReadWav(originPath, 0);
			wav2.ReadWav(originPath, 0);


			wav2.L = new float[wav1.L.Length];
			wav2.R = new float[wav1.R.Length];
			
  			double t = 0;
			for (int i = 0; t < wav1.L.Length; i++)
			{
				wav2.L[i] = wav1.L[(int)t];
				t += Math.Sin(i / 8096.0) + 1;
			}

			t = 0;
			for (int i = 0; t < wav1.R.Length; i++)
			{
				wav2.R[i] = wav1.R[(int)t];
				t += Math.Sin(i / 8080.0) + 1;
			}

			wav2.SaveWav(Disk.programFiles + "Export\\" + name + ".wav");
		}
	}
}

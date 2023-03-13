using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class Nad
	{
		public NadSample samples;

		public void MakeNad(Wav wav)
		{
			float bpm = BPMFinder.FindBMP(wav);
			int samplesCount = (int)(wav.Length / bpm);

			for (int s = 0; s < samplesCount; s++)
			{

			}
		}
	}
}
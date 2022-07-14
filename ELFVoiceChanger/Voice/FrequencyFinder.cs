using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Voice
{
	public static class FrequencyFinder
	{
		public static int minFrequency = 100;
		public static int maxFrequency = 40000;
		public static int tries = 5;

		public static int FindFrequency(Wav wav, int start, int end)
		{
			float[] mismatches = new float[maxFrequency - minFrequency];

			for (int frequency = minFrequency; frequency < maxFrequency; frequency++)
			{
				float mismatch = 0;
				int sampleDelta = (end - start) / (tries + 1); //check

				for (int startSample = start; startSample < end - 5; startSample += sampleDelta)
				{
					for (int sample = startSample; sample < startSample + frequency; sample++)
					{
						mismatch += Math.Abs(wav.L[sample] - wav.L[sample + frequency]);
						if (wav.channels == 2) //Is this necessary?
							mismatch += Math.Abs(wav.R[sample] - wav.R[sample + frequency]);
					}
					mismatch /= frequency;
				}
				mismatch /= tries;
				mismatches[frequency - minFrequency] = mismatch;
			}

			return 0;
		}
	}
}
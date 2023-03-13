using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class Nad
	{
		public int channelsCount;
		public NadSample[] samples;
		public float bps;

		public void MakeNad(Wav wav, int n)
		{
			channelsCount = n;
			bps = BPMFinder.FindBPS(wav);
			float sps = wav.sampleRate / bps;
			int nadSamplesCount = (int)(wav.Length / sps);
			samples = new NadSample[nadSamplesCount];
			double m;

			ProgressShower.ShowProgress("Making nad");

			for (int s = 0; s < nadSamplesCount; s++)
			{
				samples[s] = new NadSample(n);

				float[] periods = new float[n];
				float[] amplitudes = new float[n];

				PeriodFinder.FP_DFT_MULTI(ref samples[s].periods, ref samples[s].amplitudes, wav, (int)(sps * s), (int)sps, (int)(sps/200f));
				ProgressShower.SetProgress(1.0 * s / nadSamplesCount);
			}

			ProgressShower.CloseProgressForm();
		}

		public Wav MakeWav(Wav wavOut)
		{
			ProgressShower.ShowProgress("Making wav from nad");

			float pi2 = MathF.PI * 2;

			float sint = 0;
			float A = 1;
			double AA = 1;

			double i2 = 0;

			double mismatch = 1;
			double mismatchLimit = 1;

			float[] ampsOld = new float[channelsCount];
			float[] t = new float[channelsCount];

			for (int s = 0; s < samples.Count(); s++)
			{
				sint = 0;

				float sampleRate = wavOut.sampleRate;
				float sps = wavOut.sampleRate / bps;
				int nadSampleNumber = (int)(s / sps);

				for (int a = 0; a < channelsCount; a++)
				{
					ampsOld[a] = ampsOld[a] * 0.98f + samples[nadSampleNumber].amplitudes[a] * 0.02f;
					//periods2[j] = periods2[j] * 0.9 + periods1[j] * 0.1;

					t[a] += pi2 / samples[nadSampleNumber].periods[a];
					sint += F(t[a]) * 0.99f * ampsOld[a];
				}

				wavOut.L[s] = sint / channelsCount;
				if (wavOut.channels == 2)
					wavOut.R[s] = sint / channelsCount;

				ProgressShower.SetProgress(1.0 * s / samples.Length);
			}

			ProgressShower.CloseProgressForm();

			return wavOut;
		}

		public static float F(float t)
		{
			return MathF.Sign(MathF.Sin(t));
		}
	}
}
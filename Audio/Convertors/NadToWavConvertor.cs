using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class NadToWavConvertor
	{
		int _sampleRate;
		float _fadeTime;
		int _channels;
		string _waveForm;

		public NadToWavConvertor(int sampleRate, float fadeTime, string waveForm)
		{
			_sampleRate = sampleRate;
			_fadeTime = fadeTime;
			_waveForm =	waveForm;

			Logger.Log("Nad to wav convertor was initialized.");
		}

		public Wav Make(Nad nad)
		{
			_channels = nad._channelsCount;
			int length = _sampleRate * nad._duration;
			double[] t = new double[_channels];
			double[] tOld = new double[_channels];
			int fadeSamplesLeft = 0;
			int samplesForFade = 0;
			float pi2 = MathF.PI * 2;
			float buf = pi2 / _sampleRate;
			int progressStep = (int)(length / 2000);

			Wav wav = new Wav();
			ProgressShower.Show("Making wav from nad...");

			int ns = 0;
			for (int s = 0; s < length; s++)
			{
				int newNs = (int)((1f * s / length) * nad._samples.Length);
				if (newNs != ns)
				{
					ns = newNs;
					fadeSamplesLeft = samplesForFade;
				}
				else
					fadeSamplesLeft--;

				WriteSample(s, ns);
				Progress(s);
			}

			ProgressShower.Close();
			return wav;

			void WriteSample(int s,int ns)
			{
				float signal = 0;

				for (int c = 0; c < _channels; c++)
				{
					if (nad._samples[ns]._frequencies[c] < 20f)
						nad._samples[ns]._frequencies[c] = 20f;

					if (fadeSamplesLeft > 0)
					{
						float fade = 1 - 1f * fadeSamplesLeft / samplesForFade;
						fade = (MathF.Cos(fade * MathF.PI) + 1) / 2;
						float antifade = 1 - fade;

						float amp = nad._samples[ns - 1]._amplitudes[c] * fade;
						tOld[c] += buf * nad._samples[ns - 1]._frequencies[c];
						signal += (float)F(tOld[c]) * amp;

						amp = nad._samples[ns]._amplitudes[c] * antifade;
						t[c] += buf * nad._samples[ns]._frequencies[c];
						signal += (float)F(t[c]) * amp;
					}
					else
					{
						t[c] += buf * nad._samples[ns]._frequencies[c];
						signal += (float)F(t[c]) * nad._samples[ns]._amplitudes[c];
					}
				}

				wav.L[s] = signal / _channels;
			}

			void Progress(int s)
			{
				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / length);
			}
		}

		//

		public double F(double t)
		{
			if (_waveForm == "sin")
				return Math.Sin(t);
			else if (_waveForm == "sqaure")
				return Math.Sign(Math.Sin(t));
			else
				return Math.Sin(t);
		}

		public float F(float t)
		{
			if (_waveForm == "sin")
				return MathF.Sin(t);
			else if (_waveForm == "sqaure")
				return MathF.Sign(MathF.Sin(t));
			else
				return MathF.Sin(t);
		}
	}
}

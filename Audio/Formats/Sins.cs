using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class Sines
	{
		public static float buf = 2 * MathF.PI / AP.SampleRate; 
		public List<float> amps;
		public List<ushort> idxs;

		public Sines()
		{
			amps = new List<float>();
			idxs = new List<ushort>();
		}

		public float GetSignal(int s, string function)
		{
			float signal = 0;
			float frq;
			for (int i = 0; i < idxs.Count; i++)
			{
				frq = SpectrumFinder._frequenciesLg[idxs[i]];
				signal += amps[i] * F(function, buf * frq * s + SpectrumFinder._randomPhases[idxs[i]]);
			}

			float summ = GetSummOfAmps();

			if (summ > 0)
				signal /= MathF.Sqrt(summ);

			return signal;
		}

		public float GetSummOfAmps()
		{
			float summ = 0;
			for (int i = 0; i < amps.Count; i++)
				summ += amps[i];

			return summ;
		}

		public double GetSummOfSquaresOfAmps()
		{
			double summ = 0;
			for (int i = 0; i < amps.Count; i++)
				summ += amps[i] * amps[i];

			return summ;
		}

		public void Add(float amp, ushort idx)
		{
			int id = idxs.IndexOf(idx);
			if (id != -1)
				amps[id] += amp;
			else
			{
				idxs.Add(idx);
				amps.Add(amp);
			}
		}

		public float F(string function, float x)
		{
			if (function == "sin")
				return MathF.Sin(x);
			else if (function == "square")
				return MathF.Sign(MathF.Sin(x));
			else
				return 0;
		}
	}
}

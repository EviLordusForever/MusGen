using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen
{
	public static class WavNadNadWav
	{
		public static void Make(string wavInPath, string exportName)
		{
			Thread tr = new(Tr);
			tr.Name = "WavNadNadWav";
			tr.Start();

			void Tr()
			{
				Logger.Log($"Wav to Nad, Nad to Wav started for ({wavInPath}).");
				Wav wavIn = new Wav();
				wavIn.Read(wavInPath);
				SpectrumFinder.Init(AP._fftSize, AP._sampleRate, AP._smoothXScale, AP._smoothYScale);
				Nad nad = WavToNadConvertor.Make(wavIn);
				Wav wavOut = NadToWavConvertor.Make(nad);
				wavOut.Export(exportName);
				Logger.Log($"Wav to Nad, Nad to Wav finished. Saved as ({exportName})");
			}
		}
	}
}

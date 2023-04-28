using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
				WavToNadConvertor wtn = new WavToNadConvertor(AP._fftSize, AP._nadSamplesPerSecond, AP._channels, AP._peakSize, AP._logarithmicNad);
				NadToWavConvertor ntw = new NadToWavConvertor(wavIn.sampleRate, AP._fadeTime, AP._waveForm);
				Nad nad = wtn.Make(wavIn);
				Wav wavOut = ntw.Make(nad);
				wavOut.Export(exportName);
				Logger.Log("Wav to Nad, Nad to Wav finished.");
			}
		}
	}
}

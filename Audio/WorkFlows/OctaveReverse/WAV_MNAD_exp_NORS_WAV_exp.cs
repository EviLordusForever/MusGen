using System;
namespace MusGen
{
	public static class WAV_MNAD_exp_NORS_WAV_exp
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"WAV_MNAD_exp_NORS_WAV_exp started for\n{wavInPath}");
			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			Nad nad_O = SsToMultiNad.Make(ss);
			nad_O = NadModifySpeedAndPitch.Make(nad_O, speed, pitch);
			nad_O = NadLimitsFilter.Make(nad_O);
			nad_O.Export(exportName + " (WAV_MNAD)");
			float octaveShift = OctaveShifter.FindShift(nad_O);
			Nad nad_R = MultiNadSoftOctaveReverser.Make(nad_O, octaveShift, AP._octavesForReverse);
			nad_R = SuperEqualiser.Make(nad_R, nad_O);
			nad_R = NadLowSmoother.Make(nad_R);
			Wav wavOut = EveryNadToWav.Make(nad_R);
			wavOut.Export(exportName + " (WAV_MNAD_NORS_WAV)");
			Logger.Log($"WAV_MNAD_exp_NORS_WAV_exp finished. Saved as ({exportName})");
		}
	}
}
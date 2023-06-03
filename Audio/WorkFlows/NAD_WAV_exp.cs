namespace MusGen
{
	public static class NAD_WAV_exp
	{
		public static void Make(string nadInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"NAD_WAV_exp started for\n{nadInPath}");
			Nad nad = new Nad();
			nad.Read(nadInPath);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad = NadLowSmoother.Make(nad);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName + " (NAD_WAV)");
			Logger.Log($"NAD_WAV_exp finished. Saved as ({exportName})");
		}
	}
}

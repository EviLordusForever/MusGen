namespace MusGen
{
	public static class NadToWav
	{
		public static void Make(string nadInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Nad to Wav started for\n{nadInPath}");
			Nad nad = new Nad();
			nad.Read(nadInPath);
			nad = NadLowSmoother.Make(nad);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName);
			Logger.Log($"Nad to Wav finished. Saved as ({exportName})");
		}
	}
}

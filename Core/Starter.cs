using System.Threading;

namespace MusGen
{
	public static class Starter
	{
		public static void OnStart()
		{
			Logger.Log("Hello. App started. Good luck!");

			Thread tr = new(Tr);
			tr.Name = "Starter";
			tr.Start();

			void Tr()
			{
				Logger.Log("Tests are started...");

				Tests.HungarianAlgorithm();
				Tests.GradientDithering();
				Tests.GraphDrawerGradient();
				Tests.SPL();
				//Tests.Logging();

				Logger.Log("Tests are completed.");
			}
		}
	}
}

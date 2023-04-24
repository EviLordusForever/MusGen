using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MusGen
{
	public static class Starter
	{
		public static void OnStart()
		{
			Logger.Log("App started. Good luck!");

			Tests.TestHungarianAlgorithm();
			Tests.GradientDithering();
			Tests.GraphDrawerGradient();
			Tests.SPL();
		}
	}
}

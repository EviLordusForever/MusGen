using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class WindowFunction
	{
		public static float F(int v)
		{
			if (AP._windowFunction == "hamming")
				return HammingWindow.F(v);
			else if (AP._windowFunction == "square")
				return 1;
			else
				throw new ArgumentException("Wrong window function name.");
		}

		public static void Init()
		{
			HammingWindow.Init();
			Logger.Log("Window functions were initialized.");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MusGen
{
	public static class Start
	{
		public static void OnStart()
		{
			int[,] costMatrix = new int[,] {
			{  1,  2,  3,  4,  5 },
			{  6,  7,  8,  9, 10 },
			{ 11, 12, 13, 14, 15 },
			{ 16, 17, 18, 19, 20 },
			{ 21, 22, 23, 24, 25 }};

			Logger.Log("Hello");
			int[] res = HungarianAlgorithm.Run(costMatrix);
			Logger.Log(string.Join(' ', res));
		}
	}
}

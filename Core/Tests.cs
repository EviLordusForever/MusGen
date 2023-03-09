﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice;
using Library;

namespace MusGen.Core
{
	public static class Tests
	{
		public static void Test1()
		{
			Wav wav = new Wav();
			wav.Read(Disk2._programFiles + "test.wav", 0);
			UserAsker.Ask(wav.L.Length.ToString());

			double delme;

			PeriodFinder.FindPeriod(wav, 30000, 33000, out delme);
		}

		public static void Test2()
		{
			Wav wav = new Wav();
			wav.Read(Disk2._programFiles + "test.wav", 0);
			UserAsker.Ask(wav.L.Length.ToString());

			double[] a = new double[1000];

			for (int i = 1; i < 1000; i++)
				a[i] = wav.L[i];

			UserAsker.Ask(PeriodFinder.goertzel(a, 100, 44800, (int)wav.sampleRate).ToString());
		}

		public static void Test3()
		{

		}
	}
}

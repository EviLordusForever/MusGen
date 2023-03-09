using static ELFMusGen.Logger;
using static ELFMusGen.Params;
using Library;

namespace ELFMusGen
{
	public static class Statistics
	{
		public static float _loss;
		public static List<Section> _sections;

		public static int[,] _winsPerCore;
		public static int[] _wins;
		public static int[,] _testsPerCore;
		public static int[] _tests;
		private static float[] _predictions;

		public static float[] _scores;
		public static double[] _randomnesses;

		static Statistics()
		{
			Init();
		}

		public static void Init()
		{
			_sections = new List<Section>();

			_sections.Add(new Section(new float[][] { new float[] { -1, 1 } }));

			_sections.Add(new Section(new float[][] { new float[] { -1, -0.9f }, new float[] { 0.9f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.85f }, new float[] { 0.85f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.8f }, new float[] { 0.8f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.7f }, new float[] { 0.7f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.6f }, new float[] { 0.6f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.5f }, new float[] { 0.5f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.4f }, new float[] { 0.4f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.3f }, new float[] { 0.3f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.2f }, new float[] { 0.2f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.1f }, new float[] { 0.1f, 1 } }));

			_winsPerCore = new int[_coresCount, _sections.Count];
			_wins = new int[_sections.Count];
			_testsPerCore = new int[_coresCount, _sections.Count];
			_tests = new int[_sections.Count];
			_scores = new float[_sections.Count];
			_randomnesses = new double[_sections.Count];
		}

		public static string CalculateStatistics(NN nn, Tester tester)
		{
			_predictions = new float[tester._testsCount];

			restart:

			ClearStat();

			int testsPerCoreCount = tester._testsCount / _coresCount;

			float[] suber = new float[_coresCount];

			int alive = _coresCount;

			Thread[] subThreads = new Thread[_coresCount];

			for (int core = 0; core < _coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Name = "Core " + core;
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core);
			}

			void SubThread(object obj)
			{
				int core = (int)obj;

				for (int test = core * testsPerCoreCount; test < core * testsPerCoreCount + testsPerCoreCount; test++)
				{
					float prediction = nn.Calculate(test, tester._tests[test], false);
					_predictions[test] = prediction;

					float reality = tester._answers[test];

					suber[core] += MathF.Pow(prediction - reality, 2);

					bool win = prediction > 0 && reality > 0 || prediction < 0 && reality < 0;

					PlusToStatistics(core, prediction, win);
				}

				alive--;
			}

			long ms = DateTime.Now.Ticks;
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THE THREAD IS STACKED");
					for (int core = 0; core < _coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < _coresCount; core++)
				_loss += suber[core];

			_loss /= tester._testsCount;

			CalculateScores();
			CalculateRandomnesses();

			return StatToString();
		}

		public static void PlusToStatistics(int core, float prediction, bool win)
		{
			for (int section = 0; section < _sections.Count; section++)
				if (_sections[section].IsInSection(prediction))
				{
					_testsPerCore[core, section]++;
					if (win)
						_winsPerCore[core, section]++;
				}
		}

		public static void CalculateScores()
		{
			for (int section = 0; section < _sections.Count; section++)
			{
				for (int core = 0; core < _coresCount; core++)
				{
					_wins[section] += _winsPerCore[core, section];
					_tests[section] += _testsPerCore[core, section];
				}

				_scores[section] = MathF.Round((float)_wins[section] / _tests[section], 3);
			}
		}

		public static void CalculateRandomnesses()
		{
			for (int section = 0; section < _sections.Count; section++)
				_randomnesses[section] = Math2.CalculateRandomness(_wins[section], _tests[section]);
		}

		public static void ClearStat()
		{
			for (int section = 0; section < _sections.Count; section++)
			{
				_wins[section] = 0;
				_tests[section] = 0;
				_scores[section] = 0;

				for (int core = 0; core < _coresCount; core++)
				{
					_winsPerCore[core, section] = 0;
					_testsPerCore[core, section] = 0;
				}
			}

			_loss = 0;
		}

		static string StatToString()
		{
			string stat = "========================\n";
			for (int section = 0; section < _wins.Length; section++)
				if (_tests[section] > 0)
					stat += String.Format("{0,-25} {1,-13} {2,-17} (randomness: {3})\n", $"{_sections[section].ToString()}:", $"{_wins[section]} / {_tests[section]}", $"(winrate: {_scores[section]})", string.Format("{0:F9}", _randomnesses[section]));

			stat += $"loss: {_loss}\n";
			stat += $"========================";
			return stat;
		}

		public static string StatToCsv(string name)
		{
			string stat = name + ",";
			for (int section = 0; section < _wins.Length; section++)
				stat += $"{_scores[section]},";
			return stat;
		}

		public static void FindDetailedSectionsStatistics(Tester tester, string name)
		{
			string csv = GetDetailedStatisticsCsv(_predictions, tester, -1, 1, 0.001f, false);
			Disk2.WriteToProgramFiles($"Detailed Sections Statistics ({name}) (Both sides)", "csv", csv, false);
			Log($"Detatiled sections statistics created ({name}) (Both sides)");

			csv = GetDetailedStatisticsCsv(_predictions, tester, 0, 1, 0.001f, true);
			Disk2.WriteToProgramFiles($"Detailed Sections Statistics ({name}) (Single Side)", "csv", csv, false);
			Log($"Detatiled sections statistics created ({name}) (Single side)");			
		}

		public static string GetDetailedStatisticsCsv(float[] predictions, Tester tester, float cutterMin, float cutterMax, float step, bool SingleSide)
		{
			float predictionsCount = 0;
			float wins = 0;
			string csv = "";
			for (float cutter = cutterMin; cutter <= cutterMax; cutter += step)
			{
				predictionsCount = 0;
				wins = 0;

				for (int test = 0; test < tester._testsCount; test++)
					if (!SingleSide)
					{
						if (cutter >= 0 && predictions[test] >= cutter ||
							cutter < 0 && predictions[test] <= cutter)
							CheckWin(test);
					}
					else if (MathF.Abs(predictions[test]) >= cutter)
						CheckWin(test);

				if (predictionsCount > 0)
					csv += $"cutter:,{cutter},{wins},/,{predictionsCount},=,WINRATE:,{wins / predictionsCount},count,{predictionsCount / tester._testsCount},randomness,{Math2.CalculateRandomness(wins, (int)predictionsCount, 0.5f)}\n";
			}
			return csv;

			void CheckWin(int test)
			{
				predictionsCount++;

				if (tester._answers[test] > 0 && predictions[test] > 0 ||
					tester._answers[test] < 0 && predictions[test] < 0)
					wins++;
			}
		}
	}

	public class Section
	{
		public List<float[]> _subSections;

		public Section(params float[][] sections)
		{
			_subSections = new List<float[]>();
			foreach (float[] s in sections)
				Add(s);
		}

		public void Add(float[] section)
		{
			_subSections.Add(section);
		}

		public bool IsInSection(float number)
		{
			for (int s = 0; s < _subSections.Count(); s++)
				if (number >= _subSections[s][0] && number <= _subSections[s][1])
					return true;

			return false;
		}

		public override string ToString()
		{
			string str = "";
			for (int s = 0; s < _subSections.Count(); s++)
				str += $"({_subSections[s][0]}, {_subSections[s][1]}), ";
			return str.Remove(str.Length - 2);
		}
	}
}

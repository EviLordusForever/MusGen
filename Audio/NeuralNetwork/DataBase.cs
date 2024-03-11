using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.NeuralNetwork
{
	public static class DataBase
	{
		public static Nad[] _nads;

		public static void LoadMidis()
		{
			string[] files = Directory.GetFiles($"{DiskE._programFiles}MIDIS");

			for (int i = 0; i < files.Length; i++)
			{
				Midi midi = new Midi();
				midi.Read(files[i]);
				Nad nad = midi.ToNad();
				nad = nad.Modify(60);
			}
		}
	}
}

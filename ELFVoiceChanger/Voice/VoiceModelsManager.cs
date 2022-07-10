using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.Core;

namespace ELFVoiceChanger.Voice
{
	public static class VoiceModelsManager
	{
		public static List<string> voiceModelsNames { get; set; }

		public static void AddVoiceModel(string name)
		{
			string path = Disk.programFiles + "\\VoiceModels\\" + name;
			if (voiceModelsNames.Contains(name))
				return;

			Directory.CreateDirectory(path);
			Directory.CreateDirectory(path + "\\LetterPatterns");
			voiceModelsNames.Add(name);
		}

		public static void DeleteVoiceModel(string name)
		{
			string path = Disk.programFiles + "\\VoiceModels\\" + name;

			Disk.DeleteDirectoryWithFiles(path);
			voiceModelsNames.Remove(name);
		}

		public static void Rename(string oldName, string name)
		{
			int id = voiceModelsNames.IndexOf(oldName);
			voiceModelsNames.RemoveAt(id);
			voiceModelsNames.Insert(id, name);

			Directory.Move(Disk.programFiles + "VoiceModels\\" + oldName, Disk.programFiles + "VoiceModels\\" + name);
		}

		static VoiceModelsManager()
		{
			voiceModelsNames = new List<string>();
			string[] strs = Directory.GetDirectories(Disk.programFiles + "VoiceModels");
			foreach (string str in strs)
				voiceModelsNames.Add(TextMethods.StringAfterLast(str, "\\"));
		}
	}
}

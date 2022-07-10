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

		public static void DeleteLetterPattern(string voiceModelName, string name)
		{
			string path = "VoiceModels\\" + voiceModelName + "\\LetterPatterns\\" + name + ".wav";

			Disk.DeleteFileFromProgramFiles(path);
		}

		public static void Rename(string oldName, string name)
		{
			int id = voiceModelsNames.IndexOf(oldName);
			voiceModelsNames.RemoveAt(id);
			voiceModelsNames.Insert(id, name);

			Directory.Move(Disk.programFiles + "VoiceModels\\" + oldName, Disk.programFiles + "VoiceModels\\" + name);
		}

		public static void AddLetterPattern(string voiceModel, string pattern, string wavPath)
		{
			string newWavPath = Disk.programFiles + "\\VoiceModels\\" + voiceModel + "\\LetterPatterns\\" + pattern + ".wav";
			if (File.Exists(newWavPath))
				throw new Exception();
			File.Copy(wavPath, newWavPath);
		}

		public static void ChangeLetterPattern(string voiceModel, string pattern, string wavPath)
		{
			string newWavPath = Disk.programFiles + "\\VoiceModels\\" + voiceModel + "\\LetterPatterns\\" + pattern + ".wav";
			if (!File.Exists(newWavPath))
				throw new Exception();
			File.Delete(newWavPath);
			File.Move(wavPath, newWavPath);
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

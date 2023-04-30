﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen.Voice
{
	public static class VoiceModelsManager
	{
		public static List<string> voiceModelsNames { get; set; }

		public static void AddVoiceModel(string name)
		{
			string path = DiskE._programFiles + "\\VoiceModels\\" + name;
			if (voiceModelsNames.Contains(name))
				return;

			Directory.CreateDirectory(path);
			Directory.CreateDirectory(path + "\\LetterPatterns");
			voiceModelsNames.Add(name);
		}

		public static void DeleteVoiceModel(string name)
		{
			string path = DiskE._programFiles + "\\VoiceModels\\" + name;

			DiskE.DeleteDirectoryWithFiles(path);
			voiceModelsNames.Remove(name);
		}

		public static void DeleteLetterPattern(string voiceModelName, string name)
		{
			string path = "VoiceModels\\" + voiceModelName + "\\LetterPatterns\\" + name + ".wav";

			DiskE.DeleteFileFromProgramFiles(path);
		}

		public static void Rename(string oldName, string name)
		{
			int id = voiceModelsNames.IndexOf(oldName);
			voiceModelsNames.RemoveAt(id);
			voiceModelsNames.Insert(id, name);

			Directory.Move(DiskE._programFiles + "VoiceModels\\" + oldName, DiskE._programFiles + "VoiceModels\\" + name);
		}

		public static void RenameLetterPattern(string voiceModelName, string oldName, string name)
		{
			string oldPath = DiskE._programFiles + "VoiceModels\\" + voiceModelName + "\\LetterPatterns\\" + oldName + ".wav";
			string newPath = DiskE._programFiles + "VoiceModels\\" + voiceModelName + "\\LetterPatterns\\" + name + ".wav";

			Directory.Move(oldPath, newPath);
		}

		public static void AddLetterPattern(string voiceModel, string pattern, string wavPath)
		{
			string newWavPath = DiskE._programFiles + "\\VoiceModels\\" + voiceModel + "\\LetterPatterns\\" + pattern + ".wav";
			if (File.Exists(newWavPath))
				throw new Exception();
			File.Copy(wavPath, newWavPath);
		}

		public static void ChangeLetterPattern(string voiceModel, string pattern, string wavPath)
		{
			string newWavPath = DiskE._programFiles + "\\VoiceModels\\" + voiceModel + "\\LetterPatterns\\" + pattern + ".wav";
			if (!File.Exists(newWavPath))
				throw new Exception();
			File.Delete(newWavPath);
			File.Move(wavPath, newWavPath);
		}

		static VoiceModelsManager()
		{
			voiceModelsNames = new List<string>();
			string[] strs = Directory.GetDirectories(DiskE._programFiles + "VoiceModels");
			foreach (string str in strs)
				voiceModelsNames.Add(TextE.StringAfterLast(str, "\\"));
		}
	}
}
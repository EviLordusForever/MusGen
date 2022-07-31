using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace ELFVoiceChanger.Core
{
	public static class Disk
	{
		public static string currentDirectory;

		public static string programFiles;

		public static void SaveImage(Bitmap image, string path)
		{
			image.Save(path);
		}

		public static void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public static async void WriteToProgramFiles(string path, string text, bool savebefore) //Имя файла без пути или относительный путь, и без .txt
		{
			path = $"{programFiles}\\{path}.txt";

			Thread writerThread = new Thread(WriterThread);
			writerThread.Start();

			bool itEnds = false;
			while (writerThread.IsAlive) { }; /////////////Тут зависло однажды +2 //Изменено

			void WriterThread()
			{
				again:
				try
				{
					using (StreamWriter STR = new StreamWriter(path, savebefore, Encoding.UTF8))
						STR.Write(text);

					itEnds = true;
				}
				catch (IOException ex)
				{
					Thread.Sleep(50 + Storage.rnd.Next(250));
					goto again;
				}
			}
		}

		public static string Read(string path)
		{
			return File.ReadAllText(path, Encoding.UTF8);
		}

		public static string ReadFromProgramFiles(string path)
		{
			string fileName = path;
			path = currentDirectory;
			path += "\\ProgramFiles\\" + fileName + ".txt";
			return File.ReadAllText(path, Encoding.UTF8);
		}

		public static int ReadFromProgramFilesInt(string path)
		{
			return Convert.ToInt32(ReadFromProgramFiles(path));
		}

		public static double ReadFromProgramFilesDouble(string path)
		{
			return Convert.ToDouble(ReadFromProgramFiles(path));
		}

		public static bool ReadFromProgramFilesBool(string path)
		{
			return Convert.ToBoolean(ReadFromProgramFiles(path));
		}

		public static void RenameTxtFileInProgramFiles(string from, string to)
		{
			File.Move($"{programFiles}\\{from}.txt", $"{programFiles}\\{to}.txt");
		}

		public static void DeleteDirectoryWithFiles(string path)
		{
			string[] files = Directory.GetFiles(path);
			foreach (string file in files)
				File.Delete(file);

			string[] directories = Directory.GetDirectories(path);
			foreach (string directory in directories)
				DeleteDirectoryWithFiles(directory);

			Directory.Delete(path);
		}

		public static void ClearDirectory(string path)
		{

			string[] files = Directory.GetFiles(path);
			foreach (string file in files)
			{
				try
				{
					File.Delete(file);
				}
				catch (Exception e)
				{
				}
			}

			string[] directories = Directory.GetDirectories(path);
			foreach (string directory in directories)
				DeleteDirectoryWithFiles(directory);
		}

		public static long DirSize(string path)
		{
			DirectoryInfo d = new DirectoryInfo(path);
			return DirSize(d);
		}

		public static long DirSize(DirectoryInfo d)
		{
			long size = 0;

			// Add file sizes.
			FileInfo[] fis = d.GetFiles();
			foreach (FileInfo fi in fis)
				size += fi.Length;

			// Add subdirectory sizes.
			DirectoryInfo[] dis = d.GetDirectories();
			foreach (DirectoryInfo di in dis)
				size += DirSize(di);

			return size;
		}

		public static void DeleteFileFromProgramFiles(string path)
		{
			path = $"{currentDirectory}\\ProgramFiles\\{path}";
			File.Delete(path);
		}
	}
}

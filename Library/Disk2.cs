using System.Text;

namespace Library
{
	public static class Disk2
	{
		public static string _currentDirectory;

		public static string _programFiles;

		static Disk2()
		{
			Init();
		}

		public static void Init()
		{
			_currentDirectory = $"{Environment.CurrentDirectory}\\";

			if (!File.Exists($"{_currentDirectory}ProgramFilesPath.txt"))
				File.WriteAllText($"{_currentDirectory}\\ProgramFilesPath.txt", $"{_currentDirectory}ProgramFiles\\");

			_programFiles = File.ReadAllText($"{_currentDirectory}ProgramFilesPath.txt");

			if (_programFiles[_programFiles.Length - 1] == '\\')
				_programFiles = Text2.StringBeforeLast(_programFiles, "\\");

			while (!Directory.Exists(_programFiles))
			{
				UserAsker.SayWait(
					"HELLO!\n" +
					"PLEASE, SELECT \"ProgramFiles\" folder path.\n" +
					"This is special folder of that app.\n" +
					"It contains THAT app information.\n" +
					"Contact author if u have no that folder.");
				_programFiles = UserAsker.AskFolder(Environment.SpecialFolder.Startup, "Select ProgramFiles folder");

				if (_programFiles[_programFiles.Length - 1] == '\\')
					_programFiles = Text2.StringBeforeLast(_programFiles, "\\");
			}
			_programFiles += "\\";
			File.WriteAllText($"{_currentDirectory}\\ProgramFilesPath.txt", _programFiles);
		}

		public static void SaveImage(Bitmap image, string path)
		{
			image.Save(path);
		}

		public static void SaveImageToProgramFiles(Bitmap image, string path)
		{
			image.Save(_programFiles + path);
		}

		public static void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public static async void WriteToProgramFiles(string path, string extension, string text, bool savebefore)
		{
			path = $"{_programFiles}\\{path}.{extension}";

			Thread writerThread = new Thread(WriterThread);
			writerThread.Start();

			while (writerThread.IsAlive) { }; //

			void WriterThread()
			{
				again:
				try
				{
					using (StreamWriter STR = new StreamWriter(path, savebefore, Encoding.UTF8))
						STR.Write(text);
				}
				catch (IOException ex)
				{
					Random rnd = new Random();
					Thread.Sleep(50 + rnd.Next(250));
					goto again;
				}
			}
		}

		public static string Read(string path)
		{
			return File.ReadAllText(path, Encoding.UTF8);
		}

		public static string ReadFromProgramFilesTxt(string path)
		{
			return ReadFromProgramFiles(path, "txt");
		}

		public static string ReadFromProgramFiles(string path, string ext)
		{
			string fileName = path;
			path = _programFiles;
			path += fileName + '.' + ext;
			return File.ReadAllText(path, Encoding.UTF8);
		}

		public static int ReadFromProgramFilesInt(string path)
		{
			return Convert.ToInt32(ReadFromProgramFilesTxt(path));
		}

		public static double ReadFromProgramFilesDouble(string path)
		{
			return Convert.ToDouble(ReadFromProgramFilesTxt(path));
		}

		public static bool ReadFromProgramFilesBool(string path)
		{
			return Convert.ToBoolean(ReadFromProgramFilesTxt(path));
		}

		public static void RenameTxtFileInProgramFiles(string from, string to)
		{
			File.Move($"{_programFiles}\\{from}.txt", $"{_programFiles}\\{to}.txt");
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
					UserAsker.Ask($"I am so sorry, but {e}");
				}
			}

			string[] directories = Directory.GetDirectories(path);
			foreach (string directory in directories)
				DeleteDirectoryWithFiles(directory);
		}

		public static void DeleteFileFromProgramFiles(string path)
		{
			File.Delete($"{_programFiles}\\{path}");
		}

		public static string[] GetFilesFromProgramFiles(string directory)
		{
			return Directory.GetFiles($"{_programFiles}{directory}");
		}

		public static void RenameFileInProgramFiles(string from, string to,string ext)
		{
			File.Move($"{_programFiles}\\{from}.{ext}", $"{_programFiles}\\{to}.{ext}");
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
	}
}

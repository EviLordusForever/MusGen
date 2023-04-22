using System;
using System.Windows;
using System.Threading;
using System.IO;
using MusGen.Forms;
using Library;
using System.Linq;

namespace MusGen
{
	public static class Logger
	{
		public static int _logSize = 25000;
		public static string _logText;
		public static StreamWriter _writer;
		public static bool _updated;

		public static Thread _flusherThread;
		public static Thread _visualizerThread;

		public static void Log(string text)
		{
			FormsManager.OpenLogWindow();

			string msg = CreateMessageToShout(text);
			string date = GetDateToShow(System.DateTime.Now);
			string time = GetTimeToShow(System.DateTime.Now);
			LogToFile($"[{date}][{time}] {msg}");
			LogToWindow($"[{date}][{time}] {msg}");
			CutVisibleLog();

			void LogToFile(string text)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					_writer.Write($"{text}\r\n");
				});
			}

			void LogToWindow(string text)
			{
				_logText = $"{text}\r\n{_logText}";
				_updated = true;
			}

			void CutVisibleLog()
			{
				if (_logText.Length > _logSize + 1000)
					_logText = _logText.Remove(_logSize);
			}

			string CreateMessageToShout(string text)
			{
				string who = "";
				string during = "";
				string place = "";
				string whoe = "   ";
				string duringe = "   ";
				string placee = "   ";

				string timee = "          ";
				string datee = "            ";


				text = ModyfyText(text);

				if (whoe.Length > who.Length)
					who += whoe.Remove(whoe.Length - 1 - who.Length);
				if (duringe.Length > during.Length)
					during += duringe.Remove(duringe.Length - 1 - during.Length);
				if (placee.Length > place.Length)
					place += placee.Remove(placee.Length - 1 - place.Length);

				return $"{who} {during} {place} {text}";


				string ModyfyText(string str)
				{
					int size = 180;
					string res = "";
					str += "\r\n";

					while (true)
					{
						int indexOfR = Math.Min(str.IndexOf('\r'), str.IndexOf('\n'));
						indexOfR = Math.Min(indexOfR, str.IndexOf(Environment.NewLine));


						if (indexOfR < size - 1 && indexOfR != -1 && indexOfR + 2 < str.Length)
						{
							res += str.Remove(indexOfR + 1).Replace('\r', ' ').Replace('\n', ' ') + "\r" + datee + timee + whoe + duringe + placee + " ";
							str = str.Substring(indexOfR + 1);
						}
						else if (str.Length <= size)
						{
							res += str.Replace('\r', ' ').Replace('\n', ' ');
							break;
						}
						else if (str.Length > size)
						{
							string strRemoveSize = str.Remove(size);

							int lastSpaceIndex = size;
							if (strRemoveSize.Contains(" "))
								lastSpaceIndex = strRemoveSize.LastIndexOf(' ');

							res += str.Remove(lastSpaceIndex) + "\r" + datee + timee + whoe + duringe + placee + " ";
							str = str.Substring(lastSpaceIndex + 1);
						}
						else
						{
							res += str.Replace('\r', ' ').Replace('\n', ' ');
							break;
						}
					}

					return res;
				}
			}
		}

		public static void Log(float text)
		{
			Log(text.ToString());
		}

		public static void Log(double text)
		{
			Log(text.ToString());
		}

		public static void Log(int text)
		{
			Log(text.ToString());
		}

		static void StartVisualiser()
		{
			_visualizerThread = new Thread(VisualiserThread);
			_visualizerThread.Name = "LogVisuliser";
			_visualizerThread.Start();

			void VisualiserThread()
			{
				try
				{
					while (true)
					{
						Application.Current.Dispatcher.Invoke(() =>
						{
							if (_updated && FormsManager.logWindow.WindowState != WindowState.Minimized)
								if (FormsManager.logWindow.IsVisible)
								{
									FormsManager.logWindow.RTB.Document.Blocks.Clear();
									FormsManager.logWindow.RTB.AppendText(_logText); 

									_updated = false;
								}
						});

						Thread.Sleep(250);
					}				
				}
				catch (ThreadInterruptedException ex)
				{
				}
			}
		}

		static void StartFlusher()
		{
			_flusherThread = new Thread(FlusherThread);
			_flusherThread.Name = "LogFlusher";
			_flusherThread.Start();

			void FlusherThread()
			{
				try
				{
					Thread.Sleep(3000);

					while (true)
					{							
						_writer.Flush(); //Checked.

						Thread.Sleep(20000);
					}
				}
				catch (ThreadInterruptedException ex)
				{
					_writer.Flush();
				}
			}
		}

		public static string GetTimeToShow()
		{
			return GetTimeToShow(DateTime.Now);
		}

		public static string GetTimeToShow(DateTime dateTime)
		{
			string h = dateTime.Hour.ToString();
			string m = dateTime.Minute.ToString();
			string s = dateTime.Second.ToString();

			if (h.Length == 1)
				h = "0" + h;
			if (m.Length == 1)
				m = "0" + m;
			if (s.Length == 1)
				s = "0" + s;

			return $"{h}:{m}:{s}";
		}

		public static string GetDateToShow()
		{
			return GetDateToShow(DateTime.Now);
		}

		public static string GetDateToShow(DateTime dateTime)
		{
			string d = dateTime.Day.ToString();
			string m = dateTime.Month.ToString();
			string y = dateTime.Year.ToString();

			if (d.Length == 1)
				d = "0" + d;
			if (m.Length == 1)
				m = "0" + m;

			return $"{d}.{m}.{y}";
		}

		public static void Quit()
		{
			_visualizerThread.Interrupt();
			_flusherThread.Interrupt();
		}

		static Logger()
		{
			_logText = "";
			_writer = new StreamWriter($"{Disk2._programFiles}Logs\\log.log", true);
			StartFlusher();
			StartVisualiser();
		}
	}
}

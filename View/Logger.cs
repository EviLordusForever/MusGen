using System;
using System.Windows;
using System.Threading;
using System.IO;
using Library;
using System.Windows.Media;
using System.Windows.Documents;

namespace MusGen
{
	public static class Logger
	{
		public static int _logSize = 25000;
		public static int _fontSize = 14;
		public static int _lineHeigh = 1;
		public static int _symbols = 142;
		public static string _logText;
		public static FlowDocument _logDocument;
		public static StreamWriter _writer;
		public static bool _updated;

		public static Thread _flusherThread;
		public static Thread _visualizerThread;

		public static string _msg;
		public static string _date;
		public static string _time;
		public static string _dt;

		public static void Log(string text)
		{
			Log(text, Brushes.Yellow);
		}

		public static void Log(string text, Brush brush)
		{
			WindowsManager.OpenLogWindow();

			_msg = CreateMessageToShout(text);
			_date = GetDateToShow(DateTime.Now);
			_time = GetTimeToShow(DateTime.Now);
			_dt = $"[{_date}][{_time}]";
			LogToFile($"{_dt} {_msg}");
			LogToWindow();
			_logText = $"{_dt} {_msg}\r\n{_logText}";
			CutVisibleLog();

			void LogToFile(string text)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					_writer.Write($"{text}\r\n");
				});
			}

			void LogToWindow()
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					Paragraph paragraph = new Paragraph();

					Span span = new Span(new Run(_dt));
					span.Foreground = Brushes.Lime;

					paragraph.Inlines.Add(span);

					Span span2 = new Span(new Run(" " + _msg));
					span2.Foreground = brush;

					paragraph.Inlines.Add(span2);

					paragraph.LineHeight = _lineHeigh;
					paragraph.FontSize = _fontSize;

					_logDocument.Blocks.Add(paragraph);
				});

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
					string res = "";
					str += "\r\n";

					while (true)
					{
						int indexOfR = Math.Min(str.IndexOf('\r'), str.IndexOf('\n'));
						indexOfR = Math.Min(indexOfR, str.IndexOf(Environment.NewLine));


						if (indexOfR < _symbols - 1 && indexOfR != -1 && indexOfR + 2 < str.Length)
						{
							res += str.Remove(indexOfR + 1).Replace('\r', ' ').Replace('\n', ' ') + "\r" + datee + timee + whoe + duringe + placee + " ";
							str = str.Substring(indexOfR + 1);
						}
						else if (str.Length <= _symbols)
						{
							res += str.Replace('\r', ' ').Replace('\n', ' ');
							break;
						}
						else if (str.Length > _symbols)
						{
							string strRemoveSize = str.Remove(_symbols);

							int lastSpaceIndex = _symbols;
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
							if (_updated && WindowsManager._logWindow.WindowState != WindowState.Minimized)
								if (WindowsManager._logWindow.IsVisible)
								{
									WindowsManager._logWindow.RTB.Document = _logDocument; 

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
			Application.Current.Dispatcher.Invoke(() =>
			{
				_logDocument = new FlowDocument();
				_logDocument.PageWidth = 2048;
			});		

			_writer = new StreamWriter($"{Disk2._programFiles}Logs\\log.log", true);
			StartFlusher();
			StartVisualiser();
		}
	}
}

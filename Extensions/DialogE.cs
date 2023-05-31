﻿using System;
using System.Windows;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using WPFFolderBrowser;
using System.Diagnostics;
using MusGen;

namespace Extensions
{
	public static class DialogE
	{
		public static void ShowFolder(string path)
		{
			Process.Start("explorer.exe", "/select, \"" + path + "\"");
		}

		public static void ShowFile(string path)
		{
			Process.Start("explorer.exe", "/select, \"" + path + "\"");
		}

		public static string AskFile(string ext)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = $".{ext} files | *.{ext}";
			dialog.Title = $"Please select {ext} file";
			bool? success = dialog.ShowDialog();
			if (success == true)
				return dialog.FileName;
			else
				return "";
		}

		public static bool Ask(string q)
		{
			bool result = false;
			Application.Current.Dispatcher.Invoke(() => {
				var result0 = MessageBox.Show(q, "Hey", MessageBoxButton.YesNo);
				//Logger.Log(result0.ToString());
				//Logger.Log(MessageBoxResult.Yes.ToString());
				result = result0 == MessageBoxResult.Yes;
				//Logger.Log(result.ToString());
			});
			return result;
		}

		public static string AskFolder(Environment.SpecialFolder rootFolder, string description)
		{
			var dialog = new WPFFolderBrowserDialog("Выберите папку:");
			bool? result = dialog.ShowDialog();

			if (result == true)
				return dialog.FileName;
			else
				return dialog.InitialDirectory;
		}

		public static string AskValue(string prompt, string title, string defaultValue)
		{
			return Interaction.InputBox(prompt, title, defaultValue);
		}

		public static void SayWait(string q)
		{
			MessageBox.Show(q);
		}
	}
}

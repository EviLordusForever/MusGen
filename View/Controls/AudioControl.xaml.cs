using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Extensions;
using System.Globalization;
using MusGen.Audio.WorkFlows;
using System.IO;

namespace MusGen
{
	/// <summary>
	/// Interaction logic for WnnwControl.xaml
	/// </summary>
	public partial class AudioControl : UserControl
	{
		public AudioControl()
		{
			InitializeComponent();
		}

		public string[] _paths;
		private string[] _outNames;
		private float _pitch;
		private float _speed;
		public string _s;

		private void SelBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "audio files |*.wav;*.nad;*.mid;*.fnad";
			dialog.Title = "Please select audio file(s)";
			dialog.Multiselect = true;
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				_paths = dialog.FileNames;
				_outNames = dialog.SafeFileNames;

				for (int i = 0; i < _outNames.Length; i++)
					_outNames[i] = TextE.StringBeforeLast(_outNames[i], ".");

				SelAudioButton.Content = _outNames[0];
				if (_paths.Count() == 1)
				{
					SelAudioButton.Content = _outNames[0];
					outNameTb.Text = _outNames[0];
				}
				else
				{
					SelAudioButton.Content = $"{_paths.Length} files";
					outNameTb.Text = "";
				}
			}
		}

		private void SelImageButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image Files |*.jpg"; //
			dialog.Title = "Please select image file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				_paths = dialog.FileNames;
				string name = dialog.SafeFileName;

				SelImageButton.Content = name;
				outNameTb.Text = TextE.StringBeforeLast(name, ".");
			}
		}

		private void ProcessBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!GetPitch())
				return;
			if (!GetSpeed())
				return;

			WindowsManager.Open(ControlsManager._mainMenuControl);
			WindowsManager._mainWindow.Hide();

			Thread tr = new Thread(Tr);
			tr.Name = "Processing workflow";
			tr.Start();

			void Tr()
			{
				string outNameAddiction = "";
				if (_paths.Length > 1)
					if (outNameTb.Text != "")
						outNameAddiction = "_" + outNameTb.Text;

				RealtimeFFT.Stop();

				if (_paths.Length > 0)
					for (int i = 0; i < _outNames.Length; i++)
						Do(_outNames[i] + outNameAddiction, _paths[i]);

				RealtimeFFT.Stop();

				void Do(string outName, string path)
				{
					if (path == "" || !File.Exists(path))
						return;

					if (_s == "WAV_SORS_MNAD_WAV_exp")
						WAV_SORS_MNAD_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "FNAD_MID_exp")
						FNAD_MID_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "MIDI_MNAD_WAV_exp")
						MID_MNAD_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "WAV_MNAD_NORS_WAV_exp")
						WAV_MNAD_NORS_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "NAD_NORS_exp")
						NAD_NORS_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "WAV_FNAD_WAV_exp")
						WAV_FNAD_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "WAV_JPG_exp")
						WAV_JPG_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "NAD_JPG_exp")
						NAD_JPG_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "WAV_MNAD_exp")
						WAV_MNAD_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "WAV_MNAD_exp_NORS_WAV_exp")
						WAV_MNAD_exp_NORS_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "NAD_WAV_exp")
						NAD_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "WAV_MNAD_WAV_exp")
						WAV_MNAD_WAV_exp.Make(path, outName, _speed, _pitch);

					else if (_s == "JpegSpectrumClean")
						JpegSpectrumClean.Make(path, outName, _speed, _pitch);

					else
						outNameTb.Text = "Wrong type in list";
				}

				Application.Current.Dispatcher.Invoke(() =>
				{
					WindowsManager._mainWindow.Show();
				});
			}

			bool GetPitch()
			{
				if (pitchTb.Text == "" || pitchTb.Text == null)
				{
					_pitch = 1;
					return true;
				}
				else
				{
					if (float.TryParse(pitchTb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out _pitch))
						return _pitch > 0;
					else
						return false;
				}
			}

			bool GetSpeed()
			{
				if (speedTb.Text == "" || speedTb.Text == null)
				{
					_speed = 1;
					return true;
				}
				else
				{
					if (float.TryParse(speedTb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out _speed))
						return _speed > 0;
					else
						return false;
				}
			}
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			WindowsManager.Open(ControlsManager._mainMenuControl);
		}

		private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBoxItem selectedItem = combobox.SelectedItem as ComboBoxItem;
			_s = selectedItem.Content.ToString();

			if (SelAudioButton != null && SelImageButton != null)
			{
				if (_s.Contains("Jpeg"))
					ImageSelection();
				else
					AudioSelection();
			}

			if (pitchTb != null && speedTb != null)
			{
				if (pitchTb.Text == "" || pitchTb.Text == null)
					pitchTb.Text = "1";
				if (speedTb.Text == "" || speedTb.Text == null)
					speedTb.Text = "1";
			}

			void ImageSelection()
			{
				SelAudioButton.Visibility = Visibility.Collapsed;
				SelImageButton.Visibility = Visibility.Visible;
			}

			void AudioSelection()
			{
				SelAudioButton.Visibility = Visibility.Visible;
				SelImageButton.Visibility = Visibility.Collapsed;
			}
		}
	}
}

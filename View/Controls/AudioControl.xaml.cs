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

		public string _path;
		private string _outName;
		private float _pitch;
		private float _speed;
		public string _s;

		private void SelBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "audio files (.wav .nad) |*.wav;*.nad";
			dialog.Title = "Please select audio file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				_path = dialog.FileName;
				string name = dialog.SafeFileName;

				bool isGood = false;
				if (TextE.StringAfterLast(name, ".") == "wav")
					isGood = Wav.CheckWav(_path);
				else if (TextE.StringAfterLast(name, ".") == "nad")
					isGood = true;
				else
					throw new ArgumentException("wrong file extension");

				if (isGood)
				{
					SelAudioButton.Content = name;
					outNameTb.Text = TextE.StringBeforeLast(name, ".");
				}
				else
				{
					MessageBox.Show("Wrong wav file", "Sorry", MessageBoxButton.OK, MessageBoxImage.Error);
					SelAudioButton.Content = "Select file";
					_path = "";
				}
			}
		}

		private void SelImageButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png";
			dialog.Title = "Please select image file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				_path = dialog.FileName;
				string name = dialog.SafeFileName;

				SelImageButton.Content = name;
				outNameTb.Text = TextE.StringBeforeLast(name, ".");
			}
		}

		private void ProcessBtn_Click(object sender, RoutedEventArgs e)
		{
			_outName = outNameTb.Text;

			if (_outName == null || _path == null)
				return;
			if (_path == "" || _outName == "")
				return;
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
				RealtimeFFT.Stop();

				if (_s == "Ss octave reverse (soft, MultiNad)")
					WavToSsOctaveReverse_Soft_MultiNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Jpg to wav ss octave reverse (soft, MultiNad)")
					JpgSsOctaveReverse_Soft_MultiNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Wav to nad octave reverse (soft)")
					WavToNadOctaveReverse_Soft_MultiNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Nad to nad octave reverse (soft)")
					NadToNadOctaveReverse_Soft.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Jpg to wav nad octave reverse (soft, MultiNad)")
					JpgNadOctaveReverse_Soft_MultiNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Wav to wav (FixedNad)")
					WavToWav_FixedNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Wav to jpg")
					WavToJpg.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Nad to jpg")
					NadToJpg.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Wav to nad (MultiNad)")
					WavToMultiNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Nad to wav")
					NadToWav.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Jpg to wav (MultiNad)")
					JpgToWav_MultiNad.Make(_path, _outName, _speed, _pitch);				
				else if (_s == "Wav to wav (MultiNad)")
					WavToWav_MultiNad.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Jpg to clean jpg")
					JpegSpectrumClean.Make(_path, _outName, _speed, _pitch);
				else if (_s == "Vertical reverse (FixedNad)")
					WavVerticalReverse_FixedNad.Make(_path, _outName, _speed, _pitch);
				else
					outNameTb.Text = "Wrong type in list";

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
				if (_s.Contains("Jpg to"))
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

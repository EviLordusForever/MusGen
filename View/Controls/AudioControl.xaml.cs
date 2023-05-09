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

		public string path;
		private string outName;
		public string _s;

		private void SelBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = ".wav files | *.wav";
			dialog.Title = "Please select wav file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				path = dialog.FileName;
				string name = dialog.SafeFileName;

				bool isGood = Wav.CheckWav(path);

				if (isGood)
				{
					SelAudioButton.Content = name;
					outNameTb.Text = TextE.StringBeforeLast(name, ".wav");
				}
				else
				{
					MessageBox.Show("Wrong wav file", "Sorry", MessageBoxButton.OK, MessageBoxImage.Error);
					SelAudioButton.Content = "Select .wav file";
					path = "";
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
				path = dialog.FileName;
				string name = dialog.SafeFileName;

				SelImageButton.Content = name;
				outNameTb.Text = TextE.StringBeforeLast(name, ".");
			}
		}

		private void ProcessBtn_Click(object sender, RoutedEventArgs e)
		{
			outName = outNameTb.Text;

			if (outName == null || path == null)
				return;
			if (path == "" || outName == "")
				return;

			WindowsManager.Open(ControlsManager._mainMenuControl);
			WindowsManager._mainWindow.Hide();

			Thread tr = new Thread(Tr);
			tr.Name = "Processing workflow";
			tr.Start();

			void Tr()
			{
				RealtimeFFT.Stop();

				if (_s == "Octave reverse (soft, IFFT)")
					OctaveReverse_Soft_IFFT.Make(path, outName);
				else if (_s == "Octave reverse (soft, Nad)")
					OctaveReverse_Soft_Nad.Make(path, outName);
				else if (_s == "Vertical reverse (Nad)")
					WavVerticalReverse_Nad.Make(path, outName);
				else if (_s == "Wav to wav (Nad)")
					WavToWav_Nad.Make(path, outName);
				else if (_s == "Wav to wav (IFFT)")
					WavToWav_IFFT.Make(path, outName);
				else if (_s == "Wav to jpg")
					WavToJpg.Make(path, outName);
				else if (_s == "Jpg to wav (Nad)")
					JpgToWav_Nad.Make(path, outName);
				else if (_s == "Jpg to wav (IFFT)")
					JpgToWav_IFFT.Make(path, outName);
				else if (_s == "Jpg to wav octave reverse (Nad)")
					JpgOctaveReverse_Soft_Nad.Make(path, outName);
				else
					outNameTb.Text = "Wrong type in list";

				Application.Current.Dispatcher.Invoke(() =>
				{
					WindowsManager._mainWindow.Show();
				});
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
				if (_s == "Jpg to wav (Nad)")
					ImageSelection();
				else if (_s == "Jpg to wav (IFFT)")
					ImageSelection();
				else if (_s == "Jpg to wav octave reverse (Nad)")
					ImageSelection();
				else
					AudioSelection();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Library;

namespace MusGen.View.Windows
{
	/// <summary>
	/// Interaction logic for AudioRecreationWindow.xaml
	/// </summary>
	public partial class AudioRecreationWindow : Window
	{
		public AudioRecreationWindow()
		{
			InitializeComponent();
		}

		public string path;
		private string outName;

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
					SelBtn.Content = name;
					outNameTb.Text = Text2.StringBeforeLast(name, ".wav");
				}
				else
				{
					MessageBox.Show("Wrong wav file", "Sorry", MessageBoxButton.OK, MessageBoxImage.Error);
					SelBtn.Content = "Select .wav file";
					path = "";
				}
			}
		}

		private void ProcessBtn_Click(object sender, RoutedEventArgs e)
		{
			outName = outNameTb.Text;
			this.Close();

			Thread tr = new Thread(Tr);
			tr.Start();
			
			void Tr()
			{
				EffectsCreator.EffectFFTMulti(path, outName);
			}
		}
	}
}

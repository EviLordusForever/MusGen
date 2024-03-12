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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace MusGen
{
	/// <summary>
	/// Interaction logic for MainMenuControl.xaml
	/// </summary>
	public partial class MainMenuControl : UserControl
	{
		public MainMenuControl()
		{
			InitializeComponent();
		}

		private void NNClick(object sender, RoutedEventArgs e)
		{
			Thread myThread = new Thread(MyThread);
			myThread.Name = "EVOLUTION";
			myThread.Start();

			void MyThread()
			{
				NNWorkflow.EVOLVE();
			}
		}

		private void GeneratorClick(object sender, RoutedEventArgs e)
		{
			Thread myThread = new Thread(MyThread);
			myThread.Name = "GENERATING";
			myThread.Start();

			void MyThread()
			{
				Generator.Generate("RESULT");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			WindowsManager.Open(ControlsManager._audioControl);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			WindowsManager.OpenRealtimeFFTWindow();
			RealtimeFFT.Start("microphone");
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			WindowsManager.OpenRealtimeFFTWindow();
			RealtimeFFT.Start("system");
		}

		private void ComboBox_Selected(object sender, RoutedEventArgs e)
		{
			ComboBoxItem selectedItem = combobox.SelectedItem as ComboBoxItem;
			string s = selectedItem.Content.ToString();
			if (s == "Spectrogram type 1")
				AP._graphType = 1;
			else if (s == "Spectrogram type 2")
				AP._graphType = 2;
			else if (s == "Spectrogram type 4")
				AP._graphType = 4;

			if (WindowsManager._realtimeFFTWindow != null && WindowsManager._realtimeFFTWindow.IsLoaded)
			{
				if (s == "Spectrogram type 4")
				{
					WindowsManager._realtimeFFTWindow.piano.Visibility = Visibility.Hidden;
					WindowsManager._realtimeFFTWindow.circular.Visibility = Visibility.Visible;
				}
				else
				{
					WindowsManager._realtimeFFTWindow.piano.Visibility = Visibility.Visible;
					WindowsManager._realtimeFFTWindow.circular.Visibility = Visibility.Hidden;
				}
			}
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			WindowsManager.OpenAboutWindow();
		}
	}
}

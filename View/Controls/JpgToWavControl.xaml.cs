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
	public partial class JpgToWavControl : UserControl
	{
		public JpgToWavControl()
		{
			InitializeComponent();
		}

		public string path;
		private string outName;

		private void SelBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png";
			dialog.Title = "Please select wav file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				path = dialog.FileName;
				string name = dialog.SafeFileName;

				SelBtn.Content = name;
				outNameTb.Text = TextE.StringBeforeLast(name, ".");
			}
		}

		private void ProcessBtn_Click(object sender, RoutedEventArgs e)
		{
			outName = outNameTb.Text;
			WindowsManager.Open(ControlsManager._mainMenuControl);

			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				JpgToWav.Make(path, outName);
			}
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			WindowsManager.Open(ControlsManager._arControl);
		}
	}
}

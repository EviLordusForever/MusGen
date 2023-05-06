using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MusGen
{
	/// <summary>
	/// Interaction logic for MainControl.xaml
	/// </summary>
	public partial class ARControl : UserControl, INotifyPropertyChanged
	{
		public ARControl()
		{
			DataContext = this;
			InitializeComponent();			
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void Btn1_Click(object sender, RoutedEventArgs e)
		{
			WindowsManager.Open(ControlsManager._wnnwControl);
		}

		private void Btn2_Click(object sender, RoutedEventArgs e)
		{
			WindowsManager.Open(ControlsManager._wavToJpgControl);
		}

		private void Btn3_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			WindowsManager.Open(ControlsManager._mainMenuControl);
		}
	}
}

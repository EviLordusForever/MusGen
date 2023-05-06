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
using Extensions;
using System.Threading;

namespace MusGen
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			BitmapImage ii = new BitmapImage(new Uri($"{DiskE._programFiles}//Images//img2.png"));
			image.Source = ii;
			WindowsManager._mainWindow = this;
			WindowsManager.Open(ControlsManager._mainMenuControl);
		}
	}
}

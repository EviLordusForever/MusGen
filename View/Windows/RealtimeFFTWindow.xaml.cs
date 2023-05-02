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
using Extensions;

namespace MusGen.View.Windows
{
	/// <summary>
	/// Interaction logic for RealtimeFFTWindow.xaml
	/// </summary>
	public partial class RealtimeFFTWindow : Window
	{
        private WindowStyle _windowStyle;
        private WindowState _windowState;
        private bool _isFullScreen;

        public RealtimeFFTWindow()
		{
			InitializeComponent();

            _windowState = WindowState.Normal;
            _windowStyle = WindowStyle.SingleBorderWindow;
            _isFullScreen = true;

            FullScreen();
        }

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            if (!_isFullScreen)
                FullScreen();
            else
                RestoreFromFullScreen();
        }

        private void FullScreen()
        {
            _windowStyle = WindowStyle;
            _windowState = WindowState;

            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            _isFullScreen = true;
        }

        private void RestoreFromFullScreen()
        {
            WindowStyle = _windowStyle;
            WindowState = _windowState;
            _isFullScreen = false;
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            RealtimeFFT.StopAsync();
		}
	}
}

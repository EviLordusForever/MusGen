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

namespace MusGen.View.Windows
{
	/// <summary>
	/// Interaction logic for RealtimeFFTWindow.xaml
	/// </summary>
	public partial class RealtimeFFTWindow : Window
	{
        private WindowStyle _windowStyle;
        private ResizeMode _resizeMode;
        private WindowState _windowState;
        private bool _topmost;
        private bool _isFullScreen;

        public RealtimeFFTWindow()
		{
			InitializeComponent();

            _topmost = false;
            _windowState = WindowState.Normal;
            _resizeMode = ResizeMode.CanResize;
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
            _resizeMode = ResizeMode;
            _windowState = WindowState;
            _topmost = Topmost;

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            Topmost = true;
            _isFullScreen = true;
        }

        private void RestoreFromFullScreen()
        {
            WindowStyle = _windowStyle;
            ResizeMode = _resizeMode;
            WindowState = _windowState;
            Topmost = false;
            _isFullScreen = false;
        }
    }
}

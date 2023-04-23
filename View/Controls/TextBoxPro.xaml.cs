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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusGen.View.Controls
{
	public partial class TextBoxPro : UserControl, INotifyPropertyChanged
	{
		public TextBoxPro()
		{
			DataContext = this;
			InitializeComponent();
		}

		private string placeholder;
		private string text;

		public event PropertyChangedEventHandler? PropertyChanged;

		public string Placeholder
		{
			get { return placeholder; }
			set 
			{ 
				placeholder = value;
				OnPropertyChanged();
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				OnPropertyChanged();
			}
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			txtInput.Clear();
			txtInput.Focus();
		}

		private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(txtInput.Text))
				tbPlaceholder.Visibility = Visibility.Visible;
			else
				tbPlaceholder.Visibility = Visibility.Hidden;
		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

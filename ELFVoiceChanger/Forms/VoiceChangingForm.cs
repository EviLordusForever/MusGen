using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELFVoiceChanger.Forms
{
	public partial class VoiceChangingForm : Form
	{
		public VoiceChangingForm()
		{
			InitializeComponent();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void selectAudioFileButton_Click(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog();
		}
	}
}

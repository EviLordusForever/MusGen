using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Core
{
	public static class UserAsker
	{
		public static bool Ask(string q)
		{
			var confirmResult = MessageBox.Show(q, "Hey", MessageBoxButtons.YesNo);
			if (confirmResult == DialogResult.Yes)
				return true;
			else
				return false;
		}
	}
}

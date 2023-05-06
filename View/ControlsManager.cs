using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class ControlsManager
	{
		public static ARControl _arControl = new ARControl();
		public static WnnwControl _wnnwControl = new WnnwControl();
		public static MainMenuControl _mainMenuControl = new MainMenuControl();
		public static WavToJpgControl _wavToJpgControl = new WavToJpgControl();
	}
}

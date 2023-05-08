using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class HammingWindow
	{
        private static float[] _window;

        static HammingWindow()
        {
            Init();
        }

        public static void Init()
        {
            _window = new float[AP.FftSize];
            for (int i = 0; i < AP.FftSize; i++)
                _window[i] = 0.54f - 0.46f * MathF.Cos(2f * MathF.PI * i / (AP.FftSize - 1f));
        }

        public static float F(int v)
        {
            return _window[v];
        }
    }
}

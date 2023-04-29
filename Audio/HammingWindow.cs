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
            _window = new float[AP._fftSize];
            for (int i = 0; i < AP._fftSize; i++)
                _window[i] = 0.54f - 0.46f * MathF.Cos(2f * MathF.PI * i / (AP._fftSize - 1f));
        }

        public static float F(int v)
        {
            return _window[v];
        }
    }
}

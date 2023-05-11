using System;
using System.Linq;

namespace Extensions
{
	public static class KaiserFilter
	{
        public static float[] Make(float[] input, float samplingRate, float cutoffFrequency, int filterLength, float beta, bool showProgress)
        {
            int step = input.Length / 1000;

            if (showProgress)
                MusGen.ProgressShower.Show("Kaiser filter for wav low...");

            float[] output = new float[input.Length];
            float wc = 2 * (float)Math.PI * cutoffFrequency / samplingRate;
            float[] h = new float[filterLength];

            for (int n = 0; n < filterLength; n++)
            {
                float alpha = (float)(n - (filterLength - 1) / 2.0);
                h[n] = (float)(Math.Sin(wc * alpha) / (Math.PI * alpha) * KaiserWindow(beta, n, filterLength));
            }

            float sum = h.Sum();
            for (int n = 0; n < filterLength; n++)
                h[n] /= sum;

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = 0;
                for (int j = 0; j < filterLength; j++)
                    if (i - j >= 0)
                        output[i] += h[j] * input[i - j];

                if (showProgress)
                    if (i % step == 0)
                        MusGen.ProgressShower.Set(1.0 * i / input.Length);
            }

            if (showProgress)
                MusGen.ProgressShower.Close();

            return output;
        }

        private static float KaiserWindow(float beta, int n, int filterLength)
        {
            float alpha = (float)(2 * n) / (filterLength - 1) - 1;
            return BesselI0(beta * MathF.Sqrt(1 - alpha * alpha)) / BesselI0(beta);
        }

        private static float BesselI0(float x)
        {
            float y = 0;
            float t = 1;
            for (int k = 1; k <= 50; k++)
            {
                t *= x / (2f * k);
                y += t * t;
            }
            return y + 1;
        }
    }
}

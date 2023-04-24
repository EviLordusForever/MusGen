using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SoundPressureModel
	{
		public static Dictionary<float, float> sf;

		static SoundPressureModel()
		{
			sf = new Dictionary<float, float>();

			sf.Add(0f, 0f);
			sf.Add(20f, 0.05f);
			sf.Add(25f, 0.075f);
			sf.Add(31.5f, 0.1f);
			sf.Add(40f, 0.15f);
			sf.Add(50f, 0.2f);
			sf.Add(63f, 0.25f);
			sf.Add(80f, 1.1220f);
			sf.Add(100f, 1.4125f);
			sf.Add(125f, 1.5849f);
			sf.Add(160f, 1.4983f);
			sf.Add(200f, 1.1220f);
			sf.Add(250f, 1f);
			sf.Add(315f, 0.8913f);
			sf.Add(400f, 0.8913f);
			sf.Add(500f, 1.2589f);
			sf.Add(630f, 1.5849f);
			sf.Add(800f, 1.4983f);
			sf.Add(1000f, 1.1220f);
			sf.Add(1250f, 0.8913f);
			sf.Add(1600f, 0.7943f);
			sf.Add(2000f, 0.7943f);
			sf.Add(2500f, 1.1220f);
			sf.Add(3150f, 2.2387f);
			sf.Add(4000f, 4.4668f);
			sf.Add(5000f, 6.3096f);
			sf.Add(6300f, 5.6234f);
			sf.Add(8000f, 2.5119f);
			sf.Add(10000f, 1.2589f);
			sf.Add(12500f, 0.8913f);
			sf.Add(16000f, 0.5012f);
			sf.Add(20000f, 0.4f);
			sf.Add(25000f, 0f);
		}

		public static float GetSoundPressureLevel(float frq)
		{
			float lowerKey = sf.Keys.Where(x => x <= frq).Max();
			float upperKey = sf.Keys.Where(x => x >= frq).Min();

			float lowerValue = sf[lowerKey];
			float upperValue = sf[upperKey];

			return lowerValue + (upperValue - lowerValue) * (frq - lowerKey) / (upperKey - lowerKey);
		}
	}
}

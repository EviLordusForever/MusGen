using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class Solver
	{
		public static float[] Solve(float[][] eqs, float[] b, int Ts = 10, float speed = 1)
		{
			//[eq][c];

			float antispeed = 1 - speed;

			float[] a = new float[b.Length];
			float[] aold = new float[b.Length];
			for (int i = 0; i < b.Length; i++)
			{
				a[i] = b[i];
				aold[i] = b[i];
			}

			for (int T = 0; T < Ts; T++)
			{
				aold = a;
				a = new float[b.Length];
				for (int eq = 0; eq < b.Length; eq++)
				{
					float gg = EQUATION(eqs[eq], aold);
					gg -= SUB_EQUATION(eqs[eq][eq], aold[eq]);
					a[eq] = (b[eq] - gg) * speed + aold[eq] * antispeed;
				}

				//Logger.Log(string.Join(", ", a));
			}

			for (int eq = 0; eq < b.Length; eq++)
			{
				if (a[eq] < 0)
					a[eq] = 0;
				if (a[eq] > b[eq] * 1.3f)
					a[eq] = b[eq] * 0.5f; //////////////////////////
			}

			return a;

			float EQUATION(float[] coefs, float[] a)
			{
				float res = 0;

				for (int i = 0; i < a.Length; i++)
					res += coefs[i] * a[i];

				return res;
			}

			float SUB_EQUATION(float coef, float a)
			{
				return coef * a;
			}
		}
	}
}

using System;
using MathNet;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace Extensions
{
    public static class SolverMN
    {
        public static float[] Solve(float[,] a, float[] b)
        {
            // создаем матрицу и вектор из переданных параметров
            Matrix<float> A = Matrix<float>.Build.DenseOfArray(a);
            Vector<float> B = Vector<float>.Build.Dense(b);

            var solver = new MathNet.Numerics.LinearAlgebra.Single.Solvers.TFQMR();
            Vector<float> x = A.SolveIterative(B, solver);

            float[] result = new float[x.Count];
            for (int i = 0; i < x.Count; i++)
            {
                result[i] = x[i];
            }

            // возвращаем результат
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MusGen
{
	public static class UTests
	{
		public static void DoTests()
		{
            TestCalculator();
		}

        private static void TestCalculator()
        {
            double result = Calculator.Add(5, 7);
            Assert.Equal(result, 12);
            result = Calculator.Substract(12, 7);
            Assert.Equal(result, 5);
            result = Calculator.Multiply(9, 9);
            Assert.Equal(result, 81);
            result = Calculator.Divide(120, 4);
            Assert.Equal(result, 30);
        }
    }

    public static class Calculator
    {
        public static double Add(double a, double b)
        {
            return a + b;
        }

        public static double Substract(double a, double b)
        {
            return a - b;
        }

        public static double Multiply(double a, double b)
        {
            return a * b;
        }

        public static double Divide(double a, double b)
        {
            return a / b;
        }
    }
}

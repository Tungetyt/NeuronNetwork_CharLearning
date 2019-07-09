using System;

namespace NAI_uczenie.Models
{
    public class SigmoidalBipolarFunction : IFunction
    {
        public double Calc(double x, double lambda)
        {
            return 2 / (1 + Math.Pow(Math.E, -lambda * x)) - 1;
        }

        public double CalcDerivative(double fx, double lambda)
        {
            return lambda * (1 - fx * fx) / 2;
        }
    }
}
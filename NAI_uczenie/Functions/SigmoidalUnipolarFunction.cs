using System;

namespace NAI_uczenie.Models
{
    public class SigmoidalUnipolarFunction : IFunction
    {
        public double Calc(double x, double lambda)
        {
            return 1 / (1 + Math.Pow(Math.E, -lambda * x));
        }

        public double CalcDerivative(double fx, double lambda)
        {
            return lambda * fx * (1 - fx);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

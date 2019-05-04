using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAI_uczenie.Models
{
    public interface IFunction
    {
        double Calc(double x, double lambda);
        double CalcDerivative(double fx, double lambda);
    }
}

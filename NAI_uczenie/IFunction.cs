namespace NAI_uczenie.Models
{
    public interface IFunction
    {
        double Calc(double x, double lambda);

        double CalcDerivative(double fx, double lambda);
    }
}
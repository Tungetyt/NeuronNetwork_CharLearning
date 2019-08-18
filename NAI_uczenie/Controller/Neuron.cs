namespace NeuronNetwork_CharLearning.Models
{
    public class Neuron
    {
        public double[] Wage_Vector { get; set; }
        public double Theta { get; set; }
        public double Epsilon { get; set; } = 0.0;
        public double Y { get; set; }

        public Neuron(int numOfWages)
        {
            Wage_Vector = new double[numOfWages];
            Randomise();
        }

        public double this[int index]
        {
            get
            {
                return Wage_Vector[index];
            }
            set
            {
                Wage_Vector[index] = value;
            }
        }

        private void Randomise()
        {
            var min = -0.9;
            var max = 0.9;

            for (var wageIt = 0; wageIt < Wage_Vector.Length; wageIt++)
            {
                Wage_Vector[wageIt] = Tools.GetRandomDouble(min, max);
            }
            Theta = Tools.GetRandomDouble(min, max);
        }
    }
}
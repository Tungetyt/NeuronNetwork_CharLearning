using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuronNetwork_CharLearning.Models
{
    public class Neuron
    {
        public double[] Wages { get; set; }
        public double Theta { get; set; }
        public double Epsilon { get; set; } = 0.0;
        public double Y { get; set; }

        public Neuron(int numOfWages)
        {
            Wages = new double[numOfWages];

            var min = -0.9;
            var max = 0.9;

            //Losowanie wag i progu wejsciowego
            for (var wageIt = 0; wageIt < Wages.Length; wageIt++)
            {
                Wages[wageIt] = AdditionalStaff.GetRandomDouble(min, max);

            }
            Theta = AdditionalStaff.GetRandomDouble(min, max);
        }
    }
}
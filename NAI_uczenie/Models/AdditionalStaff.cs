using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork_CharLearning.Models
{
    public static class AdditionalStaff
    {
        public static Random rand = new Random();

        internal static double GetRandomDouble(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}
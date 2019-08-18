using System;

namespace NeuronNetwork_CharLearning.Models
{
    public static class Tools
    {
        public static Random rand = new Random();

        internal static double GetRandomDouble(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}
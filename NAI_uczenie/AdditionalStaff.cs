using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        internal static void ChangeListOrder(ObservableCollection<InputData> InputsDatas)
        {
            for (int idIt = 0; idIt < InputsDatas.Count; idIt++)
            {
                int randIdIt = rand.Next(idIt, InputsDatas.Count);
                var temp = InputsDatas[idIt];
                InputsDatas[idIt] = InputsDatas[randIdIt];
                InputsDatas[randIdIt] = temp;
            }
        }
    }
}
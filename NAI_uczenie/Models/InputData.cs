using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork_CharLearning.Models
{
    public class InputData
    {
        public double[] X_Vector { get; set; }
        public int[] D_Vector { get; set; }
        public char Label { get; set; }

        public InputData(double[] X_Vector, char Label, int[] D_Vector)
        {
            this.X_Vector = X_Vector;
            this.D_Vector = D_Vector;
            this.Label = Label;
        }
    }
}
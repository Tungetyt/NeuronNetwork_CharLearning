﻿namespace NeuronNetwork_CharLearning
{
    public struct InputData
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

        public double this[int index]
        {
            get
            {
                return X_Vector[index];
            }
            set
            {
                X_Vector[index] = value;
            }
        }
    }
}
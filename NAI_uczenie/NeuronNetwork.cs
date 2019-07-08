using NAI_uczenie.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeuronNetwork_CharLearning.Models
{
    public class NeuronNetwork
    {
        public const double alfa = 0.5; 
        public const double lambda = 1.0;
        public const double ErrorThreshold = 0.1;
        public const int maxEra = 500;
        public const int maxInputNeurons = 5;     
        public static int MaxOutputNeurons { get; set; } = 10;      
        public double[] EraErrors { get; set; }
        private ObservableCollection<InputData> InputsDatas { get; set; }
        private List<Neuron> InNeurons { get; set; }
        private List<Neuron> OutNeurons { get; set; }
        private IFunction Fun { get; set; } = new SigmoidalUnipolarFunction();

        public NeuronNetwork(ObservableCollection<InputData> InputsDatas, int numOfDistinctLabels)
        {
            this.InputsDatas = InputsDatas;
            MaxOutputNeurons = numOfDistinctLabels;
            EraErrors = new double[maxEra + 1];

            InNeurons = CreateNeurons(maxInputNeurons, InputsDatas[0].X_Vector.Length);

            OutNeurons = CreateNeurons(MaxOutputNeurons, maxInputNeurons);
        }

        public double[] Teach()
        {
            int eraIt = 1;
            for (; eraIt <= maxEra; eraIt++)
            {
                for (int idIt = 0; idIt < InputsDatas.Count(); idIt++)
                {
                    var currInputData = InputsDatas[idIt];

                    CalcInY_Vector(currInputData);

                    CalcOutY_VectorAndErrors(currInputData, eraIt);

                    TeachInNeuronsCalcEpsilon(currInputData, eraIt);

                    TeachOutNeurons();
                }

                AdditionalStaff.ChangeListOrder(InputsDatas);

                if (EraErrors[eraIt] < ErrorThreshold)
                    break;
            }

            double[] EraErrorsShortened = new double[eraIt];
            Array.Copy(EraErrors, EraErrorsShortened, eraIt);

            return EraErrorsShortened;
        }

        public char Test(double[] x_Vector)
        {
            CalcInY_Vector(x_Vector);

            CalcOutY_Vector();

            for (int idIt = 0; idIt < InputsDatas.Count; idIt++)
            {
                var currInputData = InputsDatas[idIt];
                for (int i = 0; i < MaxOutputNeurons; i++)
                {
                    if (currInputData.D_Vector[i] != (int)Math.Round(OutNeurons[i].Y))
                    {
                        break;
                    }

                    if (i == MaxOutputNeurons - 1)
                    {
                        return currInputData.Label;
                    }
                }
            }

            return ' ';
        }

        private List<Neuron> CreateNeurons(int maxNeurons, int numOfWages)
        {
            var neurons = new List<Neuron>();
            for (int i = 0; i < maxNeurons; i++)
            {
                neurons.Add(new Neuron(numOfWages));
            }
            return neurons;
        }

        private void TeachOutNeurons()
        {
            for (int outNeurIt = 0; outNeurIt < MaxOutputNeurons; outNeurIt++)
            {
                TeachOutNeuron(OutNeurons[outNeurIt], InNeurons);
            }
        }

        private void TeachInNeuronsCalcEpsilon(InputData currInputData, int eraIt)
        {
            for (int inNeurIt = 0; inNeurIt < maxInputNeurons; inNeurIt++)
            {
                Neuron inNeur = InNeurons[inNeurIt];
                CalcInEpsilon(inNeur, inNeurIt);
                TeachInNeuron(inNeur, currInputData);
            }
        }

        private void CalcInEpsilon(Neuron inNeur, int inNeurIt)
        {
            inNeur.Epsilon = 0.0;
            for (int outNeurIt = 0; outNeurIt < MaxOutputNeurons; outNeurIt++)
            {
                Neuron outNeur = OutNeurons[outNeurIt];
                inNeur.Epsilon += outNeur.Wage_Vector[inNeurIt] * outNeur.Epsilon;
            }

            inNeur.Epsilon *= Fun.CalcDerivative(inNeur.Y, lambda);
        }

        private void CalcOutY_VectorAndErrors(InputData currInputData, int eraIt)
        {
            for (int outNeurIt = 0; outNeurIt < MaxOutputNeurons; outNeurIt++)
            {
                Neuron outNeur = OutNeurons[outNeurIt];
                outNeur.Y = CalcOutY(outNeur, InNeurons);
                double y = outNeur.Y;
                double D_Y_diff = currInputData.D_Vector[outNeurIt] - y;
                outNeur.Epsilon = D_Y_diff * Fun.CalcDerivative(y, lambda);
                EraErrors[eraIt] += D_Y_diff * D_Y_diff;
            }
        }

        private void TeachInNeuron(Neuron neur, InputData inputData)
        {
            for (int i = 0; i < neur.Wage_Vector.Count(); i++)
            {
                neur.Wage_Vector[i] += alfa * neur.Epsilon * inputData.X_Vector[i];
            }
            neur.Theta += alfa * neur.Epsilon;
        }

        private void TeachOutNeuron(Neuron neur, List<Neuron> neurons)
        {
            for (int i = 0; i < neur.Wage_Vector.Count(); i++)
            {
                neur.Wage_Vector[i] += alfa * neur.Epsilon * neurons[i].Y;
            }
            neur.Theta += alfa * neur.Epsilon;
        }

        private void CalcInY_Vector(InputData currInputData)
        {
            for (int inNeurIt = 0; inNeurIt < maxInputNeurons; inNeurIt++)
            {
                Neuron inNeur = InNeurons[inNeurIt];
                inNeur.Y = CalcInY(inNeur, currInputData.X_Vector);
            }
        }

        private void CalcOutY_Vector()
        {
            for (int outNeurIt = 0; outNeurIt < MaxOutputNeurons; outNeurIt++)
            {
                var outNeur = OutNeurons[outNeurIt];
                outNeur.Y = CalcOutY(outNeur, InNeurons);
            }
        }

        private void CalcInY_Vector(double[] x_Vector)
        {
            for (int inNeurIt = 0; inNeurIt < maxInputNeurons; inNeurIt++)
            {
                var inNeur = InNeurons[inNeurIt];
                inNeur.Y = CalcInY(inNeur, x_Vector);
            }
        }

        private double CalcInY(Neuron neur, double[] x_Vector)
        {
            double net = 0.0;
            for (int i = 0; i < x_Vector.Length; i++)
                net += neur.Wage_Vector[i] * x_Vector[i];

            net += neur.Theta;
            return Fun.Calc(net, lambda);
        }

        private double CalcOutY(Neuron outNeur, List<Neuron> inNeurons)
        {
            double net = 0.0;
            for (int i = 0; i < inNeurons.Count; i++)
                net += outNeur.Wage_Vector[i] * inNeurons[i].Y;

            net += outNeur.Theta;
            return Fun.Calc(net, lambda);
        }
    }
}
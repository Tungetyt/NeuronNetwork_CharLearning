using NAI_uczenie.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeuronNetwork_CharLearning.Models
{
    public class NeuronNetwork
    {
        public const double alfa = 0.5; //learningCoefficient
        public const double lambda = 1.0;
        public const double ErrorThreshold = 0.1;
        public const int maxEra = 500;
        public const int maxInputNeurons = 5; //Zastosowac tu wzor ; 17
        public static int MaxOutputNeurons { get; set; } = 10; //SHOULDNT BE 10!!!!!!!! SHOULD BE EMPTY
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

            //generowanie Neuronow wejsciowych
            InNeurons = CreateNeurons(maxInputNeurons, InputsDatas[0].X_Vector.Length);

            //generowanie Neuronow wyjsciowych
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

                    //Obliczenie y dla warstwy wejsciowej
                    CalcInY_Vector(currInputData);

                    //Obliczenie y i epsilonow dla warstwy wyjsciowej
                    CalcOutY_VectorAndErrors(currInputData, eraIt);

                    //Obliczenie epsilonow i zmiana wag i progow dla warstwy wejsciowej
                    TeachInNeuronsCalcEpsilon(currInputData, eraIt);

                    //Zmiana wag i progow dla neuronow wyjsciowych
                    TeachOutNeurons();
                }

                //Zmiana kolejnosci
                AdditionalStaff.ChangeListOrder(InputsDatas);

                //Wylicz blad sredni i ustal czy przerwac
                if (EraErrors[eraIt] < ErrorThreshold)
                    break;
            }

            double[] EraErrorsShortened = new double[eraIt];
            Array.Copy(EraErrors, EraErrorsShortened, eraIt);

            return EraErrorsShortened;
        }

        public char Test(double[] x_Vector)
        {
            //Wylicz Y-Wektor dla Neuronow wejsc.
            CalcInY_Vector(x_Vector);

            //Wylicz Y-Wektor dla Neuronow wyjsc.
            CalcOutY_Vector();

            //Porownaj Y z D i wypluj znak jak sie zgadza
            for (int idIt = 0; idIt < InputsDatas.Count; idIt++)
            {
                var currInputData = InputsDatas[idIt];
                for (int i = 0; i < MaxOutputNeurons; i++)
                {
                    if (currInputData.D_Vector[i] != (int)Math.Round(OutNeurons[i].Y))
                    {
                        break;
                    }

                    //Jesli udalo sie dojsc do konca to zwroc znak
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
                //Wyliczanie epsilona wejsciowego
                CalcInEpsilon(inNeur, inNeurIt);
                //Zmiana wag i progow dla neuronow wejsciowych
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

        //Obliczanie Net i Y
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
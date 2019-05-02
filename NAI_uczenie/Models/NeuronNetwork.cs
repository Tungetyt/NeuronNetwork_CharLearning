using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuronNetwork_CharLearning.Models
{
    public class NeuronNetwork
    {
        public const double alfa = 0.5; //learningCoefficient
        public const double lambda = 1.0;
        public double errorThreshold = 0.1;
        public const int maxEra = 500;
        const bool isUnipolar = true;
        public int MaxInputNeurons { get; set; } //Zastosowac tu wzor, 17
        public const int maxOutputNeurons = 10;
        public double[] EraErrors { get; set; }
        ObservableCollection<InputData> InputsDatas { get; set; } = new ObservableCollection<InputData>();
        List<Neuron> InNeurons { get; set; } = new List<Neuron>();
        List<Neuron> OutNeurons { get; set; } = new List<Neuron>();

        public NeuronNetwork(ObservableCollection<InputData> InputsDatas)
        {
            this.InputsDatas = InputsDatas;

            if (isUnipolar)
            {
                errorThreshold = 0.1;
                MaxInputNeurons = 6;
            }
            else
            {
                errorThreshold = 0.00001;
                MaxInputNeurons = 100;
            }

            EraErrors  = new double[maxEra + 1];

            int xLength = InputsDatas[0].X_Vector.Length;
            //MaxInputNeurons = /*(xLength + maxOutputNeurons) / 2*/;

            //generowanie Neuronow wejsciowych
            for (var i = 0; i < MaxInputNeurons; i++)
            {
                InNeurons.Add(new Neuron(xLength));
            }

            //generowanie Neuronow wyjsciowych
            for (var i = 0; i < maxOutputNeurons; i++)
            {
                OutNeurons.Add(new Neuron(MaxInputNeurons));
            }
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
                    for (var inNeurIt = 0; inNeurIt < MaxInputNeurons; inNeurIt++)
                    {
                        Neuron inNeur = InNeurons[inNeurIt];
                        inNeur.Y = CalcY(inNeur, currInputData.X_Vector);
                    }

                    //Obliczenie y i epsilonow dla warstwy wyjsciowej
                    for (var outNeurIt = 0; outNeurIt < maxOutputNeurons; outNeurIt++)
                    {
                        Neuron outNeur = OutNeurons[outNeurIt];
                        double net = 0.0;
                        for (var i = 0; i < InNeurons.Count; i++)
                            net += outNeur.Wages[i] * InNeurons[i].Y;

                        net += outNeur.Theta; //????????????
                        outNeur.Y = CalcSigmoidalFun(net);/*CalcY(outNeur, inNeurons);*/
                        double y = outNeur.Y;
                        outNeur.Epsilon = (currInputData.DArr[outNeurIt] - y) * CalcSigmoidalFunDerivative(y);
                        EraErrors[eraIt] += outNeur.Epsilon;
                    }

                    //Obliczenie epsilonow i zmiana wag i progow dla warstwy wejsciowej
                    for (var inNeurIt = 0; inNeurIt < MaxInputNeurons; inNeurIt++)
                    {
                        Neuron inNeur = InNeurons[inNeurIt];
                        inNeur.Epsilon = 0.0;
                        for (var outNeurIt = 0; outNeurIt < maxOutputNeurons; outNeurIt++)
                        {
                            Neuron outNeur = OutNeurons[outNeurIt];
                            inNeur.Epsilon += outNeur.Wages[inNeurIt] * outNeur.Epsilon;
                        }

                        inNeur.Epsilon *= CalcSigmoidalFunDerivative(inNeur.Y);
                        EraErrors[eraIt] += inNeur.Epsilon;

                        //Zmiana wag i progow dla neuronow wejsciowych
                        for (int i = 0; i < inNeur.Wages.Count(); i++)
                        {
                            inNeur.Wages[i] += alfa * inNeur.Epsilon * currInputData.X_Vector[i];
                        }
                        inNeur.Theta += alfa * inNeur.Epsilon;
                    }

                    //Zmiana wag i progow dla neuronow wyjsciowych
                    for (var neurIt = 0; neurIt < maxOutputNeurons; neurIt++)
                    {
                        Neuron outNeur = OutNeurons[neurIt];
                        for (int i = 0; i < outNeur.Wages.Count(); i++)
                        {
                            outNeur.Wages[i] += alfa * outNeur.Epsilon * InNeurons[i].Y;
                        }
                        outNeur.Theta += alfa * outNeur.Epsilon;
                    }

                }
                
                //Zmiana kolejnosci
                for (int idIt = 0; idIt < InputsDatas.Count; idIt++)
                {
                    int randIdIt = AdditionalStaff.rand.Next(idIt, InputsDatas.Count);
                    var temp = InputsDatas[idIt];
                    InputsDatas[idIt] = InputsDatas[randIdIt];
                    InputsDatas[randIdIt] = temp;
                }

                Console.WriteLine($"Errors<{eraIt}>: {EraErrors[eraIt]}");

                //Wylicz blad sredni i ustal czy przerwac
                double avgError = 0.0;
                for(int i = 1; i < eraIt + 1; i++)
                {
                    avgError += EraErrors[i];
                }
                avgError /= eraIt;
                if (Math.Abs(avgError) < errorThreshold)
                    break;
            }

            double[] EraErrorsShortened = new double[eraIt];
            for(int i = 0; i < EraErrorsShortened.Length; i++)
            {
                EraErrorsShortened[i] = EraErrors[i];
            }
            return EraErrorsShortened;
        }

        public char Test(double[] x_Vector)
        {
            var yIn = new double[MaxInputNeurons];
            for (var neurIt = 0; neurIt < MaxInputNeurons; neurIt++)
            {
                yIn[neurIt] = CalcY(InNeurons[neurIt], x_Vector);
            }
            var yOut = new double[maxOutputNeurons];
            for (var neurIt = 0; neurIt < maxOutputNeurons; neurIt++)
            {
                yOut[neurIt] = CalcY(OutNeurons[neurIt], yIn);
            }

            for (var idIt = 0; idIt < InputsDatas.Count; idIt++)
            {
                var currInputData = InputsDatas[idIt];
                for (var i = 0; i < maxOutputNeurons; i++)
                {
                    if (currInputData.DArr[i] != (int)Math.Round(yOut[i]))
                    {
                        break;
                    }

                    //Jak dojdzie do konca to zwroc znak
                    if (i == maxOutputNeurons - 1)
                    {
                        return currInputData.Label;
                    }
                }
            }

            return ' ';
        }

        static double CalcSigmoidalFun(double x)
        {
            if(isUnipolar)
                return 1 / (1 + Math.Pow(Math.E, -lambda * x));

            return 2 / (1 + Math.Pow(Math.E, -lambda * x)) - 1;
        }

        static double CalcSigmoidalFunDerivative(double fx)
        {
            if(isUnipolar)
                return lambda * fx * (1 - fx);

            return lambda * (1 - fx * fx) / 2;
        }

        //Obliczanie Net i Y 
        static double CalcY(Neuron neur, double[] x_Vector)
        {
            double net = 0.0;
            for (var i = 0; i < x_Vector.Length; i++)
                net += neur.Wages[i] * x_Vector[i];

            net += neur.Theta; //????????????
            return CalcSigmoidalFun(net);
        }

        //static double CalcY(Neuron outNeur, List<Neuron> inNeurons)
        //{
        //    double net = 0.0;
        //    for (var i = 0; i < inNeurons.Count; i++)
        //        net += outNeur.Wages[i] * inNeurons[i].Y;

        //    net += outNeur.Theta; //????????????
        //    return CalcSignoidalUnipolar(net);
        //}
    }
}
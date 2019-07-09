using NeuronNetwork_CharLearning;
using NeuronNetwork_CharLearning.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NAI_uczenie.Tools
{
    public class DataReader
    {
        public ObservableCollection<InputData> InputsDatas;
        public int NumOfDistinctLabels;

        public DataReader(ObservableCollection<InputData> inputsDatas)
        {
            InputsDatas = inputsDatas;
        }

        public int ReadData()
        {
            InputsDatas.Clear();
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", "")}Data\data.txt";

            using StreamReader file = new StreamReader(path);
            PopulateInputsDatas(file);

            return NumOfDistinctLabels;
        }

        public void PopulateInputsDatas(StreamReader file)
        {
            string ln;
            var biggusDicus = new Dictionary<char, int[]>();
            NumOfDistinctLabels = 0;

            while ((ln = file.ReadLine()) != null)
            {
                AddDataToInputsDatas(biggusDicus, ln);
            }
        }

        private void AddDataToInputsDatas(Dictionary<char, int[]> biggusDicus, string ln)
        {
            char label = ln[ln.Length - 1];
            double[] xVector = Regex.Replace(ln, @",.", "").Select(c => (double)(c - '0')).ToArray();

            if (biggusDicus.ContainsKey(label))
            {
                InputsDatas.Add(new InputData(xVector, label, biggusDicus[label]));
            }
            else
            {
                int[] dArr = new int[NeuronNetwork.MaxOutputNeurons];
                dArr[NumOfDistinctLabels] = 1;
                NumOfDistinctLabels++;
                biggusDicus.Add(label, dArr);
                InputsDatas.Add(new InputData(xVector, label, dArr));
            }
        }
    }
}
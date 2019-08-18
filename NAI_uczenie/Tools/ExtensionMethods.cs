using NeuronNetwork_CharLearning;
using System;
using System.Collections.ObjectModel;

namespace NAI_uczenie.Tools
{
    public static class ExtensionMethods
    {
        internal static void ChangeOrder(this ObservableCollection<InputData> collection)
        {
            Random rand = new Random();
            for (int idIt = 0; idIt < collection.Count; idIt++)
            {
                int randIdIt = rand.Next(idIt, collection.Count);
                var temp = collection[idIt];
                collection[idIt] = collection[randIdIt];
                collection[randIdIt] = temp;
            }
        }
    }
}
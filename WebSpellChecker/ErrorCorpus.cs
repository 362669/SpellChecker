using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebSpellChecker
{
    public class ErrorCorpus
    {
        Dictionary<String, Dictionary<String, double>> errorModel = new Dictionary<string, Dictionary<string, double>>();
        public dynamic SubMatrix { get; set; }
        public dynamic DelMatrix { get; set; }
        public dynamic InsMatrix { get; set; }
        public dynamic TransMatrix { get; set; }


        public ErrorCorpus()
        {
            loadErrorModel();
        }

        public void loadErrorModel()
        {
            var subReader = new StreamReader("sub-plewic.json");
            SubMatrix = JsonConvert.DeserializeObject(subReader.ReadToEnd());

            var delReader = new StreamReader("del-plewic.json");
            DelMatrix = JsonConvert.DeserializeObject(delReader.ReadToEnd());

            var insReader = new StreamReader("ins-plewic.json");
            InsMatrix = JsonConvert.DeserializeObject(insReader.ReadToEnd());

            var transReader = new StreamReader("trans-plewic.json");
            TransMatrix = JsonConvert.DeserializeObject(transReader.ReadToEnd());
        }

        public double getProbability(string errorWord, string correction)
        {
            Dictionary<String, double> errorCorrections = errorModel.FirstOrDefault(t => t.Key.ToLower() == errorWord.ToLower()).Value;

            if (errorCorrections == null)
                return 0;

            return errorCorrections.FirstOrDefault(t => t.Key.ToLower() == correction.ToLower()).Value;
        }
    }
}
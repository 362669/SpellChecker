using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace WebSpellChecker
{
    public class LanguageCorpus
    {
        Dictionary<String, double> languageModel = new Dictionary<string, double>();

        public LanguageCorpus()
        {
            loadLanguageModel(this.getAllFiles("wiki"));
        }

        public List<String> getAllFiles(string targetDirectory)
        {
            List<String> files = new List<string>();

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                files.Add(fileName);
            }
            return files;
        }

        public void loadLanguageModel(List<String> files)
        {
            foreach (string file in files)
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    foreach (dynamic word in data)
                    {
                        languageModel.Add((string)word.Name, (double)word.Value["probability"]);
                    }
                }
            }
        }

        public double getCorrectWordProbability(string candidate)
        {
            double elo = languageModel.FirstOrDefault(t => t.Key == candidate).Value;
            return languageModel.FirstOrDefault(t => t.Key == candidate).Value;
        }
    }
}
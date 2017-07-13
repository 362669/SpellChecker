using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebSpellChecker
{
    public class Dictionary
    {
        public List<String>[] Words { get; set; }
        private ErrorCorpus ErrorCorpus;

        public Dictionary(ErrorCorpus errorCorpus)
        {
            ErrorCorpus = errorCorpus;
            build("odm.txt");
        }

        public double checkCandidateDistanceAndGetProbability(string error, string word)
        {
            int errorLength = error.Length;
            int wordLength = word.Length;
            int differenceCount = 0;
            string type;
            string errorDifference = null;
            string correctionDifference = null;

            if (errorLength == wordLength)
            {
                type = "sub";                          
                for (int i = 0; i < wordLength; i++)
                {
                    if (error[i] != word[i])
                    {
                        differenceCount++;
                        errorDifference = error[i].ToString(); 
                        correctionDifference = word[i].ToString();  

                        if (i < wordLength - 1 && word[i + 1] == error[i] && word[i] == error[i + 1])
                        {

                            errorDifference = error[i].ToString() + error[i + 1].ToString();
                            correctionDifference = word[i].ToString() + word[i + 1].ToString();
                            type = "trans";
                            i++;
                        }

                        if (differenceCount > 1)
                        {
                            return -1;
                        }
                    }
                }

                if (type == "sub")
                {
                    if (ErrorCorpus.SubMatrix.Property(errorDifference) == null)
                    {
                        return 0;
                    }

                    if (ErrorCorpus.SubMatrix[errorDifference].Property(correctionDifference) == null)
                    {
                        return 0;
                    }

                    else
                        return (double)ErrorCorpus.SubMatrix[errorDifference][correctionDifference];
                }

                else
                {
                    if (ErrorCorpus.TransMatrix.Property(errorDifference) == null)
                    {
                        return 0;
                    }

                    if (ErrorCorpus.TransMatrix[errorDifference].Property(correctionDifference) == null)
                    {
                        return 0;
                    }

                    else
                        return (double)ErrorCorpus.TransMatrix[errorDifference][correctionDifference];
                }

            }

            else if (errorLength < wordLength)
            {
                type = "del";
                int j = 0;
                for (int i = 0; i < wordLength; i++)
                {
                    if (i == wordLength - 1 && differenceCount == 0)
                    {
                        errorDifference = word[i - 1].ToString();
                        correctionDifference = word[i - 1].ToString() + word[i].ToString();
                    }

                    else if (error[j] != word[i])
                    {
                        differenceCount++;
                        if (differenceCount > 1)
                        {
                            return -1;
                        }

                        if (i == 0)
                        {
                            errorDifference = "~";
                            correctionDifference = "~" + word[i].ToString();
                        }
                        else
                        {
                            errorDifference = word[i - 1].ToString();
                            correctionDifference = word[i - 1].ToString() + word[i].ToString();
                        }
                    }
                    else
                    {
                        j++;
                    }
                }

                if (errorDifference != null && correctionDifference != null)
                {
                    if (ErrorCorpus.DelMatrix.Property(errorDifference) == null)
                    {
                        return 0;
                    }

                    if (ErrorCorpus.DelMatrix[errorDifference].Property(correctionDifference) == null)
                    {
                        return 0;
                    }

                    else
                        return (double)ErrorCorpus.DelMatrix[errorDifference][correctionDifference];
                }
                else
                {
                    return 0;
                }
            }

            else
            {
                type = "ins";
                int j = 0;
                for (int i = 0; i < errorLength; i++)
                {
                    if (i == errorLength - 1 && differenceCount == 0)
                    {
                        errorDifference = error[i - 1].ToString() + error[i].ToString();
                        correctionDifference = error[i - 1].ToString();
                    }

                    else if (word[j] != error[i])
                    {
                        differenceCount++;
                        if (differenceCount > 1)
                        {
                            return -1;
                        }

                        if (i == 0)
                        {
                            errorDifference = "~" + error[i].ToString();
                            correctionDifference = "~";
                        }
                        else
                        {
                            errorDifference = error[i - 1].ToString() + error[i].ToString();
                            correctionDifference = error[i - 1].ToString();
                        }
                    }
                    else
                    {
                        j++;
                    }
                }

                if (errorDifference != null && correctionDifference != null)
                {

                    if (ErrorCorpus.InsMatrix.Property(errorDifference) == null)
                    {
                        return 0;
                    }

                    if (ErrorCorpus.InsMatrix[errorDifference].Property(correctionDifference) == null)
                    {
                        return 0;
                    }

                    else
                        return (double)ErrorCorpus.InsMatrix[errorDifference][correctionDifference];
                }
                else
                {
                    return 0;
                }
            }
        }

        public void build(String filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    Words = new List<string>[45];

                    String line = sr.ReadLine();

                    while (line != null)
                    {
                        List<string> allWordsInLine = line.Split(new string[] { ", " }, StringSplitOptions.None).ToList();

                        foreach (string w in allWordsInLine)
                        {
                            if (Words[w.Trim().Length] == null)
                                Words[w.Trim().Length] = new List<String>();

                            Words[w.Trim().Length].Add(w);
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public Dictionary<string, double> generatCandidates(string error)
        {
            Dictionary<string, double> Candidates = new Dictionary<string, double>();


            int min = error.Length - 1;
            int max = error.Length + 1;

            if (min < 1) min = 1;

            for (int i = min; i <= max; i++)
            {
                foreach (string word in Words[i])
                {
                    double candidateDistanceAndProbability = this.checkCandidateDistanceAndGetProbability(error, word);

                    if (candidateDistanceAndProbability >= 0)
                    {
                        if (Candidates.ContainsKey(word))
                            continue;

                        Candidates.Add(word, candidateDistanceAndProbability);                    
                    }
                }
            }

            return Candidates;
        }

        public bool contains(string word)
        {
            if (word.Length == 0 || Words[word.Length].Contains(word))
            {
                return true;
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebSpellChecker
{
    public partial class _Default : Page
    {
        static ErrorCorpus errorCorpus = new ErrorCorpus();
        static Dictionary dictionary = new Dictionary(errorCorpus);
        static LanguageCorpus plCorpus = new LanguageCorpus();
        List<string> punctuationMarks = new List<string>(new string[] { ".", ",", "!", "?", ";", ":" });
        List<string> notToUpperList = new List<string>(new string[] { "sa" });


        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void checkButton_Click(Object sender, EventArgs e)
        {
            string text = TextArea.Value.ToLower();
            string output = "";
            string wordIfAnyFinded = "";

            bool firstOfSentence = true;

            foreach (string textWord in text.Split(' '))
            {
                if (textWord == " " || textWord == "")
                {
                    continue;
                }

                string word = textWord;
                wordIfAnyFinded = textWord;
                string beforeSpecialChar = "";
                string afterSpecialChar = "";

                foreach (string punctual in punctuationMarks)
                {
                    if (word[0].ToString() == punctual)
                    {
                        beforeSpecialChar = punctual; 
                    }

                    if (word[word.Length - 1].ToString() == punctual)
                    {
                        afterSpecialChar = punctual;
                    }

                    word = word.Replace(punctual, "");
                }

                if (dictionary.contains(word))
                {
                    if (firstOfSentence)
                    {
                        output += " " + char.ToUpper(textWord[0]) + textWord.Substring(1);
                    }
                    else
                    {
                        output += " " + textWord;
                    }
                }

                else if (dictionary.contains(char.ToUpper(word[0]) + word.Substring(1).ToLower()))
                {
                    output += " " + char.ToUpper(textWord[0]) + textWord.Substring(1);
                }

                else if (dictionary.contains(word.ToUpper()) && !notToUpperList.Contains(word))
                {
                    output += " " + textWord.ToUpper();
                }

                else
                {
                    Dictionary<String, Double> candidatesDictionary = dictionary.generatCandidates(word);
                    List<string> candidates = candidatesDictionary.Keys.ToList();
                    Dictionary<string, double> candidatesWithProbability = new Dictionary<string, double>();


                    foreach (string candidate in candidates)
                    {
                        double errorProbability = 0;
                        double corpusProbability = 0;

                        errorProbability = candidatesDictionary[candidate];
                        corpusProbability = plCorpus.getCorrectWordProbability(candidate);

                        Candidate checkedCandidate = new Candidate(candidate, corpusProbability, errorProbability);
                        candidatesWithProbability.Add(candidate, checkedCandidate.getNoisyChannelProbability());
                    }

                    var sortedDict = from entry in candidatesWithProbability orderby entry.Value descending select entry;

                    string bestCandidates = "";
                    string bestCandidate = "";

                    if (sortedDict.Any())
                    {
                        if (firstOfSentence)
                        {
                            bestCandidates = string.Join(",", sortedDict.Take(5).Select(x => "\"" + char.ToUpper(x.Key[0]) + x.Key.Substring(1) + "\":" + x.Value.ToString(new CultureInfo("en-US"))).ToArray());
                            bestCandidate = char.ToUpper(sortedDict.First().Key[0]) + sortedDict.First().Key.Substring(1);
                        }
                        else
                        {
                            bestCandidates = string.Join(",", sortedDict.Take(5).Select(x => "\"" + x.Key + "\":" + x.Value.ToString(new CultureInfo("en-US"))).ToArray());
                            bestCandidate = sortedDict.First().Key;
                        }

                        output += beforeSpecialChar +
                            "<span style=\"background-color:antiquewhite\" class=\"showCandidates\" data-candidates='{"
                            + bestCandidates +
                            "}'> " +
                            bestCandidate +
                            "</span>" +
                            afterSpecialChar;
                    }
                    else
                    {
                        if (firstOfSentence)
                        {
                            wordIfAnyFinded = char.ToUpper(word[0]) + word.Substring(1);
                        }

                        output += " " + wordIfAnyFinded;
                    }
                }

                if (afterSpecialChar == "." || afterSpecialChar == "?" || afterSpecialChar == "!")
                {
                    firstOfSentence = true;
                }
                else
                {
                    firstOfSentence = false;
                }
            }

            outputText.InnerHtml = output;
        }
    }
}
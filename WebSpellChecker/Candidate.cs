using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace WebSpellChecker
{
    public class Candidate
    {
        public string Word { get; set; }
        public double CorpusProbability { get; set; }
        public double ErrorCorpusProbability { get; set; }
        public string ErrorType { get; set; }

        public Candidate(string word, double corpusProbability, double errorCorpusProbability)
        {
            Word = word;
            CorpusProbability = corpusProbability;
            ErrorCorpusProbability = errorCorpusProbability;
        }

        public double getNoisyChannelProbability()
        {
            return CorpusProbability * ErrorCorpusProbability;
        }
    }
}
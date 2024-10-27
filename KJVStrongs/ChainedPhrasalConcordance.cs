namespace KJVStrongs
{
    public class ChainedPhrasalConcordance
    {
        public Dictionary<string, string[]> dChainedPhrases = new();
        public Dictionary<string, int> dCounts = new(); //D<Phrase, Count>

        //a chained phrasal concordance collects and counts different numbers of successive strong's phrases
        public void CreateChainedPhrasalConcordance(ref KJVData data, int intMaximumChainLength = 1)
        {
            string[] strsChainedPhrasesHistory = (string[])Array.CreateInstance(typeof(string), intMaximumChainLength); //A[this-intChainLength+1, this-intChainLength+2, .., this-intChainLength+intChainLength]

            dChainedPhrases.Clear();

            //initialize strsChainedPhrasesHistory
            for (int i = 1; i <= intMaximumChainLength; i++)
            {
                strsChainedPhrasesHistory[i - 1] = "";
            }

            foreach (Verse v in data.dVerses.Values)
            {
                foreach (Phrase p in v.dPhrases.Values)
                {
                    string strPhrase = p.strPhraseText.ToLower();
                    string strReference = v.strBookName + " " + v.intChapterNumber.ToString() + ":" + v.intVerseNumber.ToString() + "." + p.intPhraseID.ToString();
                    string[] strsChainedPhrasesHistoryCopy = (string[])Array.CreateInstance(typeof(string), intMaximumChainLength);

                    //insert new phrase into history
                    for (int intCurrentChainedPhraseElement = 1; intCurrentChainedPhraseElement < intMaximumChainLength; intCurrentChainedPhraseElement++)
                    {
                        strsChainedPhrasesHistory[intCurrentChainedPhraseElement - 1] = strsChainedPhrasesHistory[intCurrentChainedPhraseElement];
                    }

                    strsChainedPhrasesHistory[intMaximumChainLength - 1] = strPhrase; //populate the last element of the array with strPhrase

                    strsChainedPhrasesHistory.CopyTo(strsChainedPhrasesHistoryCopy, 0);

                    dChainedPhrases.Add(strReference, strsChainedPhrasesHistoryCopy);
                }
            }

            Counts();
            Write(intMaximumChainLength);
        }

        public void Counts()
        {
            foreach (string strReference in dChainedPhrases.Keys.OrderBy(a => a))
            {
                string strChainedPhraseBuilder = "";

                for (int intCurrentChainedPhraseElement = 0; intCurrentChainedPhraseElement < dChainedPhrases[strReference].Length; intCurrentChainedPhraseElement++)
                {
                    strChainedPhraseBuilder += dChainedPhrases[strReference][intCurrentChainedPhraseElement] + " ";
                }

                if (!dCounts.ContainsKey(strChainedPhraseBuilder))
                {
                    dCounts.Add(strChainedPhraseBuilder, 0);
                }

                dCounts[strChainedPhraseBuilder]++;
            }
        }

        public void Write(int intChainLength)
        {
            StreamWriter swChainedPhrasalConcordance;
            StreamWriter swChainedPhrasalConcordanceCounts;
            string strChainedPhraseBuilder = "";

            swChainedPhrasalConcordance = new StreamWriter(@"Data\Processed\ChainedPhrasalConcordance\ChainedPhrasalConcordance-" + intChainLength.ToString() + ".csv");

            swChainedPhrasalConcordance.WriteLine("Reference ^ StrongsPhraseChain");

            foreach (string strReference in dChainedPhrases.Keys.OrderBy(a => a))
            {
                strChainedPhraseBuilder = "";

                swChainedPhrasalConcordance.Write(strReference + " ^ ");

                for (int intCurrentChainedPhraseElement = 0; intCurrentChainedPhraseElement < dChainedPhrases[strReference].Length; intCurrentChainedPhraseElement++)
                {
                    strChainedPhraseBuilder += "[" + dChainedPhrases[strReference][intCurrentChainedPhraseElement] + "] ";
                }

                swChainedPhrasalConcordance.WriteLine(strChainedPhraseBuilder.Trim());
            }

            swChainedPhrasalConcordance.Close();

            swChainedPhrasalConcordanceCounts = new StreamWriter(@"Data\Processed\ChainedPhrasalConcordance\ChainedPhrasalConcordance-" + intChainLength.ToString() + "-Counts.csv");

            swChainedPhrasalConcordanceCounts.WriteLine("Phrase ^ Count");

            foreach (string strPhrase in dCounts.Where(a => a.Value > 1).OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                swChainedPhrasalConcordanceCounts.WriteLine(strPhrase + " ^ " + dCounts[strPhrase].ToString());
            }

            swChainedPhrasalConcordanceCounts.Close();
        }

        public void Read()
        {
            bool bSeenHeader = false;
            foreach (string strFilename in Directory.EnumerateFiles(
                @"Data\Processed\ChainedPhrasalConcordance", "ChainedPhrasalConcordance -*", SearchOption.TopDirectoryOnly))
            {
                if (!strFilename.Contains("Counts.csv"))
                {
                    StreamReader srCPC = new(strFilename);

                    while (!srCPC.EndOfStream)
                    {
                        string strLine = srCPC.ReadLine();

                        if (bSeenHeader == false)
                        {
                            bSeenHeader = true;
                        }
                        else
                        {
                            string[] strsLine = strLine.Split('^');
                            string strLength = strFilename.Remove(0, 27);
                            int intLength = 0;
                            string[] strsPhrases;
                            int intPhraseCounter = -1;

                            strLength = strLength.Remove(strLength.Length - 4, 4);
                            intLength = Convert.ToInt16(strLength);
                            strsPhrases = (string[])Array.CreateInstance(typeof(string), intLength);

                            foreach (string strPhrase in strsLine[1].Split(']'))
                            {
                                intPhraseCounter++;

                                strsPhrases[intPhraseCounter] = strPhrase.TrimStart('[');
                            }

                            dChainedPhrases.Add(strsLine[0].Trim(), strsPhrases);
                        }
                    }

                    srCPC.Close();
                }
            }

            bSeenHeader = false;

            foreach (string strFilename in Directory.EnumerateFiles(
                "Data", "ChainedPhrasalConcordance -*Counts.csv", SearchOption.TopDirectoryOnly))
            {
                StreamReader srCPCC = new(strFilename);

                while (!srCPCC.EndOfStream)
                {
                    string strLine = srCPCC.ReadLine();

                    if (bSeenHeader == false)
                    {
                        bSeenHeader = true;
                    }
                    else
                    {
                        string[] strsLine = strLine.Split('^');

                        if (!dCounts.ContainsKey(strsLine[0]))
                        {
                            dCounts.Add(strsLine[0], Convert.ToInt16(strsLine[1]));
                        }
                    }
                }

                srCPCC.Close();
            }
        }
    }
}

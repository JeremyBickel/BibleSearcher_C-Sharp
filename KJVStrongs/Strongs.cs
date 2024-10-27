using BLBLexicon;

namespace KJVStrongs
{
    public class StrongsSequence
    {
        public int intStrongsSequenceID = 0;
        public string strStrongsNumber = "";
        public bool bParenthecized = false;
        public bool bThing = false;
        public bool bAction = false;
        public bool bObject = false;
        public bool bPreposition = false;
    }

    public class ComplexStrongsSequences
    {
        public Dictionary<int, Dictionary<int, string>> dLocalPhraseClusters = new(); //D<VerseID, D<PhraseID, Dash-Separated-POS>>  The dash is for complex Strongs Sequences

        public void CreateComplexStrongsSequences(ref KJVData data, ref BLBLexiconData blbData)
        {
            string strStrongsLanguageIdentifier = "";
            foreach (Verse v in data.dVerses.OrderBy(a => a.Key).Select(a => a.Value))
            {
                dLocalPhraseClusters.Add(v.intVerseID, new Dictionary<int, string>());

                strStrongsLanguageIdentifier = v.intBookNumber <= 39 ? "H" : "G";

                foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                {
                    string strComplexPOS = ""; //build dash-separated POS string from a complex Strongs Sequence

                    foreach (int intStrongsSequenceID in p.dStrongsSequences.Keys.OrderBy(a => a))
                    {
                        strComplexPOS += StrongsToPOS(ref data, ref blbData, strStrongsLanguageIdentifier + p.dStrongsSequences[intStrongsSequenceID].strStrongsNumber) + "-";
                    }

                    strComplexPOS = strComplexPOS.TrimEnd('-');

                    dLocalPhraseClusters[v.intVerseID].Add(p.intPhraseID, strComplexPOS);
                }
            }

            //clustering based on original language POS doesn't work well enough, because that POS information doesn't include everything,
            //like prepositions, wherever they are.  for instance, "In the beginning" is just labelled as a noun.
            //Also, distinctions of time, place, animal, money, etc would be very helpful, in addition to normal POS tags..
        }

        public void WriteComplexStrongsSequences(ref StreamWriter swLocalPhraseClusters)
        {
            swLocalPhraseClusters.WriteLine("VerseID ^ PhraseID ^ ComplexPOS");

            foreach (int intVerseID in dLocalPhraseClusters.Keys.OrderBy(a => a))
            {
                foreach (int intPhraseID in dLocalPhraseClusters[intVerseID].Keys.OrderBy(a => a))
                {
                    swLocalPhraseClusters.WriteLine(intVerseID.ToString() + " ^ " +
                        intPhraseID.ToString() + " ^ " + dLocalPhraseClusters[intVerseID][intPhraseID]);
                }
            }

            swLocalPhraseClusters.Close();
        }

        public void ReadComplexStrongsSequences(ref StreamReader srLocalPhraseClusters)
        {
            bool bSeenHeader = false;

            while (!srLocalPhraseClusters.EndOfStream)
            {
                string strLine = srLocalPhraseClusters.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!dLocalPhraseClusters.ContainsKey(Convert.ToInt16(strsLine[0].Trim())))
                    {
                        dLocalPhraseClusters.Add(Convert.ToInt16(strsLine[0].Trim()),
                            new Dictionary<int, string>());
                    }

                    dLocalPhraseClusters[Convert.ToInt16(strsLine[0].Trim())].Add(
                        Convert.ToInt16(strsLine[1].Trim()), strsLine[2].Trim());
                }
            }

            srLocalPhraseClusters.Close();
        }

        public string StrongsToPOS(ref KJVData data, ref BLBLexiconData blbData, string strStrongsSequence) //input strStrongsSequence must begin with "H" or "G"
        {
            string strReturn = "";
            foreach (string strSequenceItem in strStrongsSequence.Split('-'))
            {
                int intNumber = Convert.ToInt32(strSequenceItem[1..]);
                if (strStrongsSequence.ToUpper().StartsWith("H"))
                {
                    strReturn += blbData.dBLBHebrewLexiconEntries[intNumber].strPOS + "-";
                }
                else
                {
                    strReturn = strStrongsSequence.ToUpper().StartsWith("G")
                        ? blbData.dBLBGreekLexiconEntries[intNumber].strPOS + "-"
                        : throw new Exception("StrongsToPOS received a bad input: strStrongsSequence");
                }
            }

            strReturn = strReturn.TrimEnd('-');

            return strReturn;
        }

        internal void WriteSSCountBySS(ref KJVData data)
        {
            StreamWriter swSSBySS = new(@"Data\StrongsSequencesBySS.csv");
            int intRowCounter = 0;

            swSSBySS.WriteLine("RowID ^ StrongsSequence ^ Count");

            foreach (string strSS in data.svo.dSSCountsOrderedBySS.Keys.OrderBy(a => a))
            {
                intRowCounter++;

                swSSBySS.WriteLine(intRowCounter.ToString() + " ^ " + strSS + " ^ " + data.svo.dSSCountsOrderedBySS[strSS].ToString());
            }

            swSSBySS.Close();
        }

        internal void WriteSSCountByCount(ref KJVData data)
        {
            StreamWriter swSSByCount = new(@"Data\StrongsSequencesByCount.csv");
            int intRowCounter = 0;

            swSSByCount.WriteLine("RowID ^ Count ^ StrongsSequence");

            intRowCounter = 0;
            foreach (string strSS in data.svo.dSSCountsOrderedByCount.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                intRowCounter++;

                swSSByCount.WriteLine(intRowCounter.ToString() + " ^ " + data.svo.dSSCountsOrderedByCount[strSS].ToString() + " ^ " + strSS);
            }

            swSSByCount.Close();
        }
    }
}

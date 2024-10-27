namespace KJVStrongs
{
    public class PhrasalConcordance
    {
        public void CreatePhrasalConcordance(ref KJVData kjvdata)
        {
            //intChainLength determines how many successive phrases will be considered for a chained phrasal concordance
            // default of 1 means to output a regular phrasal concordance

            foreach (Verse v in kjvdata.dVerses.Values)
            {
                foreach (Phrase p in v.dPhrases.Values)
                {
                    string strPhrase = kjvdata.rgxCleanPhrase.Replace(p.strPhraseText, "").Trim().ToLower();
                    string strReference = v.strBookName + " " + v.intChapterNumber.ToString() + ":" + v.intVerseNumber.ToString() + "." + p.intPhraseID.ToString();

                    if (!kjvdata.dPhrasalConcordance.ContainsKey(strPhrase))
                    {
                        kjvdata.dPhrasalConcordance.Add(strPhrase, new List<string>());
                    }

                    kjvdata.dPhrasalConcordance[strPhrase].Add(strReference);
                }
            }
        }

        public void WritePhrasalConcordance(ref KJVData kjvdata, ref StreamWriter swPhrasalConcordance)
        {
            IOrderedEnumerable<KeyValuePair<string, List<string>>> getConcordanceByPhraseCount = kjvdata.dPhrasalConcordance.OrderByDescending(a => a.Value.Count());

            swPhrasalConcordance.WriteLine("Phrase ^ Reference ^ Count");

            foreach (KeyValuePair<string, List<string>> phrase in getConcordanceByPhraseCount)
            {
                foreach (string strRef in phrase.Value)
                {
                    swPhrasalConcordance.WriteLine(phrase.Key + " ^ " + strRef + " ^ " + phrase.Value.Count.ToString());
                }
            }

            swPhrasalConcordance.Close();
        }

        public void WriteAllPhrases(ref KJVData kjvdata, ref StreamWriter swPhrases)
        {
            foreach (string strPhrase in kjvdata.dPhrasalConcordance.Keys.OrderBy(a => a))
            {
                swPhrases.WriteLine(strPhrase);
            }

            swPhrases.Close();
        }

        public void WritePhrasesByCount(ref KJVData kjvdata, ref StreamWriter swEnglishPhraseCountsByCount)
        {
            int intRowCounter = 0;
            //this copy has to be made because referenced variables cannot be used in the lambda
            Dictionary<string, List<string>> dPhrasalConcordanceLocal = kjvdata.dPhrasalConcordance;

            swEnglishPhraseCountsByCount.WriteLine("RowID ^ Count ^ Phrase");

            foreach (string strPhrase in kjvdata.dPhrasalConcordance.OrderBy(a => dPhrasalConcordanceLocal[a.Key].Count).Select(a => a.Key))
            {
                intRowCounter++;

                swEnglishPhraseCountsByCount.WriteLine(intRowCounter.ToString() + " ^ " + kjvdata.dPhrasalConcordance[strPhrase].Count().ToString() + " ^ " + strPhrase);
            }

            swEnglishPhraseCountsByCount.Close();

        }

        public void WritePhrasesByPhrase(ref KJVData kjvdata, ref StreamWriter swEnglishPhraseCountsByPhrase)
        {
            int intRowCounter = 0;

            IOrderedEnumerable<string> phrases =
                from phrase in kjvdata.dPhrasalConcordance.Keys
                orderby phrase
                select phrase;

            swEnglishPhraseCountsByPhrase.WriteLine("RowID ^ Phrase ^ Count");

            intRowCounter = 0;
            foreach (string strPhrase in phrases)
            {
                intRowCounter++;

                swEnglishPhraseCountsByPhrase.WriteLine(intRowCounter.ToString() + " ^ " + strPhrase + " ^ " + kjvdata.dPhrasalConcordance[strPhrase].Count().ToString());
            }

            swEnglishPhraseCountsByPhrase.Close();
        }

        public void WriteEnglishPhrasesWithReferences(ref KJVData kjvdata, ref StreamWriter swEnglishPhraseWithReferences)
        {

            //WARNING: VERY LARGE FILES - 10s of GBs!

            string strReferencesBuilder = "";
            int intRowCounter = 0;
            //this copy has to be made because referenced variables cannot be used in the lambda
            Dictionary<string, List<string>> dPhrasalConcordanceLocal = kjvdata.dPhrasalConcordance;

            swEnglishPhraseWithReferences.WriteLine("RowID ^ Phrase ^ AmpersandSeparatedReferences");

            foreach (string strPhrase in kjvdata.dPhrasalConcordance.OrderBy(a => dPhrasalConcordanceLocal[a.Key].Count).Select(a => a.Key))
            {
                intRowCounter++;

                swEnglishPhraseWithReferences.Write(intRowCounter.ToString() + " ^ " + strPhrase + " ^ ");

                foreach (string strReference in kjvdata.dPhrasalConcordance[strPhrase])
                {
                    strReferencesBuilder += strReference + "&";
                }

                strReferencesBuilder = strReferencesBuilder.TrimEnd('&');

                swEnglishPhraseWithReferences.WriteLine(strReferencesBuilder);
            }

            swEnglishPhraseWithReferences.Close();
        }

        public void Write(ref KJVData kjvdata, ref StreamWriter swPhrasalConcordance, ref StreamWriter swPhrases,
            ref StreamWriter swEnglishPhraseCountsByCount, ref StreamWriter swEnglishPhraseCountsByPhrase,
            ref StreamWriter swEnglishPhraseWithReferences, bool bWriteEnglishPhraseWithReferences)
        {
            WritePhrasalConcordance(ref kjvdata, ref swPhrasalConcordance);
            WriteAllPhrases(ref kjvdata, ref swPhrases);
            WritePhrasesByCount(ref kjvdata, ref swEnglishPhraseCountsByCount);
            WritePhrasesByPhrase(ref kjvdata, ref swEnglishPhraseCountsByPhrase);

            //WARNING: VERY LARGE FILES - 10s of GBs!
            if (bWriteEnglishPhraseWithReferences == true)
            {
                WriteEnglishPhrasesWithReferences(ref kjvdata, ref swEnglishPhraseWithReferences);
            }
        }

        public void Read(ref KJVData kjvdata, ref StreamReader srPhrasalConcordance)
        {
            bool bSeenHeader = false; //skips header line

            while (!srPhrasalConcordance.EndOfStream)
            {
                string strLine = srPhrasalConcordance.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!kjvdata.dPhrasalConcordance.ContainsKey(strsLine[0].Trim()))
                    {
                        kjvdata.dPhrasalConcordance.Add(strsLine[0].Trim(), new List<string>());
                    }

                    kjvdata.dPhrasalConcordance[strsLine[0].Trim()].Add(strsLine[1].Trim());
                }
            }
        }
    }
}

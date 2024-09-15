namespace KJVStrongs
{
    public class StrongsPhrasalConcordance
    {
        //REFACTOR: Do these closeness scores need to be in a different class?
        public SyllableClosenessScore syllableScoreHebrew = new();
        public SyllableClosenessScore syllableScoreGreek = new();

        public void SyllableCloseness_Greek(ref StreamWriter swSyllableScoreGreek, ref StreamReader srSyllableScoreGreek)
        {
            syllableScoreGreek.Parse(ref srSyllableScoreGreek, false);
            syllableScoreGreek.Examine(ref swSyllableScoreGreek, false);
        }

        public void SyllableCloseness_Hebrew(ref StreamWriter swSyllableScoreHebrew, ref StreamReader srSyllableScoreHebrew)
        {
            syllableScoreHebrew.Parse(ref srSyllableScoreHebrew, true);
            syllableScoreHebrew.Examine(ref swSyllableScoreHebrew, true);
        }

        //depends on VectorizeKJVStrongs for intMaximumHebrewStrongsNumber
        //this is unnecessary
        //finding whether we're dealing with extended strongs here would eliminate it
        public void CreateStrongsPhrasalConcordance(ref KJVData data)
        {
            int intLastVerseIDInLastBook = 0;
            int intCurrentVerseID = 0;
            bool bOT = true;

            foreach (string strBookName in data.dLastVerseIDInBook.OrderBy(a => a.Value).Select(a => a.Key))
            {
                int intLastVerseInBook = data.dLastVerseIDInBook[strBookName];

                if (strBookName == "matthew")
                {
                    bOT = false;
                }

                foreach (Verse v in data.dVerses.OrderBy(a => a.Key).Where(a => a.Key <= intLastVerseInBook && a.Key > intLastVerseIDInLastBook).Select(a => a.Value))
                {
                    intCurrentVerseID = v.intVerseID;

                    if (v.dPhrases.Count > 1) //support for at least 2 phrases in the verse
                    {
                        Dictionary<int, Dictionary<string, int>> dOrderedStrongsSequences = new(); //D<OrderID, D<Phrase, StrongsNumber>>
                        int intSSID = 0;

                        foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                        {
                            //flatten all strongs sequences with or without dashes
                            foreach (StrongsSequence ss in p.dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Value))
                            {
                                foreach (string strStrongsNumberPart in ss.strStrongsNumber.Split('-'))
                                {
                                    intSSID++;

                                    dOrderedStrongsSequences.Add(intSSID, new Dictionary<string, int>());

                                    if (bOT == true)
                                    {
                                        dOrderedStrongsSequences[intSSID].Add(
                                            data.rgxCleanPhrase.Replace(p.strPhraseText, "").Trim().ToLower(),
                                            Convert.ToInt32(strStrongsNumberPart));
                                    }
                                    else
                                    {
                                        dOrderedStrongsSequences[intSSID].Add(
                                                data.rgxCleanPhrase.Replace(p.strPhraseText, "").Trim().ToLower(),
                                                Convert.ToInt32(strStrongsNumberPart) + data.intMaximumHebrewStrongsNumber); //intMaximumHebrewStrongsNumber: Normalize Greek and Hebrew Strongs Numbers
                                    }
                                }
                            }
                        }

                        //Fill dStrongsPhrasalConcordanceEnglish
                        foreach (int intSSIDCounter in dOrderedStrongsSequences.OrderBy(a => a.Key).Select(a => a.Key))
                        {
                            string strPhrase = data.rgxCleanPhrase.Replace(dOrderedStrongsSequences[intSSIDCounter].First().Key, "").Trim().ToLower();
                            string strPhraseUncleaned = dOrderedStrongsSequences[intSSIDCounter].First().Key.ToLower();
                            int intStrongsNumber = 0;

                            intStrongsNumber = dOrderedStrongsSequences[intSSIDCounter].First().Value;

                            if (intSSIDCounter < dOrderedStrongsSequences.Count) //Add the first word to the data structure only if this isn't the last word in the Verse
                            {
                                //Don't count phrases across sentential boundaries
                                if (!strPhraseUncleaned.EndsWith(';') && !strPhraseUncleaned.EndsWith(':') &&
                                    !strPhraseUncleaned.EndsWith('.') && !strPhraseUncleaned.EndsWith('?') &&
                                    !strPhraseUncleaned.EndsWith('!'))
                                {
                                    if (!data.dStrongsPhrasalConcordanceEnglish.ContainsKey(strPhrase))
                                    {
                                        data.dStrongsPhrasalConcordanceEnglish.Add(strPhrase, new Dictionary<string, int>());
                                    }

                                    if (!data.dStrongsPhrasalConcordanceStrongs.ContainsKey(intStrongsNumber))
                                    {
                                        data.dStrongsPhrasalConcordanceStrongs.Add(intStrongsNumber, new Dictionary<int, int>());
                                    }
                                }
                            }

                            if (intSSIDCounter > 1)
                            {
                                string strPhraseBack = data.rgxCleanPhrase.Replace(dOrderedStrongsSequences[intSSIDCounter - 1].First().Key, "").Trim().ToLower();
                                string strPhraseBackUncleaned = dOrderedStrongsSequences[intSSIDCounter - 1].First().Key.ToLower();
                                int intStrongsNumberBack = 0;

                                intStrongsNumberBack = dOrderedStrongsSequences[intSSIDCounter - 1].First().Value;

                                //Don't count phrases across sentential boundaries
                                if (!strPhraseBackUncleaned.EndsWith(';') && !strPhraseBackUncleaned.EndsWith(':') &&
                                    !strPhraseBackUncleaned.EndsWith('.') && !strPhraseBackUncleaned.EndsWith('?') &&
                                    !strPhraseBackUncleaned.EndsWith('!'))
                                {
                                    if (!data.dStrongsPhrasalConcordanceEnglish[strPhraseBack].ContainsKey(strPhrase))
                                    {
                                        data.dStrongsPhrasalConcordanceEnglish[strPhraseBack].Add(strPhrase, 0);
                                    }

                                    if (!data.dStrongsPhrasalConcordanceStrongs[intStrongsNumberBack].ContainsKey(intStrongsNumber))
                                    {
                                        data.dStrongsPhrasalConcordanceStrongs[intStrongsNumberBack].Add(
                                           intStrongsNumber, 0);
                                    }

                                    data.dStrongsPhrasalConcordanceEnglish[strPhraseBack][strPhrase]++;
                                    data.dStrongsPhrasalConcordanceStrongs[intStrongsNumberBack][intStrongsNumber]++;
                                }
                            }
                        }


                    }
                }

                intLastVerseIDInLastBook = intCurrentVerseID;
            }
        }

        public void Write(ref KJVData data, ref StreamWriter swStrongsPhrasalConcordanceEnglish,
            ref StreamWriter swStrongsPhrasalConcordanceStrongs)
        {
            //
            //StrongsPhrasalConcordance
            //

            swStrongsPhrasalConcordanceEnglish.WriteLine("PhraseOne ^ PhraseTwo ^ Count");

            foreach (string strPhraseOne in data.dStrongsPhrasalConcordanceEnglish.OrderByDescending(a => a.Value.Count).Select(a => a.Key))
            {
                foreach (string strPhraseTwo in data.dStrongsPhrasalConcordanceEnglish[strPhraseOne].OrderByDescending(a => a.Value).Select(a => a.Key))
                {
                    swStrongsPhrasalConcordanceEnglish.WriteLine(strPhraseOne + " ^ " + strPhraseTwo + " ^ " +
                        data.dStrongsPhrasalConcordanceEnglish[strPhraseOne][strPhraseTwo].ToString());
                }
            }

            swStrongsPhrasalConcordanceEnglish.Close();

            swStrongsPhrasalConcordanceStrongs.WriteLine("StrongsOne ^ StrongsTwo ^ Count");

            foreach (int intStrongsOne in data.dStrongsPhrasalConcordanceStrongs.OrderByDescending(a => a.Value.Count).Select(a => a.Key))
            {
                foreach (int intStrongsTwo in data.dStrongsPhrasalConcordanceStrongs[intStrongsOne].OrderByDescending(a => a.Value).Select(a => a.Key))
                {
                    swStrongsPhrasalConcordanceStrongs.WriteLine(intStrongsOne + " ^ " + intStrongsTwo + " ^ " +
                        data.dStrongsPhrasalConcordanceStrongs[intStrongsOne][intStrongsTwo].ToString());
                }
            }

            swStrongsPhrasalConcordanceStrongs.Close();
        }

        public void Read(ref KJVData data, ref StreamReader srStrongsPhrasalConcordanceEnglish,
            ref StreamReader srStrongsPhrasalConcordanceStrongs)
        {
            bool bSeenHeader = false;
            while (!srStrongsPhrasalConcordanceEnglish.EndOfStream)
            {
                string strLine = srStrongsPhrasalConcordanceEnglish.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!data.dStrongsPhrasalConcordanceEnglish.ContainsKey(strsLine[0].Trim()))
                    {
                        data.dStrongsPhrasalConcordanceEnglish.Add(strsLine[0].Trim(), new Dictionary<string, int>());
                    }

                    data.dStrongsPhrasalConcordanceEnglish[strsLine[0].Trim()]
                        .Add(strsLine[1].Trim(), Convert.ToInt16(strsLine[2].Trim()));
                }
            }

            bSeenHeader = false;

            while (!srStrongsPhrasalConcordanceStrongs.EndOfStream)
            {
                string strLine = srStrongsPhrasalConcordanceStrongs.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!data.dStrongsPhrasalConcordanceStrongs.ContainsKey(Convert.ToInt16(strsLine[0].Trim())))
                    {
                        data.dStrongsPhrasalConcordanceStrongs.Add(
                            Convert.ToInt16(strsLine[0].Trim()), new Dictionary<int, int>());
                    }

                    data.dStrongsPhrasalConcordanceStrongs[Convert.ToInt16(strsLine[0].Trim())].Add(
                        Convert.ToInt16(strsLine[1].Trim()), Convert.ToInt16(strsLine[2].Trim()));
                }
            }
        }
    }
}

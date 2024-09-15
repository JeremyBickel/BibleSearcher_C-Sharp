namespace KJVStrongs
{
    public class Phrase
    {
        public int intPhraseID = 0;
        public string strPhraseText = "";
        public Dictionary<int, StrongsSequence> dStrongsSequences = new(); //D<StrongsSequenceID, StrongsSequence>
    }

    public class IntersectingPhrases
    {
        public Dictionary<int, Dictionary<int, Dictionary<string,
            Dictionary<double, Dictionary<string, string>>>>> dIntersectingPhrases = new();

        //creates AND writes
        public void CreateIntersectingPhrases(ref Dictionary<int, Verse> dVerses,
            int intVerseIDMinimum = -1, int intVerseIDMaximum = -1)
        {
            int intVerseIDMultiplier = 0;
            bool bContinue = true;

            Dictionary<string, int> dPhraseTotalCounts = new(); //D<phrase, count throughout Bible>
            Dictionary<int, List<string>> dVersePhrases = new();
            Dictionary<int, List<string>> dPhrasalIntersections = new();
            Dictionary<int, Dictionary<string, int>> dPhrasalIntersectionCounts = new();

            if (intVerseIDMinimum == -1)
            {
                intVerseIDMinimum = 1;
            }

            if (intVerseIDMaximum == -1)
            {
                intVerseIDMaximum = dVerses.Keys.Max();
            }

            for (int intVerseID = intVerseIDMinimum; intVerseID <= intVerseIDMaximum; intVerseID++)
            {
                dVersePhrases.Add(intVerseID, new List<string>());

                foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys)
                {
                    string strPhrase = dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText.Trim().ToLower();

                    dVersePhrases[intVerseID].Add(strPhrase);

                    if (!dPhraseTotalCounts.ContainsKey(strPhrase))
                    {
                        dPhraseTotalCounts.Add(strPhrase, 0);
                    }

                    dPhraseTotalCounts[strPhrase]++;
                }
            }

            while (bContinue == true)
            {
                StreamWriter swIntersectingPhrases = new(
                        @"Data\Processed\IntersectingPhrases\IntersectingPhrases_" + (intVerseIDMultiplier + 1).ToString() + ".csv");

                swIntersectingPhrases.WriteLine("VerseID1 ^ VerseID2 ^ Phrase ^ Relatedness ^ Verse1Text ^ Verse2Text");

                dPhrasalIntersections.Clear();
                dPhrasalIntersectionCounts.Clear();

                for (int intVerseID = 1 + (100 * intVerseIDMultiplier); intVerseID <= 100 + (100 * intVerseIDMultiplier); intVerseID++)//foreach (int intVerseID in dVerses.Keys)
                {
                    dPhrasalIntersectionCounts.Add(intVerseID, new Dictionary<string, int>());

                    foreach (string strPhrase in dVersePhrases[intVerseID])
                    {
                        if (!dPhrasalIntersectionCounts[intVerseID].ContainsKey(strPhrase))
                        {
                            dPhrasalIntersectionCounts[intVerseID].Add(strPhrase, 0);
                        }

                        dPhrasalIntersectionCounts[intVerseID][strPhrase]++;
                    }

                    foreach (int intVerseIDCompare in dVerses.Keys)
                    {
                        if (intVerseID != intVerseIDCompare)
                        {
                            List<string> lIntersectingPhrases = dVersePhrases[intVerseID]
                                .Intersect<string>(dVersePhrases[intVerseIDCompare]).ToList();

                            if (lIntersectingPhrases.Count > 0)
                            {
                                foreach (string strIntersectingPhrase in lIntersectingPhrases)
                                {
                                    int intPhraseCount1 = dVerses[intVerseID].dPhraseCounts[strIntersectingPhrase];
                                    int intPhraseCount2 = dVerses[intVerseIDCompare].dPhraseCounts[strIntersectingPhrase];

                                    swIntersectingPhrases.WriteLine(intVerseID.ToString() + " ^ " +
                                        intVerseIDCompare.ToString() + " ^ " + strIntersectingPhrase + " ^ " +
                                        ((double)((intPhraseCount1 + intPhraseCount2) /
                                        (double)dPhraseTotalCounts[strIntersectingPhrase])).ToString() + " ^ " +
                                        dVerses[intVerseID].strText + " ^ " + dVerses[intVerseIDCompare].strText);
                                }
                            }
                        }
                    }

                    if (intVerseID == intVerseIDMaximum)
                    {
                        StreamWriter swMinMax = new(@"Data\Processed\IntersectingPhrases\MinMax.csv");

                        bContinue = false;
                        swMinMax.WriteLine(intVerseIDMinimum.ToString() + " ^ " + intVerseIDMaximum.ToString());
                        swMinMax.Close();

                        break;
                    }
                }

                swIntersectingPhrases.Close();
                intVerseIDMultiplier++;
            }
        }

        public void ReadIntersectingPhrases()
        {
            bool bSeenHeading = false;

            foreach (string strFilename in Directory.EnumerateFiles(
                    @"Data\Processed\IntersectingPhrases", @"IntersectingPhrases_*", SearchOption.TopDirectoryOnly))
            {
                StreamReader srIntersectingPhrases = new(strFilename);

                while (!srIntersectingPhrases.EndOfStream)
                {
                    string strLine = srIntersectingPhrases.ReadLine();

                    if (bSeenHeading == false)
                    {
                        bSeenHeading = true;
                    }
                    else
                    {
                        string[] strsLine = strLine.Split('^');

                        if (!dIntersectingPhrases.ContainsKey(Convert.ToInt16(strsLine[0].Trim())))
                        {
                            dIntersectingPhrases.Add(Convert.ToInt16(strsLine[0].Trim()),
                                new Dictionary<int, Dictionary<string, Dictionary<double,
                                Dictionary<string, string>>>>());
                        }

                        if (!dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                            .ContainsKey(Convert.ToInt16(strsLine[1].Trim())))
                        {
                            dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())].Add(
                                Convert.ToInt16(strsLine[1].Trim()), new Dictionary<string,
                                Dictionary<double, Dictionary<string, string>>>());
                        }

                        if (!dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                            [Convert.ToInt16(strsLine[1].Trim())].ContainsKey(strsLine[2].Trim()))
                        {
                            dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                                [Convert.ToInt16(strsLine[1].Trim())].Add(strsLine[2].Trim(),
                                new Dictionary<double, Dictionary<string, string>>());
                        }

                        if (!dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                            [Convert.ToInt16(strsLine[1].Trim())][strsLine[2].Trim()]
                            .ContainsKey(Convert.ToDouble(strsLine[3].Trim())))
                        {
                            dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                            [Convert.ToInt16(strsLine[1].Trim())][strsLine[2].Trim()]
                            .Add(Convert.ToDouble(strsLine[3].Trim()), new Dictionary<string, string>());
                        }

                        if (!dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                            [Convert.ToInt16(strsLine[1].Trim())][strsLine[2].Trim()]
                            [Convert.ToDouble(strsLine[3].Trim())].ContainsKey(strsLine[4].Trim()))
                        {
                            dIntersectingPhrases[Convert.ToInt16(strsLine[0].Trim())]
                            [Convert.ToInt16(strsLine[1].Trim())][strsLine[2].Trim()]
                            [Convert.ToDouble(strsLine[3].Trim())].Add(strsLine[4].Trim(), strsLine[5].Trim());
                        }
                    }
                }

                srIntersectingPhrases.Close();
            }
        }
    }
}

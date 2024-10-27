namespace KJVStrongs
{
    public class Relationships
    {
        //dblRelatednessThreshold - higher is more related
        //intPhraseSignificanceThreshold - phrase counts where, to a point, lower is more significant
        //intWordSignificanceThreshold - word counts greater than this threshold are excluded
        //0.1, 1000, 5000 writes about a 1.5 gigabyte file
        //0.4, 100, 3500 writes a 4 megabyte file
        public void CreateVerseRelatednessByPhraseSimilarity(ref Dictionary<int, Verse> dVerses,
            ref Dictionary<string, int> dWordCounts, ref StreamWriter swIntersectingPhrases,
            double dblRelatednessThreshold = 0.4, int intPhraseSignificanceThreshold = 70,
            int intWordSignificanceThreshold = 3500)
        {
            Dictionary<string, int> dPhraseTotalCounts = new(); //D<phrase, count throughout Bible>
            Dictionary<int, List<string>> dVersePhrases = new(); //D<VerseID, L<phrase>>
            Dictionary<double, Dictionary<int, List<int>>> dVerseRelatednessByPhraseCount =
                new(); //D<relatedness, D<verse id 1, L<verse id 2>>>

            int intVerseIDMultiplier = 0;
            bool bContinue = true;
            int intVerseIDMax = dVerses.Keys.Max();

            //
            //PREPARE THE PHRASES
            //
            dPhraseTotalCounts.Add("", 1000000); //Make the empty string highly insignificant

            for (int intVerseID = 1; intVerseID <= intVerseIDMax; intVerseID++)
            {
                foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys)
                {
                    string strPhraseProto = dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText.Trim().ToLower();
                    string strPhrase = "";

                    //remove common words from the phrase
                    foreach (string strWord in strPhraseProto.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (strWord.Trim() != "")
                        {
                            if (dWordCounts[strWord.Trim()] < intWordSignificanceThreshold)
                            {
                                strPhrase += strWord.Trim() + " ";
                            }
                        }
                    }

                    strPhrase = strPhrase.Trim();

                    if (!dPhraseTotalCounts.ContainsKey(strPhrase))
                    {
                        dPhraseTotalCounts.Add(strPhrase, 0);
                    }

                    dPhraseTotalCounts[strPhrase]++;
                }
            }

            //
            //dVersePhrases
            //
            for (int intVerseID = 1; intVerseID <= intVerseIDMax; intVerseID++)
            {
                dVersePhrases.Add(intVerseID, new List<string>());

                foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys)
                {
                    string strPhraseProto = dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText.Trim().ToLower();
                    string strPhrase = "";

                    //remove common words from the phrase
                    foreach (string strWord in strPhraseProto.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (strWord.Trim() != "")
                        {
                            if (dWordCounts[strWord.Trim()] < intWordSignificanceThreshold)
                            {
                                strPhrase += strWord.Trim() + " ";
                            }
                        }
                    }

                    strPhrase = strPhrase.Trim();

                    //only examine significant phrases
                    if (dPhraseTotalCounts[strPhrase] <= intPhraseSignificanceThreshold)
                    {
                        dVersePhrases[intVerseID].Add(strPhrase);
                    }
                }
            }

            //
            //COMPARE THE PHRASES
            //
            while (bContinue == true)
            {
                for (int intVerseID = 1 + (100 * intVerseIDMultiplier); intVerseID <= 100 + (100 * intVerseIDMultiplier); intVerseID++)//foreach (int intVerseID in dVerses.Keys)
                {
                    foreach (int intVerseIDCompare in dVerses.Keys.Where(a => a > intVerseID)) //don't compare 2 verses in both orders (ie. 1 ^ 2 && 2 ^ 1)
                    {
                        List<string> lIntersectingPhrases = dVersePhrases[intVerseID]
                            .Intersect<string>(dVersePhrases[intVerseIDCompare]).ToList();
                        int intMaxPhraseCount = dVersePhrases[intVerseID].Count;
                        double dblRelatedness = 0.0;

                        if (intVerseID == 5 && intVerseIDCompare == 24772) //gen 1:5 && mark 14:17
                        {
                        }

                        //first assume that the first verse has more phrases, then override that count
                        //with the second verse's phrase count if it's larger
                        if (dVersePhrases[intVerseIDCompare].Count > intMaxPhraseCount)
                        {
                            intMaxPhraseCount = dVersePhrases[intVerseIDCompare].Count;
                        }

                        //the number of phrases common to both verses / the greater number of phrases in one of the verses
                        dblRelatedness = (double)(lIntersectingPhrases.Count /
                                    (double)intMaxPhraseCount);

                        if (lIntersectingPhrases.Count > 0) //don't change dVerseRelatednessByPhraseCount if there are no intersecting phrases
                        {
                            if (!dVerseRelatednessByPhraseCount.ContainsKey(dblRelatedness))
                            {
                                dVerseRelatednessByPhraseCount.Add(dblRelatedness, new Dictionary<int, List<int>>());
                            }

                            if (!dVerseRelatednessByPhraseCount[dblRelatedness].ContainsKey(intVerseID))
                            {
                                dVerseRelatednessByPhraseCount[dblRelatedness].Add(intVerseID, new List<int>());
                            }

                            dVerseRelatednessByPhraseCount[dblRelatedness][intVerseID].Add(intVerseIDCompare);
                        }
                    }

                    if (intVerseID == intVerseIDMax)
                    {
                        bContinue = false;
                        break;
                    }
                }

                intVerseIDMultiplier++;
            }

            //
            //WRITE
            //
            swIntersectingPhrases.WriteLine("Relatedness ^ VerseReference1 ^ VerseReference2 ^ VerseText1 ^ VerseText2");

            foreach (double dblRelatedness in dVerseRelatednessByPhraseCount.Keys.OrderByDescending(a => a))
            {
                foreach (int intVerseID in dVerseRelatednessByPhraseCount[dblRelatedness].Keys.OrderBy(a => a))
                {
                    foreach (int intVerseIDCompare in dVerseRelatednessByPhraseCount[dblRelatedness][intVerseID])
                    {
                        if (intVerseID == 5 && intVerseIDCompare == 24772) //gen 1:5 && mark 14:17
                        {
                        }

                        //write them to file if there are any phrases and if they are related closely enough
                        if (dblRelatedness >= dblRelatednessThreshold)
                        {
                            swIntersectingPhrases.Write(
                                dblRelatedness.ToString() + " ^ " +
                                dVerses[intVerseID].strBookName + " " +
                                dVerses[intVerseID].intChapterNumber.ToString() + ":" +
                                dVerses[intVerseID].intVerseNumber.ToString() + " ^ " +
                                dVerses[intVerseIDCompare].strBookName + " " +
                                dVerses[intVerseIDCompare].intChapterNumber.ToString() + ":" +
                                dVerses[intVerseIDCompare].intVerseNumber.ToString() + " ^ ");

                            foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys.OrderBy(a => a))
                            {
                                swIntersectingPhrases.Write(
                                    dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText.Trim() + " ");
                            }

                            swIntersectingPhrases.Write(" ^ ");

                            foreach (int intPhraseIDCompare in dVerses[intVerseIDCompare].dPhrases.Keys.OrderBy(a => a))
                            {
                                swIntersectingPhrases.Write(
                                    dVerses[intVerseIDCompare].dPhrases[intPhraseIDCompare].strPhraseText.Trim() + " ");
                            }

                            swIntersectingPhrases.WriteLine();
                        }
                    }
                }
            }

            swIntersectingPhrases.Close();
        }


    }
}

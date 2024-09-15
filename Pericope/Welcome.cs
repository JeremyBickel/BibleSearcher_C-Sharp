using KJVStrongs;

namespace Pericope
{
    public class Welcome
    {
        public PericopeData pericopedata = new();

        public Welcome(ref StreamReader srWholeBiblePericope,
            ref StreamReader srWikipediaActs, ref StreamWriter swPericopes,
                    ref StreamWriter swHeadings, ref StreamWriter swWordsPericopeHeadingsCount,
                    ref StreamWriter swHeadingsPositionWordID, ref StreamWriter swVerseIDHeadings,
                    ref StreamWriter swHeadingVerseIDs, ref KJVData kjvdata, ref StreamWriter swTSK,
                    ref StreamWriter swIntersectingPhrases, ref StreamWriter swNewPericopeWordIDs,
                    ref StreamReader srReferences, ref StreamWriter swPericopeRanges, 
                    string strTSKDirectory, string strPericopeDivisionDataDirectory)
        {
            Create(ref srWholeBiblePericope, ref srWikipediaActs, ref kjvdata, ref swTSK,
                ref swIntersectingPhrases, ref swNewPericopeWordIDs, ref srReferences, ref swPericopeRanges, 
                strTSKDirectory, strPericopeDivisionDataDirectory);

            Write(ref swPericopes, ref swHeadings, ref swWordsPericopeHeadingsCount,
                ref swHeadingsPositionWordID, ref swVerseIDHeadings, ref swHeadingVerseIDs);
        }

        public Welcome(ref StreamReader srPericopes, ref StreamReader srHeadings,
                    ref StreamReader srWordsPericopeHeadingsCount, ref StreamReader srHeadingsPositionWordID,
                    ref StreamReader srVerseIDHeadings, ref StreamReader srHeadingVerseIDs, ref StreamReader srTSK,
                    ref StreamReader srPericopeRanges)
        {
            Read(ref srPericopes, ref srHeadings,
                    ref srWordsPericopeHeadingsCount, ref srHeadingsPositionWordID,
                    ref srVerseIDHeadings, ref srHeadingVerseIDs, ref srTSK, ref srPericopeRanges);
        }

        public void Create(ref StreamReader srWholeBiblePericope,
            ref StreamReader srWikipediaActs, ref KJVData kjvdata, ref StreamWriter swTSK,
            ref StreamWriter swIntersectingPhrases, ref StreamWriter swNewPericopeWordIDs, 
            ref StreamReader srReferences, ref StreamWriter swPericopeRanges, string strTSKDirectory,
            string strPericopeDivisionDataDirectory)
        {
            pericopedata.psHeadings.ReadBereanData(ref srWholeBiblePericope);
            pericopedata.psHeadings.ReadWikipedia_Acts(ref srWikipediaActs);
            //pericopedata.psHeadings.ReadTSK(ref pericopedata.tsk.dTSKBookNameAbbreviationNormalizations);
            //pericopedata.tsk = new(ref swTSK, strTSKDirectory); //create AND write
            pericopedata.psHeadings.CreateTSK(ref swTSK, strTSKDirectory);

            pericopedata.psHeadings.FillHeadingsList(); //all headings psHeadings.lHeadings
            pericopedata.psHeadings.FillPericopeRanges(ref srReferences, ref swPericopeRanges);

            pericopedata.dWordsPericopeHeadingsCount = pericopedata.psHeadings.GetWordsPericopeHeadingsCount();

            //pericopedata.psHeadings.GetPericopeInfoObjectsFromHeadings(
            //    ref pericopedata.psHeadings.lHeadings);

            //data.psHeadings.CreateIntersectingPhrases(ref dVerses);

            //Add WordIDs to dataobjects.kjvs.kjvdata.dWordIDs for each new word in the pericope headings
            //Make sure to write the new WordIDs (through KJVStrongs class?)
            pericopedata.psHeadings.CreateNewPericopeWordIDs(ref kjvdata.dWordIDs, ref swNewPericopeWordIDs);

            pericopedata.dHeadingsPositionWordID = pericopedata.psHeadings.GetHeadingsPositionWordIDs(ref kjvdata.dWordIDs,
                pericopedata.psHeadings.lHeadings);

            //pericopedata.dPericopesLastVerse = pericopedata.psHeadings.FindLastVerseOfEachPericopeObject();

            pericopedata.dVerseIDHeadings = pericopedata.psHeadings.GetAllPericopeHeadingsFromAllVerseIDs(ref kjvdata.dVerses);
            pericopedata.dHeadingVerseIDs = pericopedata.psHeadings.GetAllVerseIDsFromAllPericopeHeadings(ref pericopedata.dVerseIDHeadings);

            pericopedata.psHeadings.CreatePericopeDivisionFiles(ref kjvdata.dVersesWithoutStrongs, strPericopeDivisionDataDirectory);
            kjvdata.relationships.CreateVerseRelatednessByPhraseSimilarity(
                ref kjvdata.dVerses, ref kjvdata.dWordCounts, ref swIntersectingPhrases, 0.4, 100, 3500);
            //kjvdata.relationships.Write()

        }

        public void Read(ref StreamReader srPericopes, ref StreamReader srHeadings,
                    ref StreamReader srWordsPericopeHeadingsCount, ref StreamReader srHeadingsPositionWordID,
                    ref StreamReader srVerseIDHeadings, ref StreamReader srHeadingVerseIDs, ref StreamReader srTSK,
                    ref StreamReader srPericopeRanges)
        {
            bool bSeenHeader = false;
            int intLastPericopeID = 0;

            //
            //dPericopes
            //
            //"Book Name ^ Start Chapter ^ Start Verse ^ End Chapter" +
            //" ^ End Verse ^ Structure ^ Pericope 1 ^ Pericope 2 ^ Pericope 3 ^ Pericope 4"
            pericopedata.psHeadings.dPericopes.Clear();
            
            while (!srPericopes.EndOfStream)
            {
                PericopeInfo pi = new();
                string strLine = srPericopes.ReadLine();
                string[] strsLine = strLine.Split('^');

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    pi.strBookName = strsLine[0];
                    pi.intStartChapterNumber = Convert.ToInt16(strsLine[1]);
                    pi.intStartVerseNumber = Convert.ToInt16(strsLine[2]);
                    pi.intEndChapterNumber = Convert.ToInt16(strsLine[3]);
                    pi.intEndVerseNumber = Convert.ToInt16(strsLine[4]);
                    pi.strStructure = strsLine[5];
                    pi.strPericope1 = strsLine[6];
                    pi.strPericope2 = strsLine[7];
                    pi.strPericope3 = strsLine[8];
                    pi.strPericope4 = strsLine[9];

                    //Pericope 1
                    if (!pericopedata.psHeadings.dPericopes.ContainsKey(strsLine[6]))
                    {
                        pericopedata.psHeadings.dPericopes.Add(strsLine[6], new List<PericopeInfo>());
                    }

                    if (!pericopedata.psHeadings.dPericopes[strsLine[6]].Contains(pi))
                    {
                        pericopedata.psHeadings.dPericopes[strsLine[6]].Add(pi);
                    }

                    //Pericope 2
                    if (!pericopedata.psHeadings.dPericopes.ContainsKey(strsLine[7]))
                    {
                        pericopedata.psHeadings.dPericopes.Add(strsLine[7], new List<PericopeInfo>());
                    }

                    if (!pericopedata.psHeadings.dPericopes[strsLine[7]].Contains(pi))
                    {
                        pericopedata.psHeadings.dPericopes[strsLine[7]].Add(pi);
                    }

                    //Pericope 3
                    if (!pericopedata.psHeadings.dPericopes.ContainsKey(strsLine[8]))
                    {
                        pericopedata.psHeadings.dPericopes.Add(strsLine[8], new List<PericopeInfo>());
                    }

                    if (!pericopedata.psHeadings.dPericopes[strsLine[8]].Contains(pi))
                    {
                        pericopedata.psHeadings.dPericopes[strsLine[8]].Add(pi);
                    }

                    //Pericope 4
                    if (!pericopedata.psHeadings.dPericopes.ContainsKey(strsLine[9]))
                    {
                        pericopedata.psHeadings.dPericopes.Add(strsLine[9], new List<PericopeInfo>());
                    }

                    if (!pericopedata.psHeadings.dPericopes[strsLine[9]].Contains(pi))
                    {
                        pericopedata.psHeadings.dPericopes[strsLine[9]].Add(pi);
                    }
                }
            }

            srPericopes.Close();

            //
            //lHeadings
            //
            pericopedata.psHeadings.lHeadings.Clear();
            bSeenHeader = false;

            while (!srHeadings.EndOfStream)
            {
                string strLine = srHeadings.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    pericopedata.psHeadings.lHeadings.Add(strLine.Trim());
                }
            }

            srHeadings.Close();

            //
            //dWordsPericopeHeadingsCount
            //
            //Word ^ Heading ^ Count of Word in Heading
            pericopedata.dWordsPericopeHeadingsCount.Clear();
            bSeenHeader = false;

            while (!srWordsPericopeHeadingsCount.EndOfStream)
            {
                string strLine = srWordsPericopeHeadingsCount.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!pericopedata.dWordsPericopeHeadingsCount.ContainsKey(strsLine[0]))
                    {
                        pericopedata.dWordsPericopeHeadingsCount.Add(
                            strsLine[0], new Dictionary<string, int>());
                    }

                    if (!pericopedata.dWordsPericopeHeadingsCount[strsLine[0]].ContainsKey(
                        strsLine[1]))
                    {
                        pericopedata.dWordsPericopeHeadingsCount[strsLine[0]].Add(
                            strsLine[1], Convert.ToInt16(strsLine[2]));
                    }

                    pericopedata.dWordsPericopeHeadingsCount[strsLine[0]][strsLine[1]] = Convert.ToInt16(strsLine[2]);
                }
            }

            srWordsPericopeHeadingsCount.Close();

            //
            //dHeadingsPositionWordID
            //
            //Heading ^ Word Position ^ Word ID
            pericopedata.dHeadingsPositionWordID.Clear();
            bSeenHeader = false;

            while (!srHeadingsPositionWordID.EndOfStream)
            {
                string strLine = srHeadingsPositionWordID.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!pericopedata.dHeadingsPositionWordID.ContainsKey(strsLine[0]))
                    {
                        pericopedata.dHeadingsPositionWordID.Add(strsLine[0], new Dictionary<int, int>());
                    }

                    if (!pericopedata.dHeadingsPositionWordID[strsLine[0]].ContainsKey(
                        Convert.ToInt16(strsLine[1])))
                    {
                        pericopedata.dHeadingsPositionWordID[strsLine[0]].Add(
                            Convert.ToInt16(strsLine[1]), Convert.ToInt32(strsLine[2]));
                    }

                    pericopedata.dHeadingsPositionWordID[strsLine[0]]
                        [Convert.ToInt16(strsLine[1])] = Convert.ToInt32(strsLine[2]);
                }
            }

            srHeadingsPositionWordID.Close();

            //
            //dVerseIDHeadings
            //
            //Verse ID ^ Pericope 1 ^ Pericope 2 ^ Pericope 3 ^ Pericope 4
            pericopedata.dVerseIDHeadings.Clear();
            bSeenHeader = false;

            while (!srVerseIDHeadings.EndOfStream)
            {
                string strLine = srVerseIDHeadings.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!pericopedata.dVerseIDHeadings.ContainsKey(Convert.ToInt16(strsLine[0])))
                    {
                        string[] strsPericopes = (string[])Array.CreateInstance(typeof(string), 4);

                        strsPericopes[0] = strsLine[1];
                        strsPericopes[1] = strsLine[2];
                        strsPericopes[2] = strsLine[3];
                        strsPericopes[3] = strsLine[4];

                        pericopedata.dVerseIDHeadings.Add(Convert.ToInt16(strsLine[0]), strsPericopes);
                    }
                }
            }

            srVerseIDHeadings.Close();

            //
            //dHeadingVerseIDs
            //
            //Pericope ^ [Verse, ID]
            pericopedata.dHeadingVerseIDs.Clear();
            bSeenHeader = false;

            while (!srHeadingVerseIDs.EndOfStream)
            {
                string strLine = srHeadingVerseIDs.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!pericopedata.dHeadingVerseIDs.ContainsKey(strsLine[0]))
                    {
                        pericopedata.dHeadingVerseIDs.Add(strsLine[0], new List<int>());
                    }

                    pericopedata.dHeadingVerseIDs[strsLine[0]].Add(Convert.ToInt16(strsLine[1]));
                }
            }

            srHeadingVerseIDs.Close();

            //
            //Treasury of Scripture Knowledge
            //
            pericopedata.psHeadings.tsk = new TreasuryOfScriptureKnowledge.Welcome(ref srTSK);

            //
            //Pericope Ranges
            //
            pericopedata.psHeadings.dRangedVerses.Clear();
            bSeenHeader = false;

            while (!srPericopeRanges.EndOfStream)
            {
                string strLine = srPericopeRanges.ReadLine();
                string[] strsLine = strLine.Split('^', StringSplitOptions.TrimEntries);
                PericopeVerse pvCurrent = new PericopeVerse();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    if (intLastPericopeID != Convert.ToInt32(strsLine[1]))
                    {
                        pericopedata.psHeadings.dRangedVerses.Add(Convert.ToInt32(strsLine[1]), new List<PericopeVerse>());
                        intLastPericopeID = Convert.ToInt32(strsLine[1]);
                    }

                    pvCurrent.intBookNumber = Convert.ToInt16(strsLine[3]);
                    pvCurrent.intChapterNumber = Convert.ToInt16(strsLine[4]);
                    pvCurrent.intVerseNumber = Convert.ToInt16(strsLine[5]);

                    pericopedata.psHeadings.dRangedVerses[Convert.ToInt32(strsLine[1])].Add(pvCurrent);
                }
            }
        }

        public void Write(ref StreamWriter swPericopes, ref StreamWriter swHeadings, ref StreamWriter swWordsPericopeHeadingsCount,
                    ref StreamWriter swHeadingsPositionWordID, ref StreamWriter swVerseIDHeadings, ref StreamWriter swHeadingVerseIDs)
        {
            swPericopes.WriteLine(pericopedata.psHeadings.dPericopes.First().Value.First().WriteHeader());

            foreach (string strHeading in pericopedata.psHeadings.dPericopes.Keys.OrderBy(a => a))
            {
                foreach (PericopeInfo pi in pericopedata.psHeadings.dPericopes[strHeading])
                {
                    swPericopes.WriteLine(pi.WriteRecord());
                }
            }

            swPericopes.Close();

            pericopedata.psHeadings.WriteHeadings(swHeadings);

            swWordsPericopeHeadingsCount.WriteLine("Word ^ Heading ^ Count of Word in Heading");

            foreach (string strWord in pericopedata.dWordsPericopeHeadingsCount.Keys.OrderBy(a => a))
            {
                foreach (string strPericope in pericopedata.dWordsPericopeHeadingsCount[strWord].Keys.OrderBy(a => a))
                {
                    swWordsPericopeHeadingsCount.WriteLine(strWord + " ^ " + strPericope + " ^ " +
                        pericopedata.dWordsPericopeHeadingsCount[strWord][strPericope].ToString());
                }
            }

            swWordsPericopeHeadingsCount.Close();

            swHeadingsPositionWordID.WriteLine("Heading ^ Word Position ^ Word ID");

            foreach (string strHeading in pericopedata.dHeadingsPositionWordID.Keys.OrderBy(a => a))
            {
                foreach (int intPosition in pericopedata.dHeadingsPositionWordID[strHeading].Keys.OrderBy(a => a))
                {
                    swHeadingsPositionWordID.WriteLine(strHeading + " ^ " + intPosition.ToString() +
                        " ^ " + pericopedata.dHeadingsPositionWordID[strHeading][intPosition].ToString());
                }
            }

            swHeadingsPositionWordID.Close();

            swVerseIDHeadings.WriteLine("Verse ID ^ Pericope 1 ^ Pericope 2 ^ Pericope 3 ^ Pericope 4");

            foreach (int intVerseID in pericopedata.dVerseIDHeadings.Keys.OrderBy(a => a))
            {
                string[] strsHeadings = pericopedata.dVerseIDHeadings[intVerseID];

                swVerseIDHeadings.WriteLine(intVerseID.ToString() + " ^ " + strsHeadings[0] + " ^ " +
                    strsHeadings[1] + " ^ " + strsHeadings[2] + " ^ " + strsHeadings[3]);
            }

            swVerseIDHeadings.Close();

            swHeadingVerseIDs.WriteLine("Pericope ^ Verse ID");

            foreach (string strPericope in pericopedata.dHeadingVerseIDs.Keys.OrderBy(a => a))
            {
                foreach (int intVerseID in pericopedata.dHeadingVerseIDs[strPericope].OrderBy(a => a))
                {
                    swHeadingVerseIDs.WriteLine(strPericope + " ^ " + intVerseID.ToString());
                }
            }

            swHeadingVerseIDs.Close();
        }
    }

    public class PericopeData
    {
        public PericopeSections psHeadings = new(); //pericope objects
        public Dictionary<string, Dictionary<string, int>> dWordsPericopeHeadingsCount = new(); //D<Pericope Word, D<Pericope Heading, WordCount>>
        public Dictionary<string, Dictionary<int, int>> dHeadingsPositionWordID = new(); //D<Heading Text.ToLower().Trim(), D<Word Position in the Heading, WordID>>>
        public Dictionary<int, string[]> dVerseIDHeadings = new(); //D<verse id, A<heading>[0-3]>
        public Dictionary<string, List<int>> dHeadingVerseIDs = new(); //D<heading, L<verse ids>>
        //Dictionary<int, int> dPericopesLastVerse = new Dictionary<int, int>();
    }
}
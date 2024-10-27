namespace KJVSynonyms
{
    public class Welcome
    {
        public Dictionary<string, List<string>> dKJVSynonyms = new(); //D<KJVStrongs lowercase word, List<synonym>>
        public Dictionary<int, List<string>> dSynonymWordIDs = new(); //D<new WordID, List<synonymous word assigned to the WordID>>
        public Dictionary<int, int> dSynonymWordPositions = new(); //D<PositionID, new WordID>
        public Dictionary<int, int> dSynonymWordCounts = new(); //D<new WordID, count>

        public Welcome(ref Dictionary<int, List<string>> dWordSynonyms,
            ref Dictionary<string, List<int>> dAllSynonyms, ref Dictionary<int, string> dWordIDs,
            ref StreamWriter swKJVSynonyms)
        {
            CreateKJVSynonyms(ref dWordSynonyms, ref dAllSynonyms, ref dWordIDs);
            WriteKJVSynonyms(ref swKJVSynonyms);
        }

        public Welcome(ref StreamReader srKJVSynonyms)
        {
            LoadKJVSynonyms(ref srKJVSynonyms);
        }

        public void CreateKJVSynonyms(ref Dictionary<int, List<string>> dWordSynonyms,
            ref Dictionary<string, List<int>> dAllSynonyms, ref Dictionary<int, string> dWordIDs)
        {
            dKJVSynonyms ??= new();
            foreach (int intWordID in dWordIDs.Keys.OrderBy(a => a))
            {
                string strWord = dWordIDs[intWordID].ToLower().Trim();

                if (dAllSynonyms.ContainsKey(strWord)) //if kjv word is in synonym db
                {
                    foreach (int intGroupID in dAllSynonyms[strWord]) //go through each group in synonym db
                    {
                        if (dWordSynonyms[intGroupID].Contains(strWord)) //if the group contains the kjv word
                        {
                            foreach (string strSynonym in dWordSynonyms[intGroupID]) //go through every word in that synonym group
                            {
                                if (dWordIDs.ContainsValue(strSynonym)) //if the synonym is another kjv word
                                {
                                    if (strWord != strSynonym) //and IF the word and synonym are different from each other
                                    {// THEN add both word and synonym to kjvsynonyms db
                                        if (!dKJVSynonyms.ContainsKey(strWord))
                                        {
                                            dKJVSynonyms.Add(strWord, new List<string>());
                                        }

                                        if (!dKJVSynonyms.ContainsKey(strSynonym))
                                        {
                                            dKJVSynonyms.Add(strSynonym, new List<string>());
                                        }

                                        if (!dKJVSynonyms[strWord].Contains(strSynonym))
                                        {
                                            dKJVSynonyms[strWord].Add(strSynonym);
                                        }

                                        if (!dKJVSynonyms[strSynonym].Contains(strWord))
                                        {
                                            dKJVSynonyms[strSynonym].Add(strWord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        public void LoadKJVSynonyms(ref StreamReader srKJVSynonyms)
        {
            while (!srKJVSynonyms.EndOfStream)
            {
                string strLine = srKJVSynonyms.ReadLine();
                string[] strsLine = strLine.Split('^');
                string strWord = strsLine[0];

                dKJVSynonyms.Add(strWord, new List<string>());

                foreach (string strSynonym in strsLine)
                {
                    if (strSynonym != strWord)
                    {
                        dKJVSynonyms[strWord].Add(strSynonym);
                    }
                }
            }

            srKJVSynonyms.Close();
        }

        public void WriteKJVSynonyms(ref StreamWriter swKJVSynonyms)
        {
            foreach (string strWord in dKJVSynonyms.Keys.OrderBy(a => a))
            {
                swKJVSynonyms.Write(strWord);

                foreach (string strSynonym in dKJVSynonyms[strWord])
                {
                    swKJVSynonyms.Write(" ^ " + strSynonym);
                }

                swKJVSynonyms.WriteLine();
            }

            swKJVSynonyms.Close();
        }

        public void RecalculateKJVWordDataWithSynonyms(ref Dictionary<int, string> dWordIDs,
            ref Dictionary<int, string> dWordPositions, ref Dictionary<string, int> dWordCounts)
        {
            Dictionary<string, int> dWordsSeen = new(); //D<word seen in dWordIDs, new WordID>

            //WordIDs
            foreach (int intWordID in dWordIDs.Keys.OrderBy(a => a))
            {
                string strWord = dWordIDs[intWordID].ToLower().Trim();
                int intNewWordID = dSynonymWordIDs.Count() + 1;

                if (dKJVSynonyms.ContainsKey(strWord))
                {
                    if (!dWordsSeen.ContainsKey(strWord))
                    {
                        dWordsSeen.Add(strWord, intNewWordID);

                        if (!dSynonymWordIDs.ContainsKey(intNewWordID))
                        {
                            dSynonymWordIDs.Add(intNewWordID, new());
                            dSynonymWordIDs[intNewWordID].Add(strWord);
                        }

                        foreach (string strSynonym in dKJVSynonyms[strWord])
                        {
                            if (!dSynonymWordIDs[intNewWordID].Contains(strSynonym))
                            {
                                dSynonymWordIDs[intNewWordID].Add(strSynonym);
                            }

                            if (!dWordsSeen.ContainsKey(strSynonym))
                            {
                                dWordsSeen.Add(strSynonym, intNewWordID);
                            }
                        }
                    }
                }
            }

            //WordPositions
            foreach (int intPositionID in dWordPositions.Keys.OrderBy(a => a))
            {
                string strWord = dWordPositions[intPositionID].ToLower().Trim();

                if (dWordsSeen.ContainsKey(strWord))
                {
                    dSynonymWordPositions.Add(intPositionID, dWordsSeen[strWord]);
                }
            }

            //WordCounts
            foreach (string strWord in dWordCounts.Keys.OrderBy(a => a))
            {
                if (dWordsSeen.ContainsKey(strWord))
                {
                    int intNewWordID = dWordsSeen[strWord.ToLower().Trim()];

                    if (!dSynonymWordCounts.ContainsKey(intNewWordID))
                    {
                        dSynonymWordCounts.Add(intNewWordID, 0);
                    }

                    dSynonymWordCounts[intNewWordID] += dWordCounts[strWord.ToLower().Trim()];
                }
            }
        }
    }
}
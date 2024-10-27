using System.Text.RegularExpressions;

namespace KJVStrongs
{
    public class Word
    {
        private readonly Regex rgxCleanedWord = new(@"[^A-Za-z0-9\-\' ]{1,}");

        public Word() { }

        public void CreateWordIDs(ref KJVData kjvdata, ref StreamWriter swWordIDs)
        {
            swWordIDs.WriteLine("WordID ^ WordText");

            foreach (Verse v in kjvdata.dVersesWithoutStrongs.OrderBy(a => a.Key).Select(a => a.Value))
            {
                foreach (string strWord in v.strText.ToLower().Trim().Split())
                {
                    string strWordCleaned = rgxCleanedWord.Replace(strWord, "").Trim();

                    if (strWordCleaned != "")
                    {
                        if (!kjvdata.dWordIDs.ContainsValue(strWordCleaned))
                        {
                            kjvdata.dWordIDs.Add(kjvdata.dWordIDs.Count() + 1, strWordCleaned);
                            swWordIDs.WriteLine(kjvdata.dWordIDs.Count().ToString() + " ^ " + strWordCleaned);
                        }
                    }
                }
            }

            swWordIDs.Close();
        }

        public void CreateWordPositions(ref KJVData kjvdata, ref StreamWriter swWordPositions)
        {
            int intPositionID = 0;
            string strLastBookName = "";

            swWordPositions.WriteLine("PositionID ^ WordText");

            foreach (Verse v in kjvdata.dVersesWithoutStrongs.OrderBy(a => a.Key).Select(a => a.Value))
            {
                foreach (string strWord in v.strText.ToLower().Trim().Split())
                {
                    string strWordCleaned = rgxCleanedWord.Replace(strWord, "").Trim();

                    if (strWordCleaned != "")
                    {
                        intPositionID++;

                        kjvdata.dWordPositions.Add(intPositionID, strWordCleaned);
                        swWordPositions.WriteLine(kjvdata.dWordPositions.Count().ToString() + " ^ " + strWordCleaned);

                        //Fill data structure dBookStartingPositions
                        if (strLastBookName != v.strBookName)
                        {
                            kjvdata.dBookStartingPositions.Add(intPositionID, v.strBookName);
                            strLastBookName = v.strBookName;
                        }
                    }
                }
            }

            swWordPositions.Close();
        }

        public void CreateWordCounts(ref KJVData kjvdata, ref StreamWriter swWordCounts)
        {
            swWordCounts.WriteLine("Word ^ Count");

            foreach (int intPosition in kjvdata.dWordPositions.Keys)
            {
                string strWord = rgxCleanedWord.Replace(kjvdata.dWordPositions[intPosition].Trim().ToLower(), "");

                if (strWord != "")
                {
                    if (!kjvdata.dWordCounts.ContainsKey(strWord))
                    {
                        kjvdata.dWordCounts.Add(strWord, 0);
                    }

                    kjvdata.dWordCounts[strWord]++;
                }
            }

            foreach (string strWord in kjvdata.dWordCounts.Keys)
            {
                swWordCounts.WriteLine(strWord + " ^ " + kjvdata.dWordCounts[strWord].ToString());
            }

            swWordCounts.Close();
        }

        public void ReadWordIDs(ref KJVData kjvdata, ref StreamReader srWordIDs)
        {
            bool bSeenHeader = false;

            while (!srWordIDs.EndOfStream)
            {
                string strLine = srWordIDs.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    kjvdata.dWordIDs.Add(Convert.ToInt32(strsLine[0]), strsLine[1].Trim());
                }
            }

            srWordIDs.Close();
        }

        public void ReadWordPositions(ref KJVData kjvdata, ref StreamReader srWordPositions)
        {
            bool bSeenHeader = false;

            while (!srWordPositions.EndOfStream)
            {
                string strLine = srWordPositions.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    kjvdata.dWordPositions.Add(Convert.ToInt32(strsLine[0]), strsLine[1]);
                }
            }

            srWordPositions.Close();
        }

        public void ReadWordCounts(ref KJVData kjvdata, ref StreamReader srWordCounts)
        {
            bool bSeenHeader = false;

            while (!srWordCounts.EndOfStream)
            {
                string strLine = srWordCounts.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    kjvdata.dWordCounts.Add(strsLine[0], Convert.ToInt32(strsLine[1]));
                }
            }

            srWordCounts.Close();
        }

        public Dictionary<string, List<string>> FindWordIDMobySynonyms(ref Dictionary<int, string> dWordIDs,
            ref Dictionary<int, List<string>> dMoby, ref Dictionary<string, List<int>> dMobySuperIndex)
        {
            Dictionary<string, List<string>> dReturn = new();
            //Dictionary<int, string> dWordIDsCopy = dWordIDs; //for use in lambda expression
            Dictionary<int, List<string>> dMobyCopy = dMoby; //for use in lambda expression

            foreach (string strSynonymPhrase in dMobySuperIndex.Keys.Intersect(dWordIDs.Values)) //a kjv word is in a moby synonym group
            {
                foreach (string strOtherSynonym in dMobySuperIndex.Keys.Intersect(dWordIDs.Values).Where(
                    a => a.Trim() != strSynonymPhrase))
                {
                    foreach (int intGroupID in dMoby.Where(a => a.Value.Contains(strSynonymPhrase))
                        .Intersect(dMobyCopy.Where(a => a.Value.Contains(strOtherSynonym)))
                        .Where(a => a.Value.Count() > 0).Select(a => a.Key))
                    {
                        if (!dReturn.ContainsKey(strSynonymPhrase))
                        {
                            dReturn.Add(strSynonymPhrase, new List<string>());
                        }

                        if (!dReturn[strSynonymPhrase].Contains(strOtherSynonym.Trim()))
                        {
                            dReturn[strSynonymPhrase].Add(strOtherSynonym.Trim());
                        }
                    }
                }
            }

            return dReturn;
        }
    }
}

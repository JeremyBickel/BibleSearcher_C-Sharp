using Moby;

namespace Synonyms
{
    public class Welcome
    {
        public Dictionary<int, List<string>> dSynonyms;
        public Dictionary<string, List<int>> dSynonymsIndex;

        public Welcome(ref StreamReader srSynonyms, ref StreamWriter swSynonyms,
            ref StreamWriter swSynonymsIndex)
        {
            AddSynonyms(ref srSynonyms);
            WriteSynonyms(ref swSynonyms, ref swSynonymsIndex);
        }

        public Welcome(ref StreamReader srSynonyms, ref StreamReader srSynonymsIndex)
        {
            ReadSynonyms(ref srSynonyms, ref srSynonymsIndex);
        }

        public void AddSynonyms(ref StreamReader srSynonyms)
        {
            dSynonyms ??= new();
            dSynonymsIndex ??= new();

            while (!srSynonyms.EndOfStream)
            {
                string strLine = srSynonyms.ReadLine();
                string[] strsLine = strLine.Split('^');

                foreach (string strSynonym in strsLine[1].Split(','))
                {
                    if (strSynonym.Trim() != "")
                    {
                        dSynonyms.Add(dSynonyms.Count + 1, new List<string>());
                        dSynonyms[dSynonyms.Count].Add(strsLine[0].ToLower().Trim());

                        if (!dSynonyms[dSynonyms.Count].Contains(strSynonym.ToLower().Trim()))
                        {
                            dSynonyms[dSynonyms.Count].Add(strSynonym.ToLower().Trim());
                        }
                    }
                }
            }

            foreach (int intGroupID in dSynonyms.Keys.OrderBy(a => a))
            {
                foreach (string strSynonym in dSynonyms[intGroupID])
                {
                    if (!dSynonymsIndex.ContainsKey(strSynonym.ToLower().Trim()))
                    {
                        dSynonymsIndex.Add(strSynonym.ToLower().Trim(), new List<int>());
                    }

                    dSynonymsIndex[strSynonym.ToLower().Trim()].Add(intGroupID);
                }
            }

            srSynonyms.Close();
        }

        public void ReadSynonyms(ref StreamReader srSynonyms, ref StreamReader srSynonymsIndex)
        {
            dSynonyms ??= new();
            dSynonymsIndex ??= new();

            while (!srSynonyms.EndOfStream)
            {
                string strLine = srSynonyms.ReadLine();
                string[] strsLine = strLine.Split('^');
                int intGroupID = Convert.ToInt32(strsLine[0]);

                if (!dSynonyms.ContainsKey(intGroupID))
                {
                    dSynonyms.Add(intGroupID, new());
                }

                for (int intCounter = 1; intCounter < strsLine.Length; intCounter++)
                {
                    dSynonyms[intGroupID].Add(strsLine[intCounter]);
                }
            }

            srSynonyms.Close();

            while (!srSynonymsIndex.EndOfStream)
            {
                string strLine = srSynonymsIndex.ReadLine();
                string[] strsLine = strLine.Split('^');
                string strSynonym = strsLine[0];

                if (!dSynonymsIndex.ContainsKey(strSynonym))
                {
                    dSynonymsIndex.Add(strSynonym, new());
                }

                for (int intCounter = 1; intCounter < strsLine.Length; intCounter++)
                {
                    dSynonymsIndex[strSynonym].Add(Convert.ToInt32(strsLine[intCounter]));
                }
            }

            srSynonymsIndex.Close();
        }

        public void WriteSynonyms(ref StreamWriter swSynonyms, ref StreamWriter swSynonymsIndex)
        {
            foreach (int intGroupID in dSynonyms.Keys.OrderBy(a => a))
            {
                swSynonyms.Write(intGroupID.ToString());

                foreach (string strSynonym in dSynonyms[intGroupID])
                {
                    swSynonyms.Write(" ^ " + strSynonym);
                }

                swSynonyms.WriteLine();
            }

            swSynonyms.Close();

            foreach (string strSynonym in dSynonymsIndex.Keys.OrderBy(a => a))
            {
                swSynonymsIndex.Write(strSynonym);

                foreach (int intGroupID in dSynonymsIndex[strSynonym])
                {
                    swSynonymsIndex.Write(" ^ " + intGroupID.ToString());
                }

                swSynonymsIndex.WriteLine();
            }

            swSynonymsIndex.Close();
        }

        public void WriteMobyIntoSynonyms(ref StreamWriter swSynonyms, ref Synonyms.Welcome synonyms, ref MobyData mobydata)
        {
            int intMaxSynonymIndex = synonyms.dSynonyms.Max(a => a.Key);

            foreach (int intMobyKey in mobydata.dMoby.Keys.OrderBy(a => a))
            {
                swSynonyms.Write((intMobyKey + intMaxSynonymIndex).ToString());

                foreach (string strMobyGroup in mobydata.dMoby[intMobyKey])
                {
                    foreach (string strMobySynonym in strMobyGroup.Split(','))
                    {
                        if (strMobySynonym.Trim() != "")
                        {
                            swSynonyms.Write(" ^ " + strMobySynonym.ToLower().Trim());
                        }
                    }
                }
            }

            swSynonyms.Close();
        }
    }
}
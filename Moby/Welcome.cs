using System.Text;

namespace Moby
{
    public class Welcome
    {
        public MobyData mobydata = new();

        public Welcome(ref StreamReader srMobyIn, ref StreamWriter swMoby,
            ref StreamWriter swMobyIndex, ref StreamWriter swMobySuperIndex)
        {
            Create(ref srMobyIn);
            Write(ref swMoby, ref swMobyIndex, ref swMobySuperIndex);
        }

        public Welcome(ref StreamReader srMoby, ref StreamReader srMobyIndex,
            ref StreamReader srMobySuperIndex)
        {
            Read(ref srMoby, ref srMobyIndex, ref srMobySuperIndex);
        }

        public void Create(ref StreamReader srMobyIn)
        {
            while (!srMobyIn.EndOfStream)
            {
                string strLine = srMobyIn.ReadLine();
                int intID = mobydata.dMoby.Count + 1;

                if (strLine.Trim().Contains(','))
                {
                    mobydata.dMoby.Add(intID, new List<string>());

                    foreach (string strPhrase in strLine.Split(','))
                    {
                        string strPhraseCleaned = strPhrase.ToLower().Trim();

                        if (strPhraseCleaned != "")
                        {
                            mobydata.dMoby[intID].Add(strPhraseCleaned);

                            if (!mobydata.dMobyIndex.ContainsKey(strPhraseCleaned))
                            {
                                mobydata.dMobyIndex.Add(strPhraseCleaned, new List<int>());
                            }

                            mobydata.dMobyIndex[strPhraseCleaned].Add(intID);

                            foreach (string strWord in strPhraseCleaned.Split())
                            {
                                string strWordCleaned = strWord.Trim();

                                if (!mobydata.dMobySuperIndex.ContainsKey(strWordCleaned))
                                {
                                    mobydata.dMobySuperIndex.Add(strWordCleaned, new List<int>());
                                }

                                mobydata.dMobySuperIndex[strWordCleaned].Add(intID);
                            }
                        }
                    }

                    if (mobydata.dMoby[intID].Count == 0)
                    {
                        mobydata.dMoby.Remove(intID);
                    }
                }
            }
        }

        public void Read(ref StreamReader srMoby, ref StreamReader srMobyIndex,
            ref StreamReader srMobySuperIndex)
        {
            //
            //Synonyms - natural case
            //
            bool bSeenHeader = false;

            while (!srMoby.EndOfStream)
            {
                string strLine = srMoby.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');
                    int intID = Convert.ToInt16(strsLine[0]);

                    mobydata.dMoby.Add(intID, new List<string>());

                    foreach (string strSynonym in strsLine[1].Split(','))
                    {
                        mobydata.dMoby[intID].Add(strSynonym.Trim());
                    }
                }
            }

            srMoby.Close();

            //
            //Index - lower case
            //
            bSeenHeader = false;

            while (!srMobyIndex.EndOfStream)
            {
                string strLine = srMobyIndex.ReadLine().ToLower();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    mobydata.dMobyIndex.Add(strsLine[0], new List<int>());

                    foreach (string strID in strsLine[1].Split(','))
                    {
                        mobydata.dMobyIndex[strsLine[0]].Add(Convert.ToInt16(strID));
                    }
                }
            }

            srMobyIndex.Close();

            //
            //Super Index
            //
            bSeenHeader = false;

            while (!srMobySuperIndex.EndOfStream)
            {
                string strLine = srMobySuperIndex.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    mobydata.dMobySuperIndex.Add(strsLine[0].Trim(), new List<int>());

                    foreach (string strGroupID in strsLine[1].Split(','))
                    {
                        if (strGroupID.Trim() != "")
                        {
                            int intGroupID = Convert.ToInt32(strGroupID.Trim().Trim(',').Trim());

                            mobydata.dMobySuperIndex[strsLine[0].Trim()].Add(intGroupID);
                        }
                    }
                }
            }

            srMobySuperIndex.Close();
        }

        public void Write(ref StreamWriter swMoby, ref StreamWriter swMobyIndex, ref StreamWriter swMobySuperIndex)
        {
            StringBuilder sbSynonyms = new();
            StringBuilder sbIndex = new();
            StringBuilder sbSuperIndex = new();

            //
            //Synonyms
            //
            swMoby.WriteLine("ID ^ Synonyms");

            foreach (int intID in mobydata.dMoby.Keys.OrderBy(a => a))
            {
                _ = sbSynonyms.Clear();

                swMoby.Write(intID.ToString() + " ^ ");

                foreach (string strSynonym in mobydata.dMoby[intID])
                {
                    _ = sbSynonyms.Append(strSynonym + " , ");
                }

                _ = sbSynonyms.Remove(sbSynonyms.Length - 3, 3);

                swMoby.WriteLine(sbSynonyms.ToString());
            }

            swMoby.Close();

            //
            //Index
            //
            swMobyIndex.WriteLine("Synonym ^ ID");

            foreach (string strSynonym in mobydata.dMobyIndex.Keys.OrderBy(a => a))
            {
                _ = sbIndex.Clear();

                swMobyIndex.Write(strSynonym + " ^ ");

                foreach (int intID in mobydata.dMobyIndex[strSynonym])
                {
                    _ = sbIndex.Append(intID.ToString() + " , ");
                }

                _ = sbIndex.Remove(sbIndex.Length - 3, 3);

                swMobyIndex.WriteLine(sbIndex.ToString());
            }

            swMobyIndex.Close();

            //
            //Super Index
            //
            swMobySuperIndex.WriteLine("Synonym Part ^ IDs");

            foreach (string strSynonymPart in mobydata.dMobySuperIndex.Keys.OrderBy(a => a))
            {
                _ = sbSuperIndex.Clear();

                swMobySuperIndex.Write(strSynonymPart + " ^ ");

                foreach (int intID in mobydata.dMobySuperIndex[strSynonymPart])
                {
                    _ = sbSuperIndex.Append(intID.ToString() + " , ");
                }

                _ = sbSuperIndex.Remove(sbSuperIndex.Length - 3, 3);

                swMobySuperIndex.WriteLine(sbSuperIndex.ToString());
            }

            swMobySuperIndex.Close();
        }
    }

    public class MobyData
    {
        public Dictionary<int, List<string>> dMoby = new(); //D<id, L<synonyms>>
        public Dictionary<string, List<int>> dMobyIndex = new(); //D<word, each group id in which the word is found>
        public Dictionary<string, List<int>> dMobySuperIndex = new(); //D<lowercase trimmed individual word, List<the groups it's found in>>
        public Dictionary<string, List<string>> dKJVSynonyms = new(); //D<KJVStrongs lowercase word, List<synonym>>
    }
}
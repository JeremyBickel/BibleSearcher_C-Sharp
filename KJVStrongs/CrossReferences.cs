namespace KJVStrongs
{
    public class CrossReferences
    {
        //encode data from Open Bible's cross_references.txt
        //ignore the third column, which are the number of votes given for the cross reference
        public Dictionary<string, List<CrossReferenceDataFrame>> dCrossReferences =
            new(); //D<Referenced Scripture, L<CrossReferenceDataFrame>>
        public Dictionary<string, string> dNormalize = new(); //D<short, normal>

        public CrossReferences()
        {
            CreateNormalizedReferences();
        }

        public void CreateNormalizedReferences()
        {
            dNormalize.Add("Gen", "Genesis");
            dNormalize.Add("Exod", "Exodus");
            dNormalize.Add("Lev", "Leviticus");
            dNormalize.Add("Num", "Numbers");
            dNormalize.Add("Deut", "Deuteronomy");
            dNormalize.Add("Josh", "Joshua");
            dNormalize.Add("Judg", "Judges");
            dNormalize.Add("Ruth", "Ruth");
            dNormalize.Add("1Sam", "1 Samuel");
            dNormalize.Add("2Sam", "2 Samuel");
            dNormalize.Add("1Kgs", "1 Kings");
            dNormalize.Add("2Kgs", "2 Kings");
            dNormalize.Add("1Chr", "1 Chronicles");
            dNormalize.Add("2Chr", "2 Chronicles");
            dNormalize.Add("Ezra", "Ezra");
            dNormalize.Add("Neh", "Nehemiah");
            dNormalize.Add("Esth", "Esther");
            dNormalize.Add("Job", "Job");
            dNormalize.Add("Ps", "Psalms");
            dNormalize.Add("Prov", "Proverbs");
            dNormalize.Add("Eccl", "Ecclesiastes");
            dNormalize.Add("Song", "Song of Solomon");
            dNormalize.Add("Isa", "Isaiah");
            dNormalize.Add("Jer", "Jeremiah");
            dNormalize.Add("Lam", "Lamentations");
            dNormalize.Add("Ezek", "Ezekiel");
            dNormalize.Add("Dan", "Daniel");
            dNormalize.Add("Hos", "Hosea");
            dNormalize.Add("Joel", "Joel");
            dNormalize.Add("Amos", "Amos");
            dNormalize.Add("Obad", "Obadiah");
            dNormalize.Add("Jonah", "Jonah");
            dNormalize.Add("Mic", "Micah");
            dNormalize.Add("Nah", "Nahum");
            dNormalize.Add("Hab", "Habakkuk");
            dNormalize.Add("Zeph", "Zephaniah");
            dNormalize.Add("Hag", "Haggai");
            dNormalize.Add("Zech", "Zechariah");
            dNormalize.Add("Mal", "Malachi");
            dNormalize.Add("Matt", "Matthew");
            dNormalize.Add("Mark", "Mark");
            dNormalize.Add("Luke", "Luke");
            dNormalize.Add("John", "John");
            dNormalize.Add("Acts", "Acts");
            dNormalize.Add("Rom", "Romans");
            dNormalize.Add("1Cor", "1 Corinthians");
            dNormalize.Add("2Cor", "2 Corinthians");
            dNormalize.Add("Gal", "Galatians");
            dNormalize.Add("Eph", "Ephesians");
            dNormalize.Add("Phil", "Philippians");
            dNormalize.Add("Col", "Colossians");
            dNormalize.Add("1Thess", "1 Thessalonians");
            dNormalize.Add("2Thess", "2 Thessalonians");
            dNormalize.Add("1Tim", "1 Timothy");
            dNormalize.Add("2Tim", "2 Timothy");
            dNormalize.Add("Titus", "Titus");
            dNormalize.Add("Phlm", "Philemon");
            dNormalize.Add("Heb", "Hebrews");
            dNormalize.Add("Jas", "James");
            dNormalize.Add("1Pet", "1 Peter");
            dNormalize.Add("2Pet", "2 Peter");
            dNormalize.Add("1John", "1 John");
            dNormalize.Add("2John", "2 John");
            dNormalize.Add("3John", "3 John");
            dNormalize.Add("Jude", "Jude");
            dNormalize.Add("Rev", "Revelation");
        }

        public void CreateCrossReferences(ref StreamReader srCRs)
        {
            while (!srCRs.EndOfStream)
            {
                CrossReferenceDataFrame data = new();
                string strLine = srCRs.ReadLine();
                string[] strsLine = strLine.Split();
                _ = CreateNormalFormReference(strsLine[1]);

                data.strReferenced = CreateNormalFormReference(strsLine[0]).First().Key;
                data.strReferencingBeginning = CreateNormalFormReference(strsLine[1]).First().Key;
                data.strReferencingEnding = CreateNormalFormReference(strsLine[1]).First().Value;
                data.intVotes = Convert.ToInt32(strsLine[2]);

                if (!dCrossReferences.ContainsKey(data.strReferenced))
                {
                    dCrossReferences.Add(data.strReferenced, new List<CrossReferenceDataFrame>());
                }

                dCrossReferences[data.strReferenced].Add(data);
            }
        }

        public void Read(ref StreamReader srCrossrefs)
        {
            bool bSeenHeader = false;

            while (!srCrossrefs.EndOfStream)
            {
                string strLine = srCrossrefs.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!dCrossReferences.ContainsKey(strsLine[1].Trim()))
                    {
                        dCrossReferences.Add(strsLine[1].Trim(), new List<CrossReferenceDataFrame>());
                    }

                    foreach (string strReferencingPart in strsLine[2].Trim().Split(','))
                    {
                        if (strReferencingPart.Trim() != "")
                        {
                            CrossReferenceDataFrame crdf = new();

                            string[] strsReferencing = strReferencingPart.Split('%');
                            string[] strsReferencingDash = strsReferencing[0].Split('-');

                            crdf.strReferencingBeginning = strsReferencingDash[0].Trim();
                            crdf.strReferencingEnding = strsReferencingDash[1].Trim();
                            crdf.intVotes = Convert.ToInt16(strsReferencing[1].Trim());

                            dCrossReferences[strsLine[1].Trim()].Add(crdf);
                        }
                    }
                }
            }

            srCrossrefs.Close();
        }

        public void Write(ref StreamWriter swCrossrefs)
        {
            int intRowCounter = 0;
            //
            //crossReferences
            //

            swCrossrefs.WriteLine("RowID ^ ReferencedPassage ^ ReferencingPassagesWithVoteCounts");

            foreach (string strReferenced in dCrossReferences.Keys.OrderBy(a => a))
            {
                string strReferencing = "";

                intRowCounter++;

                swCrossrefs.Write(intRowCounter.ToString() + " ^ " + strReferenced + " ^ ");

                foreach (CrossReferenceDataFrame crdf in dCrossReferences[strReferenced])
                {
                    strReferencing += crdf.strReferencingBeginning + "-" + crdf.strReferencingEnding +
                        "%" + crdf.intVotes.ToString() + ", "; //referenced ^ referencing 1:1%votecount, complex 1:1-referencing 1:1%votecount, referencing 1:2%votecount
                }

                _ = strReferencing.Remove(strReferencing.Length - 2); //remove the trailing ", "

                swCrossrefs.WriteLine(strReferencing);
            }

            swCrossrefs.Close();
        }

        private Dictionary<string, string> CreateNormalFormReference(string strReferenceIn)
        {
            Dictionary<string, string> dReturn = new();
            string strEndingIn = "";
            string strBeginningOut = "";
            string strEndingOut = "";
            int intPartCounter = 0;

            string strBeginningIn;
            if (strReferenceIn.Contains('-'))
            {
                string[] strsReferences = strReferenceIn.Split('-');
                strBeginningIn = strsReferences[0];
                strEndingIn = strsReferences[1];
            }
            else
            {
                strBeginningIn = strReferenceIn;
            }


            foreach (string strReferencePart in strBeginningIn.Split('.'))
            {
                intPartCounter++;

                switch (intPartCounter)
                {
                    case 1:
                        strBeginningOut = dNormalize[strReferencePart];
                        break;
                    case 2:
                        strBeginningOut += " " + strReferencePart.ToString();
                        break;
                    case 3:
                        strBeginningOut += ":" + strReferencePart.ToString();
                        break;
                }
            }

            if (strEndingIn != "")
            {
                intPartCounter = 0;

                foreach (string strReferencePart in strEndingIn.Split('.'))
                {
                    intPartCounter++;

                    switch (intPartCounter)
                    {
                        case 1:
                            strEndingOut = dNormalize[strReferencePart];
                            break;
                        case 2:
                            strEndingOut += " " + strReferencePart.ToString();
                            break;
                        case 3:
                            strEndingOut += ":" + strReferencePart.ToString();
                            break;
                    }
                }
            }
            else
            {
                strEndingOut = "";
            }

            dReturn.Add(strBeginningOut, strEndingOut);

            return dReturn;
        }
    }

    public class CrossReferenceDataFrame
    {
        public string strReferenced = "";
        public string strReferencingBeginning = "";
        public string strReferencingEnding = "";
        public int intVotes = 0;
    }
}

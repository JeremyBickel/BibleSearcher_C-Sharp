using System.Text.RegularExpressions;

namespace TreasuryOfScriptureKnowledge
{
    public class Welcome
    {
        public Dictionary<string, Dictionary<int, TSKData>> dTSK = new(); //D<book name, D<counter, TSKData>>
        public Dictionary<string, string> dTSKBookNameAbbreviationNormalizations = new(); //D<seen, normalized> for TSK

        public Welcome(ref StreamWriter swTSK, string strTSKDirectory)
        {
            Create(strTSKDirectory);
            Write(ref swTSK);
        }

        public Welcome(ref StreamReader srTSK)
        {
            Read(ref srTSK);
        }

        public void FillTSKBookNameAbbreviationNormalizationDictionary()
        {
            dTSKBookNameAbbreviationNormalizations.Clear();

            dTSKBookNameAbbreviationNormalizations.Add("Ge", "Genesis");
            dTSKBookNameAbbreviationNormalizations.Add("Ex", "Exodus");
            dTSKBookNameAbbreviationNormalizations.Add("Le", "Leviticus");
            dTSKBookNameAbbreviationNormalizations.Add("Nu", "Numbers");
            dTSKBookNameAbbreviationNormalizations.Add("De", "Deuteronomy");
            dTSKBookNameAbbreviationNormalizations.Add("Jos", "Joshua");
            dTSKBookNameAbbreviationNormalizations.Add("Jg", "Judges");
            dTSKBookNameAbbreviationNormalizations.Add("Ru", "Ruth");
            dTSKBookNameAbbreviationNormalizations.Add("1Sa", "1 Samuel");
            dTSKBookNameAbbreviationNormalizations.Add("2Sa", "2 Samuel");
            dTSKBookNameAbbreviationNormalizations.Add("1Ki", "1 Kings");
            dTSKBookNameAbbreviationNormalizations.Add("2Ki", "2 Kings");
            dTSKBookNameAbbreviationNormalizations.Add("1Ch", "1 Chronicles");
            dTSKBookNameAbbreviationNormalizations.Add("2Ch", "2 Chronicles");
            dTSKBookNameAbbreviationNormalizations.Add("Ezr", "Ezra");
            dTSKBookNameAbbreviationNormalizations.Add("Ne", "Nehemiah");
            dTSKBookNameAbbreviationNormalizations.Add("Es", "Esther");
            dTSKBookNameAbbreviationNormalizations.Add("Job", "Job");
            dTSKBookNameAbbreviationNormalizations.Add("Ps", "Psalms");
            dTSKBookNameAbbreviationNormalizations.Add("Pr", "Proverbs");
            dTSKBookNameAbbreviationNormalizations.Add("Ec", "Ecclesiastes");
            dTSKBookNameAbbreviationNormalizations.Add("Song", "Song of Solomon");
            dTSKBookNameAbbreviationNormalizations.Add("Isa", "Isaiah");
            dTSKBookNameAbbreviationNormalizations.Add("Jer", "Jeremiah");
            dTSKBookNameAbbreviationNormalizations.Add("La", "Lamentations");
            dTSKBookNameAbbreviationNormalizations.Add("Eze", "Ezekiel");
            dTSKBookNameAbbreviationNormalizations.Add("Da", "Daniel");
            dTSKBookNameAbbreviationNormalizations.Add("Ho", "Hosea");
            dTSKBookNameAbbreviationNormalizations.Add("Joe", "Joel");
            dTSKBookNameAbbreviationNormalizations.Add("Am", "Amos");
            dTSKBookNameAbbreviationNormalizations.Add("Ob", "Obadiah");
            dTSKBookNameAbbreviationNormalizations.Add("Jon", "Jonah");
            dTSKBookNameAbbreviationNormalizations.Add("Mic", "Micah");
            dTSKBookNameAbbreviationNormalizations.Add("Na", "Nahum");
            dTSKBookNameAbbreviationNormalizations.Add("Hab", "Habakkuk");
            dTSKBookNameAbbreviationNormalizations.Add("Zep", "Zephaniah");
            dTSKBookNameAbbreviationNormalizations.Add("Hag", "Haggai");
            dTSKBookNameAbbreviationNormalizations.Add("Zec", "Zechariah");
            dTSKBookNameAbbreviationNormalizations.Add("Mal", "Malachi");
            dTSKBookNameAbbreviationNormalizations.Add("Mt", "Matthew");
            dTSKBookNameAbbreviationNormalizations.Add("Mr", "Mark");
            dTSKBookNameAbbreviationNormalizations.Add("Lu", "Luke");
            dTSKBookNameAbbreviationNormalizations.Add("Joh", "John");
            dTSKBookNameAbbreviationNormalizations.Add("Ac", "Acts");
            dTSKBookNameAbbreviationNormalizations.Add("Ro", "Romans");
            dTSKBookNameAbbreviationNormalizations.Add("1Co", "1 Corinthians");
            dTSKBookNameAbbreviationNormalizations.Add("2Co", "2 Corinthians");
            dTSKBookNameAbbreviationNormalizations.Add("Ga", "Galatians");
            dTSKBookNameAbbreviationNormalizations.Add("Eph", "Ephesians");
            dTSKBookNameAbbreviationNormalizations.Add("Php", "Philippians");
            dTSKBookNameAbbreviationNormalizations.Add("Col", "Colossians");
            dTSKBookNameAbbreviationNormalizations.Add("1Th", "1 Thessalonians");
            dTSKBookNameAbbreviationNormalizations.Add("2Th", "2 Thessalonians");
            dTSKBookNameAbbreviationNormalizations.Add("1Ti", "1 Timothy");
            dTSKBookNameAbbreviationNormalizations.Add("2Ti", "2 Timothy");
            dTSKBookNameAbbreviationNormalizations.Add("Tit", "Titus");
            dTSKBookNameAbbreviationNormalizations.Add("Phm", "Philemon");
            dTSKBookNameAbbreviationNormalizations.Add("Heb", "Hebrews");
            dTSKBookNameAbbreviationNormalizations.Add("Jas", "James");
            dTSKBookNameAbbreviationNormalizations.Add("1Pe", "1 Peter");
            dTSKBookNameAbbreviationNormalizations.Add("2Pe", "2 Peter");
            dTSKBookNameAbbreviationNormalizations.Add("1Jo", "1 John");
            dTSKBookNameAbbreviationNormalizations.Add("2Jo", "2 John");
            dTSKBookNameAbbreviationNormalizations.Add("3Jo", "3 John");
            dTSKBookNameAbbreviationNormalizations.Add("Jude", "Jude");
            dTSKBookNameAbbreviationNormalizations.Add("Re", "Revelation");
        }

        public void Create(string strTSKDirectory)
        {
            bool bBody = false;
            bool bTable = false;
            Regex rgxVerses = new("<td class=\"verses\">(?<verses>[^<]{1,})</td>");
            Regex rgxSummary = new("<td class=\"summary\">(?<summary>[^<]{1,})</td>");
            Regex rgxNotSpacesLettersOrNumbers = new(@"[^a-zA-Z0-9 ]{1,}");

            FillTSKBookNameAbbreviationNormalizationDictionary();

            foreach (string strFilename in Directory.EnumerateFiles(strTSKDirectory, "*.html"))
            {
                StreamReader srTSK = new(strFilename);
                string strCurrentVerses = "";

                while (!srTSK.EndOfStream)
                {
                    string strLine = srTSK.ReadLine();

                    if (bBody == false)
                    {
                        if (strLine.Contains("<body>"))
                        {
                            bBody = true;
                        }
                    }
                    else
                    {
                        if (bTable == false)
                        {
                            if (strLine.Contains("<table>"))
                            {
                                bTable = true;
                            }
                        }
                        else
                        {
                            if (strLine.Contains("<\table>"))
                            {
                                break;
                            }
                            else if (strLine.Contains("<td"))
                            {
                                if (rgxVerses.IsMatch(strLine))
                                {
                                    strCurrentVerses = rgxVerses.Match(strLine).Groups["verses"].Value;
                                }
                                else if (rgxSummary.IsMatch(strLine))
                                {
                                    string strBookName = dTSKBookNameAbbreviationNormalizations[strCurrentVerses.Split()[0]];
                                    TSKData tskdata = new()
                                    {
                                        strVerses = strCurrentVerses,
                                        strSummary = rgxNotSpacesLettersOrNumbers.Replace(
                                        rgxSummary.Match(strLine).Groups["summary"].Value, "")
                                    };

                                    if (!dTSK.ContainsKey(strBookName))
                                    {
                                        dTSK.Add(strBookName, new Dictionary<int, TSKData>());
                                    }

                                    dTSK[strBookName].Add(dTSK[strBookName].Count + 1, tskdata);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Read(ref StreamReader srTSK)
        {
            bool bSeenHeader = false;

            while (!srTSK.EndOfStream)
            {
                string strLine = srTSK.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!dTSK.ContainsKey(strsLine[0]))
                    {
                        dTSK.Add(strsLine[0], new Dictionary<int, TSKData>());
                    }

                    if (!dTSK[strsLine[0]].ContainsKey(Convert.ToInt16(strsLine[1])))
                    {
                        dTSK[strsLine[0]].Add(Convert.ToInt16(strsLine[1]), new TSKData());
                    }

                    dTSK[strsLine[0]][Convert.ToInt16(strsLine[1])].strVerses = strsLine[2];
                    dTSK[strsLine[0]][Convert.ToInt16(strsLine[1])].strSummary = strsLine[3];
                }
            }

            srTSK.Close();
        }

        public void Write(ref StreamWriter swTSK)
        {
            swTSK.WriteLine("Book Name ^ Counter ^ Verses ^ Summary");

            foreach (string strBookName in dTSK.Keys.OrderBy(a => a))
            {
                foreach (int intCounter in dTSK[strBookName].Keys.OrderBy(a => a))
                {
                    swTSK.WriteLine(strBookName + " ^ " + intCounter.ToString() + " ^ " +
                        dTSK[strBookName][intCounter].strVerses + " ^ " +
                        dTSK[strBookName][intCounter].strSummary);
                }
            }

            swTSK.Close();
        }
    }

    public class TSKData
    {
        public string? strVerses;
        public string? strSummary;
    }
}
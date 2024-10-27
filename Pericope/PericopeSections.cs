using System.Text.RegularExpressions;

namespace Pericope
{
    public class PericopeSections
    {
        public Dictionary<string, List<PericopeInfo>> dPericopes = new(); //D<strHeading, PericopeInfo object>
        public List<string> lHeadings = new(); //every heading from all sources
        public TreasuryOfScriptureKnowledge.Welcome? tsk;
        public Regex rgxNumber = new(@"^[^0-9]{0,}[0-9]{1,}[^0-9]{0,}$");
        public Regex rgxLetter = new(@"^[^a-hj-z]{0,}[a-hj-z]{1,}[^a-hj-z]{0,}$"); //no i, which is roman numeral
        public Regex rgxRomanNumeral = new(@"^[^i]{0,}[i]{1,}[^i]{0,}$");
        public Regex rgxReference = new(@"(?<book>[^ ]{1,}) (?<startchapter>[^:]{1,}):(?<startverse>[0-9]{1,}[^-]{0,})-{0,}(?<endchapter>[0-9]{0,}):{0,}(?<endverse>[0-9]{0,})\.");
        public Regex rgxReferenceOneChapter = new(@"(?<book>[^ ]{1,}) (?<startverse>[0-9]{1,})-{0,}(?<endverse>[0-9]{0,})");
        public Regex rgxNotWords = new(@"[^a-zA-Z \-]{1,}");
        public Regex rgxNotSpacesLettersOrNumbers = new(@"[^a-zA-Z0-9 ]{1,}");
        public Dictionary<int, List<PericopeVerse>> dRangedVerses = new();

        //Fills the PericopeInfo object with the start and end chapters and verses from various forms of references
        public void FillPericopeInfoObjectWithRangedReference(ref PericopeInfo piData, string strReference, bool bFurtherVersesInPreviousChapter = false, int intPreviousChapter = 0)
        {
            //
            //1:1; 1:1-2; 1:1-1:50; 1:1-2:4; 1:1-5, 10-15 (which is 1:1-1:5, 1:10-1:15); 10-15 (chapters)
            //

            bool bDash = false; //have we seen a dash between references yet?
            string strLastChapterNumber = "";
            string strMalleableIncomingChapterNumber = ""; //if a colon appears in strReference, then the incoming intPreviousChapter is wrong and needs to be changed

            foreach (string strReferencePart in strReference.Split("-"))
            {
                int intChapterNumber = 0;
                int intVerseNumber = 0;
                string strChapterNumberBuilder = "";
                string strVerseNumberBuilder = "";

                bool bVerse = false;

                foreach (char ch in strReferencePart) //this loop goes character by character in the space-separated part of the reference
                {
                    if (!strReferencePart.Contains(':')) //this whole if statement presupposes that there won't be a colon after the dash if there's not one before it, like 1-4:7 (which should be written 1:1-4:7)
                    {
                        if (strReference.Contains(':')) //since the entire reference contains a colon, a single number is a chapter or verse depending on whether it's before or after the dash
                        {
                            if (bDash == true)
                            {
                                bVerse = true;
                            }
                        }
                        else //if neither this part or the entire reference contains any colons, then all the numbers before and after the dash are chapters, unless bFurtherVersesInPreviousChapter = true
                        {
                            bVerse = bFurtherVersesInPreviousChapter == true;
                        }
                    }

                    if (ch == ':')
                    {
                        bVerse = true;

                        if (bFurtherVersesInPreviousChapter == true)
                        {
                            strMalleableIncomingChapterNumber = strChapterNumberBuilder;
                        }

                    }
                    else if (rgxNumber.IsMatch(ch.ToString()))
                    {
                        if (bVerse == false) //chapter number
                        {
                            strChapterNumberBuilder += ch;
                        }
                        else //verse number
                        {
                            strVerseNumberBuilder += ch;
                        }
                    }
                    else
                    {
                        throw new Exception("ERROR splitting strReference on dash");
                    }
                }

                if (bFurtherVersesInPreviousChapter == true)
                {
                    strChapterNumberBuilder = strMalleableIncomingChapterNumber != "" ? strMalleableIncomingChapterNumber : intChapterNumber.ToString();
                }

                if (strChapterNumberBuilder == "")
                {
                    strChapterNumberBuilder = strLastChapterNumber;
                }

                if (strVerseNumberBuilder == "")
                {
                    //before the dash this is the first verse in the chapter
                    //or the last verse in the chapter after the dash,
                    //represented by "0" and to be calculated elsewhere
                    strVerseNumberBuilder = !bDash ? "1" : "0";
                }

                intVerseNumber = Convert.ToInt32(strVerseNumberBuilder);
                intChapterNumber = Convert.ToInt32(strChapterNumberBuilder);

                strLastChapterNumber = strChapterNumberBuilder;

                if (bDash == false)
                {
                    piData.intStartChapterNumber = intChapterNumber;
                    piData.intStartVerseNumber = intVerseNumber;
                }
                else
                {
                    piData.intEndChapterNumber = intChapterNumber;
                    piData.intEndVerseNumber = intVerseNumber;
                }

                bDash = true;
            }

            //if there's no dash (tested by the end chapter number being 0), then the whole reference is only a single verse,
            //so start and end chapter and verse numbers are the same
            if (piData.intEndChapterNumber == 0)
            {
                piData.intEndChapterNumber = piData.intStartChapterNumber;
                piData.intEndVerseNumber = piData.intStartVerseNumber;
            }
        }

        public void FillPericopeRanges(ref StreamReader srReferences, ref StreamWriter swPericopeRanges)
        {
            Dictionary<string, Dictionary<int, List<int>>> dPericopeReferences = new();
            int intBookNumber = 0;
            int intPericopeRangeRowID = 0;
            bool bFirstLine = true;
            StreamReader srLastVerseNumberInEachChapter = new(@"Z:\BibleSearcher\Data\Data\Processed\LastVerseNumberInEachChapter.csv");
            Dictionary<int, Dictionary<int, int>> dLastVerseNumbers = new();
            Dictionary<int, int> dLastVerses = FindLastVerseOfEachPericopeObject();

            _ = srLastVerseNumberInEachChapter.ReadLine(); //get past header

            while (!srLastVerseNumberInEachChapter.EndOfStream)
            {
                string strLine = srLastVerseNumberInEachChapter.ReadLine();
                string[] strsLine = strLine.Split('^');
                int intBookNumberLast = Convert.ToInt16(strsLine[0]);
                int intChapterNumberLast = Convert.ToInt16(strsLine[1]);
                int intVerseNumberLast = Convert.ToInt16(strsLine[2]);

                if (!dLastVerseNumbers.ContainsKey(intBookNumberLast))
                {
                    dLastVerseNumbers.Add(intBookNumberLast, new Dictionary<int, int>());
                }

                if (!dLastVerseNumbers[intBookNumberLast].ContainsKey(intChapterNumberLast))
                {
                    dLastVerseNumbers[intBookNumberLast].Add(intChapterNumberLast, intVerseNumberLast);
                }
            }

            srLastVerseNumberInEachChapter.Close();

            while (!srReferences.EndOfStream)
            {
                string strLine = srReferences.ReadLine();
                string[] strsLine = strLine.Split('^');

                if (bFirstLine == true)
                {
                    bFirstLine = false;
                }
                else
                {
                    if (!dPericopeReferences.ContainsKey(strsLine[0].ToLower().Replace(" ", "").Trim()))
                    {
                        dPericopeReferences.Add(strsLine[0].ToLower().Replace(" ", "").Trim(), new Dictionary<int, List<int>>());
                    }

                    if (!dPericopeReferences[strsLine[0].ToLower().Replace(" ", "").Trim()].ContainsKey(Convert.ToInt16(strsLine[1].Trim())))
                    {
                        dPericopeReferences[strsLine[0].ToLower().Replace(" ", "").Trim()].Add(Convert.ToInt16(strsLine[1].Trim()), new List<int>());
                    }

                    dPericopeReferences[strsLine[0].ToLower().Replace(" ", "").Trim()][Convert.ToInt16(strsLine[1].Trim())].Add(Convert.ToInt16(strsLine[2].Trim()));
                }
            }

            foreach (string strPericopeHeading in dPericopes.Keys.OrderBy(a => a))
            {
                foreach (PericopeInfo piCurrent in dPericopes[strPericopeHeading])
                {
                    dRangedVerses.Add(dRangedVerses.Count() + 1, new List<PericopeVerse>());

                    switch (piCurrent.strBookName.ToLower().Replace(" ", ""))
                    {

                        case "matthew":
                            intBookNumber = 40;
                            break;
                        case "mark":
                            intBookNumber = 41;
                            break;
                        case "luke":
                            intBookNumber = 42;
                            break;
                        case "john":
                            intBookNumber = 43;
                            break;
                        case "acts":
                            intBookNumber = 44;
                            break;
                        case "romans":
                            intBookNumber = 45;
                            break;
                        case "1corinthians":
                            intBookNumber = 46;
                            break;
                        case "2corinthians":
                            intBookNumber = 47;
                            break;
                        case "galatians":
                            intBookNumber = 48;
                            break;
                        case "ephesians":
                            intBookNumber = 49;
                            break;
                        case "philippians":
                            intBookNumber = 50;
                            break;
                        case "colossians":
                            intBookNumber = 51;
                            break;
                        case "1thessalonians":
                            intBookNumber = 52;
                            break;
                        case "2thessalonians":
                            intBookNumber = 53;
                            break;
                        case "1timothy":
                            intBookNumber = 54;
                            break;
                        case "2timothy":
                            intBookNumber = 55;
                            break;
                        case "titus":
                            intBookNumber = 56;
                            break;
                        case "philemon":
                            intBookNumber = 57;
                            break;
                        case "hebrews":
                            intBookNumber = 58;
                            break;
                        case "james":
                            intBookNumber = 59;
                            break;
                        case "1peter":
                            intBookNumber = 60;
                            break;
                        case "2peter":
                            intBookNumber = 61;
                            break;
                        case "1john":
                            intBookNumber = 62;
                            break;
                        case "2john":
                            intBookNumber = 63;
                            break;
                        case "3john":
                            intBookNumber = 64;
                            break;
                        case "jude":
                            intBookNumber = 65;
                            break;
                        case "revelation":
                            intBookNumber = 66;
                            break;
                        default:
                            intBookNumber = 0;
                            break;
                    }

                    //if (piCurrent.strBookName == "acts" && piCurrent.intStartChapterNumber == 16 &&
                    //    piCurrent.intStartVerseNumber == 1 &&
                    //    piCurrent.intEndChapterNumber == 20 && piCurrent.intEndVerseNumber == 0)
                    //{
                    //    piCurrent.intEndVerseNumber = 40;
                    //}

                    if (piCurrent.intStartVerseNumber == 0)
                    {
                        piCurrent.intStartVerseNumber = dLastVerseNumbers[intBookNumber][piCurrent.intStartChapterNumber];
                    }

                    if (piCurrent.intEndVerseNumber == 0)
                    {
                        piCurrent.intEndVerseNumber = dLastVerseNumbers[intBookNumber][piCurrent.intEndChapterNumber];
                    }

                    foreach (int intChapterNumber in dPericopeReferences[piCurrent.strBookName.ToLower().Replace(" ", "")]
                        .Where(a => a.Key >= piCurrent.intStartChapterNumber && a.Key <= piCurrent.intEndChapterNumber)
                        .Select(a => a.Key))
                    {
                        int intStartVerse = 0;
                        int intEndVerse = 0;

                        if (intChapterNumber == piCurrent.intStartChapterNumber)
                        {
                            intStartVerse = piCurrent.intStartVerseNumber;
                        }
                        else
                        {
                            intStartVerse = 1;
                        }

                        if (intChapterNumber == piCurrent.intEndChapterNumber)
                        {
                            intEndVerse = piCurrent.intEndVerseNumber;
                        }
                        else
                        {
                            intEndVerse = dPericopeReferences[piCurrent.strBookName.ToLower().Replace(" ", "")][intChapterNumber].Max();
                        }

                        for (int intVerseNumber = intStartVerse; intVerseNumber <= intEndVerse; intVerseNumber++)
                        {
                            PericopeVerse pvCurrent = new PericopeVerse();

                            if (intBookNumber >= 40)
                            {
                                pvCurrent.strBookName = piCurrent.strBookName;
                                pvCurrent.intBookNumber = intBookNumber;
                                pvCurrent.intChapterNumber = intChapterNumber;
                                pvCurrent.intVerseNumber = intVerseNumber;
                                pvCurrent.strPericope1 = piCurrent.strPericope1;
                                pvCurrent.strPericope2 = piCurrent.strPericope2;
                                pvCurrent.strPericope3 = piCurrent.strPericope3;
                                pvCurrent.strPericope4 = piCurrent.strPericope4;

                                dRangedVerses[dRangedVerses.Count()].Add(pvCurrent);
                                
                            }
                        }
                    }
                }
            }

            swPericopeRanges.WriteLine("Row ID ^ Pericope ID ^ Book Name ^ Book Number ^ Chapter Number ^ Verse Number ^ Pericope 1 ^ Pericope 2 ^ Pericope 3 ^ Pericope 4");

            foreach (int intPericopeRangeIndex in dRangedVerses.Keys.OrderBy(a => a))
            {   
                foreach (PericopeVerse pvCurrent in dRangedVerses[intPericopeRangeIndex])
                {
                    intPericopeRangeRowID++;

                    swPericopeRanges.WriteLine(intPericopeRangeRowID.ToString() + " ^ " +
                        intPericopeRangeIndex.ToString() + " ^ " +
                        pvCurrent.strBookName + " ^ " +
                        pvCurrent.intBookNumber.ToString() + " ^ " +
                        pvCurrent.intChapterNumber.ToString() + " ^ " +
                        pvCurrent.intVerseNumber.ToString() + " ^ " +
                        pvCurrent.strPericope1 + " ^ " +
                        pvCurrent.strPericope2 + " ^ " +
                        pvCurrent.strPericope3 + " ^ " +
                        pvCurrent.strPericope4);
                }
            }

            swPericopeRanges.Close();
        }

        public void ReadBereanData(ref StreamReader srWholeBiblePericope)
        {
            string strBookName = ""; //current Bible book name
            string strLastStructureElement = ""; //the last single element structure
            string strCurrentNumber1 = ""; //track the single elements of the current structure
            string strCurrentLetter = ""; //track the single elements of the current structure
            string strCurrentRomanNumeral = ""; //track the single elements of the current structure
            string strCurrentNumber2 = ""; //track the single elements of the current structure
            string strCurrentUnknown = ""; //track the single elements of the current structure
            int intPericopeID = 0;
            int intLineNumber = 0;

            while (!srWholeBiblePericope.EndOfStream)
            {
                string strLine = srWholeBiblePericope.ReadLine().Trim();

                if (intLineNumber > 0)
                {
                    if (strLine != "") //skip empty lines
                    {
                        string[] strsWords = strLine.Split();
                        string strReference = "";
                        string strPericope = "";
                        string strStructureElement = ""; //single element structure

                        bool bBookName;
                        if (!strsWords[0].EndsWith('.')) //Bible book name
                        {
                            bBookName = true;
                            strBookName = strLine;

                            //Normalize strBookName to the book names in the Verse objects (from kjvstrongs.csv)
                            strBookName = strBookName.Replace(" ", "").ToLower();
                        }
                        else //outline data
                        {
                            bBookName = false;
                            intPericopeID = dPericopes.Count + 1;

                            for (int intWordCounter = 0; intWordCounter < strsWords.Length; intWordCounter++)
                            {
                                string strWord = strsWords[intWordCounter];

                                if (intWordCounter == 0) //data structure ID
                                {
                                    strStructureElement = strWord.TrimEnd('.');
                                }
                                else if (strWord.StartsWith("(")) //reference data has no spaces, so it's all one word here
                                {
                                    strReference = strWord.TrimStart('(').TrimEnd(')');
                                }
                                else
                                {
                                    strPericope += rgxNotSpacesLettersOrNumbers.Replace(strWord, "") + " ";
                                }
                            }
                        }

                        if (bBookName == false)
                        {
                            //parse data to class
                            PericopeInfo piData = new();
                            //_ = new(); //D<order id, reference element>; Example for 1 Chronicles 1:2 - D<1, "1">, D<2, "Chronicles">, D<3, "1">, D<4, ":">, D<5, "2"> 
                            string[] strsStructureData;

                            piData.intID = intPericopeID;
                            piData.strBookName = strBookName;

                            //a composite structure based on the single element structures
                            strsStructureData = NextStructureID(strStructureElement, strLastStructureElement, ref strCurrentNumber1,
                                ref strCurrentLetter, ref strCurrentRomanNumeral, ref strCurrentNumber2, ref strCurrentUnknown);
                            piData.strStructure = strsStructureData[0];

                            switch (strsStructureData[1])
                            {
                                case "First Number":
                                    piData.strPericope1 = strPericope.Trim();
                                    break;
                                case "Letter":
                                    piData.strPericope2 = strPericope.Trim();
                                    break;
                                case "Roman Numeral":
                                    piData.strPericope3 = strPericope.Trim();
                                    break;
                                case "Second Number":
                                    piData.strPericope4 = strPericope.Trim();
                                    break;
                                default:
                                    throw new Exception("Error in Pericope Heading Type");
                            }

                            strLastStructureElement = strStructureElement;

                            FillPericopeInfoObjectWithRangedReference(ref piData, strReference);
                            
                            if (!dPericopes.ContainsKey(strPericope.Trim()))
                            {
                                dPericopes.Add(strPericope.Trim(), new List<PericopeInfo>());
                            }

                            dPericopes[strPericope.Trim()].Add(piData);
                        }
                    }
                }

                intLineNumber++;
            }

            srWholeBiblePericope.Close();
        }

        public void ReadWikipedia_Acts(ref StreamReader srWikipediaActs)
        {
            int intLineCounter = 0;
            Regex rgxReference = new(@"\((?<ref>[^\(]{1,})\)$"); //1:1; 1:1-2; 1:1-1:50; 1:1-2:4; 1:1-5, 10-15; *-*, **-**, ***-***, ...

            while (!srWikipediaActs.EndOfStream)
            {
                string strLine = srWikipediaActs.ReadLine().ToString();

                intLineCounter++;

                if (intLineCounter > 1) //skip remark line
                {
                    if (strLine.Trim() != "") //skip blank lines
                    {
                        Match m = rgxReference.Match(strLine);

                        if (!m.Success)
                        {
                            throw new Exception("This should never happen; change rgxReference in ReadWikipedia_Acts()");
                        }

                        string strReference = m.Groups["ref"].Value.Trim();
                        string strHeading = rgxNotSpacesLettersOrNumbers.Replace(strLine.Remove(
                            strLine.Length - strReference.Length - 2).Trim(), "");
                        int intLastChapter = 0;

                        foreach (string strReferenceComma in strReference.Split(","))
                        {
                            PericopeInfo piData = new()
                            {
                                strBookName = "acts",
                                intID = dPericopes.Count + 1
                            };

                            if (piData.strPericope1 == "")
                            {
                                piData.strPericope1 = strHeading;
                            }
                            else if (piData.strPericope2 == "")
                            {
                                piData.strPericope2 = strHeading;
                            }
                            else if (piData.strPericope3 == "")
                            {
                                piData.strPericope3 = strHeading;
                            }
                            else
                            {
                                piData.strPericope4 = piData.strPericope4 == ""
                                    ? strHeading
                                    : throw new Exception("Too many nested pericope headings in ReadWikipedia_Acts \r\n " +
                                                                    piData.intID.ToString() + " " + strHeading);
                            }

                            if (intLastChapter == 0) //support for 1:1-3, 7-10 when 1:1-1:3, 1:7-1:10 and not 1:1-1:3, (chapters) 7-10
                            {
                                FillPericopeInfoObjectWithRangedReference(ref piData, strReferenceComma.Trim());
                            }
                            else
                            {
                                FillPericopeInfoObjectWithRangedReference(ref piData, strReferenceComma.Trim(), true, intLastChapter);
                            }

                            intLastChapter = piData.intEndChapterNumber; //support for 1:1-3, 10-15, where 10-15 are verses

                            if (!dPericopes.ContainsKey(strHeading))
                            {
                                dPericopes.Add(strHeading, new List<PericopeInfo>());
                            }

                            dPericopes[strHeading].Add(piData);
                        }
                    }
                }
            }

            srWikipediaActs.Close();
        }

        //public void ReadTSK(ref Dictionary<string, string> dBooknameAbbreviations)
        //{
        //    bool bExpectVerses = false;
        //    bool bExpectHeading = false;
        //    Regex rgxVerses = new(@"<td class=""verses"">(?<verses>[^<]{1,})</td>");
        //    Regex rgxHeading = new(@"<td class=""summary"">(?<heading>[^<]{1,})</td>");
        //    Dictionary<string, List<string>> dTSK = new(); //D<heading, L<verses>>

        //    foreach (string strFilename in Directory.EnumerateFiles(@"Data\TSK_Headings\"))
        //    {
        //        StreamReader srTSK = new(strFilename);
        //        string strVerses = "";
        //        string strHeading = "";
        //        while (!srTSK.EndOfStream)
        //        {
        //            string strLine = srTSK.ReadLine().Trim();

        //            if (bExpectHeading == true &&
        //                rgxHeading.IsMatch(strLine))
        //            {
        //                strHeading = rgxHeading.Match(strLine).Groups["heading"].Value;

        //                if (!dTSK.ContainsKey(strHeading))
        //                {
        //                    dTSK.Add(strHeading, new List<string>());
        //                }

        //                dTSK[strHeading].Add(strVerses);
        //            }
        //            else if (bExpectVerses == true &&
        //                rgxVerses.IsMatch(strLine))
        //            {
        //                strVerses = rgxVerses.Match(strLine).Groups["verses"].Value;
        //                bExpectVerses = false;
        //                bExpectHeading = true;
        //            }
        //            else if (strLine == "</tr>")
        //            {
        //                bExpectVerses = false;
        //                bExpectHeading = false;

        //                PericopeInfo pdCurrent = CreatePericopeInfoFromTSKData(ref dBooknameAbbreviations, strHeading, strVerses);

        //                if (!dPericopes.ContainsKey(pdCurrent.strPericope1))
        //                {
        //                    dPericopes.Add(pdCurrent.strPericope1, new List<PericopeInfo>());
        //                }

        //                dPericopes[pdCurrent.strPericope1].Add(pdCurrent);
        //            }
        //            else if (strLine == "<tr>")
        //            {
        //                bExpectVerses = true;
        //            }
        //        }

        //        srTSK.Close();
        //    }
        //}

        public void CreateTSK(ref StreamWriter swTSK, string strTSKDirectory)
        {
            tsk = new(ref swTSK, strTSKDirectory);

            //Add TSK data to dPericopes
            foreach (string strBookName in tsk.dTSK.Keys.OrderBy(a => a))
            {
                foreach (int intCounter in tsk.dTSK[strBookName].Keys.OrderBy(a => a))
                {
                    string strSummary = tsk.dTSK[strBookName][intCounter].strSummary;

                    if (!dPericopes.ContainsKey(strSummary))
                    {
                        dPericopes.Add(strSummary, new List<PericopeInfo>());
                    }

                    dPericopes[strSummary].Add(CreatePericopeInfoFromTSKData(ref tsk.dTSKBookNameAbbreviationNormalizations,
                        strSummary, tsk.dTSK[strBookName][intCounter].strVerses));
                }
            }
        }

        public PericopeInfo CreatePericopeInfoFromTSKData(ref Dictionary<string, string> dBooknameAbbreviations, string strHeading, string strVerses)
        {
            PericopeInfo pdReturn = new();
            string strBookName = "";
            int intStartChapterNumber = 0;
            int intStartVerseNumber = 0;
            int intEndChapterNumber = 0;
            int intEndVerseNumber = 0;
            int intColonCount = strVerses.Count(a => a == ':');

            Match? mVerses = null;

            if (intColonCount is 2 or 1)
            {
                mVerses = rgxReference.Match(strVerses);
                strBookName = dBooknameAbbreviations[mVerses.Groups["book"].Value];
                intStartChapterNumber = Convert.ToInt16(mVerses.Groups["startchapter"].Value);
                intStartVerseNumber = Convert.ToInt16(mVerses.Groups["startverse"].Value);

                if (mVerses.Groups["endchapter"].Value != "")
                {
                    if (intColonCount == 2) //the regex will mistake an end verse for the end chapter when only one colon is present
                    {
                        intEndChapterNumber = Convert.ToInt16(mVerses.Groups["endchapter"].Value);

                        if (mVerses.Groups["endverse"].Value != "")
                        {
                            intEndVerseNumber = Convert.ToInt16(mVerses.Groups["endverse"].Value);
                        }
                    }

                    if (intColonCount == 1)
                    {
                        intEndChapterNumber = intStartChapterNumber;

                        intEndVerseNumber = strVerses.Contains('-') ? Convert.ToInt16(mVerses.Groups["endchapter"].Value) : intStartVerseNumber;
                    }
                }
            }
            else if (intColonCount == 0)
            {
                mVerses = rgxReferenceOneChapter.Match(strVerses);
                strBookName = dBooknameAbbreviations[mVerses.Groups["book"].Value];
                intStartChapterNumber = 1;
                intEndChapterNumber = 1;
                intStartVerseNumber = Convert.ToInt16(mVerses.Groups["startverse"].Value);

                intEndVerseNumber = strVerses.Contains('-') ? Convert.ToInt16(mVerses.Groups["endverse"].Value) : intStartVerseNumber;
            }

            //I don't know how the code misses this, but here's a fix. So, fix the above sometime.
            if (intEndChapterNumber == 0)
            {
                intEndChapterNumber = intStartChapterNumber;
            }

            if (intEndVerseNumber == 0)
            {
                intEndVerseNumber = intStartVerseNumber;
            }

            pdReturn.intID = dPericopes.Count + 1;

            pdReturn.strBookName = strBookName;
            pdReturn.strPericope1 = strHeading;
            pdReturn.intStartChapterNumber = intStartChapterNumber;
            pdReturn.intStartVerseNumber = intStartVerseNumber;
            pdReturn.intEndChapterNumber = intEndChapterNumber;
            pdReturn.intEndVerseNumber = intEndVerseNumber;

            return pdReturn;
        }

        public string[] NextStructureID(string strIncomingStructureElement, string strLastStructureElement,
            ref string strCurrentNumber1, ref string strCurrentLetter, ref string strCurrentRomanNumeral,
            ref string strCurrentNumber2, ref string strCurrentUnknown)
        {
            // Number, lowercase letter, roman numeral, number
            string[] strsReturn = (string[])Array.CreateInstance(typeof(string), 2);

            if (rgxNumber.IsMatch(strIncomingStructureElement))
            {
                if (rgxRomanNumeral.IsMatch(strLastStructureElement) && Convert.ToInt32(strIncomingStructureElement) > 1) //add after the roman numeral (strCurrentNumber2)
                {
                    strCurrentNumber2 = strIncomingStructureElement;
                    strCurrentUnknown = "";
                    strsReturn[1] = "Second Number";
                }
                else //1 && after a roman numeral OR >1 && not after a roman numeral
                {
                    strCurrentNumber1 = strIncomingStructureElement;
                    strCurrentLetter = "";
                    strCurrentRomanNumeral = "";
                    strCurrentNumber2 = "";
                    strCurrentUnknown = "";
                    strsReturn[1] = "First Number";
                }
            }
            else if (rgxLetter.IsMatch(strIncomingStructureElement))
            {
                strCurrentLetter = strIncomingStructureElement;
                strCurrentRomanNumeral = "";
                strCurrentNumber2 = "";
                strCurrentUnknown = "";
                strsReturn[1] = "Letter";
            }
            else if (rgxRomanNumeral.IsMatch(strIncomingStructureElement))
            {
                strCurrentRomanNumeral = strIncomingStructureElement;
                strCurrentNumber2 = "";
                strCurrentUnknown = "";
                strsReturn[1] = "Roman Numeral";
            }
            else
            {
                //throw new Exception("ERROR finding Structure ID");
                strCurrentUnknown = strIncomingStructureElement;
            }

            strsReturn[0] = strCurrentNumber1 + strCurrentLetter + strCurrentRomanNumeral + strCurrentNumber2 + strCurrentUnknown;

            return strsReturn;
        }

        public void FillHeadingsList()
        {
            foreach (List<PericopeInfo> lData in dPericopes.Values)
            {
                foreach (PericopeInfo piData in lData)
                {
                    if (piData.strPericope1 != "")
                    {
                        if (!lHeadings.Contains(piData.strPericope1))
                        {
                            lHeadings.Add(piData.strPericope1);
                        }
                    }

                    if (piData.strPericope2 != "")
                    {
                        if (!lHeadings.Contains(piData.strPericope2))
                        {
                            lHeadings.Add(piData.strPericope2);
                        }
                    }

                    if (piData.strPericope3 != "")
                    {
                        if (!lHeadings.Contains(piData.strPericope3))
                        {
                            lHeadings.Add(piData.strPericope3);
                        }
                    }

                    if (piData.strPericope4 != "")
                    {
                        if (!lHeadings.Contains(piData.strPericope4))
                        {
                            lHeadings.Add(piData.strPericope4);
                        }
                    }
                }
            }
        }

        public void WriteHeadings(StreamWriter swHeadings)
        {
            swHeadings.WriteLine("Pericope");

            foreach (string strHeading in lHeadings.OrderBy(a => a))
            {
                swHeadings.WriteLine(strHeading);
            }

            swHeadings.Close();
        }

        //return D<1, L<ANY-search headings>> D<2, L<ALL-search headings>>
        public Dictionary<int, List<string>> Search(List<string> lSearchTerms)
        {
            Dictionary<int, List<string>> dReturn = new();

            List<string> lSearchAny = new();
            List<string> lSearchAll = new();

            foreach (string strSearchTerm in lSearchTerms)
            {
                foreach (string strHeading in lHeadings)
                {
                    if (strHeading.Contains(strSearchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        lSearchAny.Add(strHeading);
                    }
                }
            }

            foreach (string strHeading in lHeadings)
            {
                int intFoundTerms = 0;

                foreach (string strSearchTerm in lSearchTerms)
                {
                    if (strHeading.Contains(strSearchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        intFoundTerms++;
                    }
                }

                if (lSearchTerms.Count == intFoundTerms)
                {
                    lSearchAll.Add(strHeading);
                }
            }

            dReturn.Add(1, lSearchAny);
            dReturn.Add(2, lSearchAll);

            return dReturn;
        }

        public List<PericopeInfo> GetPericopeInfoObjectFromHeading(string strHeading)
        {
            return dPericopes[strHeading];
        }

        public List<PericopeInfo> GetPericopeInfoObjectsFromHeadings(ref List<string> lHeadings)
        {
            List<PericopeInfo> lAllSearchedPericopeInfo = new();

            foreach (string strPericope in lHeadings)
            {
                foreach (PericopeInfo pdCurrent in GetPericopeInfoObjectFromHeading(strPericope))
                {
                    lAllSearchedPericopeInfo.Add(pdCurrent);
                }
            }

            return lAllSearchedPericopeInfo;
        }

        public string[] GetHeadingsFromReference(string strBookName, int intChapterNumber, int intVerseNumber)
        {
            string[] strsReturn = (string[])Array.CreateInstance(typeof(string), 4);

            //instantiate the return array with empty strings so no nulls come through
            strsReturn[0] = "";
            strsReturn[1] = "";
            strsReturn[2] = "";
            strsReturn[3] = "";

            foreach (List<PericopeInfo> lCurrent in dPericopes.Values)
            {
                foreach (PericopeInfo piCurrent in lCurrent)
                {
                    bool bContinue = false;

                    if (strBookName == piCurrent.strBookName &&
                        intChapterNumber == piCurrent.intStartChapterNumber &&
                        intVerseNumber >= piCurrent.intStartVerseNumber &&
                        intVerseNumber <= piCurrent.intEndVerseNumber)
                    {
                        if (piCurrent.strPericope1.Trim() != "")
                        {
                            strsReturn[0] = piCurrent.strPericope1.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope2.Trim() != "")
                        {
                            strsReturn[1] = piCurrent.strPericope2.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope3.Trim() != "")
                        {
                            strsReturn[2] = piCurrent.strPericope3.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope4.Trim() != "")
                        {
                            strsReturn[3] = piCurrent.strPericope4.Trim();
                            bContinue = true;
                        }

                        if (bContinue == false)
                        {
                            throw new Exception("Error in PericopedSections.GetHeadingFromReference 1");
                        }
                    }
                    else if (strBookName == piCurrent.strBookName &&
                        intChapterNumber == piCurrent.intEndChapterNumber &&
                        intVerseNumber <= piCurrent.intEndVerseNumber)
                    {
                        if (piCurrent.strPericope1.Trim() != "")
                        {
                            strsReturn[0] = piCurrent.strPericope1.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope2.Trim() != "")
                        {
                            strsReturn[1] = piCurrent.strPericope2.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope3.Trim() != "")
                        {
                            strsReturn[2] = piCurrent.strPericope3.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope4.Trim() != "")
                        {
                            strsReturn[3] = piCurrent.strPericope4.Trim();
                            bContinue = true;
                        }

                        if (bContinue == false)
                        {
                            throw new Exception("Error in PericopedSections.GetHeadingFromReference 2");
                        }
                    }
                    else if (strBookName == piCurrent.strBookName &&
                        intChapterNumber > piCurrent.intStartChapterNumber &&
                        intChapterNumber < piCurrent.intEndChapterNumber)
                    {
                        if (piCurrent.strPericope1.Trim() != "")
                        {
                            strsReturn[0] = piCurrent.strPericope1.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope2.Trim() != "")
                        {
                            strsReturn[1] = piCurrent.strPericope2.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope3.Trim() != "")
                        {
                            strsReturn[2] = piCurrent.strPericope3.Trim();
                            bContinue = true;
                        }

                        if (piCurrent.strPericope4.Trim() != "")
                        {
                            strsReturn[3] = piCurrent.strPericope4.Trim();
                            bContinue = true;
                        }

                        if (bContinue == false)
                        {
                            throw new Exception("Error in PericopedSections.GetHeadingFromReference 3");
                        }
                    }
                }
            }

            return strsReturn;
        }

        //returns D<verseid, the last heading entry (strHeading1, 2, 3, or 4) for the verseid>
        //Remove overlapping pericope headings and accept only the most inner ones
        public Dictionary<int, string> GetLeafPericopeFromAllVerseIDs(ref Dictionary<int, KJVStrongs.Verse> dVerses)
        {
            Dictionary<int, string> dReturn = new();
            Dictionary<int, string[]> dAllPericopes = GetAllPericopeHeadingsFromAllVerseIDs(ref dVerses);

            foreach (int intVerseID in dAllPericopes.Keys)
            {
                string strHeading1 = dAllPericopes[intVerseID][0];
                string strHeading2 = dAllPericopes[intVerseID][1];
                string strHeading3 = dAllPericopes[intVerseID][2];
                string strHeading4 = dAllPericopes[intVerseID][3];

                string strLeafPericope = "";

                if (strHeading4 != null)
                {
                    strLeafPericope = strHeading4;
                }
                else if (strHeading3 != null)
                {
                    strLeafPericope = strHeading3;
                }
                else if (strHeading2 != null)
                {
                    strLeafPericope = strHeading2;
                }
                else if (strHeading1 != null)
                {
                    strLeafPericope = strHeading1;
                }

                dReturn.Add(intVerseID, strLeafPericope);
            }

            return dReturn;
        }

        public List<string> CreateNewWordIDsForHeadings(ref Dictionary<int, string> dWordIDs)
        {
            List<string> lHeadingWordsNotFound = new();

            if (lHeadings.Count == 0)
            {
                FillHeadingsList();
            }

            foreach (string strHeading in lHeadings)
            {
                foreach (string strWord in strHeading.ToLower().Trim().Split())
                {
                    if (strWord.Trim() != "")
                    {
                        if (!dWordIDs.Values.Contains(strWord))
                        {
                            if (!lHeadingWordsNotFound.Contains(strWord))
                            {
                                lHeadingWordsNotFound.Add(strWord);
                            }
                        }
                    }
                }
            }

            return lHeadingWordsNotFound;
        }

        //returns D<position, word id> for each word in a position
        //run CreateNewWordIDsForHeadings first to ensure each word has a wordid
        public Dictionary<int, int> GetHeadingPositionsWordIDs(ref Dictionary<int, string> dWordIDs, string strHeading)
        {
            Dictionary<int, int> dHeadingWordIDs = new(); //D<Position, WordID>

            if (strHeading.Trim() == "")
            {
                return dHeadingWordIDs;
            }

            foreach (string strWord in strHeading.ToLower().Trim().Split())
            {
                if (strWord.Trim() != "")
                {
                    if (dWordIDs.Values.Contains(strWord))
                    {
                        dHeadingWordIDs.Add(dHeadingWordIDs.Count() + 1, dWordIDs.Where(a => a.Value.ToLower().Trim() == strWord).Select(a => a.Key).First());
                    }
                    else //this might throw only if CreateNewWordIDsForHeadings hasn't been called yet
                    {
                        throw new Exception("PericopedSections.GetHeadingUWIDs found a word in the heading that does not exist in the Bible text");
                    }
                }
            }

            return dHeadingWordIDs;
        }

        //returns D<Heading index, D<Word Position in the Heading, WordID>>
        //runs GetHeadingPositionsWordIDs for each heading
        public Dictionary<string, Dictionary<int, int>> GetHeadingsPositionWordIDs(ref Dictionary<int, string> dWordIDs, List<string> lHeadings)
        {
            Dictionary<string, Dictionary<int, int>> dReturn = new();

            foreach (string strHeading in lHeadings)
            {
                Dictionary<int, int> dHeadingWordIDs = GetHeadingPositionsWordIDs(ref dWordIDs, strHeading);

                dReturn.Add(strHeading, dHeadingWordIDs);
            }

            return dReturn;
        }

        public void CreateNewPericopeWordIDs(ref Dictionary<int, string> dWordIDs,
            ref StreamWriter swNewPericopeWordIDs)
        {
            //add WordIDs for new words in headings and write new WordIDs
            List<string> lNewPericopeWordIDs = CreateNewWordIDsForHeadings(ref dWordIDs);

            foreach (string strNewPericopeWordID in lNewPericopeWordIDs)
            {
                dWordIDs.Add(dWordIDs.Count() + 1, strNewPericopeWordID);
                swNewPericopeWordIDs.WriteLine(dWordIDs.Count().ToString() + " ^ " + strNewPericopeWordID);
            }

            swNewPericopeWordIDs.Close();
        }

        //D<word, D<pericope heading, word count in this heading>>
        public Dictionary<string, Dictionary<string, int>> GetWordsPericopeHeadingsCount()
        {
            Dictionary<string, Dictionary<string, int>> dReturn = new();

            if (lHeadings.Count == 0)
            {
                FillHeadingsList();
            }

            foreach (string strHeading in lHeadings)
            {
                foreach (string strWord in strHeading.ToLower().Trim().Split())
                {
                    string strCleanedWord = rgxNotWords.Replace(strWord, "");

                    if (strCleanedWord.Trim() != "" &&
                        !dReturn.Keys.Contains(strCleanedWord))
                    {
                        dReturn.Add(strCleanedWord, new Dictionary<string, int>());
                    }
                    else if (strCleanedWord.Trim() != "")
                    {
                        if (!dReturn[strCleanedWord].ContainsKey(strHeading))
                        {
                            dReturn[strCleanedWord].Add(strHeading, 1);
                        }
                        else
                        {
                            dReturn[strCleanedWord][strHeading]++;
                        }
                    }
                }
            }

            return dReturn;
        }

        public Dictionary<int, string[]> GetAllPericopeHeadingsFromAllVerseIDs(ref Dictionary<int, KJVStrongs.Verse> dVerses) //the reason lReferences is used instead of kjvs.dVerses is to remove a dependency from this class on KJVStrongs
        {
            //Returns D<VerseID, A[OrderedPericopeHeadings]>
            //From string[]: 0-BookName 1-ChapterNumber 2-VerseNumber 3-VerseID
            //OrderedPericopeHeadings are ordered by heading structure (ie. first number, letter, roman numeral, second number)

            Dictionary<int, string[]> dReturn = new(); //D<VerseID, A[headings]>

            foreach (int intVerseID in dVerses.Keys.OrderBy(a => a))
            {
                //strsHeadings[] is ordered by heading structure (ie. first number, letter, roman numeral, second number)
                string[] strsHeadings = GetHeadingsFromReference(dVerses[intVerseID].strBookName.Trim(),
                    dVerses[intVerseID].intChapterNumber, dVerses[intVerseID].intVerseNumber);

                dReturn.Add(intVerseID, strsHeadings);
            }

            return dReturn;
        }

        public void CreatePericopeDivisionFiles(ref Dictionary<int, KJVStrongs.Verse> dVerses, string strDataDirectory)
        {
            StreamWriter sw1 = new(strDataDirectory + "VersePericopeDivisions1.csv");
            StreamWriter sw2 = new(strDataDirectory + "VersePericopeDivisions2.csv");
            StreamWriter sw3 = new(strDataDirectory + "VersePericopeDivisions3.csv");
            StreamWriter sw4 = new(strDataDirectory + "VersePericopeDivisions4.csv");

            Dictionary<int, string[]> dVerseIDPericopes = GetAllPericopeHeadingsFromAllVerseIDs(ref dVerses);

            sw1.WriteLine("verse id ^ verse text ^ pericope");
            sw2.WriteLine("verse id ^ verse text ^ pericope");
            sw3.WriteLine("verse id ^ verse text ^ pericope");
            sw4.WriteLine("verse id ^ verse text ^ pericope");

            foreach (int intVerseID in dVerseIDPericopes.Keys.OrderBy(a => a))
            {
                if (dVerseIDPericopes[intVerseID][0].Trim() != "")
                {
                    sw1.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ " +
                        dVerseIDPericopes[intVerseID][0]);
                }
                else
                {
                    sw1.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ null");
                }

                if (dVerseIDPericopes[intVerseID][1].Trim() != "")
                {
                    sw2.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ " +
                        dVerseIDPericopes[intVerseID][1]);
                }
                else
                {
                    sw2.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ null");
                }

                if (dVerseIDPericopes[intVerseID][2].Trim() != "")
                {
                    sw3.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID] + " ^ " +
                        dVerseIDPericopes[intVerseID][2]);
                }
                else
                {
                    sw3.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ null");
                }

                if (dVerseIDPericopes[intVerseID][3].Trim() != "")
                {
                    sw4.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ " +
                        dVerseIDPericopes[intVerseID][3]);
                }
                else
                {
                    sw4.WriteLine(intVerseID.ToString() + " ^ " + dVerses[intVerseID].strText + " ^ null");
                }
            }

            sw1.Close();
            sw2.Close();
            sw3.Close();
            sw4.Close();
        }

        //Returns D<heading, L<verse ids>>
        public Dictionary<string, List<int>> GetAllVerseIDsFromAllPericopeHeadings(ref Dictionary<int, string[]> dVerseIDHeadings)
        {
            Dictionary<string, List<int>> dReturn = new();

            foreach (int intVerseID in dVerseIDHeadings.Keys)
            {
                foreach (string strHeading in dVerseIDHeadings[intVerseID])
                {
                    if (strHeading != null)
                    {
                        if (!dReturn.ContainsKey(strHeading))
                        {
                            dReturn.Add(strHeading, new List<int>());
                        }

                        if (!dReturn[strHeading].Contains(intVerseID))
                        {
                            try
                            {
                                dReturn[strHeading].Add(intVerseID);
                            }
                            catch
                            {
                                throw new Exception("dPericopeHeadingsVerses needs a bigger Verse array");
                            }
                        }
                    }
                }
            }

            return dReturn;
        }

        public void AddHeadingsToPericopesDictionary(ref List<string> lHeadings)
        {
            foreach (PericopeInfo pi in GetPericopeInfoObjectsFromHeadings(ref lHeadings))
            {
                if (pi.strPericope1.Trim() != "")
                {
                    if (!dPericopes.ContainsKey(pi.strPericope1))
                    {
                        dPericopes.Add(pi.strPericope1, new List<PericopeInfo>());
                    }

                    if (!dPericopes[pi.strPericope1].Contains(pi))
                    {
                        dPericopes[pi.strPericope1].Add(pi);
                    }
                }

                if (pi.strPericope2.Trim() != "")
                {
                    if (!dPericopes.ContainsKey(pi.strPericope2))
                    {
                        dPericopes.Add(pi.strPericope2, new List<PericopeInfo>());
                    }

                    if (!dPericopes[pi.strPericope2].Contains(pi))
                    {
                        dPericopes[pi.strPericope2].Add(pi);
                    }
                }

                if (pi.strPericope3.Trim() != "")
                {
                    if (!dPericopes.ContainsKey(pi.strPericope3))
                    {
                        dPericopes.Add(pi.strPericope3, new List<PericopeInfo>());
                    }

                    if (!dPericopes[pi.strPericope3].Contains(pi))
                    {
                        dPericopes[pi.strPericope3].Add(pi);
                    }
                }

                if (pi.strPericope4.Trim() != "")
                {
                    if (!dPericopes.ContainsKey(pi.strPericope4))
                    {
                        dPericopes.Add(pi.strPericope4, new List<PericopeInfo>());
                    }

                    if (!dPericopes[pi.strPericope4].Contains(pi))
                    {
                        dPericopes[pi.strPericope4].Add(pi);
                    }
                }
            }
        }

        public Dictionary<int, int> FindLastVerseOfEachPericopeObject()
        {
            Dictionary<int, int> dReturn = new();

            foreach (string strPericopeHeading in dPericopes.Keys)
            {
                foreach (PericopeInfo pdCurrent in dPericopes[strPericopeHeading])
                {
                    if (!dReturn.ContainsKey(pdCurrent.intID))
                    {
                        dReturn.Add(pdCurrent.intID, pdCurrent.intEndVerseNumber);
                    }
                }
            }

            return dReturn;
        }

        //public Dictionary<string, Dictionary<string, double>> PericopePhraseHashCount(ref Dictionary<int, Verse> dVerses,
        //    ref Dictionary<int, string[]> dVerseIDHeadings, bool bLeavesOnly) //returns D<phrase, D<pericope, count / total count>>
        //{
        //    //Dictionary<string, List<int>> dPericopeVerses = GetAllVerseIDsFromAllPericopeHeadings(ref dVerses, ref dVerseIDHeadings);
        //    //Dictionary<string, Dictionary<string, int>> dPericopePhraseCounts = new Dictionary<string, Dictionary<string, int>>(); //D<pericope, D<phrase, count>>
        //    //Dictionary<string, Dictionary<int, Dictionary<string, int>>> dPericopeVersePhraseCounts = new Dictionary<string, Dictionary<int, Dictionary<string, int>>>(); //D<pericope, D<verse id, D<phrase, count>>>
        //    Dictionary<string, int> dPhraseTotalCounts = new Dictionary<string, int>(); //D<phrase, count throughout Bible>
        //    //Dictionary<string, Dictionary<string, double>> dPericopePhraseRatios = new Dictionary<string, Dictionary<string, double>>(); //D<Pericope, D<Phrase, (double)TotalCountRatio>>, where TotalCountRatio = pericope phrase count / total text phrase count
        //    //Dictionary<string, List<string>> dTopPericopePhrase = new Dictionary<string, List<string>>();

        //    Dictionary<int, List<string>> dVersePhrases = new Dictionary<int, List<string>>();
        //    //Dictionary<int, Dictionary<int, List<string>>> dIntersectingPhrases =
        //    //    new Dictionary<int, Dictionary<int, List<string>>>();
        //    //Dictionary<int, Dictionary<int, Dictionary<string, int>>> dIntersectingPhraseCounts =
        //    //    new Dictionary<int, Dictionary<int, Dictionary<string, int>>>(); //D<verse id, D<verse id compare, D<phrase, total count of phrase among the two verses>>>
        //    Dictionary<int, Dictionary<int, Dictionary<string, double>>> dRelatedness =
        //        new Dictionary<int, Dictionary<int, Dictionary<string, double>>>();
        //    Dictionary<int, List<string>> dPhrasalIntersections =
        //        new Dictionary<int, List<string>>();
        //    Dictionary<int, Dictionary<string, int>> dPhrasalIntersectionCounts =
        //        new Dictionary<int, Dictionary<string, int>>();

        //    int intVerseIDMultiplier = 0;
        //    bool bContinue = true;
        //    int intVerseIDMax = dVerses.Keys.Max();

        //    for (int intVerseID = 1; intVerseID <= intVerseIDMax; intVerseID++)
        //    {   
        //        dVersePhrases.Add(intVerseID, new List<string>());

        //        foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys)
        //        {
        //            string strPhrase = dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText.Trim().ToLower();

        //            dVersePhrases[intVerseID].Add(strPhrase);

        //            if (!dPhraseTotalCounts.ContainsKey(strPhrase))
        //            {
        //                dPhraseTotalCounts.Add(strPhrase, 0);
        //            }

        //            dPhraseTotalCounts[strPhrase]++;
        //        }
        //    }

        //    while (bContinue == true)
        //    {
        //        StreamWriter swIntersecingPhrases = new StreamWriter(
        //                @"Data\IntersectingPhrases_" + (intVerseIDMultiplier + 1).ToString() + ".csv");

        //        swIntersecingPhrases.WriteLine("VerseID1 ^ VerseID2 ^ Phrase ^ Relatedness ^ Verse1Text ^ Verse2Text");

        //        dPhrasalIntersections.Clear();
        //        dPhrasalIntersectionCounts.Clear();

        //        for (int intVerseID = 1 + (100 * intVerseIDMultiplier); intVerseID <= 100 + (100 * intVerseIDMultiplier); intVerseID++)//foreach (int intVerseID in dVerses.Keys)
        //        {
        //            dPhrasalIntersectionCounts.Add(intVerseID, new Dictionary<string, int>());

        //            foreach (string strPhrase in dVersePhrases[intVerseID])
        //            {

        //                if (!dPhrasalIntersectionCounts[intVerseID].ContainsKey(strPhrase))
        //                {
        //                    dPhrasalIntersectionCounts[intVerseID].Add(strPhrase, 0);
        //                }

        //                dPhrasalIntersectionCounts[intVerseID][strPhrase]++;
        //            }

        //            foreach (int intVerseIDCompare in dVerses.Keys)
        //            {
        //                if (intVerseID != intVerseIDCompare)
        //                {
        //                    List<string> lIntersectingPhrases = dVersePhrases[intVerseID]
        //                        .Intersect<string>(dVersePhrases[intVerseIDCompare]).ToList();
        //                    //StringBuilder sbIntersectingPhrases = new StringBuilder();

        //                    if (lIntersectingPhrases.Count > 0)
        //                    {
        //                        //dPhrasalIntersections.Add(intVerseIDCompare, new List<string>());
        //                        //dPhrasalIntersectionCounts.Add(intVerseIDCompare, new Dictionary<string, int>());

        //                        //dPhrasalIntersections[intVerseIDCompare] = lIntersectingPhrases;

        //                        foreach (string strIntersectingPhrase in lIntersectingPhrases)
        //                        {
        //                            int intPhraseCount1 = dVerses[intVerseID].dPhraseCounts[strIntersectingPhrase];
        //                            int intPhraseCount2 = dVerses[intVerseIDCompare].dPhraseCounts[strIntersectingPhrase];

        //                            //sbIntersectingPhrases.Append(strIntersectingPhrase + " ^ " +
        //                            //    ((double)((double)(intPhraseCount1 + intPhraseCount2) /
        //                            //    (double)dPhraseTotalCounts[strIntersectingPhrase])).ToString() + " ^ ");

        //                            swIntersecingPhrases.WriteLine(intVerseID.ToString() + " ^ " +
        //                                intVerseIDCompare.ToString() + " ^ " + strIntersectingPhrase + " ^ " +
        //                                ((double)((double)(intPhraseCount1 + intPhraseCount2) /
        //                                (double)dPhraseTotalCounts[strIntersectingPhrase])).ToString() + " ^ " +
        //                                dVerses[intVerseID].strText + " ^ " + dVerses[intVerseIDCompare].strText);

        //                            //dPhrasalIntersectionCounts[intVerseIDCompare]
        //                            //    .Add(strIntersectingPhrase, intPhraseCount1 + intPhraseCount2);
        //                        }

        //                        //sbIntersectingPhrases.Remove(sbIntersectingPhrases.Length - 4, 3);

        //                        //swIntersecingPhrases.WriteLine(intVerseID.ToString() + " ^ " +
        //                        //        intVerseIDCompare.ToString() + " ^ " + sbIntersectingPhrases.ToString());
        //                    } 
        //                }
        //            }

        //            if (intVerseID == intVerseIDMax)
        //            {
        //                bContinue = false;
        //                break;
        //            }
        //        }

        //        swIntersecingPhrases.Close();
        //        intVerseIDMultiplier++;
        //    }

        //    //foreach (int intVerseID in dIntersectingPhrases.Keys)
        //    //{
        //    //    dRelatedness.Add(intVerseID, new Dictionary<int, Dictionary<string, double>>());

        //    //    foreach (int intVerseIDCompare in dIntersectingPhrases[intVerseID].Keys)
        //    //    {
        //    //        dRelatedness[intVerseID].Add(intVerseIDCompare, new Dictionary<string, double>());

        //    //        foreach (string strPhrase in dIntersectingPhrases[intVerseID][intVerseIDCompare])
        //    //        {
        //    //            double dblRelatedness = (double)((double)dIntersectingPhraseCounts[intVerseID][intVerseIDCompare][strPhrase] /
        //    //                (double)dPhraseTotalCounts[strPhrase]);

        //    //            dRelatedness[intVerseID][intVerseIDCompare].Add(strPhrase, dblRelatedness);
        //    //        }
        //    //    }
        //    //}

        //    //foreach (string strPericope in dPericopeVerses.Keys)
        //    //{
        //    //    if (!dPericopeVersePhraseCounts.ContainsKey(strPericope))
        //    //    {
        //    //        dPericopeVersePhraseCounts.Add(strPericope, new Dictionary<int, Dictionary<string, int>>());
        //    //    }

        //    //    //if (!dPericopePhraseCounts.ContainsKey(strPericope))
        //    //    //{
        //    //    //    dPericopePhraseCounts.Add(strPericope, new Dictionary<string, int>());
        //    //    //}

        //    //    foreach (int intVerseID in dPericopeVerses[strPericope])
        //    //    {
        //    //        foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys.OrderBy(a => a))
        //    //        {
        //    //            Phrase phPhrase = dVerses[intVerseID].dPhrases[intPhraseID];
        //    //            string strPhrase = phPhrase.strPhraseText;

        //    //            if (!dPericopeVersePhraseCounts[strPericope].ContainsKey(intVerseID))
        //    //            {
        //    //                dPericopeVersePhraseCounts[strPericope].Add(intVerseID, new Dictionary<string, int>());
        //    //            }

        //    //            if (!dPericopeVersePhraseCounts[strPericope][intVerseID].ContainsKey(strPhrase))
        //    //            {
        //    //                dPericopeVersePhraseCounts[strPericope][intVerseID].Add(strPhrase, 0);
        //    //            }

        //    //            dPericopeVersePhraseCounts[strPericope][intVerseID][strPhrase]++;

        //    //            //if (!dPericopePhraseCounts[strPericope].ContainsKey(strPhrase))
        //    //            //{
        //    //            //    dPericopePhraseCounts[strPericope].Add(strPhrase, 0);
        //    //            //}

        //    //            //dPericopePhraseCounts[strPericope][strPhrase]++;

        //    //            if (!dPhraseTotalCounts.ContainsKey(strPhrase))
        //    //            {
        //    //                dPhraseTotalCounts.Add(strPhrase, 0);
        //    //            }

        //    //            dPhraseTotalCounts[strPhrase]++;
        //    //        }
        //    //    }
        //    //}



        //    //D<pericope, D<verse id, D<pericope compare, D<verse id compare, L<phrases>>>>>
        //    //Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<int, List<string>>>>> dIntersectionStrength =
        //    //    new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<int, List<string>>>>>();

        //    //foreach (string strPericope in dPericopeVersePhraseCounts.Keys)
        //    //{
        //    //    if (!dIntersectionStrength.ContainsKey(strPericope))
        //    //    {
        //    //        dIntersectionStrength.Add(strPericope, new Dictionary<int, Dictionary<string, Dictionary<int, List<string>>>>());
        //    //    }

        //    //    foreach (int intVerseID in dPericopeVersePhraseCounts[strPericope].Keys)
        //    //    {
        //    //        if (!dIntersectionStrength[strPericope].ContainsKey(intVerseID))
        //    //        {
        //    //            dIntersectionStrength[strPericope].Add(intVerseID, new Dictionary<string, Dictionary<int, List<string>>>());
        //    //        }

        //    //        foreach (string strPericopeCompare in dPericopeVersePhraseCounts.Keys)
        //    //        {
        //    //            if (!dIntersectionStrength.ContainsKey(strPericopeCompare))
        //    //            {
        //    //                dIntersectionStrength.Add(strPericopeCompare, new Dictionary<int, Dictionary<string, Dictionary<int, List<string>>>>());
        //    //            }

        //    //            foreach (int intVerseIDCompare in dPericopeVersePhraseCounts[strPericopeCompare].Keys)
        //    //            {
        //    //                if (!dIntersectionStrength[strPericopeCompare].ContainsKey(intVerseIDCompare))
        //    //                {
        //    //                    dIntersectionStrength[strPericopeCompare].Add(intVerseIDCompare, new Dictionary<string, Dictionary<int, List<string>>>());
        //    //                }

        //    //                if (strPericope != strPericopeCompare)
        //    //                {
        //    //                    if (dPericopeVersePhraseCounts[strPericopeCompare].ContainsKey(intVerseIDCompare))
        //    //                    {
        //    //                        var varIntersect = dPericopeVersePhraseCounts[strPericope][intVerseID].Keys
        //    //                            .Intersect<string>(dPericopeVersePhraseCounts[strPericopeCompare][intVerseIDCompare].Keys);
        //    //                        List<string> lIntersect = new List<string>();

        //    //                        foreach (string strIntersect in varIntersect)
        //    //                        {
        //    //                            lIntersect.Add(strIntersect);
        //    //                        }

        //    //                        if (!dIntersectionStrength[strPericope][intVerseID].ContainsKey(strPericopeCompare))
        //    //                        {
        //    //                            dIntersectionStrength[strPericope][intVerseID].Add(strPericopeCompare, new Dictionary<int, List<string>>());
        //    //                            dIntersectionStrength[strPericope][intVerseID][strPericopeCompare].Add(intVerseIDCompare, lIntersect);
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            if (!dIntersectionStrength[strPericope][intVerseID][strPericopeCompare].ContainsKey(intVerseIDCompare))
        //    //                            {
        //    //                                dIntersectionStrength[strPericope][intVerseID][strPericopeCompare].Add(intVerseIDCompare, new List<string>());
        //    //                            }

        //    //                            dIntersectionStrength[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare].AddRange(lIntersect);
        //    //                            dIntersectionStrength[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare] =
        //    //                                dIntersectionStrength[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare].Distinct().ToList();
        //    //                        }
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //The number of common phrases under each pair of pericopes divided by
        //    //the number of phrases in the first pericope in that pair
        //    //This should be a good measure of relatedness between the passages 
        //    //under the pair of pericopes.
        //    //Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<int, double>>>> dIntersectionStrengthRatio =
        //    //   new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<int, double>>>>();

        //    //foreach (string strPericope in dIntersectionStrength.Keys)
        //    //{
        //    //    foreach (int intVerseID in dPericopeVersePhraseCounts[strPericope].Keys)
        //    //    {
        //    //        foreach (string strPericopeCompare in dIntersectionStrength[strPericope][intVerseID].Keys)
        //    //        {
        //    //            foreach (int intVerseIDCompare in dIntersectionStrength[strPericope][intVerseID][strPericopeCompare].Keys)
        //    //            {
        //    //                if (strPericope != strPericopeCompare)
        //    //                {
        //    //                    if (!dIntersectionStrengthRatio.ContainsKey(strPericope))
        //    //                    {
        //    //                        dIntersectionStrengthRatio.Add(strPericope, new Dictionary<int, Dictionary<string, Dictionary<int, double>>>());                                    
        //    //                    }

        //    //                    if (!dIntersectionStrengthRatio[strPericope].ContainsKey(intVerseID))
        //    //                    {
        //    //                        dIntersectionStrengthRatio[strPericope].Add(intVerseID, new Dictionary<string, Dictionary<int, double>>());
        //    //                    }

        //    //                    if (!dIntersectionStrengthRatio[strPericope][intVerseID].ContainsKey(strPericopeCompare))
        //    //                    {
        //    //                        dIntersectionStrengthRatio[strPericope][intVerseID].Add(strPericopeCompare, new Dictionary<int, double>());
        //    //                    }

        //    //                    if (!dIntersectionStrengthRatio[strPericope][intVerseID][strPericopeCompare].ContainsKey(intVerseIDCompare))
        //    //                    {
        //    //                        dIntersectionStrengthRatio[strPericope][intVerseID][strPericopeCompare].Add(intVerseIDCompare,
        //    //                            (double)dIntersectionStrength[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare].Count /
        //    //                            dPericopeVersePhraseCounts[strPericope][intVerseID].Count);
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        //average in each subsequent ratio
        //    //                        dIntersectionStrengthRatio[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare] += 
        //    //                            (double)dIntersectionStrength[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare].Count /
        //    //                            dPericopeVersePhraseCounts[strPericope][intVerseID].Count;
        //    //                        dIntersectionStrengthRatio[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare] *= 0.5;
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //Dictionary<double, List<Dictionary<string, string>>> dRelatedness =
        //    //    new Dictionary<double, List<Dictionary<string, string>>>();
        //    //Dictionary<double, List<Dictionary<string, string>>> dOrderedRelatedness =
        //    //    new Dictionary<double, List<Dictionary<string, string>>>();

        //    //foreach (string strPericope in dIntersectionStrengthRatio.Keys)
        //    //{
        //    //    foreach (int intVerseID in dIntersectionStrengthRatio[strPericope].Keys)
        //    //    {
        //    //        foreach (string strPericopeCompare in dIntersectionStrengthRatio[strPericope][intVerseID].Keys)
        //    //        {
        //    //            foreach (int intVerseIDCompare in dIntersectionStrengthRatio[strPericope][intVerseID][strPericopeCompare].Keys)
        //    //            {
        //    //                double dblRelatedness = dIntersectionStrengthRatio[strPericope][intVerseID][strPericopeCompare][intVerseIDCompare];
        //    //                Dictionary<string, string> dPericopePair = new Dictionary<string, string>();

        //    //                dPericopePair.Add(strPericope, strPericopeCompare);

        //    //                if (!dRelatedness.ContainsKey(dblRelatedness))
        //    //                {
        //    //                    dRelatedness.Add(dblRelatedness, new List<Dictionary<string, string>>());
        //    //                }

        //    //                dRelatedness[dblRelatedness].Add(dPericopePair);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //List<string> lLeafPericopes = GetLeafPericopes(ref dVerses);

        //    //foreach (double dblRelatedness in dRelatedness.Keys.OrderByDescending(a => a))
        //    //{
        //    //    Dictionary<string, string> dPericopePairClone = new Dictionary<string, string>();

        //    //    dOrderedRelatedness.Add(dblRelatedness, new List<Dictionary<string, string>>());

        //    //    foreach (Dictionary<string, string> dPericopePair in dRelatedness[dblRelatedness])
        //    //    {
        //    //        if (bLeavesOnly == true)
        //    //        {
        //    //            foreach (string strPericopeTest in dPericopePair.Keys)
        //    //            {
        //    //                if (lLeafPericopes.Contains(strPericopeTest) &&
        //    //                    lLeafPericopes.Contains(dPericopePair[strPericopeTest]) &&
        //    //                    !dPericopePairClone.ContainsKey(strPericopeTest))
        //    //                {
        //    //                    dPericopePairClone.Add(strPericopeTest, dPericopePair[strPericopeTest]);
        //    //                }
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            dPericopePairClone = dPericopePair;
        //    //        }

        //    //        dOrderedRelatedness[dblRelatedness].Add(dPericopePairClone);
        //    //    }
        //    //}

        //    //foreach (string strPericope in dPericopeVerses.Keys)
        //    //{
        //    //    foreach (int intVerseID in dPericopeVerses[strPericope])
        //    //    {
        //    //        foreach (int intPhraseID in dVerses[intVerseID].dPhrases.Keys.OrderBy(a => a))
        //    //        {
        //    //            string strPhrase = dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText;

        //    //            if (!dPericopePhraseRatios.ContainsKey(strPericope))
        //    //            {
        //    //                dPericopePhraseRatios.Add(strPericope, new Dictionary<string, double>());
        //    //            }

        //    //            if (!dPericopePhraseRatios[strPericope].ContainsKey(strPhrase))
        //    //            {
        //    //                dPericopePhraseRatios[strPericope].Add(strPhrase,
        //    //                    (double)dPericopeVersePhraseCounts[strPericope][intVerseID][strPhrase] / 
        //    //                    (double)dPhraseTotalCounts[strPhrase]);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //foreach (string strPericope in dPericopePhraseRatios.Keys)
        //    //{
        //    //    Dictionary<string, double> dPhraseCountsTemp = dPericopePhraseRatios[strPericope];
        //    //    double dblTopRatio = dPhraseCountsTemp.Min(a => a.Value);

        //    //    var a = from b in dPhraseCountsTemp
        //    //            where b.Value == dblTopRatio
        //    //            select b.Key;

        //    //    foreach (string strPhrase in a)
        //    //    {
        //    //        if (!dTopPericopePhrase.ContainsKey(strPericope))
        //    //        {
        //    //            dTopPericopePhrase.Add(strPericope, new List<string>());
        //    //        }

        //    //        dTopPericopePhrase[strPericope].Add(strPhrase);
        //    //    }
        //    //}

        //    //IOrderedEnumerable<KeyValuePair<string, List<string>>> d = dTopPericopePhrase.OrderByDescending(a => a.Value.Count);

        //    //if (bLeavesOnly == true)
        //    //{
        //    //    List<string> lLeafPericopes = GetLeafPericopes(ref dVerses);
        //    //    Dictionary<double, List<Dictionary<string, string>>> dPericopeRemovals =
        //    //        new Dictionary<double, List<Dictionary<string, string>>>();

        //    //    foreach (string strPericope in lLeafPericopes)
        //    //    {
        //    //        dPericopePhraseRatios.Remove(strPericope);
        //    //    }


        //    //    foreach (double dblRelatedness in dOrderedRelatedness.Keys.OrderByDescending(a => a))
        //    //    {
        //    //        foreach (Dictionary<string, string> dPericopePair in dOrderedRelatedness[dblRelatedness])
        //    //        {
        //    //            foreach (string strPericopeTest in dPericopePair.Keys)
        //    //            {
        //    //                if (!lLeafPericopes.Contains(strPericopeTest))
        //    //                {
        //    //                    if (!dPericopeRemovals.ContainsKey(dblRelatedness))
        //    //                    {
        //    //                        dPericopeRemovals.Add(dblRelatedness, new List<Dictionary<string, string>>());
        //    //                    }

        //    //                    if (!dPericopeRemovals[dblRelatedness].Contains(dPericopePair))
        //    //                    {
        //    //                        dPericopeRemovals[dblRelatedness].Add(dPericopePair);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }

        //    //    foreach (double dblRelatedness in dPericopeRemovals.Keys)
        //    //    {
        //    //        foreach (Dictionary<string, string> dPericopeRemove in dPericopeRemovals[dblRelatedness])
        //    //        {
        //    //            dOrderedRelatedness[dblRelatedness].Remove(dPericopeRemove);
        //    //        }
        //    //    }
        //    //}

        //    //StreamWriter swOrderedRelatedness = new StreamWriter(@"Data\PericopePassage_OrderedRelatedness.txt");

        //    //foreach (double dblRelatedness in dOrderedRelatedness.Keys.OrderByDescending(a => a))
        //    //{
        //    //    foreach (Dictionary<string, string> dPericopePair in dOrderedRelatedness[dblRelatedness])
        //    //    {
        //    //        foreach (string strPericopeTemp in dPericopePair.Keys)
        //    //        {
        //    //            swOrderedRelatedness.WriteLine(dblRelatedness.ToString() + " ^ " + strPericopeTemp
        //    //                + " ^ " + dPericopePair[strPericopeTemp]);
        //    //        }
        //    //    }
        //    //}

        //    //swOrderedRelatedness.Close();

        //    //return dPericopePhraseRatios;
        //    return null;
        //}

        public class WordCount
        {
            public Dictionary<string, int> dTextWordCount = new(); //D<word text, count in all pericopes>
            public Dictionary<string, int> dPericopeWordCount = new(); //D<pericope word text, count in all pericopes>
            public Dictionary<string, Dictionary<string, int>> dWordCountHeadings = new(); //D<word, D<heading in which the word is found, instance count in that heading>>

            public IEnumerable<KeyValuePair<string, int>> GetTextWordCountByCountDescending(ref Dictionary<int, string> dWordIDs, Dictionary<string, PericopeInfo> dPericopes)
            {
                IOrderedEnumerable<KeyValuePair<string, int>> varDescendingText =
                    from record in dTextWordCount
                    orderby record.Key ascending
                    orderby record.Value descending
                    select record;

                foreach (string strPericope in dPericopes.Keys)
                {
                    foreach (string strWord in strPericope.Split())
                    {
                        if (dWordIDs.Values.Contains(strWord))
                        {
                            //dPericopeWordCount.Add(dHeadingWordIDs.Count() + 1, dWordIDs.Where(a => a.Value.ToLower().Trim() == strWord).Select(a => a.Key).First());
                        }
                    }
                }
                return varDescendingText;
            }

            public IEnumerable<KeyValuePair<string, int>> GetPericopeWordCountByCountDescending()
            {
                IOrderedEnumerable<KeyValuePair<string, int>> varDescendingPericope =
                    from record in dPericopeWordCount
                    orderby record.Key ascending
                    orderby record.Value descending
                    select record;

                return varDescendingPericope;
            }

            public void GetWordCountsPerHeading(Dictionary<string, PericopeInfo> dPericopes)
            {
                foreach (string strPericope in dPericopes.Keys)
                {
                    foreach (string strWord in strPericope.Split())
                    {
                        if (!dWordCountHeadings.ContainsKey(strWord.ToLower()))
                        {
                            dWordCountHeadings.Add(strWord.ToLower(), new Dictionary<string, int>());
                        }

                        if (!dWordCountHeadings[strWord.ToLower()].ContainsKey(strPericope))
                        {
                            dWordCountHeadings[strWord.ToLower()].Add(strPericope, 0);
                        }

                        dWordCountHeadings[strWord.ToLower()][strPericope]++;
                    }
                }
            }
        }
    }

    public class PericopeVerse
    {
        public string strBookName = "";
        public int intBookNumber = 0;
        public int intChapterNumber = 0;
        public int intVerseNumber = 0;
        public string strPericope1 = "";
        public string strPericope2 = "";
        public string strPericope3 = "";
        public string strPericope4 = "";
    }

    public class PericopeInfo
    {
        public int intID = 0; //unique to the whole Bible

        //The following reference information is the start and end of the referenced passage
        public string strBookName = "";

        public int intStartChapterNumber = 0;
        public int intStartVerseNumber = 0;

        public int intEndChapterNumber = 0;
        public int intEndVerseNumber = 0; //when this is 0, it's the last verse of the chapter, which will have to be looked up

        //Structure
        public string strStructure = ""; //outline ID unique to the whole book

        public string strPericope1 = ""; //First Number Heading in Berean Data
        public string strPericope2 = ""; //Letter Heading in Berean Data
        public string strPericope3 = ""; //Roman Numeral Heading in Berean Data
        public string strPericope4 = ""; //Second Number Heading in Berean Data

        public string WriteHeader()
        {
            return "Book Name ^ Start Chapter ^ Start Verse ^ End Chapter" +
                " ^ End Verse ^ Structure ^ Pericope 1 ^ Pericope 2 ^ Pericope 3 ^ Pericope 4";
        }

        public string WriteRecord()
        {
            return strBookName + " ^ " + intStartChapterNumber.ToString() + " ^ " +
                intStartVerseNumber.ToString() + " ^ " + intEndChapterNumber.ToString() +
                " ^ " + intEndVerseNumber.ToString() + " ^ " + strStructure + " ^ " +
                strPericope1 + " ^ " + strPericope2 + " ^ " + strPericope3 +
                " ^ " + strPericope4;
        }
    }
}

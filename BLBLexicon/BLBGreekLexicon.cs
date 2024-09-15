using System.Text;
using System.Text.RegularExpressions;

namespace BLBLexicon
{
    public class BLBGreekLexicon
    {
        public int intLexID = 0;
        public string strTransliteration = "";
        public string strPronunciation = "";
        public Dictionary<string, int> dAVTranslations = new();
        public Dictionary<string, string> dExtraTDNTInformation = new(); //D<translation, tdnt reference>; rarely TDNT information will appear in the AV translation data in curly brackets; example G5207
        public List<string> lConjugated = new(); //these translations are representative of various conjugated forms present in the text, represented by their counts being in parenthesis
        public int intTotalTranslated = 0;
        public bool bRoot = false;
        public string strConnection = ""; //corresponding to #, from #, from an unused root (apparently meaning to turn), etc.
        public string strTWOTNumber = "";
        public string strPOS = "";
        public string strGender = "";
        public Dictionary<string, string> dLexicalEntries = new(); //D<Entry number with letter sub-indexes, Lexical Entry>

        public void CreateBLBGreekLexicon(ref StreamReader srBLBGreekLexiconIn,
            ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries)
        {
            ///
            ///NOTE: There's no G2717, G3203-G3302
            ///NOTE: G4483 inserts 9 as the count for the AV translation "say" (there was no count given in the input file),
            /// even though there are 11 instances in KJV, in order to fit the total of 26
            ///

            //Lex["18"] = "
            //agathos {ag-ath-os'}|
            //AV - good 77, good thing 14, that which is good+3588 8, <BR> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;the thing which is good+3588 1, well 1, benefit 1; 102|
            //a primary word|
            //TDNT - 1:10,3|
            //adj|
            //INDENT0--<b>1)</b> of good constitution or nature
            //INDENT0--<b>2)</b> useful, salutary
            //INDENT0--<b>3)</b> good, pleasant, agreeable, joyful, happy
            //INDENT0--<b>4)</b> excellent, distinguished
            //INDENT0--<b>5)</b> upright, honourable
            //";

            int intPartNumber = 0; //Which part of the ';'-separated line are we in?
            int intLexID = 0;
            Regex rgxEnumeration = new(@"(?<ind>INDENT)(?<inum>[0-9]{1,})\-\-[ ]{0,}(?<enum>[a-z0-9]{1,})\)");
            Regex rgxAVTranslationCountWithParenthesis = new(@" \({1}[0-9]{1,}\){1}");
            Regex rgxAVTranslationCountWithoutParenthesis = new(@" [0-9]{1,}");
            Regex rgxSingleDigit = new(@"[0-9]");
            Regex rgxExtraTDNTInformation = new(@"\{TDNT [0-9 \:\,]{1,}\}");
            List<string> lParses = new();
            List<string> lRoots = new();
            List<string> lNonRoots = new();
            //StreamReader srRoots = new StreamReader(@"Data\GreekRoots.txt");
            //StreamReader srNonRoots = new StreamReader(@"Data\GreekNon-Roots.txt");

            //while (!srRoots.EndOfStream)
            //{
            //    lRoots.Add(srRoots.ReadLine().Trim());
            //}

            //srRoots.Close();

            //while (!srNonRoots.EndOfStream)
            //{
            //    lNonRoots.Add(srNonRoots.ReadLine().Trim());
            //}

            //srNonRoots.Close();

            while (!srBLBGreekLexiconIn.EndOfStream)
            {
                string strLine = srBLBGreekLexiconIn.ReadLine();
                int intEqualsIndex = strLine.IndexOf('=');
                string strFirstPart = strLine[..intEqualsIndex];
                string strSecondPart = strLine[(intEqualsIndex + 1)..];
                string strLexID = strFirstPart[5..]; //Lex[" = 5
                int intNextQuoteIndex = strLexID.IndexOf('"');
                Dictionary<string, string> dEntries = new();

                BLBGreekLexicon blbLex = new();

                intLexID = Convert.ToInt32(strLexID[..intNextQuoteIndex]);

                //Clean up strSecondPart
                strSecondPart = strSecondPart[2..];
                strSecondPart = strSecondPart[..^2];
                strSecondPart = strSecondPart.Replace("<BR>", "").Replace("&nbsp;", "");
                //strSecondPart = rgxINDENT.Replace(strSecondPart, "");

                blbLex.intLexID = intLexID;

                if (intLexID > 0) //account for metadata at top of data file HebLex.js
                {
                    intPartNumber = 0;

                    foreach (string strPart in strSecondPart.Split('|'))
                    {
                        intPartNumber++;

                        if (intPartNumber == 1) //Transliteration {Pronunciation}
                        {
                            foreach (string strPart1Part in strPart.Split())
                            {
                                if (strPart1Part.StartsWith('{'))
                                {
                                    blbLex.strPronunciation = strPart1Part.TrimStart('{').TrimEnd('}');

                                    //only accept the first entry if there are more than one
                                    //example: G3588 - the - ho, hey, and to are all entries, but only ho should be accepted here
                                    break;
                                }
                                else
                                {
                                    blbLex.strTransliteration = strPart1Part;
                                }
                            }
                        }
                        else if (intPartNumber == 2)//2 - AV
                        {
                            string strPart2 = strPart[5..]; //Removes "AV - " from the beginning
                            string strPart2Part1 = strPart2.Split(";")[0];

                            blbLex.intTotalTranslated = Convert.ToInt32(strPart2.Split(";")[1]);

                            if (rgxExtraTDNTInformation.IsMatch(strPart2Part1))
                            {
                                foreach (Match mExtra in rgxExtraTDNTInformation.Matches(strPart2Part1))
                                {
                                    int intLastCommaIndex = strPart2Part1[..(mExtra.Index - 1)].LastIndexOf(',');
                                    string strTranslationText = strPart2Part1.Substring(intLastCommaIndex + 1, mExtra.Index - intLastCommaIndex - 1);

                                    blbLex.dExtraTDNTInformation.Add(strTranslationText, mExtra.Value);
                                }

                                strPart2Part1 = rgxExtraTDNTInformation.Replace(strPart2Part1, "");
                            }

                            foreach (string strPart2Part in strPart2Part1.Split(',').Select(a => a.Trim()))
                            {
                                int intTranslationCountIndex = -1;
                                int intTranslationCount = 0;
                                int intTranslationCountIndexFinal = -1;
                                string strTranslation = "";

                                if (rgxAVTranslationCountWithoutParenthesis.IsMatch(strPart2Part))
                                {
                                    intTranslationCountIndex = rgxAVTranslationCountWithoutParenthesis.Matches(strPart2Part).Last().Index; //index at start of sequence of space then numbers
                                    intTranslationCountIndexFinal = rgxSingleDigit.Matches(strPart2Part).Last().Index; //index of last number
                                    intTranslationCount = Convert.ToInt32(strPart2Part.Substring(intTranslationCountIndex, intTranslationCountIndexFinal - intTranslationCountIndex + 1));
                                    strTranslation = strPart2Part[..intTranslationCountIndex].Trim();
                                }
                                else
                                {
                                    if (strPart2Part.Trim().Contains(' '))
                                    {
                                        strTranslation = strPart2Part.Trim();
                                        strTranslation = strTranslation.Split()[0];
                                    }
                                    else
                                    {
                                        strTranslation = strPart2Part.Trim();
                                    }
                                }

                                if (rgxAVTranslationCountWithParenthesis.IsMatch(strPart2Part))
                                {
                                    blbLex.lConjugated.Add(strTranslation);
                                }

                                if (blbLex.dAVTranslations.ContainsKey(strTranslation))
                                {
                                    blbLex.dAVTranslations[strTranslation] += intTranslationCount;
                                }
                                else
                                {
                                    blbLex.dAVTranslations.Add(strTranslation, intTranslationCount);
                                }
                            }

                        }
                        else if (intPartNumber == 3) //3 - a root or from which words meaning what?
                        {
                            string strConnectionPart = Regex.Replace(strPart, @"\s+", " ").Trim(); //condenses multiple whitespace to a single space; important since I manually stripped these while creating the "Root" and "Non-Root" phrase files

                            foreach (string strRoot in lRoots)
                            {
                                if (strPart == Regex.Replace(strRoot, @"\s+", " ").Trim()) //just in case I missed condensing whitespace in a connection phrase
                                {
                                    blbLex.bRoot = true;
                                    break;
                                }
                            }

                            blbLex.strConnection = strPart;
                        }
                        else if (intPartNumber == 4) //4 - TWOT #
                        {
                            if (strPart.Trim().Length > 0)
                            {
                                try
                                {
                                    blbLex.strTWOTNumber = strPart.Split("-")[1].Trim();
                                }
                                catch { }
                            }
                        }
                        else if (intPartNumber == 5) //5 - pos ?pr? gender
                        {
                            if (!lParses.Contains(strPart))
                            {
                                lParses.Add(strPart);
                            }

                            blbLex.strPOS = strPart;
                        }
                        else if (intPartNumber == 6) //6 - entries
                        {
                            //INDENT0--<b>1)</b> perish, vanish, go astray, be destroyed
                            //INDENT3-- <b>a)</b> (Qal)
                            //INDENT7--<b>1)</b> perish, die, be exterminated
                            //INDENT7--<b>2)</b> perish, vanish (fig.)
                            //INDENT7--<b>3)</b> be lost, strayed
                            //INDENT3-- <b>b)</b> (Piel)
                            //INDENT7--<b>1)</b> to destroy, kill, cause to perish, to give up (as lost), exterminate
                            //INDENT7--<b>2)</b> to blot out, do away with, cause to vanish, (fig.)
                            //INDENT7--<b>3)</b> cause to stray, lose
                            //INDENT3-- <b>c)</b> (Hiphil)
                            //INDENT7--<b>1)</b> to destroy, put to death
                            //INDENT12--   <b>a)</b> of divine judgment
                            //INDENT7--<b>2)</b> object name of kings (fig.)

                            string strPart6 = strPart.Replace("<b>", "").Replace("</b>", "").Trim();
                            string strCurrentNumber = "";
                            MatchCollection mmEntries = rgxEnumeration.Matches(strPart6);
                            Dictionary<int, int> dIndentIndexes = new();
                            string strWholeEntry = "";
                            string strLast0 = "";
                            string strLast3 = "";
                            string strLast7 = "";
                            string strLast12 = "";
                            string strLast18 = "";

                            foreach (Match mEntry in mmEntries)
                            {
                                dIndentIndexes.Add(dIndentIndexes.Count() + 1, mEntry.Index);
                            }

                            foreach (int intIndentOrder in dIndentIndexes.OrderBy(a => a.Key).Select(a => a.Key))
                            {
                                strWholeEntry = intIndentOrder < dIndentIndexes.Count()
                                    ? strPart6[dIndentIndexes[intIndentOrder]..dIndentIndexes[intIndentOrder + 1]]
                                    : strPart6[dIndentIndexes[intIndentOrder]..];

                                Match mEntry = rgxEnumeration.Match(strWholeEntry);
                                string strEntryText = strWholeEntry[mEntry.Captures[0].Length..].Trim();
                                string strEnum = "";

                                //0, 3, 7, 12, 18
                                if (mEntry.Groups["inum"].Value == "0")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast0 = strEnum;
                                    strLast3 = "";
                                    strLast7 = "";
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0;
                                }
                                else if (mEntry.Groups["inum"].Value == "3")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast3 = strEnum;
                                    strLast7 = "";
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3;
                                }
                                else if (mEntry.Groups["inum"].Value == "7")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast7 = strEnum;
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3 + strLast7;
                                }
                                else if (mEntry.Groups["inum"].Value == "12")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast12 = strEnum;
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3 + strLast7 + strLast12;
                                }
                                else if (mEntry.Groups["inum"].Value == "18")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast18 = strEnum;
                                    strCurrentNumber = strLast0 + strLast3 + strLast7 + strLast12 + strLast18;
                                }
                                else
                                {
                                    throw new Exception("Lexical Entry Error");
                                }

                                dEntries.Add(strCurrentNumber, strEntryText);
                            }

                            blbLex.dLexicalEntries = dEntries;
                        }
                    }

                    dBLBGreekLexiconEntries.Add(intLexID, blbLex);
                }
            }
        }

        public void FillBLBGreekLexicon(ref StreamReader srBLBGreekLexicon,
            ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries)
        {
            bool bSeenHeader = false;

            while (!srBLBGreekLexicon.EndOfStream)
            {
                string strLine = srBLBGreekLexicon.ReadLine();

                if (bSeenHeader == true)
                {
                    //dBLBGreekLexiconEntries.First().Value.intLexID
                    //dBLBGreekLexiconEntries.First().Value.bRoot
                    //dBLBGreekLexiconEntries.First().Value.intTotalTranslated
                    //dBLBGreekLexiconEntries.First().Value.strConnection
                    //dBLBGreekLexiconEntries.First().Value.strGender
                    //dBLBGreekLexiconEntries.First().Value.strPOS
                    //dBLBGreekLexiconEntries.First().Value.strPronunciation
                    //dBLBGreekLexiconEntries.First().Value.strTransliteration
                    //dBLBGreekLexiconEntries.First().Value.strTWOTNumber
                    //dBLBGreekLexiconEntries.First().Value.lConjugated
                    //dBLBGreekLexiconEntries.First().Value.dAVTranslations
                    //dBLBGreekLexiconEntries.First().Value.dExtraTDNTInformation
                    //dBLBGreekLexiconEntries.First().Value.dLexicalEntries

                    int intConjugatedLeftBracket = strLine.IndexOf('~');
                    int intConjugatedRightBracket = strLine.IndexOf('%');
                    int intTranslationsLeftBracket = strLine.IndexOf('~', intConjugatedLeftBracket + 1);
                    int intTranslationsRightBracket = strLine.IndexOf('%', intConjugatedRightBracket + 1);
                    int intExtraTDNTLeftBracket = strLine.IndexOf('~', intTranslationsLeftBracket + 1);
                    int intExtraTDNTRightBracket = strLine.IndexOf('%', intTranslationsRightBracket + 1);
                    int intLexicalLeftBracket = strLine.IndexOf('~', intExtraTDNTLeftBracket + 1);
                    int intLexicalRightBracket = strLine.IndexOf('%', intExtraTDNTRightBracket + 1);

                    string strFirstPart = strLine[..intConjugatedLeftBracket];
                    string strSecondPart = strLine.Substring(intConjugatedLeftBracket,
                        intConjugatedRightBracket - intConjugatedLeftBracket + 1)
                        .TrimStart('~').TrimEnd('%');
                    string strThirdPart = strLine.Substring(intTranslationsLeftBracket,
                        intTranslationsRightBracket - intTranslationsLeftBracket + 1)
                        .TrimStart('~').TrimEnd('%');
                    string strFourthPart = strLine.Substring(intExtraTDNTLeftBracket,
                        intExtraTDNTRightBracket - intExtraTDNTLeftBracket + 1)
                        .TrimStart('~').TrimEnd('%');
                    string strFifthPart = strLine.Substring(intLexicalLeftBracket,
                        intLexicalRightBracket - intLexicalLeftBracket + 1)
                        .TrimStart('~').TrimEnd('%');

                    int intPartCounter = 0;

                    BLBGreekLexicon blbGreekLexicon = new();
                    //_ = new();
                    Dictionary<string, int> dAVTranslations = new();
                    Dictionary<string, string> dExtraTDNTInformation = new();
                    Dictionary<string, string> dLexicalEntries = new();

                    foreach (string strPart in strFirstPart.Split("^"))
                    {
                        intPartCounter++;

                        switch (intPartCounter)
                        {
                            case 1:
                                blbGreekLexicon.intLexID = Convert.ToInt16(strPart);
                                break;
                            case 2:
                                blbGreekLexicon.bRoot = Convert.ToBoolean(strPart);
                                break;
                            case 3:
                                blbGreekLexicon.intTotalTranslated = Convert.ToInt16(strPart);
                                break;
                            case 4:
                                blbGreekLexicon.strConnection = strPart;
                                break;
                            case 5:
                                blbGreekLexicon.strGender = strPart;
                                break;
                            case 6:
                                blbGreekLexicon.strPOS = strPart;
                                break;
                            case 7:
                                blbGreekLexicon.strPronunciation = strPart;
                                break;
                            case 8:
                                blbGreekLexicon.strTransliteration = strPart;
                                break;
                            case 9:
                                blbGreekLexicon.strTWOTNumber = strPart;
                                break;
                        }
                    }

                    foreach (string strConjugated in strSecondPart.Split('^'))
                    {
                        if (strConjugated.Trim() != "")
                        {
                            lConjugated.Add(strConjugated.Trim());
                        }
                    }

                    blbGreekLexicon.lConjugated = lConjugated;

                    foreach (string strAVTranslation in strThirdPart.Split('{'))
                    {
                        string[] strsParts = strAVTranslation.Split('^');

                        if (strsParts.Length >= 2)
                        {
                            dAVTranslations.Add(strsParts[0],
                                Convert.ToInt16(strsParts[1].Trim().TrimEnd('}')));
                        }
                    }

                    blbGreekLexicon.dAVTranslations = dAVTranslations;

                    foreach (string strExtra in strFourthPart.Split('{'))
                    {
                        string[] strsParts = strExtra.Split('^');

                        if (strsParts.Length >= 2)
                        {
                            dExtraTDNTInformation.Add(strsParts[0], strsParts[1].TrimEnd('}'));
                        }
                    }

                    blbGreekLexicon.dExtraTDNTInformation = dExtraTDNTInformation;

                    foreach (string strLexicalEntryID in strFifthPart.Split('{'))
                    {
                        string[] strsParts = strLexicalEntryID.Split('^');

                        if (strsParts.Length >= 2)
                        {
                            dLexicalEntries.Add(strsParts[0], strsParts[1].TrimEnd('}'));
                        }
                    }

                    blbGreekLexicon.dLexicalEntries = dLexicalEntries;

                    dBLBGreekLexiconEntries.Add(blbGreekLexicon.intLexID, blbGreekLexicon);
                }
                else
                {
                    bSeenHeader = true;
                }

            }

            srBLBGreekLexicon.Close();
        }

        public void WriteBLBGreekLexicon(ref StreamWriter swBLBGreekLexicon,
            ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries)
        {
            StringBuilder sbConjugated = new();
            StringBuilder sbTranslations = new();
            StringBuilder sbExtraTDNT = new();
            StringBuilder sbLexicalEntries = new();

            swBLBGreekLexicon.WriteLine("LexicalID ^ IsRoot ^ Total Translated ^ " +
                "Connection ^ Gender ^ POS ^ Pronunciation ^ Transliteration ^ " +
                "TWOT Number ^ ~Conjugated% ^ ~Translation Counts% ^ ~Extra TDNT Information% ^ " +
                "~Lexical Hierarchy%");

            foreach (int intLexID in dBLBGreekLexiconEntries.Keys.OrderBy(a => a))
            {
                swBLBGreekLexicon.Write(intLexID.ToString() + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].bRoot + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].intTotalTranslated + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].strConnection + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].strGender + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].strPOS + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].strPronunciation + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].strTransliteration + " ^ " +
                    dBLBGreekLexiconEntries[intLexID].strTWOTNumber + " ^ ");

                _ = sbConjugated.Clear();

                if (dBLBGreekLexiconEntries[intLexID].lConjugated.Count > 0)
                {
                    foreach (string strConjugated in dBLBGreekLexiconEntries[intLexID].lConjugated)
                    {
                        _ = sbConjugated.Append(strConjugated + " ^ ");
                    }

                    _ = sbConjugated.Remove(sbConjugated.Length - 3, 3);
                }

                swBLBGreekLexicon.Write("~" + sbConjugated.ToString() + "% ^ ");

                _ = sbTranslations.Clear();

                foreach (string strTranslation in dBLBGreekLexiconEntries[intLexID].dAVTranslations.Keys.OrderBy(a => a))
                {
                    _ = sbTranslations.Append("{" + strTranslation + " ^ " +
                        dBLBGreekLexiconEntries[intLexID]
                        .dAVTranslations[strTranslation].ToString() + "} ^ ");
                }

                _ = sbTranslations.Remove(sbTranslations.Length - 3, 3);

                swBLBGreekLexicon.Write("~" + sbTranslations.ToString() + "% ^ ");

                _ = sbExtraTDNT.Clear();

                if (dBLBGreekLexiconEntries[intLexID].dExtraTDNTInformation.Count > 0)
                {
                    foreach (string strTranslation in dBLBGreekLexiconEntries[intLexID].dExtraTDNTInformation.Keys.OrderBy(a => a))
                    {
                        _ = sbExtraTDNT.Append("{" + strTranslation + " ^ " +
                            dBLBGreekLexiconEntries[intLexID]
                            .dExtraTDNTInformation[strTranslation] + "} ^ ");
                    }

                    _ = sbExtraTDNT.Remove(sbExtraTDNT.Length - 3, 3);
                }

                swBLBGreekLexicon.Write("~" + sbExtraTDNT.ToString() + "% ^ ");

                _ = sbLexicalEntries.Clear();

                foreach (string strLexicalID in dBLBGreekLexiconEntries[intLexID].dLexicalEntries.Keys)
                {
                    _ = sbLexicalEntries.Append("{" + strLexicalID + " ^ " +
                        dBLBGreekLexiconEntries[intLexID]
                        .dLexicalEntries[strLexicalID] + "} ^ ");
                }

                _ = sbLexicalEntries.Remove(sbLexicalEntries.Length - 3, 3);

                swBLBGreekLexicon.WriteLine("~" + sbLexicalEntries.ToString() + "%");
            }

            swBLBGreekLexicon.Close();
        }
    }
}

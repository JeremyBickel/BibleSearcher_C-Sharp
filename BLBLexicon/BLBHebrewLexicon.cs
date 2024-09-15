using System.Text;
using System.Text.RegularExpressions;

namespace BLBLexicon
{
    public class BLBHebrewLexicon
    {
        //Lex["01"] = "'ab {awb}|AV - father 1205, chief 2, families 2,
        //desire 1, fatherless + 0369 1,<BR> &nbsp;&nbsp;&nbsp;&nbsp;
        //&nbsp;forefathers + 07223 1, patrimony 1, prince 1, principal 1;
        //1215|a root|TWOT - 4a|n m|INDENT0--<b>1)</b> father of an
        //individualINDENT0--<b>2)</b> of God as father of his
        //peopleINDENT0--<b>3)</b> head or founder of a household, group,
        //family, or clanINDENT0--<b>4)</b> ancestorINDENT3-- <b>a)</b>
        //grandfather, forefathers -- of personINDENT3-- <b>b)</b> of
        //peopleINDENT0--<b>5)</b> originator or patron of a class,
        //profession, or artINDENT0--<b>6)</b> of producer,
        //generator (fig.)INDENT0--<b>7)</b> of benevolence and
        //protection (fig.)INDENT0--<b>8)</b> term of respect and
        //honourINDENT0--<b>9)</b> ruler or chief (spec.)";

        //Lex["02"] = "'ab (Aramaic) {ab}|AV - father 9; 9|corresponding
        //to 01|TWOT - 2553|n m|INDENT0--<b>1)</b> father";

        public int intLexID = 0;
        public string strTransliteration = "";
        public string strPronunciation = "";
        public Dictionary<string, int> dAVTranslations = new();
        public int intTotalTranslated = 0;
        public bool bRoot = false;
        public bool bAramaic = false;
        public string strConnection = ""; //corresponding to #, from #, from an unused root (apparently meaning to turn), etc.
        public string strTWOTNumber = "";
        public string strPOS = "";
        public string strGender = "";
        public Dictionary<string, string> dLexicalEntries = new(); //D<Entry number with letter sub-indexes, Lexical Entry>

        public void CreateBLBHebrewLexicon(ref StreamReader srBLBHebrewLexiconIn,
            ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries,
            ref StreamReader srRoots, ref StreamReader srNonRoots)
        {
            //Lex["01"] = "'ab {awb}|AV - father 1205, chief 2, families 2,
            //desire 1, fatherless + 0369 1,<BR> &nbsp;&nbsp;&nbsp;&nbsp;
            //&nbsp;forefathers + 07223 1, patrimony 1, prince 1, principal 1;
            //1215|a root|TWOT - 4a|n m|INDENT0--<b>1)</b> father of an
            //individualINDENT0--<b>2)</b> of God as father of his
            //peopleINDENT0--<b>3)</b> head or founder of a household, group,
            //family, or clanINDENT0--<b>4)</b> ancestorINDENT3-- <b>a)</b>
            //grandfather, forefathers -- of personINDENT3-- <b>b)</b> of
            //peopleINDENT0--<b>5)</b> originator or patron of a class,
            //profession, or artINDENT0--<b>6)</b> of producer,
            //generator (fig.)INDENT0--<b>7)</b> of benevolence and
            //protection (fig.)INDENT0--<b>8)</b> term of respect and
            //honourINDENT0--<b>9)</b> ruler or chief (spec.)";

            //Lex["02"] = "'ab (Aramaic) {ab}|AV - father 9; 9|corresponding
            //to 01|TWOT - 2553|n m|INDENT0--<b>1)</b> father";

            int intPartNumber = 0; //Which part of the ';'-separated line are we in?
            int intLexID = 0;
            Regex rgxEnumeration = new(@"(?<ind>INDENT)(?<inum>[0-9]{1,})\-\-[ ]{0,}(?<enum>[a-z0-9]{1,})\)");
            List<string> lPOSs = new(); //every POS
            List<string> lRoots = new();
            List<string> lNonRoots = new();

            while (!srRoots.EndOfStream)
            {
                lRoots.Add(srRoots.ReadLine().Trim());
            }

            srRoots.Close();

            while (!srNonRoots.EndOfStream)
            {
                lNonRoots.Add(srNonRoots.ReadLine().Trim());
            }

            srNonRoots.Close();

            while (!srBLBHebrewLexiconIn.EndOfStream)
            {
                string strLine = srBLBHebrewLexiconIn.ReadLine();
                int intEqualsIndex = strLine.IndexOf('=');
                string strFirstPart = strLine[..intEqualsIndex];
                string strSecondPart = strLine[(intEqualsIndex + 1)..];
                string strLexID = strFirstPart[5..]; //Lex[" = 5
                int intNextQuoteIndex = strLexID.IndexOf('"');
                Dictionary<string, string> dEntries = new();

                BLBHebrewLexicon blbLex = new();

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

                        if (intPartNumber == 1) //Transliteration ?(Aramaic)? {Pronunciation}
                        {
                            foreach (string strPart1Part in strPart.Split())
                            {
                                if (strPart1Part == "(Aramaic)")
                                {
                                    blbLex.bAramaic = true;
                                }
                                else if (strPart1Part.StartsWith('{'))
                                {
                                    blbLex.strPronunciation = strPart1Part.TrimStart('{').TrimEnd('}');
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

                            foreach (string strPart2Part in strPart2Part1.Split(',').Select(a => a.Trim()))
                            {
                                int intTranslationCountIndex = -1;
                                int intTranslationCount = 0;
                                string strTranslation = "";

                                if (Regex.IsMatch(strPart2Part, "[0-9]"))
                                {
                                    intTranslationCountIndex = Regex.Matches(strPart2Part, " [0-9]{1,}").Last().Index; //index at start of sequence of space then numbers
                                    int intTranslationCountIndexFinal = Regex.Matches(strPart2Part, "[0-9]").Last().Index; //index of last number
                                    intTranslationCount = Convert.ToInt32(strPart2Part.Substring(intTranslationCountIndex, intTranslationCountIndexFinal - intTranslationCountIndex + 1));
                                    strTranslation = strPart2Part[..intTranslationCountIndex].Trim();
                                }
                                else
                                {
                                    strTranslation = strPart2Part.Trim();
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
                                blbLex.strTWOTNumber = strPart.Split("-")[1].Trim();
                            }
                        }
                        else if (intPartNumber == 5) //5 - pos ?pr? gender
                        {
                            if (!lPOSs.Contains(strPart))
                            {
                                lPOSs.Add(strPart);
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

                    dBLBHebrewLexiconEntries.Add(intLexID, blbLex);
                }
            }
        }

        public void FillBLBHebrewLexicon(ref StreamReader srBLBLexicalEntries,
            ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            bool bSeenHeader = false;

            while (!srBLBLexicalEntries.EndOfStream)
            {
                string strLine = srBLBLexicalEntries.ReadLine();

                if (bSeenHeader == true)
                {
                    int intTranslationsLeftBracket = strLine.IndexOf('~');
                    int intTranslationsRightBracket = strLine.IndexOf('%');
                    int intLexicalLeftBracket = strLine.IndexOf('~', intTranslationsLeftBracket + 1);
                    int intLexicalRightBracket = strLine.IndexOf('%', intTranslationsRightBracket + 1);

                    string strFirstPart = strLine[..intTranslationsLeftBracket];
                    string strSecondPart = strLine.Substring(intTranslationsLeftBracket,
                        intTranslationsRightBracket - intTranslationsLeftBracket + 1)
                        .TrimStart('~').TrimEnd('%');
                    string strThirdPart = strLine.Substring(intLexicalLeftBracket,
                        intLexicalRightBracket - intLexicalLeftBracket + 1)
                        .TrimStart('~').TrimEnd('%');

                    int intPartCounter = 0;

                    BLBHebrewLexicon blbHebrewLexicon = new();
                    Dictionary<string, int> dAVTranslations = new();
                    Dictionary<string, string> dLexicalEntries = new();

                    foreach (string strPart in strFirstPart.Split("^"))
                    {
                        intPartCounter++;

                        switch (intPartCounter)
                        {
                            case 1:
                                blbHebrewLexicon.intLexID = Convert.ToInt16(strPart);
                                break;
                            case 2:
                                blbHebrewLexicon.intTotalTranslated = Convert.ToInt16(strPart);
                                break;
                            case 3:
                                blbHebrewLexicon.strConnection = strPart;
                                break;
                            case 4:
                                blbHebrewLexicon.strGender = strPart;
                                break;
                            case 5:
                                blbHebrewLexicon.strPOS = strPart;
                                break;
                            case 6:
                                blbHebrewLexicon.strPronunciation = strPart;
                                break;
                            case 7:
                                blbHebrewLexicon.strTransliteration = strPart;
                                break;
                            case 8:
                                blbHebrewLexicon.strTWOTNumber = strPart;
                                break;
                            case 9:
                                blbHebrewLexicon.bAramaic = Convert.ToBoolean(strPart);
                                break;
                            case 10:
                                blbHebrewLexicon.bRoot = Convert.ToBoolean(strPart);
                                break;
                        }
                    }

                    foreach (string strAVTranslation in strSecondPart.Split('{'))
                    {
                        if (strAVTranslation.Trim() != "")
                        {
                            string[] strsParts = strAVTranslation.Split('^');

                            dAVTranslations.Add(strsParts[0],
                                Convert.ToInt16(strsParts[1].Trim().TrimEnd('}')));
                        }
                    }

                    blbHebrewLexicon.dAVTranslations = dAVTranslations;

                    foreach (string strLexicalEntryID in strThirdPart.Split('{'))
                    {
                        if (strLexicalEntryID.Trim() != "")
                        {
                            string[] strsParts = strLexicalEntryID.Split('^');

                            dLexicalEntries.Add(strsParts[0], strsParts[1].Trim().TrimEnd('}'));
                        }
                    }

                    blbHebrewLexicon.dLexicalEntries = dLexicalEntries;

                    dBLBHebrewLexiconEntries.Add(blbHebrewLexicon.intLexID, blbHebrewLexicon);
                }
                else
                {
                    bSeenHeader = true;
                }

            }

            srBLBLexicalEntries.Close();
        }

        public void WriteBLBHebrewLexicon(ref StreamWriter swBLBLexicalEntries,
            ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            StringBuilder sbTranslations = new();
            StringBuilder sbLexicalEntries = new();

            swBLBLexicalEntries.WriteLine("LexicalID ^ Total Translated ^ Connection ^ " +
                "Gender ^ POS ^ Pronunciation ^ Transliteration ^ TWOT Number ^ IsAramaic ^ " +
                "IsRoot ^ ~Translation Counts% ^ ~Lexical Hierarchy%");

            foreach (int intLexID in dBLBHebrewLexiconEntries.Keys.OrderBy(a => a))
            {
                swBLBLexicalEntries.Write(intLexID.ToString() + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].intTotalTranslated + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].strConnection + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].strGender + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].strPOS + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].strPronunciation + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].strTransliteration + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].strTWOTNumber + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].bAramaic + " ^ " +
                dBLBHebrewLexiconEntries[intLexID].bRoot + " ^ ");

                _ = sbTranslations.Clear();

                foreach (string strTranslation in dBLBHebrewLexiconEntries[intLexID].dAVTranslations.Keys.OrderBy(a => a))
                {
                    _ = sbTranslations.Append("{" + strTranslation + " ^ " +
                        dBLBHebrewLexiconEntries[intLexID]
                        .dAVTranslations[strTranslation].ToString() + "} ^ ");
                }

                _ = sbTranslations.Remove(sbTranslations.Length - 3, 3);

                swBLBLexicalEntries.Write("~" + sbTranslations.ToString() + "% ^ ");

                _ = sbLexicalEntries.Clear();

                foreach (string strLexicalID in dBLBHebrewLexiconEntries[intLexID].dLexicalEntries.Keys)
                {
                    _ = sbLexicalEntries.Append("{" + strLexicalID + " ^ " +
                        dBLBHebrewLexiconEntries[intLexID]
                        .dLexicalEntries[strLexicalID] + "} ^ ");
                }

                _ = sbLexicalEntries.Remove(sbLexicalEntries.Length - 3, 3);

                swBLBLexicalEntries.WriteLine("~" + sbLexicalEntries.ToString() + "%");
            }

            swBLBLexicalEntries.Close();
        }
    }
}

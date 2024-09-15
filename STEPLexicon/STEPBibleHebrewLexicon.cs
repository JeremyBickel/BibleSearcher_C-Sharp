namespace STEPLexicon
{
    public class Lexicon
    {
        //Gen.46.18-12	Gen.46.18-12	לְיַעֲקֹב	לְ/יַעֲקֹ֔ב	HR/Npm	H9005=ל=to/H3290=יַעֲקֹב=Jacob_§Jacob|Israel@Gen.25.26

        public string strParse = "";
        public string strStrongs = "";
        public int intWordOrderID = 0; //separated by /
        public int intTranslationOrderID = 0; //separated by |
        public string strTranslation = ""; //if intWordOrderID == 1 then strTranslation == "to"; if intWordOrderID == 2 then strTranslation == "Jacob"
        public string strTranslationAlternate = ""; //if intWordOrderID == 1 then strTranslation == ""; if intWordOrderID == 2 then strTranslation == "Jacob"
        public string strTranslationAlternate2 = ""; //if intWordOrderID == 1 then strTranslation == ""; if intWordOrderID == 2 then strTranslation == ""
        public string strTranslationSemantic = ""; //if intWordOrderID == 1 then strTranslation == ""; if intWordOrderID == 2 then strTranslation == "Israel@Gen.25.26"

        public Lexicon() { }
    }

    public class STEPBibleHebrewLexicon
    {
        public void CreateSTEPBibleHebrewLexicon(ref STEPData stepdata, ref StreamReader srSTEPHebrew)
        {
            //Gen.46.18-12	Gen.46.18-12	לְיַעֲקֹב	לְ/יַעֲקֹ֔ב	HR/Npm	H9005=ל=to/H3290=יַעֲקֹב=Jacob_§Jacob|Israel@Gen.25.26
            if (stepdata.dSTEPBookNameAbbreviationNormalizations.Count < 66)
            {
                FillSTEPBookNameAbbreviationNormalizationDictionary(ref stepdata);
            }

            stepdata.shebLexicon.Create(ref stepdata, ref srSTEPHebrew);
        }

        public void FillSTEPBookNameAbbreviationNormalizationDictionary(ref STEPData stepdata)
        {
            stepdata.dSTEPBookNameAbbreviationNormalizations.Clear();

            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Gen", "Genesis");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Exo", "Exodus");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Lev", "Leviticus");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Num", "Numbers");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Deu", "Deuteronomy");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Jos", "Joshua");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Jdg", "Judges");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Rut", "Ruth");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("1Sa", "1 Samuel");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("2Sa", "2 Samuel");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("1Ki", "1 Kings");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("2Ki", "2 Kings");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("1Ch", "1 Chronicles");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("2Ch", "2 Chronicles");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Ezr", "Ezra");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Neh", "Nehemiah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Est", "Esther");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Job", "Job");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Psa", "Psalms");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Pro", "Proverbs");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Ecc", "Ecclesiastes");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Sng", "Song of Solomon");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Isa", "Isaiah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Jer", "Jeremiah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Lam", "Lamentations");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Ezk", "Ezekiel");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Dan", "Daniel");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Hos", "Hosea");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Jol", "Joel");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Amo", "Amos");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Oba", "Obadiah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Jon", "Jonah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Mic", "Micah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Nam", "Nahum");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Hab", "Habakkuk");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Zep", "Zephaniah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Hag", "Haggai");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Zec", "Zechariah");
            stepdata.dSTEPBookNameAbbreviationNormalizations.Add("Mal", "Malachi");
        }

        public void Create(ref STEPData stepdata, ref StreamReader srSTEPHebrew)
        {
            //Gen.46.18-12	Gen.46.18-12	לְיַעֲקֹב	לְ/יַעֲקֹ֔ב	HR/Npm	H9005=ל=to/H3290=יַעֲקֹב=Jacob_§Jacob|Israel@Gen.25.26

            int intLineCounter = 0;

            while (!srSTEPHebrew.EndOfStream)
            {
                string strLine = srSTEPHebrew.ReadLine();

                if (strLine != "")
                {
                    string strExtended = "";
                    int intPartCounter = 0;

                    //the alternate parse is Ncmpc/Sp3ms in HC/Npm//Ncmpc/Sp3ms <- double // splits an alternate

                    //Sometimes a morphology has more than one possibility.  Note the /_/ in the example which separates these possibilities:
                    //2Ki.19.37-08.Q	2Ki.19.37-08q	וְשַׂרְאֶצֶר בָּנָיו	וְ/שַׂרְאֶ֤צֶר/ /בָּנָי/ו֙	HC/Npm//Ncmpc/Sp3ms	H9002=ו=and/H8272=שַׁרְאֶ֫צֶר=Sharezer_§Sharezer@2Ki.19.37/_/H1121a=בֵּן=son_§1_child|son/H9023=Ps3m=his

                    intLineCounter++;

                    if (intLineCounter is 11188 or 79574)
                    {//What??
                    }

                    stepdata.strReference = strLine.Split()[0];

                    foreach (string strPart in strLine.Split('\t'))
                    {
                        intPartCounter++;

                        if (intPartCounter == 5)
                        {
                            int intParsePartCounter = 0;

                            if (!strPart.Contains("//"))
                            {
                                foreach (string strParsePart in strPart.Split('/'))
                                {
                                    intParsePartCounter++;

                                    if (!stepdata.dHebrewLexicon.ContainsKey(intLineCounter))
                                    {
                                        stepdata.dHebrewLexicon.Add(intLineCounter, new Dictionary<int, Lexicon>());
                                    }

                                    stepdata.dHebrewLexicon[intLineCounter].Add(stepdata.dHebrewLexicon[intLineCounter].Count() + 1, new Lexicon());
                                    stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strParse = strParsePart;
                                }
                            }
                            else
                            {
                                int intAlternatePartCounter = 0;

                                foreach (string strAlternatePart in strPart.Split("//"))
                                {
                                    intAlternatePartCounter++;

                                    foreach (string strParsePart in strAlternatePart.Split('/'))
                                    {
                                        intParsePartCounter++;

                                        if (!stepdata.dHebrewLexicon.ContainsKey(intLineCounter))
                                        {
                                            stepdata.dHebrewLexicon.Add(intLineCounter, new Dictionary<int, Lexicon>());
                                            stepdata.dHebrewLexicon[intLineCounter].Add(1, new Lexicon());
                                        }
                                        if (intAlternatePartCounter == 1)
                                        {
                                            stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strParse = strParsePart;
                                        }
                                        else if (intAlternatePartCounter == 2)
                                        {
                                            stepdata.dHebrewLexicon[intLineCounter].Add(stepdata.dHebrewLexicon[intLineCounter].Count() + 1, new Lexicon());
                                            stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strParse = strParsePart;
                                        }
                                        else
                                        {
                                            throw new Exception("More than two alternate parses using '//'");
                                        }
                                    }
                                }
                            }
                        }
                        else if (intPartCounter > 5) //strExtended might have spaces in it
                        {
                            strExtended += strPart;
                        }
                    }

                    intPartCounter = 0;

                    //Process strExtended
                    foreach (string strPart in strExtended.Split('/'))
                    {
                        int intPart2Counter = 0;

                        if (strPart == "_") //alternate
                        {
                            stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].intWordOrderID =
                                stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count() - 1].intWordOrderID;
                            intPartCounter = 0; //the next strPart starts a new morphology
                        }
                        else
                        {

                            intPartCounter++;

                            if (!stepdata.dHebrewLexicon[intLineCounter].ContainsKey(intPartCounter)) //there are more words than parses (ie. when the word doesn't have a parse; eg. H9016=׃=verseEnd)
                            {
                                stepdata.dHebrewLexicon[intLineCounter].Add(intPartCounter, new Lexicon());
                            }

                            stepdata.dHebrewLexicon[intLineCounter][intPartCounter].intWordOrderID = intPartCounter;

                        }

                        if (intPartCounter > 0)
                        {
                            foreach (string strPart2 in strPart.Split('='))
                            {
                                intPart2Counter++;

                                if (intPart2Counter == 1) //Strongs number
                                {
                                    stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strStrongs = strPart2;
                                }
                                else if (intPart2Counter == 3) //Translations
                                {
                                    int intPart3Counter = 0;

                                    foreach (string strPart3 in strPart2.Split('|'))
                                    {
                                        //Jacob_§Jacob|Israel@Gen.25.26|sea_§1_sea_§Ephron@Jos.18.15

                                        intPart3Counter++;

                                        stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].intTranslationOrderID = intPart3Counter;

                                        if (strPart3.Contains('§'))
                                        {
                                            int intPart4Counter = 0;

                                            foreach (string strPart4 in strPart3.Split('§'))
                                            {
                                                string strPart4Cleaned = strPart4;

                                                while (strPart4Cleaned.Contains("_"))
                                                {
                                                    strPart4Cleaned = strPart4Cleaned.Trim('_')[strPart4Cleaned.IndexOf('_')..];
                                                }

                                                intPart4Counter++;

                                                if (intPart4Counter == 1)
                                                {
                                                    stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strTranslation = strPart4.Trim('_');
                                                }
                                                else if (intPart4Counter == 2)
                                                {
                                                    stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strTranslationAlternate = strPart4Cleaned;
                                                }
                                                else if (intPart4Counter == 3)
                                                {
                                                    stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strTranslationAlternate2 = strPart4Cleaned;
                                                }
                                                else if (intPart4Counter > 3)
                                                {
                                                    throw new Exception("intPart4Counter > 3");
                                                }
                                            }
                                        }
                                        else if (strPart3.Contains('@'))
                                        {
                                            stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strTranslationSemantic = strPart3;
                                        }
                                        else
                                        {
                                            stepdata.dHebrewLexicon[intLineCounter][stepdata.dHebrewLexicon[intLineCounter].Count()].strTranslation = strPart3;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } //end while

            //var a = data.dLexicon.Where(a=>a.Value.Count() > 1).Select(a => a);
        }

        public void Read(ref STEPData stepdata, ref StreamReader srSTEPHebrew)
        {
            bool bSeenHeader = false;

            while (!srSTEPHebrew.EndOfStream)
            {
                string strLine = srSTEPHebrew.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!stepdata.dHebrewLexicon.ContainsKey(Convert.ToInt32(strsLine[0])))
                    {
                        stepdata.dHebrewLexicon.Add(Convert.ToInt32(strsLine[0]),
                            new Dictionary<int, Lexicon>());
                    }

                    if (!stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])].ContainsKey(
                            Convert.ToInt16(strsLine[1])))
                    {
                        stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])].Add(
                            Convert.ToInt16(strsLine[1]), new Lexicon());
                    }

                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].strParse = strsLine[2];
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].strStrongs = strsLine[3];
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].intWordOrderID = Convert.ToInt16(strsLine[4]);
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].intTranslationOrderID = Convert.ToInt16(strsLine[5]);
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].strTranslation = strsLine[6];
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].strTranslationAlternate = strsLine[7];
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].strTranslationAlternate2 = strsLine[8];
                    stepdata.dHebrewLexicon[Convert.ToInt32(strsLine[0])]
                        [Convert.ToInt16(strsLine[1])].strTranslationSemantic = strsLine[9];
                }
            }
        }

        public void Write(ref STEPData stepdata, ref StreamWriter swSTEPHebrew)
        {
            swSTEPHebrew.WriteLine("Line Number ^ Part Number ^ Parse ^ Strongs Number ^ Word Order ID ^ " +
                "Translation Order ID ^ Translation ^ Translation Alternate ^ Translation Alternate 2 ^ " +
                "Translation Semantic");

            foreach (int intLineNumber in stepdata.dHebrewLexicon.Keys.OrderBy(a => a))
            {
                foreach (int intPartNumber in stepdata.dHebrewLexicon[intLineNumber].Keys.OrderBy(a => a))
                {
                    swSTEPHebrew.WriteLine(intLineNumber.ToString() + " ^ " + intPartNumber.ToString() + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].strParse + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].strStrongs + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].intWordOrderID.ToString() + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].intTranslationOrderID.ToString() + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].strTranslation + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].strTranslationAlternate + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].strTranslationAlternate2 + " ^ " +
                        stepdata.dHebrewLexicon[intLineNumber][intPartNumber].strTranslationSemantic);
                }
            }

            swSTEPHebrew.Close();
        }
    }
}

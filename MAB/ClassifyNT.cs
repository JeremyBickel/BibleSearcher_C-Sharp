using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MAB
{
    public class ClassifyMABNT
    {
        public Dictionary<int, Word> dWords = new Dictionary<int, Word>();
        public Dictionary<int, string> dGlosses = new();
        public Dictionary<int, string> dClauses = new();
        public Dictionary<int, string> dSubclauses = new();

        Regex rgxVID = new Regex(@"<vid id=""v(?<book>[0-9]{1,2})\.(?<chapter>[0-9]{1,})\.(?<verse>[0-9]{1,})"" onclick=""luV\([0-9]{1,}\)"">(?<id>[0-9]{1,})</vid>");
        Regex rgxCLInfo = new Regex(@"<clinfo>(?<clinfo>[^<]{1,})</clinfo>");
        Regex rgxConnector = new Regex(@"<connector>(?<connector>[^<]{1,})</connector>");
        Regex rgxCLLevel = new Regex(@"<cllevel>(?<cllevel>[^<]{1,})</cllevel>");
        Regex rgxSubCLInfo = new Regex(@"<subclinfo>(?<subclinfo>[^<]{1,})</subclinfo>");
        Regex rgxFuncInfo = new Regex(@"<funcinfo>(?<funcinfo>[^<]{1,})</funcinfo>");
        Regex rgxWordID = new Regex(@"<wordid>(?<wordid>[^<]{1,})</wordid>");
        Regex rgxGrk = new Regex(@"<grk id=""(?<grkid>[^""]{1,})"" cl='(?<cl>[^']{1,})' sn='(?<sn>[^']{1,})' morph='(?<morph>[^']{1,})' class=""(?<class>[^""]{1,})"" onclick=""(?<onclick>[^""]{1,})"" onmouseover=""(?<onmouseover>[^""]{1,})"" onmouseout=""(?<onmouseout>[^""]{1,})"" ondblclick=""(?<ondblclick>[^""]{1,})"">(?<greekword>[^<]{1,})</grk>");
        Regex rgxGloss = new Regex(@"<gloss>(?<gloss>[^<]{1,})</gloss>");
        bool bConnector = false;
        bool bSubClause = false; //format control; after subcluause, either function or word appears. if function, don't insert newline before it, but if word, do.
        bool bPastLine1 = false; //format control; only puts the 2 blank lines before a new verse after the first verse

        public void TransformMABNTXML(ref StreamReader srMABNT, ref StreamWriter swProcessedMABNT)
        {   
            while (!srMABNT.EndOfStream)
            {
                string strLine = srMABNT.ReadLine();
                
                bool bContinue = true;

                if (strLine.Contains("Matthew^40^6^1^16"))
                {
                    string a = "";
                }

                if (strLine.Contains("14:8"))
                {
                    string s = "";
                }

                while (bContinue == true)
                {
                    string strCheck = TestNext(ref strLine, ref swProcessedMABNT);

                    if (strCheck == "Error")
                    {
                        throw new Exception("Error in TestNext Check");
                    }
                    else if (strCheck == "V")
                    {
                        if (rgxVID.IsMatch(strLine))
                        {
                            Match m = rgxVID.Match(strLine);

                            if (bPastLine1 == true)
                            {
                                swProcessedMABNT.WriteLine(); //finish the last verse
                                swProcessedMABNT.WriteLine(); //put extra space between verses for readability
                            }
                            else
                            {
                                bPastLine1 = false;
                            }

                            swProcessedMABNT.Write("Book: " + Convert.ToInt16(m.Groups["book"].Value) +
                            " Chapter: " + Convert.ToInt16(m.Groups["chapter"].Value) +
                            " Verse: " + Convert.ToInt16(m.Groups["verse"].Value));

                            swProcessedMABNT.WriteLine();

                            strLine = strLine.Remove(0, m.Index + m.Length); //this gets everything from the start, including any residual junk
                        }
                    }
                    else if (strCheck == "C")
                    {
                        if (bConnector == false)
                        {
                            if (rgxCLInfo.IsMatch(strLine))
                            {
                                Match m = rgxCLInfo.Match(strLine);

                                if (m.Groups["clinfo"].Value == "14:8.w1-w18")
                                {
                                    string a = "";
                                }
                                swProcessedMABNT.Write(" Clause: " + m.Groups["clinfo"].Value);

                                strLine = strLine.Remove(0, m.Index + m.Length);
                            }
                        }
                        else
                        {
                            if (rgxCLInfo.IsMatch(strLine))
                            {
                                Match m = rgxCLInfo.Match(strLine);

                                if (m.Groups["clinfo"].Value == "14:8.w1-w18")
                                {
                                    string a = "";
                                }
                                swProcessedMABNT.Write("@" + m.Groups["clinfo"].Value);

                                strLine = strLine.Remove(0, m.Index + m.Length);

                                bConnector = false;
                            }
                        }

                        swProcessedMABNT.WriteLine();
                    }
                    else if (strCheck == "At")
                    {
                        bConnector = true;
                    }
                    else if (strCheck == "CL")
                    {
                        if (rgxCLLevel.IsMatch(strLine))
                        {
                            Match m = rgxCLLevel.Match(strLine);

                            swProcessedMABNT.WriteLine(); //finish the last line
                            swProcessedMABNT.Write(" ClauseLevel: " + Convert.ToInt16(m.Groups["cllevel"].Length));

                            strLine = strLine.Remove(0, m.Index + m.Length); //this gets everything from the start, including any residual junk
                        }
                    }
                    else if (strCheck == "S")
                    {
                        if (rgxSubCLInfo.IsMatch(strLine))
                        {
                            Match mm = rgxSubCLInfo.Match(strLine);

                            swProcessedMABNT.Write(" SubClause: " + mm.Groups["subclinfo"].Value);

                            strLine = strLine.Remove(0, mm.Index + mm.Length);

                            bSubClause = true;
                        }
                    }
                    else if (strCheck == "F")
                    {
                        if (rgxFuncInfo.IsMatch(strLine))
                        {
                            Match mm = rgxFuncInfo.Match(strLine);

                            swProcessedMABNT.Write(" Function: " + mm.Groups["funcinfo"].Value);

                            strLine = strLine.Remove(0, mm.Index + mm.Length);

                            swProcessedMABNT.WriteLine();

                            if (bSubClause == true)
                            {
                                bSubClause = false;
                            }
                        }
                    }
                    else if (strCheck == "W")
                    {
                        if (rgxWordID.IsMatch(strLine))
                        {
                            Match m = rgxWordID.Match(strLine);

                            if (bSubClause == true)
                            {
                                swProcessedMABNT.WriteLine();
                                bSubClause = false;
                            }

                            swProcessedMABNT.Write(" WordID: " + m.Groups["wordid"].Value);

                            strLine = strLine.Remove(0, m.Index + m.Length);
                        }
                    }
                    else if (strCheck == "Greek")
                    {
                        if (rgxGrk.IsMatch(strLine))
                        {
                            Match mm = rgxGrk.Match(strLine);

                            swProcessedMABNT.Write(" GreekID: " + mm.Groups["grkid"].Value + " GreekCL: " +
                                mm.Groups["cl"].Value + " Strongs: " + mm.Groups["sn"].Value + " Morphology: " +
                                mm.Groups["morph"].Value + " GreekWord: " + mm.Groups["greekword"].Value);

                            strLine = strLine.Remove(0, mm.Index + mm.Length);

                            swProcessedMABNT.WriteLine();
                        }
                    }
                    else if (strCheck == "Gloss")
                    {
                        if (rgxGloss.IsMatch(strLine))
                        {
                            Match mm = rgxGloss.Match(strLine);

                            swProcessedMABNT.Write(" Gloss: " + mm.Groups["gloss"].Value);

                            strLine = strLine.Remove(0, mm.Index + mm.Length);

                            swProcessedMABNT.WriteLine();
                            swProcessedMABNT.WriteLine();
                        }
                    }
                    else if (strCheck == "Done" || strCheck == "VV")
                    {
                        bContinue = false;
                    }
                }
            }

            swProcessedMABNT.Close();
        }

        public string TestNext(ref string strLineCopy, ref StreamWriter swProcessedMABNT)
        {
            //test for next out of 8
            int intV = 10000000;
            int intC = 10000000; //Ten Million should be longer than any of these lines, right??
            int intAt = 10000000;
            int intCL = 10000000;
            int intS = 10000000; //These are effectively NULL
            int intF = 10000000;
            int intW = 10000000;
            int intGreek = 10000000;
            int intGloss = 10000000;

            if (rgxVID.IsMatch(strLineCopy))
            {
                intV = rgxVID.Match(strLineCopy).Index;
            }

            if (rgxCLInfo.IsMatch(strLineCopy))
            {
                intC = rgxCLInfo.Match(strLineCopy).Index;
            }

            if (rgxConnector.IsMatch(strLineCopy))
            {
                intAt = rgxConnector.Match(strLineCopy).Index;
            }

            if (rgxCLLevel.IsMatch(strLineCopy))
            {
                intCL = rgxCLLevel.Match(strLineCopy).Index;
            }

            if (rgxSubCLInfo.IsMatch(strLineCopy))
            {
                intS = rgxSubCLInfo.Match(strLineCopy).Index;
            }

            if (rgxFuncInfo.IsMatch(strLineCopy))
            {
                intF = rgxFuncInfo.Match(strLineCopy).Index;
            }

            if (rgxWordID.IsMatch(strLineCopy))
            {
                intW = rgxWordID.Match(strLineCopy).Index;
            }

            if (rgxGrk.IsMatch(strLineCopy))
            {
                intGreek = rgxGrk.Match(strLineCopy).Index;
            }

            if (rgxGloss.IsMatch(strLineCopy))
            {
                intGloss = rgxGloss.Match(strLineCopy).Index;
            }

            if (strLineCopy == "<verses>")
            {
                return "VV";
            }
            else if (intV < intC && intV < intAt && intV < intCL && intV < intS && intV < intF && intV < intW && intV < intGreek && intV < intGloss && intV != 10000000)
            {
                return "V";
            }
            else if (intC < intV && intC < intAt && intC < intCL && intC < intS && intC < intF && intC < intW && intC < intGreek && intC < intGloss && intC != 10000000)
            {
                return "C";
            }
            else if (intAt < intV && intAt < intC && intAt < intCL && intAt < intS && intAt < intF && intAt < intW && intAt < intGreek && intAt < intGloss && intAt != 10000000)
            {
                if (bConnector == false) //it loops through once and turns on this bool, then it comes back here and returns "C"
                {
                    return "At";
                }
                else
                {
                    return "C";
                }
            }
            else if (intCL < intV && intCL < intC && intCL < intAt && intCL < intS && intCL < intF && intCL < intW && intCL < intGreek && intCL < intGloss && intCL != 10000000)
            {
                return "CL";
            }
            else if (intS < intV && intS < intC && intS < intAt && intS < intCL && intS < intF && intS < intW && intS < intGreek && intS < intGloss && intS != 10000000)
            {
                return "S";
            }
            else if (intF < intV && intF < intC && intF < intAt && intF < intCL && intF < intS && intF < intW && intF < intGreek && intF < intGloss && intF != 10000000)
            {
                return "F";
            }
            else if (intW < intV && intW < intC && intW < intAt && intW < intCL && intW < intS && intW < intF && intW < intGreek && intW < intGloss && intW != 10000000)
            {
                return "W";
            }
            else if (intGreek < intV && intGreek < intC && intGreek < intAt && intGreek < intCL && intGreek < intS && intGreek < intF && intGreek < intW && intGreek < intGloss && intGreek != 10000000)
            {
                return "Greek";
            }
            else if (intGloss < intV && intGloss < intC && intGloss < intAt && intGloss < intCL && intGloss < intS && intGloss < intF && intGloss < intW && intGloss < intGreek && intGloss != 10000000)
            {
                return "Gloss";
            }
            else if (intV == 10000000 && intC == 10000000 && intAt == 10000000 && intCL == 10000000 && intS == 10000000 && intF == 10000000 && intW == 10000000 && intGreek == 10000000 && intGloss == 10000000)
            {
                return "Done";
            }
            else
            {
                return "Error";
            }
        }

        public class Word
        {
            public int intBookNumber = 0;
            public int intChapterNumber = 0;
            public int intVerseNumber = 0;
            public int intWordID = 0;
            public int intGreekID = 0;
            public int intClauseLevel = 0;
            public int intGreekClauseLevel = 0;

            public string strGloss = "";
            public string strWordFunction = "";
            public string strClause = "";
            public string strClauseFunction = "";
            public string strSubclause = "";
            public string strSubclauseFunction = "";
            public string strStrongs = "";
            public string strMorphology = "";
            public string strGreekWord = "";
        }

        public void PostProcessArrows(ref StreamReader srMABNT, ref StreamWriter swProcessedMABNT,
            ref StreamWriter swWords)
        {
            bool bArrow = false;
            string strCurrentType = "";
            string strLastFunctionType = "";
            Dictionary<int, string> dArrows = new();
            Word wCurrentWord = new Word();
            Word wTemp = new Word();
            int intWordCounter = 0;
            int intMaxWordsIndex = 0;

            while (!srMABNT.EndOfStream)
            {
                string strLine = srMABNT.ReadLine().Trim().ToLower();
                string[] strsLine = strLine.Split();

                if (strLine != "")
                {
                    if (strLine.StartsWith("book:"))
                    {
                        if (dArrows.Count() > 0)
                        {
                            //write last verse
                            for (int intIndex = 1; intIndex <= dArrows.Count(); intIndex++)
                            {
                                swProcessedMABNT.Write(dArrows[intIndex] + " ");
                            }

                            swProcessedMABNT.WriteLine();
                            dArrows.Clear();
                        }
                    }

                    for (int intIndex = 0; intIndex < strsLine.Length; intIndex++)
                    {
                        string strPart = strsLine[intIndex].ToLower().Trim();

                        if (strPart.StartsWith("book:") || strPart.StartsWith("chapter:") ||
                        strPart.StartsWith("verse:") || strPart.StartsWith("clauselevel:") ||
                        strPart.StartsWith("clause:") || strPart.StartsWith("subclause:") ||
                        strPart.StartsWith("function:") || strPart.StartsWith("wordid:") ||
                        strPart.StartsWith("greekid:") || strPart.StartsWith("greekcl:") ||
                        strPart.StartsWith("strongs:") || strPart.StartsWith("morphology:") ||
                        strPart.StartsWith("greekword:") || strPart.StartsWith("gloss:"))
                        {
                            dArrows.Add(dArrows.Count() + 1, strPart + " ");
                            strCurrentType = strPart.TrimEnd(':');

                            if (strPart.StartsWith("wordid:") || strPart.StartsWith("clause:") 
                                || strPart.StartsWith("subclause:"))
                            {
                                strLastFunctionType = strPart.TrimEnd(':');
                            }
                        }
                        else
                        {
                            if (strPart == "→")
                            {
                                bArrow = true;
                                dArrows.Remove(dArrows.Count()); //removes "function: "
                            }
                            else if (strPart == "←")
                            {
                                //bool bBacktrack = true;
                                //bool bDoubleBacktrack = true;
                                //int intMaxIndex = dArrows.Count();
                                int intRemoveIndex = 0;
                                //string strClause = "";
                                //string strSubclause = "";

                                dArrows.Remove(dArrows.Count()); //removes "function: "

                                if (dArrows[dArrows.Count()].Contains("clause:"))
                                {
                                    intRemoveIndex = dArrows[dArrows.Count()].IndexOf("clause:");
                                 
                                    if (dArrows[dArrows.Count()].StartsWith("clause:"))
                                    {
                                        //remove the whole line, so we're not left with an empty line
                                        dArrows.Remove(dArrows.Count());
                                    }
                                    else
                                    { //this is to preserve anything that comes before "clause:" in the line (eg. "subclause:")
                                        dArrows[dArrows.Count()] = dArrows[dArrows.Count()].Remove(intRemoveIndex);
                                    }
                                }
                                
                                //intMaxIndex--;

                                //while (bDoubleBacktrack == true)
                                //{
                                    //remove the clause part of the last subclause line so
                                    // a successive backward and then forward arrow works by
                                    // connecting that correct subclause to the next forward
                                    // subclause (which is incorrectly labelled "clause:" throughout)
                                    //if (dArrows[intMaxIndex].StartsWith("subclause:"))// ||
                                //        dArrows[intMaxIndex].StartsWith("clause:") ||
                                //        bBacktrack == false)
                                    //{
                                    //    if (dArrows[2] == "verse: 4")
                                    //    {
                                    //        string strBreak = "";
                                    //    }
                                    //    if (dArrows[intMaxIndex].Contains(" clause:"))
                                    //    {
                                    //        int intClauseIndex = dArrows[intMaxIndex].IndexOf(" clause:");
                                    //        dArrows[intMaxIndex].Remove(intClauseIndex);
                                    //    }
                                //        if (bBacktrack == true)
                                //        {
                                //            strClause = dArrows[intMaxIndex];
                                //            intMaxIndex--;
                                //        }
                                //        else
                                //        {
                                //            if (dArrows[intMaxIndex].StartsWith("function:"))
                                //            {
                                //                intMaxIndex--;
                                //            }
                                //            else
                                //            {
                                //            //    if (strClause.StartsWith("clause:"))
                                //            //    {
                                //            //        strClause = strClause.Remove(0, 7);
                                //            //    }

                                //                //if (dArrows[intMaxIndex].StartsWith("clause:"))
                                //                //{
                                //                //    dArrows[intMaxIndex] = dArrows[intMaxIndex].Remove(0, 7);
                                //                //}
                                //                //else if (dArrows[intMaxIndex].StartsWith("subclause:"))
                                //                //{
                                //                //    dArrows[intMaxIndex]  = dArrows[intMaxIndex].Remove(0, 10);
                                //                //}

                                //                dArrows[intMaxIndex] = dArrows[intMaxIndex].Remove(0, 10).Trim() + " <-- " + strClause.Remove(0,7).Trim();
                                //                strSubclause = dArrows[intMaxIndex];

                                //                intMinRemoveIndex = intMaxIndex + 1;
                                                //bDoubleBacktrack = false;
                                //            }
                                //        }

                                        //bBacktrack = false;
                                    //}
                                    //else
                                    //{
                                    //    intMaxIndex--;
                                    //}
                                //}

                                //intMaxIndex = dArrows.Max(a => a.Key);
                                //intMinRemoveIndex = intMaxIndex;
                                //intMaxIndex = dArrows.Count();

                                //for (int intRemoveIndex = intMinRemoveIndex; intRemoveIndex <= intMaxIndex; intRemoveIndex++)
                                //{
                                //    dArrows.Remove(intRemoveIndex);
                                //}

                                ////reclaim clause
                                //for (int intBackwardsIndex = dArrows.Max(a=>a.Key); intBackwardsIndex > 0; intBackwardsIndex--)
                                //{
                                //    if (dArrows[intBackwardsIndex].StartsWith("clause:"))
                                //    {
                                //        wCurrentWord.strClause = dArrows[intBackwardsIndex].Remove(0, 7).Trim();
                                //        break;
                                //    }
                                //}

                                //wCurrentWord.strSubclause = strSubclause;
                            }
                            else if (bArrow == true)
                            {
                                bool bBacktrack = true;
                                int intMaxIndex = dArrows.Count();

                                if (wCurrentWord.intBookNumber == 40 && wCurrentWord.intChapterNumber == 14 && wCurrentWord.intVerseNumber == 8)
                                {
                                    string s = "";
                                    //are you about to remove something important, like "clause: " in the wrong spot?
                                    //maybe a caveat here or in the following while block to test for "clause@(sub?)clause-->(sub?)clause"
                                }

                                dArrows.Remove(dArrows.Count()); //removes "clause: " or "subclause: "
                                intMaxIndex--;

                                while (bBacktrack == true)
                                {
                                    if (dArrows[intMaxIndex].StartsWith("subclause: ") ||
                                        dArrows[intMaxIndex].StartsWith("clause: "))
                                    {
                                        int intFirstColonIndex = dArrows[intMaxIndex].IndexOf(':');

                                        strCurrentType = dArrows[intMaxIndex].Substring(0, intFirstColonIndex).Trim();

                                        if (!dArrows[intMaxIndex].Contains('@'))
                                        { 
                                            //only remove the "clause:" if it's not a compound @ and arrow
                                            //  ie. clause: 14:8.w8-w9 @14:8.w1-w18 --> 14:8.w11-w18 
                                            dArrows[intMaxIndex] = dArrows[intMaxIndex].Remove(0, intFirstColonIndex + 1).Trim();
                                        }

                                        dArrows[intMaxIndex] += " --> " + strPart;
                                        bBacktrack = false;

                                        wCurrentWord.strSubclause = dArrows[intMaxIndex];
                                    }
                                    else
                                    {
                                        intMaxIndex--;
                                    }
                                }

                                bArrow = false;
                            }
                            else
                            {
                                dArrows[dArrows.Count()] += strPart + " "; //this handles the @, also

                                switch (strCurrentType)
                                {
                                    case "clause":
                                        wCurrentWord.strClause = dArrows[dArrows.Count()].Remove(0, 7).Trim();
                                        break;
                                    case "subclause":
                                        wCurrentWord.strSubclause = dArrows[dArrows.Count()].Remove(0, 10).Trim();
                                        break;
                                    case "clauselevel":
                                        wCurrentWord.intClauseLevel = Convert.ToInt16(dArrows[dArrows.Count()].Remove(0, 12).Trim());
                                        break;
                                    case "book":
                                        wCurrentWord.intBookNumber = Convert.ToInt16(dArrows[dArrows.Count()].Remove(0, 5).Trim());
                                        break;
                                    case "chapter":
                                        wCurrentWord.intChapterNumber = Convert.ToInt16(dArrows[dArrows.Count()].Remove(0, 8).Trim());
                                        break;
                                    case "verse":
                                        wCurrentWord.intVerseNumber = Convert.ToInt16(dArrows[dArrows.Count()].Remove(0, 6).Trim());
                                        break;
                                    case "wordid":
                                        wCurrentWord.intWordID = Convert.ToInt32(dArrows[dArrows.Count()].Remove(0, 7).Trim().Substring(1).Trim());
                                        break;
                                    case "function":
                                        if (strLastFunctionType == "wordid")
                                        {
                                            wCurrentWord.strWordFunction = dArrows[dArrows.Count()].Remove(0, 9).Trim();
                                        }
                                        else if (strLastFunctionType == "clause")
                                        {
                                            wCurrentWord.strClauseFunction = dArrows[dArrows.Count()].Remove(0, 9).Trim();
                                        }
                                        else if (strLastFunctionType == "subclause")
                                        {
                                            wCurrentWord.strSubclauseFunction = dArrows[dArrows.Count()].Remove(0, 9).Trim();
                                        }
                                        break;
                                    case "gloss":
                                        if (intIndex == strsLine.Count() - 1) //this only works if gloss: is the only label in the line
                                        {
                                            wCurrentWord.strGloss = dArrows[dArrows.Count()].Remove(0, 6).Trim();

                                            if (wCurrentWord.strSubclauseFunction.StartsWith("clause:"))
                                            {
                                                wCurrentWord.strSubclauseFunction = wCurrentWord.strSubclauseFunction.Remove(0, 7).Trim();
                                            }

                                            if (wCurrentWord.strClauseFunction.StartsWith("subclause:"))
                                            {
                                                wCurrentWord.strClauseFunction = wCurrentWord.strClauseFunction.Remove(0, 10).Trim();
                                            }

                                            if (wCurrentWord.strSubclauseFunction.StartsWith("subclause:"))
                                            {
                                                wCurrentWord.strSubclauseFunction = wCurrentWord.strSubclauseFunction.Remove(0, 10).Trim();
                                            }

                                            if (wCurrentWord.strClauseFunction.StartsWith("clause:"))
                                            {
                                                wCurrentWord.strClauseFunction = wCurrentWord.strClauseFunction.Remove(0, 7).Trim();
                                            }

                                            intWordCounter++;

                                            dWords.Add(intWordCounter, wCurrentWord);
                                            dGlosses.Add(intWordCounter, wCurrentWord.strGloss);

                                            if (wCurrentWord.strClause != "")
                                            {
                                                dClauses.Add(intWordCounter, wCurrentWord.strClause);
                                            }

                                            if (wCurrentWord.strSubclause != "")
                                            {
                                                dSubclauses.Add(intWordCounter, wCurrentWord.strSubclause);
                                            }

                                            //not making a new wCurrentWord overwrites its properties inside the dictionaries,
                                            //but making a new wCurrentWord removes all the properties,
                                            //so this is to get the old properties into a new instance of Word
                                            wTemp = new();
                                            
                                            wTemp.intBookNumber = wCurrentWord.intBookNumber;
                                            wTemp.intChapterNumber = wCurrentWord.intChapterNumber;
                                            wTemp.intVerseNumber = wCurrentWord.intVerseNumber;
                                            wTemp.intWordID = wCurrentWord.intWordID;
                                            wTemp.strWordFunction = wCurrentWord.strWordFunction.Trim();
                                            wTemp.intClauseLevel = wCurrentWord.intClauseLevel;
                                            wTemp.strClause = wCurrentWord.strClause.Trim();
                                            wTemp.strClauseFunction = wCurrentWord.strClauseFunction.Trim();
                                            wTemp.strSubclause = wCurrentWord.strSubclause.Trim();
                                            wTemp.strSubclauseFunction = wCurrentWord.strSubclauseFunction.Trim();
                                            wTemp.intGreekID = wCurrentWord.intGreekID;
                                            wTemp.intGreekClauseLevel = wCurrentWord.intGreekClauseLevel;
                                            wTemp.strStrongs = wCurrentWord.strStrongs.Trim();
                                            wTemp.strMorphology = wCurrentWord.strMorphology.Trim();
                                            wTemp.strGreekWord = wCurrentWord.strGreekWord.Trim();
                                            wTemp.strGloss = wCurrentWord.strGloss.Trim();

                                            wCurrentWord = new();

                                            wCurrentWord.intBookNumber = wTemp.intBookNumber;
                                            wCurrentWord.intChapterNumber = wTemp.intChapterNumber;
                                            wCurrentWord.intVerseNumber = wTemp.intVerseNumber;
                                            wCurrentWord.intWordID = wTemp.intWordID;
                                            wCurrentWord.strWordFunction = wTemp.strWordFunction;
                                            wCurrentWord.intClauseLevel = wTemp.intClauseLevel;
                                            wCurrentWord.strClause = wTemp.strClause;
                                            wCurrentWord.strClauseFunction = wTemp.strClauseFunction;
                                            wCurrentWord.strSubclause = wTemp.strSubclause;
                                            wCurrentWord.strSubclauseFunction = wTemp.strSubclauseFunction;
                                            wCurrentWord.intGreekID = wTemp.intGreekID;
                                            wCurrentWord.intGreekClauseLevel = wTemp.intGreekClauseLevel;
                                            wCurrentWord.strStrongs = wTemp.strStrongs;
                                            wCurrentWord.strMorphology = wTemp.strMorphology;
                                            wCurrentWord.strGreekWord = wTemp.strGreekWord;
                                            wCurrentWord.strGloss = wTemp.strGloss;
                                        }
                                        break;
                                    case "greekid":
                                        wCurrentWord.intGreekID = Convert.ToInt32(dArrows[dArrows.Count()].Remove(0, 8).Trim().Substring(1).Trim());
                                        break;
                                    case "greekcl":
                                        wCurrentWord.intGreekClauseLevel = Convert.ToInt16(dArrows[dArrows.Count()].Remove(0, 8).Trim().Substring(1).Trim());
                                        break;
                                    case "strongs":
                                        wCurrentWord.strStrongs = dArrows[dArrows.Count()].Remove(0, 8).Trim().Substring(1).Trim();
                                        break;
                                    case "morphology":
                                        wCurrentWord.strMorphology = dArrows[dArrows.Count()].Remove(0, 11).Trim();
                                        break;
                                    case "greekword":
                                        wCurrentWord.strGreekWord = dArrows[dArrows.Count()].Remove(0, 10).Trim();
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            //write last verse
            for (int intIndex = 1; intIndex <= dArrows.Count(); intIndex++)
            {
                swProcessedMABNT.Write(dArrows[intIndex] + " ");
            }

            swProcessedMABNT.WriteLine();
            dArrows.Clear();
            swProcessedMABNT.Close();

            swWords.WriteLine("Gloss ^ Book Number ^ Chapter Number ^ Verse Number ^ Word ID ^ Word Function ^ Clause Level" +
                " ^ Clause ^ Clause Function ^ Subclause ^ Subclause Function ^ Greek Clause Level ^ Greek ID ^ Strongs" +
                " ^ Morphology ^ Greek Word");

            intMaxWordsIndex = dWords.Keys.Max(a => a);

            for (int intWordsIndex = 1; intWordsIndex <= intMaxWordsIndex; intWordsIndex++)
            {
                swWords.WriteLine(dWords[intWordsIndex].strGloss + " ^ " + dWords[intWordsIndex].intBookNumber.ToString() +
                    " ^ " + dWords[intWordsIndex].intChapterNumber.ToString() + " ^ " + dWords[intWordsIndex].intVerseNumber.ToString() +
                    " ^ " + dWords[intWordsIndex].intWordID.ToString() + " ^ " + dWords[intWordsIndex].strWordFunction +
                    " ^ " + dWords[intWordsIndex].intClauseLevel.ToString() + " ^ " + dWords[intWordsIndex].strClause +
                    " ^ " + dWords[intWordsIndex].strClauseFunction + " ^ " + dWords[intWordsIndex].strSubclause +
                    " ^ " + dWords[intWordsIndex].strSubclauseFunction + " ^ " + dWords[intWordsIndex].intGreekClauseLevel.ToString() +
                    " ^ " + dWords[intWordsIndex].intGreekID.ToString() + " ^ " + dWords[intWordsIndex].strStrongs +
                    " ^ " + dWords[intWordsIndex].strMorphology + " ^ " + dWords[intWordsIndex].strGreekWord);
            }

            swWords.Close();
        }

        //Normalizes the morphology column by placing dashes between each letter
        public void NormalizeMorphology(string strMABNTFilename, string strRMACMorphologyFilename)
        {
            //THIS ASSUMES THE MORPHOLOGY IS IN THE 15TH COLUMN

            StreamReader srInput;
            StreamReader srMorph;
            StreamWriter swOutput;
            StringBuilder sbOutput = new();
            bool bPastFirstLine = false;
            Dictionary<string, string> dMorphologyCodes = new();

            srInput = new(strMABNTFilename);
            srMorph = new(strRMACMorphologyFilename);

            while (!srMorph.EndOfStream)
            {
                string strLine = srMorph.ReadLine();
                string[] strsLine = strLine.Split('\t');

                if (bPastFirstLine == false)
                {
                    bPastFirstLine = true;
                }
                else if (!dMorphologyCodes.ContainsKey(strsLine[2].ToLower()))
                {
                    dMorphologyCodes.Add(strsLine[2].ToLower(), strsLine[3]);
                }
            }

            srMorph.Close();
            bPastFirstLine = false;

            while (!srInput.EndOfStream)
            {
                string strLine = srInput.ReadLine();
                string[] strsLine = strLine.Split('^');
                int intColumnCount = strsLine.Length;

                if (bPastFirstLine == false)
                {
                    sbOutput.AppendLine("Gloss ^ Book Number ^ Chapter Number ^ Verse Number ^ Word ID ^ " +
                        "Word Function ^ Clause Level ^ Clause ^ Clause Function ^ Subclause ^ " +
                        "Subclause Function ^ Greek Clause Level ^ Greek ID ^ Strongs ^ Morphology ^ Greek Word");
                    bPastFirstLine = true;
                }
                else
                {
                    //just repeat the first 14 columns
                    for (int intIndex = 0; intIndex < 14; intIndex++)
                    {
                        sbOutput.Append(strsLine[intIndex].Trim());
                        sbOutput.Append(" ^ ");
                    }

                    //the morphology
                    //foreach (char chrMorphologyPart in strsLine[14])
                    //{
                    //    if (chrMorphologyPart != '-' & chrMorphologyPart != ' ')
                    //    {
                    //        sbOutput.Append(chrMorphologyPart);
                    //        sbOutput.Append('-');
                    //    }
                    //}
                    sbOutput.Append(dMorphologyCodes[strsLine[14].Trim()]);
                    
                    //remove the last dash
                    //sbOutput.Remove(sbOutput.Length - 1, 1);

                    //the last column
                    sbOutput.Append(" ^ ");
                    sbOutput.AppendLine(strsLine[15].Trim());
                }
            }

            srInput.Close();

            swOutput = new(strMABNTFilename);

            swOutput.Write(sbOutput.ToString());
            swOutput.Close();
        }
    }

}

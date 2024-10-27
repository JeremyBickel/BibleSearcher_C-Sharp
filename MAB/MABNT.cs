using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB
{
    public class VersesNT
    {
        public Dictionary<int, VerseNT> dVersesNT = new Dictionary<int, VerseNT>();

        public void Load(ref StreamReader srMABNT)
        {
            while (!srMABNT.EndOfStream)
            {
                string strLine = srMABNT.ReadLine().Trim();
                string[] strsLine = strLine.Split();
                string strCurrentLabel = "";
                int intCurrentBookNumber = 0;
                int intCurrentChapterNumber = 0;
                int intCurrentVerseNumber = 0;
                int intClauseLevel = 0;
                int intGreekID = 0;
                string strLast1 = "";
                string strLast2 = "";
                string strLast3 = "";
                string strLast1Type = "";
                string strLast2Type = "";
                string strLast3Type = "";
                bool bBackArrow = false;

                for (int intCounter = 0; intCounter < strsLine.Length; intCounter++)
                {
                    string strPart = strsLine[intCounter].Trim().ToLower();

                    if (strPart != "")
                    {
                        if (strPart.StartsWith("function:"))
                        {
                            if (strCurrentLabel == "subclause")
                            {
                                strCurrentLabel = "subclausefunction";
                            }
                            else if (strCurrentLabel == "wordid")
                            {
                                strCurrentLabel = "wordfunction";
                            }
                            else
                            {
                                strCurrentLabel = strPart.TrimEnd(':');
                            }
                        }
                        else if (strPart.StartsWith("@"))
                        {
                            int intVerseID = dVersesNT.Max(a => a.Key);
                            int intClauseID = 0;

                            try
                            {
                                intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                            }
                            catch
                            {
                                intClauseID = 1;
                                dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                            }

                            dVersesNT[intVerseID].dClauses[intClauseID].strClause += " " + strPart;
                        }
                        else if (strPart.EndsWith(':'))
                        {
                            strCurrentLabel = strPart.Substring(0, strPart.Length - 1).ToLower();
                        }
                        else if (strPart.StartsWith("-->"))
                        {
                            strCurrentLabel = "arrow";
                        }
                        else if (strPart.StartsWith("<--"))
                        {
                            bBackArrow = true;
                        }
                        else
                        {
                            if (strCurrentLabel == "book")
                            {
                                intCurrentBookNumber = Convert.ToInt16(strPart);
                            }
                            else if (strCurrentLabel == "chapter") //chapter
                            {
                                intCurrentChapterNumber = Convert.ToInt16(strPart);
                            }
                            else if (strCurrentLabel == "verse") //verse
                            {
                                //all these ids should be .count() + 1 before creation and .count()
                                //if they are assigned after they are created, except for wordid,
                                //which is taken directly from the data and not caluculated like
                                //the rest, but they are always dWords.Count() + 1 on creation and
                                //.Count() afterward.
                                int intVerseID = 0;

                                try
                                {
                                    intVerseID = dVersesNT.Max(a => a.Key) + 1;
                                }
                                catch
                                {
                                    intVerseID = 1;
                                }

                                intCurrentVerseNumber = Convert.ToInt16(strPart);

                                dVersesNT.Add(intVerseID, new VerseNT());
                                dVersesNT[intVerseID].intBookNumber = intCurrentBookNumber;
                                dVersesNT[intVerseID].intChapterNumber = intCurrentChapterNumber;
                                dVersesNT[intVerseID].intVerseNumber = intCurrentVerseNumber;

                            }
                            else if (strCurrentLabel == "clauselevel") //clauselevel
                            {
                                intClauseLevel = Convert.ToInt16(strPart);
                            }
                            else if (strCurrentLabel == "clause") //clause
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);

                                if (bBackArrow == true)
                                {
                                    int intClauseID = 0;

                                    try
                                    {
                                        intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                                    }
                                    catch
                                    {
                                        intClauseID = 1;
                                        dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                    }

                                    dVersesNT[intVerseID].dClauses[intClauseID].strClause += " <-- " + strPart;
                                }
                                else
                                {
                                    int intClauseID = 0;

                                    try
                                    {
                                        intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key) + 1;
                                    }
                                    catch
                                    {
                                        intClauseID = 1;
                                    }

                                    dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                    dVersesNT[intVerseID].dClauses[intClauseID].intClauseLevel = intClauseLevel;
                                    dVersesNT[intVerseID].dClauses[intClauseID].strClause = strPart;

                                    //push clause
                                    strLast3 = strLast2;
                                    strLast3Type = strLast2Type;
                                    strLast2 = strLast1;
                                    strLast2Type = strLast1Type;
                                    strLast1 = strPart;
                                    strLast1Type = "clause";
                                }
                            }
                            else if (strCurrentLabel == "arrow")
                            {
                                if (intCurrentBookNumber == 40 && intCurrentChapterNumber == 14 && 
                                    intCurrentVerseNumber == 8)
                                {
                                    string s = "";
                                }

                                if (strLast1Type == "subclause")
                                {
                                    int intVerseID = dVersesNT.Max(a => a.Key);
                                    int intClauseID = 0;
                                    int intSubclauseID = 0;

                                    try
                                    {
                                        intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                                    }
                                    catch
                                    {
                                        intClauseID = 1;
                                        dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                    }

                                    try
                                    {
                                        intSubclauseID = dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Max(a => a.Key);
                                    }
                                    catch
                                    {
                                        intSubclauseID = 1;
                                        dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Add(intSubclauseID, new SubclauseNT());
                                    }

                                    dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses[intSubclauseID].strSubclause += " --> " + strPart;
                                    strCurrentLabel = "";
                                }
                                else if (strLast1Type == "clause")
                                {
                                    int intVerseID = dVersesNT.Max(a => a.Key);
                                    int intClauseID = 0;

                                    try
                                    {
                                        intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                                    }
                                    catch
                                    {
                                        intClauseID = 1;
                                        dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                    }

                                    dVersesNT[intVerseID].dClauses[intClauseID].strClause += " --> " + strPart;
                                    strCurrentLabel = "";
                                }
                                else
                                {
                                    throw new Exception("An arrow was found after something other than a clause or a subclause. Add code.");
                                }
                            }
                            else if (strCurrentLabel == "subclause")
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intClauseID = 0;
                                int intSubclauseID = 0;

                                try
                                {
                                    intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                                }
                                catch
                                {
                                    intClauseID = 1;
                                }

                                if (!dVersesNT[intVerseID].dClauses.ContainsKey(intClauseID))
                                {
                                    dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                }

                                try
                                {
                                    intSubclauseID = dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Max(a => a.Key) + 1;
                                }
                                catch
                                {
                                    intSubclauseID = 1;
                                }

                                dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Add(intSubclauseID, new SubclauseNT());
                                dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses[intSubclauseID].strSubclause = strPart;

                                //push clause
                                strLast3 = strLast2;
                                strLast3Type = strLast2Type;
                                strLast2 = strLast1;
                                strLast2Type = strLast1Type;
                                strLast1 = strPart;
                                strLast1Type = "subclause";
                            }
                            else if (strCurrentLabel == "subclausefunction")
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intClauseID = 0;
                                int intSubclauseID = 0;

                                try
                                {
                                    intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                                }
                                catch
                                {
                                    intClauseID = 1;
                                    dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                }

                                try
                                {
                                    intSubclauseID = dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Max(a => a.Key);
                                }
                                catch
                                {
                                    intSubclauseID = 1;
                                    dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Add(intSubclauseID, new SubclauseNT());
                                }

                                dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses[intSubclauseID].strFunction = strPart;
                            }
                            else if (strCurrentLabel == "wordfunction")
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key);

                                dVersesNT[intVerseID].dWords[intWordID].strFunction = strPart;
                            }
                            else if (strCurrentLabel == "wordid")
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intClauseID = 0;
                                int intSubclauseID = 0;
                                int intWordID = Convert.ToInt32(strPart.TrimStart('w')); //the wordid is taken from the data, but it is the same as if it were calculated like the other ids

                                try
                                {
                                    intClauseID = dVersesNT[intVerseID].dClauses.Max(a => a.Key);
                                }
                                catch
                                {
                                    intClauseID = 1;
                                    dVersesNT[intVerseID].dClauses.Add(intClauseID, new ClauseNT());
                                }

                                try
                                {
                                    intSubclauseID = dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Max(a => a.Key);
                                }
                                catch
                                {
                                    intSubclauseID = 1;
                                    dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses.Add(intSubclauseID, new SubclauseNT());
                                }

                                dVersesNT[intVerseID].dWords.Add(intWordID, new WordNT());
                                dVersesNT[intVerseID].dWords[intWordID].intWordID = intWordID;

                                dVersesNT[intVerseID].dClauses[intClauseID].dSubclauses[intSubclauseID].lWordIDs.Add(intWordID);
                            }
                            else if (strCurrentLabel == "greekid")
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key); //wordids are always .count() after creation

                                intGreekID = Convert.ToInt32(strPart.TrimStart('w'));

                                dVersesNT[intVerseID].dWords[intWordID].greek = new GreekNT();
                                dVersesNT[intVerseID].dWords[intWordID].greek.intID = intGreekID;
                            }
                            else if (strCurrentLabel == "greekcl") //greekcl
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key); //wordids are always .count() after creation

                                dVersesNT[intVerseID].dWords[intWordID].greek.intClauseLevel =
                                    Convert.ToInt32(strPart.TrimStart('c'));
                            }
                            else if (strCurrentLabel == "strongs") //strongs
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key); //wordids are always .count() after creation

                                dVersesNT[intVerseID].dWords[intWordID].greek.intStrongs =
                                    Convert.ToInt32(strPart.TrimStart('g'));
                            }
                            else if (strCurrentLabel == "morphology") //morphology
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key); //wordids are always .count() after creation

                                dVersesNT[intVerseID].dWords[intWordID].greek.strMorphology = strPart;
                            }
                            else if (strCurrentLabel == "greekword") //greekword
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key); //wordids are always .count() after creation

                                dVersesNT[intVerseID].dWords[intWordID].greek.strGreekWord = strPart;
                            }
                            else if (strCurrentLabel == "gloss")
                            {
                                int intVerseID = dVersesNT.Max(a => a.Key);
                                int intWordID = dVersesNT[intVerseID].dWords.Max(a => a.Key); //wordids are always .count() after creation

                                dVersesNT[intVerseID].dWords[intWordID].strGloss = strPart;
                            }
                        }
                    }
                }

                //swProcessedMABNT.Write("Book: " + Convert.ToInt16(m.Groups["book"].Value) +
                //            " Chapter: " + Convert.ToInt16(m.Groups["chapter"].Value) +
                //            " Verse: " + Convert.ToInt16(m.Groups["verse"].Value));
                //swProcessedMABNT.Write(" ClauseLevel: " + Convert.ToInt16(m.Groups["cllevel"].Length));
                //swProcessedMABNT.Write(" SubClause: " + mm.Groups["subclinfo"].Value);
                //swProcessedMABNT.Write(" Function: " + mm.Groups["funcinfo"].Value);
                //swProcessedMABNT.Write(" WordID: " + m.Groups["wordid"].Value);
                //swProcessedMABNT.Write(" GreekID: " + mm.Groups["grkid"].Value + " GreekCL: " +
                //                mm.Groups["cl"].Value + " Strongs: " + mm.Groups["sn"].Value + " Morphology: " +
                //                mm.Groups["morph"].Value + " GreekWord: " + mm.Groups["greekword"].Value);
                //swProcessedMABNT.Write(" Gloss: " + mm.Groups["gloss"].Value);
            }

            srMABNT.Close();
        }
    }

    public class VerseNT
    {
        public int intBookNumber = 0;
        public int intChapterNumber = 0;
        public int intVerseNumber = 0;

        public Dictionary<int, ClauseNT> dClauses = new();
        public Dictionary<int, WordNT> dWords = new();
    }

    public class ClauseNT
    {
        public int intClauseLevel = 0;
        public string strClause = "";

        public Dictionary<int, SubclauseNT> dSubclauses = new();
    }

    public class SubclauseNT
    {
        public string strSubclause = "";
        public string strFunction = "";

        public List<int> lWordIDs = new List<int>();
    }

    public class WordNT
    {
        public int intWordID = 0;
        public string strFunction = "";
        public string strGloss = "";
        public GreekNT greek = new();
    }

    public class GreekNT
    {
        public int intID = 0;
        public int intClauseLevel = 0;
        public int intStrongs = 0;
        public string strMorphology = "";
        public string strGreekWord = "";
    }
}
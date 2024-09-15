using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MAB
{
    public class VersesOT
    {
        public Dictionary<int, VerseOT> dVerses = new();
        private Dictionary<string, string> dHebrewParseCodes = new();

        public void ReadHebrewParseCodes(ref StreamReader srHebrewParseCodes)
        {
            while (!srHebrewParseCodes.EndOfStream)
            {
                string strLine = srHebrewParseCodes.ReadLine();
                string[] strsLine = strLine.Split("\t");
                string strParseCode = strsLine[0].Trim();
                string strParseText = strsLine[1].Trim();

                if (!dHebrewParseCodes.ContainsKey(strParseCode))
                {
                    dHebrewParseCodes.Add(strParseCode, strParseText);
                }
            }
        }

        public void Load(ref StreamReader srMABOT)
        {
            while (!srMABOT.EndOfStream)
            {
                string strLine = srMABOT.ReadLine();
                string[] strsLine = strLine.Split('^');
                
                for(int intCounter = 0; intCounter < strsLine.Length; intCounter++)
                {
                    string strPart = strsLine[intCounter].Trim();

                    if (strPart.Trim() != "")
                    {
                        if (!strPart.Contains(':')) //eg. "v1.1.1"
                        {
                            string[] strsVerseID;

                            dVerses.Add(dVerses.Count + 1, new VerseOT());

                            strsVerseID = strPart.Split('.');

                            dVerses[dVerses.Count].intBook = Convert.ToInt16(strsVerseID[0].Substring(1)); //the first character is 'v'
                            dVerses[dVerses.Count].intChapter = Convert.ToInt16(strsVerseID[1]);
                            dVerses[dVerses.Count].intVerse = Convert.ToInt16(strsVerseID[2]);
                        }
                        else
                        {
                            string[] strsPart = strPart.Split(':');

                            switch (strsPart[0])
                            {
                                case "c":
                                    dVerses[dVerses.Count].dClauses.Add(
                                        dVerses[dVerses.Count].dClauses.Count + 1, new ClauseOT());

                                    if (strsPart[1].Contains('.')) //referencing clause
                                    {
                                        string strThisID = strsPart[1].Split('.')[0].Trim().Substring(1);
                                        string strReferencedID = strsPart[1].Split('.')[1].Trim().Substring(1);

                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                            .intID = Convert.ToInt32(strThisID);

                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                            .intReferencedID = Convert.ToInt32(strReferencedID);
                                    }
                                    else
                                    {
                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                            .intID = Convert.ToInt32(strsPart[1].Trim().Substring(1));

                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                            .intReferencedID = 0;
                                    }
                                    break;
                                case "CKind":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .strKind = strsPart[1].Trim();
                                    break;
                                case "CTyp":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .strType = strsPart[1].Trim();
                                    break;
                                case "p":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases.Add(
                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases.Count + 1, new PhraseOT());

                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .intPID = Convert.ToInt32(strsPart[1].Trim().Substring(1));
                                    break;
                                case "pd":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .strPD = strsPart[1].Trim();
                                    break;
                                case "pt":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .strPT = strsPart[1].Trim();
                                    break;
                                case "pu":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .strPU = strsPart[1].Trim();
                                    break;
                                case "w":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords.Add(
                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords.Count + 1, new WordOT());

                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords[
                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords.Count]
                                                .intWID = Convert.ToInt32(strsPart[1].Trim().Substring(2));
                                    break;
                                case "we":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords[
                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords.Count]
                                                .strEnglish = strsPart[1].Trim();
                                    break;
                                case "wp":
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords[
                                        dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                        .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                            .dWords.Count]
                                                .strParse = dHebrewParseCodes[strsPart[1].Trim()];
                                    break;
                            }
                        }
                    }
                }
            }

            srMABOT.Close();
        }

        public void WriteParseCombinations(ref StreamWriter swCombinations, ref StreamWriter swWordCombinations,
            ref StreamWriter swWordKind, ref StreamWriter swWordType, ref StreamWriter swWordPD, ref StreamWriter swWordPT,
            ref StreamWriter swWordPU, ref StreamWriter swFormatted)
        {
            Dictionary<string, List<int>> dCombinations = new();
            Dictionary<string, List<string>> dWordCombinations = new();
            Dictionary<string, List<string>> dWordKind = new();
            Dictionary<string, List<string>> dWordType = new();
            Dictionary<string, List<string>> dWordPD = new();
            Dictionary<string, List<string>> dWordPT = new();
            Dictionary<string, List<string>> dWordPU = new();

            foreach (int intVerseID in dVerses.Keys.OrderBy(a => a))
            {
                MAB.VerseOT vCurrent = dVerses[intVerseID];
                
                swFormatted.WriteLine(vCurrent.intBook.ToString("##") + '.' +
                    vCurrent.intChapter.ToString("###") + '.' +
                    vCurrent.intVerse.ToString("##") + " - " + intVerseID.ToString());

                //c = id, referencedid, kind, type
                //p = id, pd, pt, pu
                //w = id, english, parse

                foreach (int intClauseID in vCurrent.dClauses.Keys.OrderBy(a => a))
                {
                    swFormatted.WriteLine(vCurrent.dClauses[intClauseID].intID.ToString() + '.' +
                        vCurrent.dClauses[intClauseID].intReferencedID.ToString() + " CKind: " +
                        vCurrent.dClauses[intClauseID].strKind + " CType: " +
                        vCurrent.dClauses[intClauseID].strType);

                    foreach (int intPhraseID in vCurrent.dClauses[intClauseID].dPhrases.Keys.OrderBy(a => a))
                    {
                        swFormatted.Write("\t" + vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].intPID.ToString("######"));
                            
                        if (vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPD.Trim() != "")
                        {
                            swFormatted.Write("    Det: " + vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPD);
                        }

                        if (vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPT.Trim() != "")
                        {
                            swFormatted.Write("   Text: " + vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPT);
                        }

                        if (vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPU.Trim() != "")
                        {
                            swFormatted.Write("  Undet: " + vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPU);
                        }

                        swFormatted.WriteLine();

                        foreach (int intWordID in vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].dWords.Keys.OrderBy(a => a))
                        {
                            string strWord = vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].dWords[intWordID].strEnglish;
                            string strCombination = vCurrent.dClauses[intClauseID].strKind + "|" +
                                vCurrent.dClauses[intClauseID].strType + "|" +
                                vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPD + "|" +
                                vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPT + "|" +
                                vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPU + "|" +
                                vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].dWords[intWordID].strParse;

                            swFormatted.WriteLine("\t\t" + vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].dWords[intWordID].intWID.ToString("######") +
                                " " + vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].dWords[intWordID].strEnglish + "\t\t" +
                                vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].dWords[intWordID].strParse);

                            if (!dCombinations.ContainsKey(strCombination))
                            {
                                dCombinations.Add(strCombination, new List<int>());
                            }

                            if (!dWordCombinations.ContainsKey(strWord))
                            {
                                dWordCombinations.Add(strWord, new List<string>());
                                dWordKind.Add(strWord, new List<string>());
                                dWordType.Add(strWord, new List<string>());
                                dWordPD.Add(strWord, new List<string>());
                                dWordPT.Add(strWord, new List<string>());
                                dWordPU.Add(strWord, new List<string>());
                            }


                            dCombinations[strCombination].Add(intVerseID);
                            dWordCombinations[strWord].Add(strCombination);
                            dWordKind[strWord].Add(vCurrent.dClauses[intClauseID].strKind);
                            dWordType[strWord].Add(vCurrent.dClauses[intClauseID].strType);
                            dWordPD[strWord].Add(vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPD);
                            dWordPT[strWord].Add(vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPT);
                            dWordPU[strWord].Add(vCurrent.dClauses[intClauseID].dPhrases[intPhraseID].strPU);
                        }
                    }
                }

            }
            swFormatted.Close();

            foreach (string strKey in dCombinations.Keys.OrderBy(a => a))
            {
                swCombinations.Write(strKey + " - " + dCombinations[strKey].Count.ToString());

                foreach (int intVerseID in dCombinations[strKey].Distinct().OrderBy(a=>a))
                {
                    swCombinations.Write(" ^ " + intVerseID.ToString());
                }

                swCombinations.WriteLine();
            }

            swCombinations.Close();

            foreach(string strKey in dWordCombinations.Keys.OrderBy(a => a))
            {
                swWordCombinations.Write(strKey + " ^ " + dWordCombinations[strKey].Count.ToString());

                foreach (string strCombination in dWordCombinations[strKey].Distinct().OrderBy(a=>a))
                {
                    swWordCombinations.Write(" ^ " + strCombination);
                }

                swWordCombinations.WriteLine();
            }

            swWordCombinations.Close();

            foreach (string strKey in dWordKind.Keys.OrderBy(a => a))
            {
                swWordKind.Write(strKey + " ^ " + dWordKind[strKey].Count.ToString());

                foreach (string strCombination in dWordKind[strKey].Distinct().OrderBy(a => a))
                {
                    swWordKind.Write(" ^ " + strCombination);
                }

                swWordKind.WriteLine();
            }

            swWordKind.Close();

            foreach (string strKey in dWordType.Keys.OrderBy(a => a))
            {
                swWordType.Write(strKey + " ^ " + dWordType[strKey].Count.ToString());

                foreach (string strCombination in dWordType[strKey].Distinct().OrderBy(a => a))
                {
                    swWordType.Write(" ^ " + strCombination);
                }

                swWordType.WriteLine();
            }

            swWordType.Close();

            foreach (string strKey in dWordPD.Keys.OrderBy(a => a))
            {
                swWordPD.Write(strKey + " ^ " + dWordPD[strKey].Count.ToString());

                foreach (string strCombination in dWordPD[strKey].Distinct().OrderBy(a => a))
                {
                    swWordPD.Write(" ^ " + strCombination);
                }

                swWordPD.WriteLine();
            }

            swWordPD.Close();

            foreach (string strKey in dWordPT.Keys.OrderBy(a => a))
            {
                swWordPT.Write(strKey + " ^ " + dWordPT[strKey].Count.ToString());

                foreach (string strCombination in dWordPT[strKey].Distinct().OrderBy(a => a))
                {
                    swWordPT.Write(" ^ " + strCombination);
                }

                swWordPT.WriteLine();
            }

            swWordPT.Close();

            foreach (string strKey in dWordPU.Keys.OrderBy(a => a))
            {
                swWordPU.Write(strKey + " ^ " + dWordPU[strKey].Count.ToString());

                foreach (string strCombination in dWordPU[strKey].Distinct().OrderBy(a=>a))
                {
                    swWordPU.Write(" ^ " + strCombination);
                }

                swWordPU.WriteLine();
            }

            swWordPU.Close();
        }
    }

    public class VerseOT
    {
        public int intBook = 0;
        public int intChapter = 0;
        public int intVerse = 0;

        public Dictionary<int, ClauseOT> dClauses = new();
    }

    public class ClauseOT
    {
        public int intID = 0;
        public int intReferencedID = 0;
        public string strKind = "";
        public string strType = "";

        public Dictionary<int, PhraseOT> dPhrases = new();
    }

    public class PhraseOT
    {
        public int intPID = 0;
        public string strPD = "";
        public string strPT = "";
        public string strPU = "";

        public Dictionary<int, WordOT> dWords = new();
    }

    public class WordOT
    {
        public int intWID = 0;
        public string strEnglish = "";
        public string strParse = "";
    }
}

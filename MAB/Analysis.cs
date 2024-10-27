using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pericope;

namespace MAB
{
    public class PericopeRange
    {
        public string strBookName = "";
        public int intStartChapter = 0;
        public int intEndChapter = 0;
        public int intStartVerse = 0;
        public int intEndVerse = 0;

        public PericopeRange() { }
    }
    internal class Analysis
    {
        public Dictionary<string, List<string>> dWFM = new();  //D<Word Function + " ^ " + Morphology, List<Gloss>>
        public Dictionary<string, List<string>> dWFSF = new(); //D<Word Function + " ^ " + Subclause Function, List<Gloss>>
        public Dictionary<string, List<string>> dWFCF = new(); //D<Word Function + " ^ " + Clause Function, List<Gloss>>
        public Dictionary<string, List<string>> dMSF = new(); //D<Morphology + " ^ " + Subclause Function, List<Gloss>>
        public Dictionary<string, List<string>> dMCF = new(); //D<Morphology + " ^ " + Clause Function, List<Gloss>>
        public Dictionary<string, List<string>> dSFCF = new(); //D<Subclause Function + " ^ " + Clause Function, List<Gloss>>
        public Dictionary<string, List<string>> dAll = new(); //D<Word Function + " ^ " + Morphology + " ^ " + Subclause Function + " ^ " + Clause Function, List<Gloss>>

        public Dictionary<string, int> dCounts = new(); //D<Gloss + " ^ " + [WFM|WFSF|WFCF|MSF|MCF|SFCF], Count of that gloss in that other Dictionary>

        public Dictionary<string, int> dMorphologyCounts = new(); //D<morphology 1 , morphology 2 , ... ^ count> ; every morphology inside a subclause
        public Dictionary<string, int> dWordFunctionCounts = new(); //D<word function 1 , word function 2 , ... ^ count> ; every word function inside a subclause
        public Dictionary<string, int> dSubclauseFunctionCounts = new(); //D<subclause function 1 , subclause function 2 , ... ^ count> ; every subclause inside a clause
        
        //requires pericopes
        public Dictionary<string, int> dClauseFunctionCounts = new(); //D<clause function 1 , clause function 2 , ... ^ count> ; every clause function between pericopes

        public Dictionary<string, Dictionary<string, int>> dMSFCounts = new(); //D<morphology 1 , morphology 2 , ... ^ D<subclause function, count>> ; every morphology inside a subclause function of a certain type, then that subclause function and the morphology count inside that subclause function
        public Dictionary<string, Dictionary<string, int>> dWFSFCounts = new(); //D<word function 1 , word function 2 , ... ^ D<subclause function, count>> ; every word function inside a subclause function of a certain type, then that subclause function and the word function count inside that subclause function
        public Dictionary<string, Dictionary<string, int>> dSFCFCounts = new(); //D<subclause function 1 , subclause function 2 , ... ^ D<clause function, count>> ; every subclause function inside a clause function of a certain type, then that clause function and the subclause function count in that clause function

        public Dictionary<string, Dictionary<string, int>> dMorphologyPhraseCounts = new(); //same key as dMorphologyCounts, but the value is D<phrase, count>
        public Dictionary<string, Dictionary<string, int>> dWordFunctionPhraseCounts = new(); //same key as dWordFunctionCounts, but the value is D<phrase, count>
        public Dictionary<string, Dictionary<string, int>> dSubclauseFunctionPhraseCounts = new(); //same key as dSubclauseFunctionCounts, but the value is D<phrase, count>

        //requires pericopes
        public Dictionary<string, Dictionary<string, int>> dClauseFunctionPhraseCounts = new(); //same key as dClauseFunctionCounts, but the value is D<phrase, count>

        //D<whole gloss from entire subclause, D<SF, Count>>
        public Dictionary<string, Dictionary<string, int>> dGlossSFCounts = new();

        public Analysis(ref Dictionary<int, ClassifyMABNT.Word> dWords, 
            ref Dictionary<int, List<PericopeVerse>> dRangedVerses, ref StreamWriter swWFM,
            ref StreamWriter swWFSF, ref StreamWriter swWFCF, ref StreamWriter swMSF,
            ref StreamWriter swMCF, ref StreamWriter swSFCF, ref StreamWriter swAll,
            ref StreamWriter swCounts, ref StreamWriter swCountsByCount, ref StreamWriter swGlossSFCounts)
        {
            FeaturePairsFeatureAllAndCounts(ref dWords, ref swWFM, ref swWFSF, ref swWFCF, ref swMSF,
                ref swMCF, ref swSFCF, ref swAll, ref swCounts, ref swCountsByCount, ref swGlossSFCounts);

            FeaturePhrasesAndCounts(ref dWords, ref dRangedVerses);
        }

        public void FeaturePairsFeatureAllAndCounts(ref Dictionary<int, ClassifyMABNT.Word> dWords, ref StreamWriter swWFM,
          ref StreamWriter swWFSF, ref StreamWriter swWFCF, ref StreamWriter swMSF,
          ref StreamWriter swMCF, ref StreamWriter swSFCF, ref StreamWriter swAll,
          ref StreamWriter swCounts, ref StreamWriter swCountsByCount, ref StreamWriter swGlossSFCounts)
        {
            string strCurrentSubclause = "";
            string strBuildingGloss = "";

            foreach (int intWordID in dWords.Keys.OrderBy(a => a))
            {
                ClassifyMABNT.Word wCurrent = dWords[intWordID];

                if (wCurrent != null)
                {
                    string strWFM = wCurrent.strWordFunction + " ^ " + wCurrent.strMorphology;
                    string strWFSF = wCurrent.strWordFunction + " ^ " + wCurrent.strSubclauseFunction;
                    string strWFCF = wCurrent.strWordFunction + " ^ " + wCurrent.strClauseFunction;
                    string strMSF = wCurrent.strMorphology + " ^ " + wCurrent.strSubclauseFunction;
                    string strMCF = wCurrent.strMorphology + " ^ " + wCurrent.strClauseFunction;
                    string strSFCF = wCurrent.strSubclauseFunction + " ^ " + wCurrent.strClauseFunction;

                    string strAll = wCurrent.strWordFunction + " ^ " + wCurrent.strMorphology
                        + " ^ " + wCurrent.strSubclauseFunction + " ^ " + wCurrent.strClauseFunction;

                    string strCountStub = wCurrent.strGloss + " , ";

                    if (!dWFM.ContainsKey(strWFM))
                    {
                        dWFM.Add(strWFM, new());
                    }

                    if (!dWFM[strWFM].Contains(wCurrent.strGloss))
                    {
                        dWFM[strWFM].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strMorphology))
                    {
                        dCounts.Add(strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strMorphology, 0);
                    }

                    dCounts[strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strMorphology]++;



                    if (!dWFSF.ContainsKey(strWFSF))
                    {
                        dWFSF.Add(strWFSF, new());
                    }

                    if (!dWFSF[strWFSF].Contains(wCurrent.strGloss))
                    {
                        dWFSF[strWFSF].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strSubclauseFunction))
                    {
                        dCounts.Add(strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strSubclauseFunction, 0);
                    }

                    dCounts[strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strSubclauseFunction]++;



                    if (!dWFCF.ContainsKey(strWFCF))
                    {
                        dWFCF.Add(strWFCF, new());
                    }

                    if (!dWFCF[strWFCF].Contains(wCurrent.strGloss))
                    {
                        dWFCF[strWFCF].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strClauseFunction))
                    {
                        dCounts.Add(strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strClauseFunction, 0);
                    }

                    dCounts[strCountStub + wCurrent.strWordFunction + " , " + wCurrent.strClauseFunction]++;



                    if (!dMSF.ContainsKey(strMSF))
                    {
                        dMSF.Add(strMSF, new());
                    }

                    if (!dMSF[strMSF].Contains(wCurrent.strGloss))
                    {
                        dMSF[strMSF].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strCountStub + wCurrent.strMorphology + " , " + wCurrent.strSubclauseFunction))
                    {
                        dCounts.Add(strCountStub + wCurrent.strMorphology + " , " + wCurrent.strSubclauseFunction, 0);
                    }

                    dCounts[strCountStub + wCurrent.strMorphology + " , " + wCurrent.strSubclauseFunction]++;



                    if (!dMCF.ContainsKey(strMCF))
                    {
                        dMCF.Add(strMCF, new());
                    }

                    if (!dMCF[strMCF].Contains(wCurrent.strGloss))
                    {
                        dMCF[strMCF].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strCountStub + wCurrent.strMorphology + " , " + wCurrent.strClauseFunction))
                    {
                        dCounts.Add(strCountStub + wCurrent.strMorphology + " , " + wCurrent.strClauseFunction, 0);
                    }

                    dCounts[strCountStub + wCurrent.strMorphology + " , " + wCurrent.strClauseFunction]++;



                    if (!dSFCF.ContainsKey(strSFCF))
                    {
                        dSFCF.Add(strSFCF, new());
                    }

                    if (!dSFCF[strSFCF].Contains(wCurrent.strGloss))
                    {
                        dSFCF[strSFCF].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strCountStub + wCurrent.strSubclauseFunction + " , " + wCurrent.strClauseFunction))
                    {
                        dCounts.Add(strCountStub + wCurrent.strSubclauseFunction + " , " + wCurrent.strClauseFunction, 0);
                    }

                    dCounts[strCountStub + wCurrent.strSubclauseFunction + " , " + wCurrent.strClauseFunction]++;



                    if (!dAll.ContainsKey(strAll))
                    {
                        dAll.Add(strAll, new());
                    }

                    if (!dAll[strAll].Contains(wCurrent.strGloss))
                    {
                        dAll[strAll].Add(wCurrent.strGloss);
                    }

                    if (!dCounts.ContainsKey(strAll + " ^ " + wCurrent.strGloss))
                    {
                        dCounts.Add(strAll + " ^ " + wCurrent.strGloss, 0);
                    }

                    dCounts[strAll + " ^ " + wCurrent.strGloss]++;

                    if (wCurrent.strSubclause != strCurrentSubclause &&
                        strCurrentSubclause != "")
                    {
                        strBuildingGloss = strBuildingGloss.Trim();

                        //whole gloss from last subclause is built
                        if (!dGlossSFCounts.ContainsKey(strBuildingGloss))
                        {
                            dGlossSFCounts.Add(strBuildingGloss, new());
                        }

                        if (!dGlossSFCounts[strBuildingGloss].ContainsKey(wCurrent.strSubclauseFunction))
                        {
                            dGlossSFCounts[strBuildingGloss].Add(wCurrent.strSubclauseFunction, 0);
                        }

                        dGlossSFCounts[strBuildingGloss][wCurrent.strSubclauseFunction]++;

                        if (!strBuildingGloss.EndsWith(" "))
                        {
                            strBuildingGloss = " " + wCurrent.strGloss + " ";
                        }
                        else
                        {
                            strBuildingGloss = wCurrent.strGloss + " ";
                        }

                        strCurrentSubclause = wCurrent.strSubclause;
                    }
                    else
                    {
                        //build whole gloss
                        if (!strBuildingGloss.EndsWith(" "))
                        {
                            strBuildingGloss += " " + wCurrent.strGloss + " ";
                        }
                        else
                        {
                            strBuildingGloss = wCurrent.strGloss + " ";
                        }

                        strCurrentSubclause = wCurrent.strSubclause;
                    }
                }
            }

            //capture the last entry to dGlossSFCounts
            if (!dGlossSFCounts.ContainsKey(strBuildingGloss))
            {
                dGlossSFCounts.Add(strBuildingGloss, new());
            }

            if (!dGlossSFCounts[strBuildingGloss].ContainsKey(strCurrentSubclause))
            {
                dGlossSFCounts[strBuildingGloss].Add(strCurrentSubclause, 0);
            }

            dGlossSFCounts[strBuildingGloss][strCurrentSubclause]++;


            swWFM.WriteLine("Word Function ^ Morphology ^ Gloss");
            foreach (string strKey in dWFM.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dWFM[strKey])
                {
                    swWFM.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swWFM.Close();

            swWFSF.WriteLine("Word Function ^ Subclause Function ^ Gloss");
            foreach (string strKey in dWFSF.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dWFSF[strKey])
                {
                    swWFSF.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swWFSF.Close();

            swWFCF.WriteLine("Word Function ^ Clause Function ^ Gloss");
            foreach (string strKey in dWFCF.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dWFCF[strKey])
                {
                    swWFCF.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swWFCF.Close();

            swMSF.WriteLine("Morphology ^ Subclause Function ^ Gloss");
            foreach (string strKey in dMSF.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dMSF[strKey])
                {
                    swMSF.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swMSF.Close();

            swMCF.WriteLine("Morphology ^ Clause Function ^ Gloss");
            foreach (string strKey in dMCF.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dMCF[strKey])
                {
                    swMCF.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swMCF.Close();

            swSFCF.WriteLine("Subclause Function ^ Clause Function ^ Gloss");
            foreach (string strKey in dSFCF.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dSFCF[strKey])
                {
                    swSFCF.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swSFCF.Close();

            swAll.WriteLine("Word Function ^ Morphology ^ Subclause Function ^ Clause Function ^ Gloss");
            foreach (string strKey in dAll.Keys.OrderBy(a => a))
            {
                foreach (string strGloss in dAll[strKey])
                {
                    swAll.WriteLine(strKey.Trim() + " ^ " + strGloss);
                }
            }
            swAll.Close();

            swCounts.WriteLine("Mesh ^ Count");
            foreach (string strKey in dCounts.Keys.OrderBy(a => a))
            {
                swCounts.WriteLine(strKey.Trim() + " ^ " + dCounts[strKey].ToString());
            }
            swCounts.Close();

            swCountsByCount.WriteLine("Mesh ^ Count");
            foreach (string strKey in dCounts.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                swCountsByCount.WriteLine(strKey.Trim() + " ^ " + dCounts[strKey].ToString());
            }
            swCountsByCount.Close();

            swGlossSFCounts.WriteLine("Gloss ^ Subclause Function ^ Count");
            foreach (string strGloss in dGlossSFCounts.Keys.OrderBy(a => a))
            {
                foreach (string strSF in dGlossSFCounts[strGloss].OrderByDescending(a=>a.Value)
                    .Select(a=>a.Key))
                {
                    swGlossSFCounts.WriteLine(strGloss.Trim() + " ^ " + strSF.Trim() + " ^ " +
                        dGlossSFCounts[strGloss][strSF].ToString().Trim());
                }
            }
            swGlossSFCounts.Close();
        }

        //requires running FeaturePairsFeatureAllAndCounts() first
        public void FeaturePhrasesAndCounts(ref Dictionary<int, ClassifyMABNT.Word> dWords,
            ref Dictionary<int, List<PericopeVerse>> dRangedVerses)
        {
            //public Dictionary<string, int> dMorphologyCounts = new(); //D<morphology 1 , morphology 2 , ... ^ count> ; every morphology inside a subclause
            //public Dictionary<string, int> dWordFunctionCounts = new(); //D<word function 1 , word function 2 , ... ^ count> ; every word function inside a subclause
            //public Dictionary<string, int> dSubclauseFunctionCounts = new(); //D<subclause function 1 , subclause function 2 , ... ^ count> ; every subclause inside a clause

            ////requires pericopes
            //public Dictionary<string, int> dClauseFunctionCounts = new(); //D<clause function 1 , clause function 2 , ... ^ count> ; every clause function between pericopes

            //public Dictionary<string, Dictionary<string, int>> dMSFCounts = new(); //D<morphology 1 , morphology 2 , ... ^ D<subclause function, count>> ; every morphology inside a subclause function of a certain type, then that subclause function and the morphology count inside that subclause function
            //public Dictionary<string, Dictionary<string, int>> dWFSFCounts = new(); //D<word function 1 , word function 2 , ... ^ D<subclause function, count>> ; every word function inside a subclause function of a certain type, then that subclause function and the word function count inside that subclause function
            //public Dictionary<string, Dictionary<string, int>> dSFCFCounts = new(); //D<subclause function 1 , subclause function 2 , ... ^ D<clause function, count>> ; every subclause function inside a clause function of a certain type, then that clause function and the subclause function count in that clause function

            //public Dictionary<string, Dictionary<string, int>> dMorphologyPhraseCounts = new(); //D<morphology 1 , morphology 2 , ... ^ D<word phrase, count>>
            //public Dictionary<string, Dictionary<string, int>> dWordFunctionPhraseCounts = new(); //D<word function 1 , word function 2 , ... ^ D<word phrase, count>>
            //public Dictionary<string, Dictionary<string, int>> dSubclauseFunctionPhraseCounts = new(); //D<subclause function 1 , subclause function 2 , ... ^ D<word phrase, count>>

            //parsing controls for below
            string strCurrentSubclause = ""; //Subclause
            string strCurrentClause = ""; //Clause

            //Dictionary keys
            string strCurrentM = ""; //Morphology 1 , Morphology 2 , ...
            string strCurrentWF = ""; //Word Function 1 , Word Function 2 , ...
            string strCurrentSF = ""; //Subclause Function 1 , Subclause Function 2 , ...
            string strCurrentCF = ""; //Clause Function 1 , Clause Function 2 , ...

            //Value Dictionary keys
            string strCurrentSubclauseGlosses = ""; //Subclause Glosses

            ClassifyMABNT.Word wCurrent = new();

            //Clause Functions
            foreach (int intPericopeRangeIndex in dRangedVerses.Keys.OrderBy(a => a))
            {
                //save last inter-pericope data
                if (strCurrentCF != "")
                {
                    //dClauseFunctionCounts
                    if (!dClauseFunctionCounts.ContainsKey(strCurrentCF))
                    {
                        dClauseFunctionCounts.Add(strCurrentCF, 0);
                    }

                    dClauseFunctionCounts[strCurrentCF]++;
                }

                //start building next inter-pericope data
                strCurrentCF = wCurrent.strClauseFunction;

                foreach (PericopeVerse pvCurrent in dRangedVerses[intPericopeRangeIndex])
                {
                    //inside the same pericope as last time
                    strCurrentCF += " , " + wCurrent.strClauseFunction;
                }
            }

            //Word Functions, Morphologies and Subclause Functions
            foreach (int intWordID in dWords.Keys.OrderBy(a => a))
            {
                wCurrent = dWords[intWordID];

                if (wCurrent != null)
                {
                    if (wCurrent.strClause != strCurrentClause)
                    {
                        //
                        //save last inter-clause data
                        //
                        if (strCurrentSF != "")
                        {
                            //dSubclauseFunctionCounts
                            if (!dSubclauseFunctionCounts.ContainsKey(strCurrentSF))
                            {
                                dSubclauseFunctionCounts.Add(strCurrentSF, 0);
                            }

                            dSubclauseFunctionCounts[strCurrentSF]++;

                            //dSFCFCounts
                            if (!dSFCFCounts.ContainsKey(strCurrentSF))
                            {
                                dSFCFCounts.Add(strCurrentSF, new Dictionary<string, int>());
                            }

                            if (!dSFCFCounts[strCurrentSF].ContainsKey(wCurrent.strClauseFunction))
                            {
                                dSFCFCounts[strCurrentSF].Add(wCurrent.strClauseFunction, 0);
                            }

                            dSFCFCounts[strCurrentSF][wCurrent.strClauseFunction]++;

                            //dSubclauseFunctionPhraseCounts
                            if (!dSubclauseFunctionPhraseCounts.ContainsKey(strCurrentSF))
                            {
                                dSubclauseFunctionPhraseCounts.Add(strCurrentSF, new Dictionary<string, int>());
                            }

                            if (!dSubclauseFunctionPhraseCounts[strCurrentSF].ContainsKey(strCurrentSubclauseGlosses))
                            {
                                dSubclauseFunctionPhraseCounts[strCurrentSF].Add(strCurrentSubclauseGlosses, 0);
                            }

                            dSubclauseFunctionPhraseCounts[strCurrentSF][strCurrentSubclauseGlosses]++;
                        }

                        //start building next inter-clause data
                        strCurrentSubclauseGlosses = wCurrent.strGloss;
                        strCurrentSF = wCurrent.strSubclauseFunction;
                    }
                    else
                    {
                        //inside the same clause as last time
                        strCurrentSubclauseGlosses += " , " + wCurrent.strGloss;
                        strCurrentSF += " , " + wCurrent.strSubclauseFunction;
                    }

                    if (wCurrent.strSubclause != strCurrentSubclause)
                    {
                        //
                        //save last inter-subclause data
                        //

                        if (strCurrentM != "")
                        {
                            //dMorphologyCounts
                            if (!dMorphologyCounts.ContainsKey(strCurrentM))
                            {
                                dMorphologyCounts.Add(strCurrentM, 0);
                            }

                            dMorphologyCounts[strCurrentM]++;

                            //dMSFCounts
                            if (!dMSFCounts.ContainsKey(strCurrentM))
                            {
                                dMSFCounts.Add(strCurrentM, new Dictionary<string, int>());
                            }

                            if (!dMSFCounts[strCurrentM].ContainsKey(wCurrent.strSubclauseFunction))
                            {
                                dMSFCounts[strCurrentM].Add(wCurrent.strSubclauseFunction, 0);
                            }

                            dMSFCounts[strCurrentM][wCurrent.strSubclauseFunction]++;

                            //dMorphologyPhraseCounts
                            if (!dMorphologyPhraseCounts.ContainsKey(strCurrentM))
                            {
                                dMorphologyPhraseCounts.Add(strCurrentM, new Dictionary<string, int>());
                            }

                            if (!dMorphologyPhraseCounts[strCurrentM].ContainsKey(strCurrentSubclauseGlosses))
                            {
                                dMorphologyPhraseCounts[strCurrentM].Add(strCurrentSubclauseGlosses, 0);
                            }

                            dMorphologyPhraseCounts[strCurrentM][strCurrentSubclauseGlosses]++;

                            //

                            //dWordFunctionCounts
                            if (!dWordFunctionCounts.ContainsKey(strCurrentWF))
                            {
                                dWordFunctionCounts.Add(strCurrentWF, 0);
                            }

                            dWordFunctionCounts[strCurrentWF]++;

                            //dWFSFCounts
                            if (!dWFSFCounts.ContainsKey(strCurrentWF))
                            {
                                dWFSFCounts.Add(strCurrentWF, new Dictionary<string, int>());
                            }

                            if (!dWFSFCounts[strCurrentWF].ContainsKey(wCurrent.strSubclauseFunction))
                            {
                                dWFSFCounts[strCurrentWF].Add(wCurrent.strSubclauseFunction, 0);
                            }

                            dWFSFCounts[strCurrentWF][wCurrent.strSubclauseFunction]++;

                            //dWordFunctionPhraseCounts
                            if (!dWordFunctionPhraseCounts.ContainsKey(strCurrentWF))
                            {
                                dWordFunctionPhraseCounts.Add(strCurrentWF, new Dictionary<string, int>());
                            }

                            if (!dWordFunctionPhraseCounts[strCurrentWF].ContainsKey(strCurrentSubclauseGlosses))
                            {
                                dWordFunctionPhraseCounts[strCurrentWF].Add(strCurrentSubclauseGlosses, 0);
                            }

                            dWordFunctionPhraseCounts[strCurrentWF][strCurrentSubclauseGlosses]++;
                        }

                        //start new inter-subclause data

                        strCurrentSubclauseGlosses = wCurrent.strGloss;
                        strCurrentSubclause = wCurrent.strSubclause;
                        strCurrentM = wCurrent.strMorphology;
                        strCurrentWF = wCurrent.strWordFunction;
                    }
                    else //inside the same subclause as last time
                    {
                        strCurrentSubclauseGlosses += " , " + wCurrent.strGloss;
                        strCurrentSubclause += " , " + wCurrent.strSubclause;
                        strCurrentM += " , " + wCurrent.strMorphology;
                        strCurrentWF += " , " + wCurrent.strWordFunction;
                    }
                }
            }

            //save everything one last time
            
            //dClauseFunctionCounts
            if (!dClauseFunctionCounts.ContainsKey(strCurrentCF))
            {
                dClauseFunctionCounts.Add(strCurrentCF, 0);
            }

            dClauseFunctionCounts[strCurrentCF]++;

            //dSubclauseFunctionCounts
            if (!dSubclauseFunctionCounts.ContainsKey(strCurrentSF))
            {
                dSubclauseFunctionCounts.Add(strCurrentSF, 0);
            }

            dSubclauseFunctionCounts[strCurrentSF]++;

            //dSFCFCounts
            if (!dSFCFCounts.ContainsKey(strCurrentSF))
            {
                dSFCFCounts.Add(strCurrentSF, new Dictionary<string, int>());
            }

            if (!dSFCFCounts[strCurrentSF].ContainsKey(wCurrent.strClauseFunction))
            {
                dSFCFCounts[strCurrentSF].Add(wCurrent.strClauseFunction, 0);
            }

            dSFCFCounts[strCurrentSF][wCurrent.strClauseFunction]++;

            //dSubclauseFunctionPhraseCounts
            if (!dSubclauseFunctionPhraseCounts.ContainsKey(strCurrentSF))
            {
                dSubclauseFunctionPhraseCounts.Add(strCurrentSF, new Dictionary<string, int>());
            }

            if (!dSubclauseFunctionPhraseCounts[strCurrentSF].ContainsKey(strCurrentSubclauseGlosses))
            {
                dSubclauseFunctionPhraseCounts[strCurrentSF].Add(strCurrentSubclauseGlosses, 0);
            }

            dSubclauseFunctionPhraseCounts[strCurrentSF][strCurrentSubclauseGlosses]++;

            //dMorphologyCounts
            if (!dMorphologyCounts.ContainsKey(strCurrentM))
            {
                dMorphologyCounts.Add(strCurrentM, 0);
            }

            dMorphologyCounts[strCurrentM]++;

            //dMSFCounts
            if (!dMSFCounts.ContainsKey(strCurrentM))
            {
                dMSFCounts.Add(strCurrentM, new Dictionary<string, int>());
            }

            if (!dMSFCounts[strCurrentM].ContainsKey(wCurrent.strSubclauseFunction))
            {
                dMSFCounts[strCurrentM].Add(wCurrent.strSubclauseFunction, 0);
            }

            dMSFCounts[strCurrentM][wCurrent.strSubclauseFunction]++;

            //dMorphologyPhraseCounts
            if (!dMorphologyPhraseCounts.ContainsKey(strCurrentM))
            {
                dMorphologyPhraseCounts.Add(strCurrentM, new Dictionary<string, int>());
            }

            if (!dMorphologyPhraseCounts[strCurrentM].ContainsKey(strCurrentSubclauseGlosses))
            {
                dMorphologyPhraseCounts[strCurrentM].Add(strCurrentSubclauseGlosses, 0);
            }

            dMorphologyPhraseCounts[strCurrentM][strCurrentSubclauseGlosses]++;

            //dWordFunctionCounts
            if (!dWordFunctionCounts.ContainsKey(strCurrentWF))
            {
                dWordFunctionCounts.Add(strCurrentWF, 0);
            }

            dWordFunctionCounts[strCurrentWF]++;

            //dWFSFCounts
            if (!dWFSFCounts.ContainsKey(strCurrentWF))
            {
                dWFSFCounts.Add(strCurrentWF, new Dictionary<string, int>());
            }

            if (!dWFSFCounts[strCurrentWF].ContainsKey(wCurrent.strSubclauseFunction))
            {
                dWFSFCounts[strCurrentWF].Add(wCurrent.strSubclauseFunction, 0);
            }

            dWFSFCounts[strCurrentWF][wCurrent.strSubclauseFunction]++;

            //dWordFunctionPhraseCounts
            if (!dWordFunctionPhraseCounts.ContainsKey(strCurrentWF))
            {
                dWordFunctionPhraseCounts.Add(strCurrentWF, new Dictionary<string, int>());
            }

            if (!dWordFunctionPhraseCounts[strCurrentWF].ContainsKey(strCurrentSubclauseGlosses))
            {
                dWordFunctionPhraseCounts[strCurrentWF].Add(strCurrentSubclauseGlosses, 0);
            }

            dWordFunctionPhraseCounts[strCurrentWF][strCurrentSubclauseGlosses]++;
        }
    }
}

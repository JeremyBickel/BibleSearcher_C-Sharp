using BLBLexicon;

namespace KJVStrongs
{
    public class SVO
    {
        public Dictionary<string, int> dSSs = new(); //D<strongs sequence, count>
        public Dictionary<string, int> dSSCountsOrderedBySS = new(); //D<strongs sequence, count> (ordered by ss)
        public Dictionary<string, int> dSSCountsOrderedByCount = new(); //D<strongs sequence, count> (ordered by count)
        public Dictionary<string, int> dSSComplexCountsOrderedByCount = new(); //D<multiple strongs sequences for a single phrase separated by a dash, count>

        public Dictionary<int, SVOData> dSVOs = new();
        public Dictionary<int, SVOData> dSimpleSVOs = new(); //Contains one subject, one verb and one or more objects

        internal void CalculateSVOs(ref KJVData kjvdata, ref BLBLexiconData blbData,
            ref StreamWriter swSSComplex, ref StreamWriter swSSComplexTranslation,
            ref StreamWriter swSimpleSVOs)
        {
            //Find subject
            //Find verbs
            //Find objects
            //Find relationships between these three
            //
            //Find relationships between subjects in similar contexts??

            //Doing this in Hebrew is too hard for me, so I'm opting to do it for the KJV text,
            // using the Strongs numbers as a phrase separator.

            CreateSS(ref kjvdata);
            CreateSSComplex(ref swSSComplex, ref swSSComplexTranslation, ref blbData);

            FillSVODictionary(ref kjvdata, ref blbData, ref dSVOs);
            CreateSimpleSVOs(ref swSimpleSVOs, ref dSVOs);
        }


        //fills dSSs, dSSCountsOrderedBySS, and dSSCountsOrderedByCount
        public void CreateSS(ref KJVData kjvdata)
        {
            string strStrongsLanguageIdentifier = "";

            foreach (Verse v in kjvdata.dVerses.OrderBy(a => a.Key).Select(a => a.Value))
            {
                strStrongsLanguageIdentifier = v.intVerseID <= 23145 ? "H" : "G";

                foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                {
                    string strSS = "";

                    foreach (StrongsSequence ss in p.dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Value))
                    {
                        strSS += strStrongsLanguageIdentifier + ss.strStrongsNumber + "-";
                    }

                    strSS = strSS.TrimEnd('-');

                    if (dSSs.ContainsKey(strSS))
                    {
                        dSSs[strSS]++;
                    }
                    else
                    {
                        dSSs.Add(strSS, 1);
                    }

                    ////Prepositional Phrases
                    //if (p.strPhraseText.Trim().Split().Count() == 1)
                    //{
                    //}
                    //else
                    //{
                    //    // if (PrepositionList.Contains(p.strPhraseText.Trim().Split()[0])){} //Prepositional Phrase

                    //    foreach (string strWord in p.strPhraseText.Trim().Split())
                    //    {

                    //    }
                    //}
                }
            }

            foreach (string strSS in dSSs.OrderBy(a => a.Key).Select(a => a.Key))
            {
                dSSCountsOrderedBySS.Add(strSS, dSSs[strSS]);
            }

            foreach (string strSS in dSSs.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                dSSCountsOrderedByCount.Add(strSS, dSSs[strSS]);
            }
        }

        //fills dSSComplexCountsOrderedByCount
        public void CreateSSComplex(ref StreamWriter swSSComplex,
            ref StreamWriter swSSComplexTranslation, ref BLBLexiconData blbData)
        {
            int intSSComplexLineID = 0;

            //Output files headers
            swSSComplex.WriteLine("PhraseID ^ ComplexPhraseStrongsSequence ^ PhraseCount");
            swSSComplexTranslation.WriteLine("PhraseID ^ ComplexPhraseEnglishText ^ PhraseCount");

            //Complex Phrases have more than one Strong's number, separated by a dash
            foreach (string strSS in dSSs.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                if (strSS.Contains('-'))
                {
                    string strTranslation = "";

                    intSSComplexLineID++;

                    dSSComplexCountsOrderedByCount.Add(strSS, dSSs[strSS]);
                    swSSComplex.WriteLine(intSSComplexLineID.ToString() + " ^ " + strSS + " ^ " + dSSs[strSS]);

                    foreach (string strSS1 in strSS.Split('-'))
                    {
                        if (strSS.StartsWith('H'))
                        {
                            strTranslation += blbData.dBLBHebrewLexiconEntries[Convert.ToInt32(strSS1[1..])].dAVTranslations.OrderByDescending(a => a.Value).Select(a => a.Key).First() + "-";
                        }
                        else if (strSS.StartsWith('G'))
                        {
                            strTranslation += blbData.dBLBHebrewLexiconEntries[Convert.ToInt32(strSS1[1..])].dAVTranslations.OrderByDescending(a => a.Value).Select(a => a.Key).First() + "-";
                        }
                        else
                        {
                            throw new Exception("Counts for Phrases with Multiple Strongs Numbers, First English Translation, SS Language Error");
                        }
                    }

                    strTranslation = strTranslation.TrimEnd('-');
                    swSSComplexTranslation.WriteLine(intSSComplexLineID.ToString() + " ^ " + strTranslation + " ^ " + dSSs[strSS]);
                }
            }

            swSSComplex.Close();
            swSSComplexTranslation.Close();
        }

        //fills dSVOs
        public void FillSVODictionary(ref KJVData kjvdata, ref BLBLexiconData blbData,
            ref Dictionary<int, SVOData> dSVOs)
        {
            string strStrongsLanguageIdentifier = "";
            string strPOSSequence = "";
            ComplexStrongsSequences css = new();

            //Convert Strongs Sequences into POS Sequences
            foreach (Verse v in kjvdata.dVerses.OrderBy(a => a.Key).Select(a => a.Value))
            {
                bool bVerbSeen = false; //this simple scheme assigns nouns before the verb to subject and after the verb to object

                dSVOs.Add(v.intVerseID, new SVOData());

                strStrongsLanguageIdentifier = v.intBookNumber <= 39 ? "H" : "G";

                foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                {
                    string strSS = "";

                    foreach (StrongsSequence ss in p.dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Value))
                    {
                        strSS += strStrongsLanguageIdentifier + ss.strStrongsNumber + "-";
                    }

                    strSS = strSS.TrimEnd('-');

                    foreach (string strSequencePart in strSS.Split('-'))
                    {
                        strPOSSequence = css.StrongsToPOS(ref kjvdata, ref blbData, strSequencePart);

                        switch (strPOSSequence.Split()[0])
                        {
                            case "n":
                                if (bVerbSeen == false)
                                {
                                    if (!dSVOs[v.intVerseID].dSubjects.ContainsKey(p.intPhraseID))
                                    {
                                        dSVOs[v.intVerseID].dSubjects.Add(p.intPhraseID, "");
                                    }

                                    dSVOs[v.intVerseID].dSubjects[p.intPhraseID] += p.strPhraseText + "-";
                                }
                                else
                                {
                                    if (!dSVOs[v.intVerseID].dObjectRelations.ContainsKey(p.intPhraseID))
                                    {
                                        dSVOs[v.intVerseID].dObjectRelations.Add(p.intPhraseID, new ObjectRelation());
                                    }

                                    dSVOs[v.intVerseID].dObjectRelations[p.intPhraseID].strObject += p.strPhraseText + "-";
                                }

                                break;

                            case "v":
                                bVerbSeen = true;

                                if (!dSVOs[v.intVerseID].dVerbs.ContainsKey(p.intPhraseID))
                                {
                                    dSVOs[v.intVerseID].dVerbs.Add(p.intPhraseID, "");
                                }

                                dSVOs[v.intVerseID].dVerbs[p.intPhraseID] += p.strPhraseText + "-";

                                break;
                        }
                    }
                }

                //Clean the dash from the ends of all the phrases
                foreach (int intPhraseID in v.dPhrases.Keys)
                {
                    if (dSVOs[v.intVerseID].dVerbs.ContainsKey(intPhraseID))
                    {
                        dSVOs[v.intVerseID].dVerbs[intPhraseID] = dSVOs[v.intVerseID].dVerbs[intPhraseID].TrimEnd('-');
                    }

                    if (dSVOs[v.intVerseID].dSubjects.ContainsKey(intPhraseID))
                    {
                        dSVOs[v.intVerseID].dSubjects[intPhraseID] = dSVOs[v.intVerseID].dSubjects[intPhraseID].TrimEnd('-');
                    }

                    if (dSVOs[v.intVerseID].dObjectRelations.ContainsKey(intPhraseID))
                    {
                        dSVOs[v.intVerseID].dObjectRelations[intPhraseID].strObject = dSVOs[v.intVerseID].dObjectRelations[intPhraseID].strObject.TrimEnd('-');
                    }
                }
            }
        }

        public void CreateSimpleSVOs(ref StreamWriter swSimpleSVOs, ref Dictionary<int, SVOData> dSVOs)
        {
            //Isolate verses with a single verb and a single subject to start building a knowledge base
            swSimpleSVOs.WriteLine("VerseID ^ Subject ^ Verb ^ AmpersandSeparatedObjects");

            foreach (int intVerseID in dSVOs.Where(a => a.Value.dVerbs.Count == 1 && a.Value.dSubjects.Count == 1).Select(a => a.Key))
            {
                string strObjects = "";

                swSimpleSVOs.Write(intVerseID.ToString() + " ^ " + dSVOs[intVerseID].dSubjects.First().Value + " ^ " +
                    dSVOs[intVerseID].dVerbs.First().Value + " ^ ");

                foreach (string strObject in dSVOs[intVerseID].dObjectRelations.OrderBy(a => a.Key).Select(a => a.Value.strObject))
                {
                    strObjects += strObject + " & ";
                }

                strObjects = strObjects.TrimEnd('&');
                swSimpleSVOs.Write(strObjects);
                swSimpleSVOs.WriteLine();
            }

            swSimpleSVOs.Close();
        }

        public void Read(ref KJVData kjvdata, ref StreamReader srSSComplex,
            ref StreamReader srSSComplexTranslation, ref StreamReader srSimpleSVOs)
        {

            //swSSComplexTranslation.WriteLine("PhraseID ^ ComplexPhraseEnglishText ^ PhraseCount");
            //swSimpleSVOs.WriteLine("VerseID ^ Subject ^ Verb ^ AmpersandSeparatedObjects");
            //foreach (string strSS in dSSs.OrderBy(a => a.Key).Select(a => a.Key))
            //{
            //    dSSCountsOrderedBySS.Add(strSS, dSSs[strSS]);
            //}

            //foreach (string strSS in dSSs.OrderByDescending(a => a.Value).Select(a => a.Key))
            //{
            //    dSSCountsOrderedByCount.Add(strSS, dSSs[strSS]);
            //}
            //      dSSComplexCountsOrderedByCount.Add(strSS, dSSs[strSS]);
            //
            bool bSeenHeader = false;

            while (!srSSComplex.EndOfStream)
            {
                string strLine = srSSComplex.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!dSSComplexCountsOrderedByCount.ContainsKey(strsLine[1].Trim()))
                    {
                        dSSComplexCountsOrderedByCount.Add(strsLine[1].Trim(),
                            Convert.ToInt16(strsLine[2].Trim()));
                    }
                }
            }

            srSSComplex.Close();

            CreateSS(ref kjvdata);

            bSeenHeader = false;

            while (!srSSComplexTranslation.EndOfStream)
            {
                string strLine = srSSComplexTranslation.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    dSSComplexCountsOrderedByCount[strsLine[1].Trim()] = Convert.ToInt16(strsLine[2].Trim());
                }
            }

            srSSComplexTranslation.Close();

            bSeenHeader = false;

            while (!srSimpleSVOs.EndOfStream)
            {
                string strLine = srSimpleSVOs.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');
                    int intObjectCounter = 0;

                    if (!dSimpleSVOs.ContainsKey(Convert.ToInt16(strsLine[0])))
                    {
                        dSimpleSVOs.Add(Convert.ToInt16(strsLine[0]), new SVOData());
                    }

                    dSimpleSVOs[Convert.ToInt16(strsLine[0])].dSubjects.Add(1, strsLine[1]);
                    dSimpleSVOs[Convert.ToInt16(strsLine[0])].dVerbs.Add(1, strsLine[2]);

                    foreach (string strObject in strsLine[3].Split('&'))
                    {
                        intObjectCounter++;

                        ObjectRelation orTemp = new()
                        {
                            strObject = strObject
                        };

                        dSimpleSVOs[Convert.ToInt16(strsLine[0])]
                            .dObjectRelations.Add(intObjectCounter, orTemp);
                    }
                }
            }

            srSimpleSVOs.Close();
        }
    }

    public class ObjectRelation
    {
        public string strObject = "";
        public string strRelation = ""; //eg. the preposition
    }

    public class SVOData
    {
        public Dictionary<int, string> dSubjects = new(); //D<Phrase ID, Phrase Text>
        public Dictionary<int, string> dVerbs = new(); //D<Phrase ID, Phrase Text>
        public Dictionary<int, ObjectRelation> dObjectRelations = new(); //D<Phrase ID, ObjectRelation object>
    }
}

using BLBLexicon;
using System.Text.RegularExpressions;

namespace KJVStrongs
{
    public class Welcome
    {
        public KJVData kjvdata = new();
        public IntersectingPhrases iphrases = new();
        public ComplexStrongsSequences csseq = new();

        public Welcome(ref BLBLexiconData blbData, ref StreamReader srBible, ref StreamWriter swRawText, ref StreamWriter swVerses,
            ref StreamWriter swLastVerseIDInBook, ref StreamWriter swKJVStrongsVector, ref StreamWriter swKJVStrongsVectorStrongsOnly,
            ref StreamWriter swStrongsMajorWordEnglish, ref StreamWriter swWordIDs, ref StreamWriter swPositionIDs,
            ref StreamWriter swWordCounts, ref StreamWriter swBookWordCounts, ref StreamWriter swPhrasalConcordance, ref StreamWriter swPhrases,
            ref StreamWriter swEnglishPhraseCountsByCount, ref StreamWriter swEnglishPhraseCountsByPhrase,
            ref StreamWriter swEnglishPhraseWithReferences, ref StreamWriter swStrongsPhrasalConcordanceEnglish,
            ref StreamWriter swStrongsPhrasalConcordanceStrongs, ref StreamWriter swSSComplex,
            ref StreamWriter swSSComplexTranslation, ref StreamWriter swSimpleSVOs, ref StreamWriter swLocalPhraseClusters,
            ref StreamWriter swCrossrefs, ref StreamWriter swSyllableScoreHebrew, ref StreamReader srSyllableScoreHebrew,
            ref StreamWriter swSyllableScoreGreek, ref StreamReader srSyllableScoreGreek, ref StreamReader srCRs,
            ref StreamWriter swReferences, bool bWriteEnglishPhraseWithReferences)
        {
            //this Create() also writes
            Create(ref blbData, ref srBible, ref swRawText, ref swVerses, ref swLastVerseIDInBook, ref swKJVStrongsVector,
                ref swKJVStrongsVectorStrongsOnly, ref swStrongsMajorWordEnglish, ref swWordIDs,
                ref swPositionIDs, ref swWordCounts, ref swBookWordCounts, ref swPhrasalConcordance, ref swPhrases, ref swEnglishPhraseCountsByCount,
                ref swEnglishPhraseCountsByPhrase, ref swEnglishPhraseWithReferences, ref swStrongsPhrasalConcordanceEnglish,
                ref swStrongsPhrasalConcordanceStrongs, ref swSSComplex, ref swSSComplexTranslation,
                ref swSimpleSVOs, ref swLocalPhraseClusters, ref swCrossrefs, ref swSyllableScoreHebrew, ref srSyllableScoreHebrew,
                ref swSyllableScoreGreek, ref srSyllableScoreGreek, ref srCRs, ref swReferences, bWriteEnglishPhraseWithReferences);
        }

        public Welcome(ref StreamReader srLastVerseIDInBook, ref StreamReader srVerses,
            ref StreamReader srReferences, ref StreamReader srKJVStrongsVector,
            ref StreamReader srWordIDs, ref StreamReader srWordPositions, ref StreamReader srWordCounts,
            ref StreamReader srBookWordCounts, ref StreamReader srPhrasalConcordance,
            ref StreamReader srStrongsPhrasalConcordanceEnglish, ref StreamReader srStrongsPhrasalConcordanceStrongs,
            ref StreamReader srSSComplex, ref StreamReader srSSComplexTranslation, ref StreamReader srSimpleSVOs,
            ref StreamReader srLocalPhraseClusters, ref StreamReader srCrossrefs, ref StreamReader srSyllableScoreHebrew,
            ref StreamReader srSyllableScoreGreek, ref StreamReader srRawText)
        {
            Read(ref srLastVerseIDInBook, ref srVerses, ref srReferences, ref srKJVStrongsVector,
                ref srWordIDs, ref srWordPositions, ref srWordCounts,
                ref srBookWordCounts, ref srPhrasalConcordance,
                ref srStrongsPhrasalConcordanceEnglish, ref srStrongsPhrasalConcordanceStrongs,
                ref srSSComplex, ref srSSComplexTranslation, ref srSimpleSVOs, ref srLocalPhraseClusters,
                ref srCrossrefs, ref srSyllableScoreHebrew, ref srSyllableScoreGreek, ref srRawText);
        }

        //creates kjvdata.dLastVerseInBook and kjvdata.dVerses,
        //but without Verse.dPhrases or Verse.dPhraseCounts
        public void FillKJVStrongs(ref StreamReader srBible, ref StreamWriter swRawText)
        {
            string strLastBookName = "";
            int intBookNumber = 0;
            int intVerseID = 0;

            Regex rgxRemoveStrongs = new(@" {0,}\{ {0,}\d{1,} {0,} \} {0,}"); //removes { strongs number } from verse text
            Regex rgxPunctuation = new(@"[^\w\s]");

            StreamWriter swLastVerseNumberInEachChapter = new(@"Z:\BibleSearcher\Data\Data\Processed\LastVerseNumberInEachChapter.csv");
            int intLastBookNumber = 0;
            int intLastChapterNumber = 0;
            int intLastVerseNumber = 0;

            _ = srBible.ReadLine(); //Go past the header

            swLastVerseNumberInEachChapter.WriteLine("BookNumber^ChapterNumber^VerseNumber");

            while (!srBible.EndOfStream)
            {
                string strLine = srBible.ReadLine();
                Verse v = new(); //verse without strongs
                Verse vs = new(); //verse with strongs
                int intComma = strLine.IndexOf(",");

                intVerseID++;

                v.intVerseID = intVerseID;
                vs.intVerseID = v.intVerseID;

                v.strBookName = strLine[..intComma];
                vs.strBookName = v.strBookName;

                if (v.strBookName != strLastBookName)
                {
                    if (intBookNumber > 0)
                    {
                        kjvdata.dLastVerseIDInBook.Add(strLastBookName, intVerseID - 1);
                    }

                    strLastBookName = v.strBookName;
                    intBookNumber++;
                }

                v.intBookNumber = intBookNumber;
                vs.intBookNumber = v.intBookNumber;

                strLine = strLine.Remove(0, intComma + 1);
                intComma = strLine.IndexOf(",");
                v.intChapterNumber = Convert.ToInt32(strLine[..intComma]);
                vs.intChapterNumber = v.intChapterNumber;

                strLine = strLine.Remove(0, intComma + 1);
                intComma = strLine.IndexOf(",");
                v.intVerseNumber = Convert.ToInt32(strLine[..intComma]);
                vs.intVerseNumber = v.intVerseNumber;

                strLine = strLine.Remove(0, intComma + 1);
                strLine = strLine.Trim();

                //With Strongs

                //Some phrases contain multiple spaces between words. Cut them out.
                foreach (string strWord in strLine.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                {
                    vs.strText += strWord + " ";
                }

                vs.strText = vs.strText.Trim();

                kjvdata.dVerses.Add(vs.intVerseID, vs);

                //Without Strongs or punctuation
                strLine = rgxPunctuation.Replace(rgxRemoveStrongs.Replace(strLine, " "), "");

                //Some phrases contain multiple spaces between words. Cut them out.
                foreach (string strWord in strLine.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                {
                    v.strText += strWord + " ";
                }

                v.strText = v.strText.Trim();

                kjvdata.dVersesWithoutStrongs.Add(v.intVerseID, v);

                swRawText.WriteLine(v.strText.ToLower(), "");

                //LastVerseNumberInEachChapter
                if (v.intChapterNumber != intLastChapterNumber)
                {
                    swLastVerseNumberInEachChapter.Write(intLastBookNumber + "^");
                    swLastVerseNumberInEachChapter.Write(intLastChapterNumber + "^");
                    swLastVerseNumberInEachChapter.WriteLine(intLastVerseNumber);

                    intLastChapterNumber = v.intChapterNumber;
                }

                if (v.intBookNumber != intLastBookNumber)
                {
                    intLastBookNumber = v.intBookNumber;
                }

                if (v.intVerseNumber != intLastVerseNumber)
                {
                    intLastVerseNumber = v.intVerseNumber;
                }
            }

            kjvdata.dLastVerseIDInBook.Add("revelation", intVerseID);
            swLastVerseNumberInEachChapter.WriteLine(intLastBookNumber + "^" + intLastChapterNumber + "^" + intLastVerseNumber);

            swRawText.Close();
            swLastVerseNumberInEachChapter.Close();
        }

        //creates kjvdata.dVerses.dPhrases and kjvdata.dVerses.dPhraseCounts
        public void ParseKJVStrongs(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            //Regex rgxText = new(@"(?<phrase>[a-z'A-Z ]{0,})(?<strongs>(\{\({0,} [0-9]{1,} \){0,}\}){1,})");
            Regex rgxText = new(@"(?<phrase>[^{]{1,})((?<strongs>\{\({0,} [0-9]{1,} \){0,}\} {0,}){1,})");
            Regex rgxLoweredRemovePunctuation = new(@"[^a-z ]{1,}");

            foreach (int intVerseID in kjvdata.dVerses.Keys)
            {
                int intPhraseID = 0;

                string strLanguage = intVerseID <= 23145 ? "Hebrew" : "Greek";
                foreach (Match mPhrase in rgxText.Matches(kjvdata.dVerses[intVerseID].strText))
                {
                    int intCaptureID = 0;
                    string strPhrase = rgxLoweredRemovePunctuation.Replace(mPhrase.Groups["phrase"].Value.Trim().ToLower(), "").Trim();

                    intPhraseID++;

                    kjvdata.dVerses[intVerseID].dPhrases.Add(intPhraseID, new Phrase());
                    kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].intPhraseID = intPhraseID;
                    kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText = strPhrase;

                    if (!kjvdata.dVerses[intVerseID].dPhraseCounts.ContainsKey(strPhrase.ToLower()))
                    {
                        kjvdata.dVerses[intVerseID].dPhraseCounts.Add(strPhrase, 0);
                    }

                    kjvdata.dVerses[intVerseID].dPhraseCounts[strPhrase]++;

                    foreach (Capture capStrongs in mPhrase.Groups["strongs"].Captures)
                    {
                        Regex rgxLowerLetters = new(@"[^a-z\/ ]");
                        bool bParenthecized = capStrongs.Value.Contains("(");
                        string strStrongsNumber = capStrongs.Value.Trim().TrimStart('{').TrimEnd('}').TrimStart('(').TrimEnd(')').Trim();
                        string strPOS = "";
                        int intStrongsNumber = Convert.ToInt32(strStrongsNumber);

                        intCaptureID++;

                        kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences.Add(intCaptureID, new StrongsSequence());
                        kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].intStrongsSequenceID = intCaptureID;
                        kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].strStrongsNumber = strStrongsNumber;
                        kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bParenthecized = bParenthecized;

                        //Combine POS into main categories
                        strPOS = rgxLowerLetters.Replace(dBLBHebrewLexiconEntries[Convert.ToInt32(strStrongsNumber)].strPOS.ToLower(), "").Trim();

                        foreach (string strPOSPart in strPOS.Split())
                        {
                            if (strLanguage == "Hebrew")
                            {
                                if (intStrongsNumber is 853 or 3487) //H853, H3487 means "Direct Object"
                                {
                                    kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bObject = true;
                                }
                            }

                            if (strPOSPart is "n" or "adj" or "adv" or "pr" or
                                "pron" or "adjective" or "adverb" or "adj/subst")
                            {
                                kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bThing = true;
                            }
                            else if (strPOSPart is "v" or "v/n" or "verb" or "verbal")
                            {
                                kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bAction = true;
                            }

                            else if (strPOSPart is "prep" or "prep/conj" or "preposition")
                            {
                                kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bPreposition = true;
                            }
                        }
                    }
                }
            }
        }

        //creates intsStrongsPhrases = A[verse id, phrase id, strongs number]
        //creates 
        public void VectorizeKJVStrongs(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries,
            ref StreamWriter swKJVStrongsVector, ref StreamWriter swKJVStrongsVectorStrongsOnly,
            ref StreamWriter swStrongsMajorWordEnglish)
        {
            int intMaximumRowLength = 0; // dVerses.Max(a => a.Value.dPhrases.Count);
            int intMaximumHebrewVerseID = 23145;

            string strStrongsOnly = "";

            swKJVStrongsVector.WriteLine("VerseID ^ PhraseID ^ MultipleStrongsCount ^ StrongsNumber");

            foreach (Verse v in kjvdata.dVerses.OrderBy(a => a.Key).Select(a => a.Value))
            {
                int intRowLength = 0;

                //Find the maximum Hebrew Strongs number AND maximum row length
                //it's 8674, but make sure here, in case the data file changes to include extended Strong's numbers
                foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                {
                    foreach (StrongsSequence ss in p.dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Value))
                    {
                        foreach (string strStrongsNumber in ss.strStrongsNumber.ToUpper().Split('-'))
                        {
                            intRowLength++;

                            if (v.intVerseID <= intMaximumHebrewVerseID)
                            {
                                int intStrongsNumber = Convert.ToInt32(strStrongsNumber);

                                if (intStrongsNumber > kjvdata.intMaximumHebrewStrongsNumber)
                                {
                                    kjvdata.intMaximumHebrewStrongsNumber = intStrongsNumber;
                                }
                            }
                        }
                    }
                }

                if (intRowLength > intMaximumRowLength)
                {
                    intMaximumRowLength = intRowLength;
                }
            }

            //initialize the data structure
            kjvdata.intsStrongsPhrases = new int[kjvdata.dVerses.Count, intMaximumRowLength, 4]; //4 slots for multiple strong's numbers (strongs sequences)

            foreach (Verse v in kjvdata.dVerses.OrderBy(a => a.Key).Select(a => a.Value))
            {
                foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                {
                    foreach (StrongsSequence ss in p.dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Value))
                    {
                        int intStrongsNumber = 0;
                        int intStrongsPartIncrease = -1;

                        foreach (string strStrongsNumber in ss.strStrongsNumber.ToUpper().Split('-'))
                        {
                            intStrongsPartIncrease++;

                            intStrongsNumber = v.intVerseID > intMaximumHebrewVerseID
                                ? Convert.ToInt32(ss.strStrongsNumber) + kjvdata.intMaximumHebrewStrongsNumber
                                : Convert.ToInt32(ss.strStrongsNumber);

                            kjvdata.intsStrongsPhrases[v.intVerseID - 1, p.intPhraseID - 1, intStrongsPartIncrease] = intStrongsNumber;
                        }
                    }
                }
            }

            //write data files - for verses past 23145, subtract 8674 from the Strong's number to get the greek Strong's number
            for (int intVerseID = 0; intVerseID < kjvdata.intsStrongsPhrases.GetLength(0); intVerseID++)
            {
                swStrongsMajorWordEnglish.Write((intVerseID + 1).ToString() + " ^ ");

                for (int intPhraseID = 0; intPhraseID < kjvdata.intsStrongsPhrases.GetLength(1); intPhraseID++)
                {
                    for (int intSSIncrease = 0; intSSIncrease < 4; intSSIncrease++)
                    {
                        //REMEMBER:intVerseID, intPhraseID and intSSIncrease are altered here with -1
                        if (kjvdata.intsStrongsPhrases[intVerseID, intPhraseID, intSSIncrease] > 0)
                        {
                            int intStrongsNumber = kjvdata.intsStrongsPhrases[intVerseID, intPhraseID, intSSIncrease];

                            //write +1 to the IDs, because they are zero-based in the array
                            swKJVStrongsVector.WriteLine((intVerseID + 1).ToString() + " ^ " + //sparse vector
                                                    (intPhraseID + 1).ToString() + " ^ " +
                                                    (intSSIncrease + 1).ToString() + " ^ " +
                                                    intStrongsNumber.ToString());

                            if (intStrongsNumber > 0) //dense vector
                            {
                                strStrongsOnly += kjvdata.intsStrongsPhrases[intVerseID, intPhraseID, intSSIncrease].ToString() + " ";
                            }

                            if (intStrongsNumber > 0)
                            {
                                if (intStrongsNumber > 8674)
                                {
                                    foreach (string strKey in dBLBHebrewLexiconEntries[intStrongsNumber - 8674].dAVTranslations.Keys)
                                    {
                                        swStrongsMajorWordEnglish.Write(strKey + "/");
                                    }

                                    swStrongsMajorWordEnglish.Write(" ^ ");
                                }
                                else
                                {
                                    foreach (string strKey in dBLBHebrewLexiconEntries[intStrongsNumber].dAVTranslations.Keys)
                                    {
                                        swStrongsMajorWordEnglish.Write(strKey + "/");
                                    }

                                    swStrongsMajorWordEnglish.Write(" ^ ");
                                }

                            }
                        }
                    }
                }

                swKJVStrongsVectorStrongsOnly.WriteLine(strStrongsOnly.Trim());
                swStrongsMajorWordEnglish.WriteLine();
                strStrongsOnly = "";
            }

            swKJVStrongsVector.Close();
            swKJVStrongsVectorStrongsOnly.Close();
            swStrongsMajorWordEnglish.Close();
        }

        //Returns A[book name, chapter number, verse number, verseid]
        public List<string[]> CreateReferences()
        {
            List<string[]> lReturn = new();

            foreach (int intVerseID in kjvdata.dVerses.Keys)
            {
                string[] strsReference = (string[])Array.CreateInstance(typeof(string), 4);

                strsReference[0] = kjvdata.dVerses[intVerseID].strBookName;
                strsReference[1] = kjvdata.dVerses[intVerseID].intChapterNumber.ToString();
                strsReference[2] = kjvdata.dVerses[intVerseID].intVerseNumber.ToString();
                strsReference[3] = intVerseID.ToString();

                lReturn.Add(strsReference);
            }

            return lReturn;
        }

        public void Create(ref BLBLexiconData blbData, ref StreamReader srBible, ref StreamWriter swRawText,
            ref StreamWriter swVerses, ref StreamWriter swLastVerseIDInBook, ref StreamWriter swKJVStrongsVector,
            ref StreamWriter swKJVStrongsVectorStrongsOnly, ref StreamWriter swStrongsMajorWordEnglish,
            ref StreamWriter swWordIDs, ref StreamWriter swPositionIDs, ref StreamWriter swWordCounts,
            ref StreamWriter swBookWordCounts, ref StreamWriter swPhrasalConcordance, ref StreamWriter swPhrases,
            ref StreamWriter swEnglishPhraseCountsByCount, ref StreamWriter swEnglishPhraseCountsByPhrase,
            ref StreamWriter swEnglishPhraseWithReferences, ref StreamWriter swStrongsPhrasalConcordanceEnglish,
            ref StreamWriter swStrongsPhrasalConcordanceStrongs, ref StreamWriter swSSComplex, ref StreamWriter swSSComplexTranslation,
            ref StreamWriter swSimpleSVOs, ref StreamWriter swLocalPhraseClusters, ref StreamWriter swCrossrefs,
            ref StreamWriter swSyllableScoreHebrew, ref StreamReader srSyllableScoreHebrew, ref StreamWriter swSyllableScoreGreek,
            ref StreamReader srSyllableScoreGreek, ref StreamReader srCRs, ref StreamWriter swReferences,
            bool bWriteEnglishPhraseWithReferences)
        {
            FillKJVStrongs(ref srBible, ref swRawText);
            ParseKJVStrongs(ref blbData.dBLBHebrewLexiconEntries);
            VectorizeKJVStrongs(ref blbData.dBLBHebrewLexiconEntries, ref swKJVStrongsVector, ref swKJVStrongsVectorStrongsOnly,
                ref swStrongsMajorWordEnglish);

            WriteVerses(ref kjvdata, ref swVerses);
            WriteLastVerseIDInBook(ref kjvdata, ref swLastVerseIDInBook);

            kjvdata.lReferences = CreateReferences();
            WriteReferences(ref kjvdata, ref swReferences);

            kjvdata.word.CreateWordIDs(ref kjvdata, ref swWordIDs);
            kjvdata.word.CreateWordPositions(ref kjvdata, ref swPositionIDs);
            kjvdata.word.CreateWordCounts(ref kjvdata, ref swWordCounts);

            kjvdata.wr.CreateWordRelevance(ref kjvdata);
            kjvdata.wr.WriteWordRelevance(ref swBookWordCounts);

            kjvdata.pconc.CreatePhrasalConcordance(ref kjvdata);
            kjvdata.pconc.Write(ref kjvdata, ref swPhrasalConcordance, ref swPhrases,
                ref swEnglishPhraseCountsByCount, ref swEnglishPhraseCountsByPhrase,
                ref swEnglishPhraseWithReferences, bWriteEnglishPhraseWithReferences);
            kjvdata.spconc.CreateStrongsPhrasalConcordance(ref kjvdata);
            kjvdata.spconc.Write(ref kjvdata, ref swStrongsPhrasalConcordanceEnglish, ref swStrongsPhrasalConcordanceStrongs);

            kjvdata.svo.CalculateSVOs(ref kjvdata, ref blbData, ref swSSComplex,
                ref swSSComplexTranslation, ref swSimpleSVOs); //Inline Write()

            csseq.CreateComplexStrongsSequences(ref kjvdata, ref blbData);
            csseq.WriteComplexStrongsSequences(ref swLocalPhraseClusters);

            kjvdata.crossReferences.CreateCrossReferences(ref srCRs);
            kjvdata.crossReferences.Write(ref swCrossrefs);
            kjvdata.spconc.SyllableCloseness_Hebrew(ref swSyllableScoreHebrew, ref srSyllableScoreHebrew); //Inline Write()
            kjvdata.spconc.SyllableCloseness_Greek(ref swSyllableScoreGreek, ref srSyllableScoreGreek); //Inline Write()
        }

        public void Read(ref StreamReader srLastVerseIDInBook, ref StreamReader srVerses,
            ref StreamReader srReferences, ref StreamReader srKJVStrongsVector,
            ref StreamReader srWordIDs, ref StreamReader srWordPositions, ref StreamReader srWordCounts,
            ref StreamReader srBookWordCounts, ref StreamReader srPhrasalConcordance,
            ref StreamReader srStrongsPhrasalConcordanceEnglish, ref StreamReader srStrongsPhrasalConcordanceStrongs,
            ref StreamReader srSSComplex, ref StreamReader srSSComplexTranslation, ref StreamReader srSimpleSVOs,
            ref StreamReader srLocalPhraseClusters, ref StreamReader srCrossrefs, ref StreamReader srSyllableScoreHebrew,
            ref StreamReader srSyllableScoreGreek, ref StreamReader srRawText)
        {
            bool bSeenHeader = false;

            //
            //Last VerseID in Each Book
            //
            bSeenHeader = false;

            while (!srLastVerseIDInBook.EndOfStream)
            {
                string strLine = srLastVerseIDInBook.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    kjvdata.dLastVerseIDInBook.Add(strsLine[0], Convert.ToInt16(strsLine[1]));
                }
            }

            srLastVerseIDInBook.Close();

            //
            //Verses
            //
            ReadVerses(ref kjvdata, ref srVerses);

            //
            //References
            //
            ReadReferences(ref kjvdata, ref srReferences);

            //
            //KJVStrongsVector
            //
            bSeenHeader = false;

            kjvdata.intsStrongsPhrases ??= (int[,,])Array.CreateInstance(typeof(int), kjvdata.dVerses.Count,
                    kjvdata.dVerses.Max(a => a.Value.dPhrases.Count), 4);

            while (!srKJVStrongsVector.EndOfStream)
            {
                string strLine = srKJVStrongsVector.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    //REMEMBER: strsLine[0]-strsLine[2] have been altered from their actual values by -1 to work in the array
                    kjvdata.intsStrongsPhrases[Convert.ToInt16(strsLine[0].Trim()) - 1,
                        Convert.ToInt16(strsLine[1].Trim()) - 1, Convert.ToInt16(strsLine[2].Trim()) - 1] =
                        Convert.ToInt16(strsLine[3].Trim());
                }
            }

            srKJVStrongsVector.Close();

            //
            //WordIDs
            //
            kjvdata.word.ReadWordIDs(ref kjvdata, ref srWordIDs);

            //
            //WordPositions
            //
            kjvdata.word.ReadWordPositions(ref kjvdata, ref srWordPositions);

            //
            //WordCounts
            //
            kjvdata.word.ReadWordCounts(ref kjvdata, ref srWordCounts);

            //
            //WordRelevance.BookWordCounts
            //
            kjvdata.wr.ReadWordRelevance(ref srBookWordCounts);

            //
            //PhrasalConcordance
            //
            kjvdata.pconc.Read(ref kjvdata, ref srPhrasalConcordance);

            //
            //StrongsPhrasalConcordance
            //
            kjvdata.spconc.Read(ref kjvdata, ref srStrongsPhrasalConcordanceEnglish,
                ref srStrongsPhrasalConcordanceStrongs);

            //
            //IntersectingPhrases
            //
            iphrases.ReadIntersectingPhrases();

            //
            //ChainedPhrasalConcordance
            //
            kjvdata.cpconc.Read();

            //
            //SVO
            //
            kjvdata.svo.Read(ref kjvdata, ref srSSComplex, ref srSSComplexTranslation, ref srSimpleSVOs);

            //
            //ComplexStrongsSequences
            //
            csseq.ReadComplexStrongsSequences(ref srLocalPhraseClusters);

            //
            //CrossReferences
            //
            kjvdata.crossReferences.Read(ref srCrossrefs);

            //
            //SyllableClosenessScore
            //
            kjvdata.spconc.syllableScoreHebrew.ReadSyllableScore(ref srSyllableScoreHebrew, true);
            kjvdata.spconc.syllableScoreGreek.ReadSyllableScore(ref srSyllableScoreGreek, false);

            //
            //RawText
            //
            ReadRawText(ref kjvdata, ref srRawText);
        }

        public void ReadVerses(ref KJVData kjvdata, ref StreamReader srVerses)
        {
            bool bSeenHeader = false;
            Regex rgxStrongsPhrase = new(@"(?<phrase>[^\[])(\[(?<strongs>[0-9]{1,})\] {0,}){1,}");
            int intPhraseID = 0;

            while (!srVerses.EndOfStream)
            {
                string strLine = srVerses.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');
                    int intStrongsSequenceID = 0; //The Strong's Sequence IDs start over for each verse

                    if (!kjvdata.dVerses.ContainsKey(Convert.ToInt16(strsLine[0])))
                    {
                        kjvdata.dVerses.Add(Convert.ToInt16(strsLine[0]), new Verse());
                    }

                    kjvdata.dVerses[Convert.ToInt16(strsLine[0])].intBookNumber =
                        Convert.ToInt16(strsLine[1]);
                    kjvdata.dVerses[Convert.ToInt16(strsLine[0])].strBookName = strsLine[2];
                    kjvdata.dVerses[Convert.ToInt16(strsLine[0])].intChapterNumber =
                        Convert.ToInt16(strsLine[3]);
                    kjvdata.dVerses[Convert.ToInt16(strsLine[0])].intVerseNumber =
                        Convert.ToInt16(strsLine[4]);

                    foreach (string strStrongsPhrase in strsLine[5].Split('}'))
                    {
                        Match mPhrase = rgxStrongsPhrase.Match(strStrongsPhrase.TrimStart('{'));
                        Phrase p = new();

                        intPhraseID++;

                        p.intPhraseID = intPhraseID;
                        p.strPhraseText = mPhrase.Groups["phrase"].Value;

                        foreach (Capture capSS in mPhrase.Groups["strongs"].Captures)
                        {
                            StrongsSequence ss = new();

                            intStrongsSequenceID++;

                            ss.intStrongsSequenceID = intStrongsSequenceID;
                            ss.strStrongsNumber = capSS.Value;

                            p.dStrongsSequences.Add(intStrongsSequenceID, ss);
                        }

                        kjvdata.dVerses[Convert.ToInt16(strsLine[0])].dPhrases.Add(intPhraseID, p);
                    }
                }
            }

            srVerses.Close();
        }

        //verse, including dPhrases and dPhraseCounts
        public void WriteVerses(ref KJVData kjvdata, ref StreamWriter swVerses)
        {
            swVerses.WriteLine("VerseID ^ BookNumber ^ BookName ^ ChapterNumber ^ VerseNumber ^ Phrases ^ PhraseCounts");

            foreach (int intVerseID in kjvdata.dVerses.Keys.OrderBy(a => a))
            {
                string strLine = intVerseID.ToString();

                strLine += " ^ " + kjvdata.dVerses[intVerseID].intBookNumber.ToString() + " ^ " + kjvdata.dVerses[intVerseID].strBookName +
                    " ^ " + kjvdata.dVerses[intVerseID].intChapterNumber.ToString() + " ^ " + kjvdata.dVerses[intVerseID].intVerseNumber.ToString() + " ^ ";

                foreach (int intPhraseID in kjvdata.dVerses[intVerseID].dPhrases.Keys.OrderBy(a => a))
                {
                    strLine += "{" + kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText + " ";

                    foreach (int intSSID in kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences.Keys.OrderBy(a => a))
                    {
                        strLine += "[" + kjvdata.dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intSSID].strStrongsNumber + "] ";
                    }

                    strLine = strLine.TrimEnd();
                    strLine += "} ";
                }

                strLine += " ^ ";

                foreach (string strPhrase in kjvdata.dVerses[intVerseID].dPhraseCounts.Keys.OrderBy(a => a))
                {
                    strLine += "{" + kjvdata.dVerses[intVerseID].dPhraseCounts[strPhrase].ToString() + "} ";
                }

                swVerses.WriteLine(strLine.TrimEnd());
            }

            swVerses.Close();
        }

        public void ReadLastVerseIDInBook(ref KJVData kjvdata, StreamReader srLastVerseIDInBook)
        {
            bool bSeenHeader = false;

            while (!srLastVerseIDInBook.EndOfStream)
            {
                string strLine = srLastVerseIDInBook.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!kjvdata.dLastVerseIDInBook.ContainsKey(strsLine[0]))
                    {
                        kjvdata.dLastVerseIDInBook.Add(strsLine[0], Convert.ToInt16(strsLine[1]));
                    }
                }
            }

            srLastVerseIDInBook.Close();
        }

        public void WriteLastVerseIDInBook(ref KJVData kjvdata, ref StreamWriter swLastVerseIDInBook)
        {
            swLastVerseIDInBook.WriteLine("Book Name ^ Last Verse ID");

            foreach (string strBookName in kjvdata.dLastVerseIDInBook.OrderBy(a => a.Value).Select(a => a.Key))
            {
                int intLastVerseInBook = kjvdata.dLastVerseIDInBook[strBookName];

                swLastVerseIDInBook.WriteLine(strBookName + " ^ " + intLastVerseInBook.ToString());
            }

            swLastVerseIDInBook.Close();
        }

        public void ReadReferences(ref KJVData kjvdata, ref StreamReader srReferences)
        {
            bool bSeenHeader = false;

            while (!srReferences.EndOfStream)
            {
                string strLine = srReferences.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');
                    string[] strsReference = (string[])Array.CreateInstance(typeof(string), 4);

                    strsReference[0] = strsLine[0];
                    strsReference[1] = strsLine[1];
                    strsReference[2] = strsLine[2];
                    strsReference[3] = strsLine[3];

                    kjvdata.lReferences.Add(strsReference);
                }
            }

            srReferences.Close();
        }

        public void WriteReferences(ref KJVData kjvdata, ref StreamWriter swReferences)
        {
            //
            //kjvdata.lReferences
            //
            swReferences.WriteLine("BookName ^ Chapter ^ Verse ^ VerseID");

            foreach (string[] strsReference in kjvdata.lReferences)
            {
                swReferences.WriteLine(strsReference[0] + " ^ " + strsReference[1] + " ^ " +
                    strsReference[2] + " ^ " + strsReference[3]);
            }

            swReferences.Close();
        }

        public void ReadRawText(ref KJVData kjvdata, ref StreamReader srRawText)
        {
            int intLineCounter = 0;

            while (!srRawText.EndOfStream)
            {
                string strLine = srRawText.ReadLine();

                intLineCounter++;

                kjvdata.dRawText.Add(intLineCounter, strLine);
            }

            srRawText.Close();
        }
    }

    public class KJVData
    {
        public Regex rgxCleanPhrase = new(@"[^a-zA-Z ]");

        public int intMaximumHebrewStrongsNumber = 0;  //Normally, this is 8674, but it's calculated in VectorizeKJVStrongs in case the data uses extended Strong's numbers
        public int[,,]? intsStrongsPhrases; //[VerseID - 1, PhraseID - 1, StrongsPartIncrease] = StrongsNumber; StrongsPartIncrease is for multiple Strong's numbers (StrongsSequence) the ? means this is nullable

        public Dictionary<int, Verse> dVerses = new();
        public Dictionary<int, Verse> dVersesWithoutStrongs = new();
        public Dictionary<string, int> dLastVerseIDInBook = new(); //D<BookName, LastVerseID>
        public Dictionary<int, string> dWordIDs = new(); //D<WordID, Word Text>
        public Dictionary<int, string> dWordPositions = new(); //D<PositionID, Word text>
        public Dictionary<string, int> dWordCounts = new(); //D<Word, Count>
        public Dictionary<int, string> dBookStartingPositions = new(); //D<Book Starting PositionID, Book Name>
        public Dictionary<string, List<string>> dPhrasalConcordance = new(); //D<Phrase, L<ReferenceStrings>>
        //D<strongs english phrase translation number 1, D<strongs english phrase translation number 2, count>>
        public Dictionary<string, Dictionary<string, int>> dStrongsPhrasalConcordanceEnglish = new(); //D<Strongs-Based English Phrase 1, D<Strongs-Based English Phrase 2, Count>>
        public Dictionary<int, Dictionary<int, int>> dStrongsPhrasalConcordanceStrongs = new(); //D<Strongs Number 1, D<Strongs Number 2, Count>>
        public Dictionary<int, string> dRawText = new(); //D<line|verse w/o reference, text>

        //KJVStrongs namespace objects
        public Word word = new();
        public SVO svo = new();
        public CrossReferences crossReferences = new();
        public PhrasalConcordance pconc = new();
        public StrongsPhrasalConcordance spconc = new();
        public ChainedPhrasalConcordance cpconc = new();
        public List<string[]> lReferences = new();
        public Relationships relationships = new();
        public WordRelevance wr = new();

    }

    public class WordData
    {
        public int intWordID;
        public string? strWord;
    }
}
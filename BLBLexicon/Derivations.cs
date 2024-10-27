using System.Text;
using System.Text.RegularExpressions;

namespace BLBLexicon
{
    public class Derivations
    {
        //MAKE SURE: ??LexiconID = Strongs Number??
        public Dictionary<int, List<string>> dDerivations = new(); //D<Strongs Number, L<Whole text from which Connections are extracted>
        public Dictionary<int, List<string>> dConnections = new(); //D<Strongs Number, L<G||HStrongs numbers derived from>>

        public Dictionary<int, BLBHebrewLexicon> dHRoots = new(); //D<Rooted Strongs Number, Lexicon Object>
        public Dictionary<int, BLBHebrewLexicon> dHNonRoots = new(); //D<NonRooted Strongs Number, Lexicon Object>
        public Dictionary<int, BLBHebrewLexicon> dHAramaic = new(); //D<Aramaic Strongs Number, Lexicon Object>
        public Dictionary<int, BLBHebrewLexicon> dHNonAramaic = new(); //D<NonAramaic Strongs Number, Lexicon Object>
        public Dictionary<int, BLBHebrewLexicon> dHRootedAramaic = new(); //D<RootedAramaic Strongs Number, Lexicon Object>

        public void CreateDerivatives(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries,
            ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries)
        {
            FindBLBGreekDerivations(ref dBLBGreekLexiconEntries);
            BuildBLBGreekDerivationStructures(ref dBLBGreekLexiconEntries);

            dHRoots = HRoots(ref dBLBHebrewLexiconEntries);
            dHNonRoots = HNonRoots(ref dBLBHebrewLexiconEntries);
            dHAramaic = Aramaic(ref dBLBHebrewLexiconEntries);
            dHNonAramaic = NotAramaic(ref dBLBHebrewLexiconEntries);
            dHRootedAramaic = RootedAramaic(ref dBLBHebrewLexiconEntries);
        }

        public void FillDerivations(
            ref StreamReader srBLBDerivativesDerivations,
            ref StreamReader srBLBDerivativesConnections,
            ref StreamReader srBLBDerivativesHRoots,
            ref StreamReader srBLBDerivativesHNonRoots,
            ref StreamReader srBLBDerivativesHAramaic,
            ref StreamReader srBLBDerivativesHNonAramaic,
            ref StreamReader srBLBDerivativesHRootedAramaic,
            ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            bool bSeenHeader = false;

            //
            //dDerivations
            //
            while (!srBLBDerivativesDerivations.EndOfStream)
            {
                string strLine = srBLBDerivativesDerivations.ReadLine();

                if (bSeenHeader == false)
                { //Strongs Number ^ Derived By
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');
                    int intStrongsNumber = Convert.ToInt16(strsLine[0]);

                    dDerivations.Add(intStrongsNumber, new List<string>());

                    foreach (string strDerivedBy in strsLine[1].Split(','))
                    {
                        dDerivations[intStrongsNumber].Add(strDerivedBy);
                    }
                }
            }

            srBLBDerivativesDerivations.Close();

            //
            //dConnections
            //

            bSeenHeader = false;

            while (!srBLBDerivativesConnections.EndOfStream)
            {
                string strLine = srBLBDerivativesConnections.ReadLine();

                if (bSeenHeader == false)
                { //Strongs Number ^ Derived From
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');
                    int intStrongsNumber = Convert.ToInt16(strsLine[0].Trim()[1..]);

                    dConnections.Add(intStrongsNumber, new List<string>());

                    foreach (string strDerivedFrom in strsLine[1].Split(','))
                    {
                        dConnections[intStrongsNumber].Add(strDerivedFrom.Trim()[1..]);
                    }
                }
            }

            srBLBDerivativesConnections.Close();

            bSeenHeader = false;

            //
            //dHRoots
            //

            //
            while (!srBLBDerivativesHRoots.EndOfStream)
            {
                string strLine = srBLBDerivativesHRoots.ReadLine();

                if (bSeenHeader == false)
                { //Hebrew Roots Strongs Number
                    bSeenHeader = true;
                }
                else
                {
                    int intStrongsNumber = Convert.ToInt16(strLine);

                    dHRoots.Add(intStrongsNumber, dBLBHebrewLexiconEntries[intStrongsNumber]);
                }
            }

            srBLBDerivativesHRoots.Close();

            bSeenHeader = false;

            //
            //dHNonRoots
            //

            //
            while (!srBLBDerivativesHNonRoots.EndOfStream)
            {
                string strLine = srBLBDerivativesHNonRoots.ReadLine();

                if (bSeenHeader == false)
                { //Hebrew NonRoots Strongs Number
                    bSeenHeader = true;
                }
                else
                {
                    int intStrongsNumber = Convert.ToInt16(strLine);

                    dHNonRoots.Add(intStrongsNumber, dBLBHebrewLexiconEntries[intStrongsNumber]);
                }
            }

            srBLBDerivativesHNonRoots.Close();

            bSeenHeader = false;

            //
            //dHAramaic
            //

            //
            while (!srBLBDerivativesHAramaic.EndOfStream)
            {
                string strLine = srBLBDerivativesHAramaic.ReadLine();

                if (bSeenHeader == false)
                { //Hebrew Aramaic Strongs Number
                    bSeenHeader = true;
                }
                else
                {
                    int intStrongsNumber = Convert.ToInt16(strLine);

                    dHAramaic.Add(intStrongsNumber, dBLBHebrewLexiconEntries[intStrongsNumber]);
                }
            }

            srBLBDerivativesHAramaic.Close();

            bSeenHeader = false;

            //
            //dHNonAramaic
            //

            //
            while (!srBLBDerivativesHNonAramaic.EndOfStream)
            {
                string strLine = srBLBDerivativesHNonAramaic.ReadLine();

                if (bSeenHeader == false)
                { //Hebrew NonAramaic Strongs Number
                    bSeenHeader = true;
                }
                else
                {
                    int intStrongsNumber = Convert.ToInt16(strLine);

                    dHNonAramaic.Add(intStrongsNumber, dBLBHebrewLexiconEntries[intStrongsNumber]);
                }
            }

            srBLBDerivativesHNonAramaic.Close();

            bSeenHeader = false;

            //
            //dHRootedAramaic
            //

            //
            while (!srBLBDerivativesHRootedAramaic.EndOfStream)
            {
                string strLine = srBLBDerivativesHRootedAramaic.ReadLine();

                if (bSeenHeader == false)
                { //Hebrew RootedAramaic Strongs Number
                    bSeenHeader = true;
                }
                else
                {
                    int intStrongsNumber = Convert.ToInt16(strLine);

                    dHRootedAramaic.Add(intStrongsNumber, dBLBHebrewLexiconEntries[intStrongsNumber]);
                }
            }

            srBLBDerivativesHRootedAramaic.Close();
        }

        public void WriteDerivations(
            ref StreamWriter swBLBDerivativesDerivations,
            ref StreamWriter swBLBDerivativesConnections,
            ref StreamWriter swBLBDerivativesHRoots,
            ref StreamWriter swBLBDerivativesHNonRoots,
            ref StreamWriter swBLBDerivativesHAramaic,
            ref StreamWriter swBLBDerivativesHNonAramaic,
            ref StreamWriter swBLBDerivativesHRootedAramaic)
        {
            StringBuilder sbDerivations = new();
            StringBuilder sbConnections = new();

            //
            // dDerivations
            //
            swBLBDerivativesDerivations.WriteLine("Strongs Number ^ Derived By");

            foreach (int intLexiconID in dDerivations.Keys.OrderBy(a => a))
            {
                _ = sbDerivations.Clear();

                swBLBDerivativesDerivations.Write(intLexiconID.ToString() + " ^ ");

                foreach (string strDerivation in dDerivations[intLexiconID].OrderBy(a => a))
                {
                    _ = sbDerivations.Append(strDerivation + " , ");
                }

                _ = sbDerivations.Remove(sbDerivations.Length - 3, 3);

                swBLBDerivativesDerivations.WriteLine(sbDerivations.ToString());
            }

            swBLBDerivativesDerivations.Close();

            //
            //dConnections
            //
            swBLBDerivativesConnections.WriteLine("Strongs Number ^ Derived From");

            foreach (int intStrongs in dConnections.Keys.OrderBy(a => a))
            {
                _ = sbConnections.Clear();

                swBLBDerivativesConnections.Write("g" + intStrongs.ToString() + " ^ ");

                foreach (string strStrongsDerivedFrom in dConnections[intStrongs].OrderBy(a => a))
                {
                    _ = sbConnections.Append(strStrongsDerivedFrom + " , ");
                }

                _ = sbConnections.Remove(sbConnections.Length - 3, 3);

                swBLBDerivativesConnections.WriteLine(sbConnections.ToString());
            }

            swBLBDerivativesConnections.Close();

            //
            //HRoots
            //
            swBLBDerivativesHRoots.WriteLine("Hebrew Roots Strongs Number");

            foreach (int intStrongsNumber in dHRoots.Keys.OrderBy(a => a))
            {
                swBLBDerivativesHRoots.WriteLine(intStrongsNumber.ToString());
            }

            swBLBDerivativesHRoots.Close();

            //
            //HNonRoots
            //
            swBLBDerivativesHNonRoots.WriteLine("Hebrew NonRoots Strongs Number");

            foreach (int intStrongsNumber in dHNonRoots.Keys.OrderBy(a => a))
            {
                swBLBDerivativesHNonRoots.WriteLine(intStrongsNumber.ToString());
            }

            swBLBDerivativesHNonRoots.Close();

            //
            //HAramaic
            //
            swBLBDerivativesHAramaic.WriteLine("Hebrew Aramaic Strongs Number");

            foreach (int intStrongsNumber in dHAramaic.Keys.OrderBy(a => a))
            {
                swBLBDerivativesHAramaic.WriteLine(intStrongsNumber.ToString());
            }

            swBLBDerivativesHAramaic.Close();

            //
            //HNonAramaic
            //
            swBLBDerivativesHNonAramaic.WriteLine("Hebrew NonAramaic Strongs Number");

            foreach (int intStrongsNumber in dHNonAramaic.Keys.OrderBy(a => a))
            {
                swBLBDerivativesHNonAramaic.WriteLine(intStrongsNumber.ToString());
            }

            swBLBDerivativesHNonAramaic.Close();

            //
            //HRootedAramaic
            //
            swBLBDerivativesHRootedAramaic.WriteLine("Hebrew Rooted Aramaic Strongs Number");

            foreach (int intStrongsNumber in dHRootedAramaic.Keys.OrderBy(a => a))
            {
                swBLBDerivativesHRootedAramaic.WriteLine(intStrongsNumber.ToString());
            }

            swBLBDerivativesHRootedAramaic.Close();
        }

        //requires dBLBGreekLexiconEntries
        public void FindBLBGreekDerivations(ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries)
        {
            foreach (int intLexiconID in dBLBGreekLexiconEntries.Keys.OrderBy(a => a))
            {
                BLBGreekLexicon bglDerivation = dBLBGreekLexiconEntries[intLexiconID];
                string strDerivedBy = bglDerivation.strConnection;

                if (!dDerivations.ContainsKey(intLexiconID))
                {
                    dDerivations.Add(intLexiconID, new List<string>());
                }

                if (!dDerivations[intLexiconID].Contains(strDerivedBy))
                {
                    dDerivations[intLexiconID].Add(strDerivedBy);
                }
            }
        }

        //depends on FindBLBGreekDerivations, so long as that function still writes its data file
        public void BuildBLBGreekDerivationStructures(ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries) //requires dBLBGreekLexiconEntries and dBLBHebrewLexiconEntries
        {
            //StreamReader srGreekDerivation = new(@"Data/Processed/BLBGreekDerivations.txt");
            Regex rgxNumbers = new(@"[0-9]{1,}");

            foreach (int intLexiconID in dBLBGreekLexiconEntries.Keys.OrderBy(a => a))
            {
                string strConnections = dBLBGreekLexiconEntries[intLexiconID].strConnection.ToLower();

                if (rgxNumbers.IsMatch(strConnections))
                {
                    foreach (Match mConnectionNumber in rgxNumbers.Matches(strConnections))
                    {
                        string strDerivationNumber = mConnectionNumber.Value;

                        if (!dConnections.ContainsKey(intLexiconID))
                        {
                            dConnections.Add(intLexiconID, new List<string>());
                        }

                        dConnections[intLexiconID].Add("g" + mConnectionNumber.Value);
                    }
                }

                //if (strDerivedBy.Contains("of Hebrew origin"))
                //{
                //    bHebrewOrigin = true;
                //    strDerivedBy.Replace("of Hebrew origin", "");
                //}
            }
        }

        public Dictionary<int, BLBHebrewLexicon> HRoots(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bRoot == true)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        public Dictionary<int, BLBHebrewLexicon> HNonRoots(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bRoot == false)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        public Dictionary<int, BLBHebrewLexicon> Aramaic(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bAramaic == true)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        public Dictionary<int, BLBHebrewLexicon> NotAramaic(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bAramaic == false)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        public Dictionary<int, BLBHebrewLexicon> RootedAramaic(ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries)
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bAramaic == true && dBLBHebrewLexiconEntries[intKey].bRoot == true)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }


        //public void TranslationDerivationExpander(ref StreamWriter swGreekDerivations,
        //    string strDerivation, bool bIsHebrew)
        //{
        //    int intTranslationCounter = 0;
        //    string strDerivationNumber = strDerivation;

        //    if (strDerivation.StartsWith("h") || strDerivation.StartsWith("g"))
        //    {
        //        strDerivationNumber = strDerivation.Substring(1);
        //    }
        //    else
        //    {
        //        strDerivationNumber = strDerivation;
        //    }

        //    if (bIsHebrew == false)
        //    {
        //        int intTranslationCount = dBLBGreekLexiconEntries
        //                        [Convert.ToInt16(strDerivationNumber)].dAVTranslations.Count();
        //        string strConnection = dBLBGreekLexiconEntries
        //                            [Convert.ToInt16(strDerivationNumber)]
        //                            .strConnection;

        //        if (rgxNumbers.IsMatch(strConnection))
        //        {
        //            foreach (Match mTranslation in rgxNumbers.Matches(strConnection))
        //            {
        //                string strTranslation_NumbersExpanded = "";
        //                int intExpansionCounter = 0;

        //                intTranslationCounter++;

        //                //if (rgxTranslationPlusNumbers.IsMatch(strConnection))
        //                //{
        //                    int intExpansionCount = rgxNumbers.Matches(strConnection).Count();

        //                    //foreach (Match mTranslationPlusNumbers in
        //                    //    rgxTranslationPlusNumbers.Matches(strTranslation))
        //                    //{
        //                        intExpansionCounter++;

        //                        strTranslation_NumbersExpanded += mTranslation.Value + " + ";

        //                        foreach (string strTranslationPlusNumbers in dBLBGreekLexiconEntries[
        //                            Convert.ToInt16(mTranslation.Value)]
        //                            .dAVTranslations.OrderBy(a => a.Value).Select(a => a.Key))
        //                        {
        //                            swGreekDerivations.Write("[");

        //                            TranslationDerivationExpander(ref swGreekDerivations,
        //                                mTranslation.Value, bIsHebrew);

        //                            swGreekDerivations.Write("]");
        //                        }
        //                    //}
        //                //}
        //                //else
        //                //{
        //                //    strTranslation_NumbersExpanded = strTranslation;
        //                //}

        //                if (intTranslationCounter < intTranslationCount)
        //                {
        //                    swGreekDerivations.Write(strTranslation_NumbersExpanded + ", ");
        //                }
        //                else
        //                {
        //                    swGreekDerivations.Write(strTranslation_NumbersExpanded);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        int intTranslationCount = dBLBHebrewLexiconEntries
        //                        [Convert.ToInt16(strDerivationNumber)].dAVTranslations.Count();
        //        string strConnection = dBLBHebrewLexiconEntries
        //                            [Convert.ToInt16(strDerivationNumber)]
        //                            .strConnection;

        //        if (rgxNumbers.IsMatch(strConnection))
        //        {
        //            foreach (Match mTranslation in rgxNumbers.Matches(strConnection))
        //            {
        //                string strTranslation_NumbersExpanded = "";
        //                int intExpansionCounter = 0;

        //                intTranslationCounter++;

        //                //if (rgxTranslationPlusNumbers.IsMatch(strConnection))
        //                //{
        //                int intExpansionCount = rgxNumbers.Matches(strConnection).Count();

        //                //foreach (Match mTranslationPlusNumbers in
        //                //    rgxTranslationPlusNumbers.Matches(strTranslation))
        //                //{
        //                intExpansionCounter++;

        //                strTranslation_NumbersExpanded += mTranslation.Value + " + ";

        //                foreach (string strTranslationPlusNumbers in dBLBHebrewLexiconEntries[
        //                    Convert.ToInt16(mTranslation.Value)]
        //                    .dAVTranslations.OrderBy(a => a.Value).Select(a => a.Key))
        //                {
        //                    swGreekDerivations.Write("[");

        //                    TranslationDerivationExpander(ref swGreekDerivations,
        //                        mTranslation.Value, bIsHebrew);

        //                    swGreekDerivations.Write("]");
        //                }
        //                //}
        //                //}
        //                //else
        //                //{
        //                //    strTranslation_NumbersExpanded = strTranslation;
        //                //}

        //                if (intTranslationCounter < intTranslationCount)
        //                {
        //                    swGreekDerivations.Write(strTranslation_NumbersExpanded + ", ");
        //                }
        //                else
        //                {
        //                    swGreekDerivations.Write(strTranslation_NumbersExpanded);
        //                }
        //            }
        //        }
        //    }
        //}


    }
}

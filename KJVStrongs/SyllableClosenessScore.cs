namespace KJVStrongs
{
    public class SyllableClosenessScore
    {
        //Creates a Syllable-wise Closeness score

        public Dictionary<int, Dictionary<int, string>> dWordSyllablesHebrew = new(); //D<Strong's Number, D<Syllable Order ID, Syllable>>
        public Dictionary<int, Dictionary<int, double>> dClosenessHebrew = new(); //D<Strong's Number of Target Word, D<Strong's Number of Comparing Word, Relative Closeness>> where Relative Closeness is from 0.0 to <1.0 and is calculated as the number of same syllables IN ORDER (without stress punctuation) divided by greatest number of syllables in either word; if all syllables, for instance were the same except the first one, the score would be 0.0, because closeness is counted in order through the syllables
        public Dictionary<int, Dictionary<int, string>> dWordSyllablesGreek = new(); //D<Strong's Number, D<Syllable Order ID, Syllable>>
        public Dictionary<int, Dictionary<int, double>> dClosenessGreek = new(); //D<Strong's Number of Target Word, D<Strong's Number of Comparing Word, Relative Closeness>> where Relative Closeness is from 0.0 to <1.0 and is calculated as the number of same syllables IN ORDER (without stress punctuation) divided by greatest number of syllables in either word; if all syllables, for instance were the same except the first one, the score would be 0.0, because closeness is counted in order through the syllables

        public void Parse(ref StreamReader srSyllableScore, bool bIsHebrew)
        {
            int intLineNumber = 0;

            while (!srSyllableScore.EndOfStream)
            {
                string strLine = srSyllableScore.ReadLine();

                intLineNumber++;

                if (intLineNumber > 1) //The first line has Column Headers
                {
                    int intColumnNumber = 0;
                    int intStrongsNumber = 0;
                    int intPronunciationColumn = bIsHebrew == true ? 2 : 3;
                    foreach (string strColumn in strLine.Split('^'))
                    {
                        intColumnNumber++;

                        if (intColumnNumber == 1)
                        {
                            intStrongsNumber = Convert.ToInt32(strColumn.Trim());

                            if (bIsHebrew == true)
                            {
                                dWordSyllablesHebrew.Add(intStrongsNumber, new Dictionary<int, string>());
                            }
                            else
                            {
                                dWordSyllablesGreek.Add(intStrongsNumber, new Dictionary<int, string>());
                            }
                        }

                        if (intColumnNumber == intPronunciationColumn)
                        {
                            int intSyllableOrderID = 0;

                            foreach (string strSyllable in strColumn.Split('-'))
                            {
                                intSyllableOrderID++;

                                if (bIsHebrew == true)
                                {
                                    dWordSyllablesHebrew[intStrongsNumber].Add(intSyllableOrderID, strSyllable.Trim("'".ToCharArray()[0]).Trim()); //Trim away stress punctuation
                                }
                                else
                                {
                                    dWordSyllablesGreek[intStrongsNumber].Add(intSyllableOrderID, strSyllable.Trim("'".ToCharArray()[0]).Trim()); //Trim away stress punctuation
                                }
                            }
                        }
                    }
                }
            }
        }

        //after processing, calls Write()
        public void Examine(ref StreamWriter swSyllableScore, bool bIsHebrew)
        {
            Dictionary<string, List<int>> dNonZero = new(); //D<First Syllable, Strong's Numbers of Words which first syllable matches>
            string strCurrentSyllable = "";
            int intCurrentGroupID = 0;

            swSyllableScore.WriteLine("GroupID ^ StrongsNumberOne ^ StrongsNumberTwo ^ SyllableWiseClosenessScore ^ NumberOfMatchingSyllables" +
                " ^ WordOneSyllableCount ^ WordTwoSyllableCount ^ WordOneText ^ WordTwoText");

            if (bIsHebrew == true)
            {
                foreach (int intStrongsNumber in dWordSyllablesHebrew.Keys.OrderBy(a => a))
                {
                    strCurrentSyllable = dWordSyllablesHebrew[intStrongsNumber][1];

                    if (!dNonZero.ContainsKey(strCurrentSyllable))
                    {
                        dNonZero.Add(strCurrentSyllable, new List<int>());
                    }

                    dNonZero[strCurrentSyllable].Add(intStrongsNumber);
                }

                foreach (string strFirstSyllable in dNonZero.Keys.OrderBy(a => a))
                {
                    intCurrentGroupID++;

                    foreach (int intStrongsNumber in dNonZero[strFirstSyllable])
                    {
                        int intCurrentSyllableCount = dWordSyllablesHebrew[intStrongsNumber].Count;

                        foreach (int intStrongsNumberCompare in dNonZero[strFirstSyllable].Where(a => a > intStrongsNumber)) //Where: Don't compare a word with itself or any word that has already compared with it; eg. 5 ^ 4 && 4 ^ 5
                        {
                            int intCompareSyllableCount = dWordSyllablesHebrew[intStrongsNumberCompare].Count;
                            int intCommonSyllableCount = 0;

                            foreach (int intSyllablePosition in dWordSyllablesHebrew[intStrongsNumber].Keys) //does the first word have more syllables than the second word?
                            {
                                if (dWordSyllablesHebrew[intStrongsNumberCompare].ContainsKey(intSyllablePosition))
                                {

                                    if (dWordSyllablesHebrew[intStrongsNumber][intSyllablePosition] == //do the two words share this syllable?
                                        dWordSyllablesHebrew[intStrongsNumberCompare][intSyllablePosition])
                                    {
                                        intCommonSyllableCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            _ = intCurrentSyllableCount > intCompareSyllableCount
                                ? Write(ref swSyllableScore, intCommonSyllableCount, intCompareSyllableCount, intStrongsNumber, intStrongsNumberCompare, intCurrentGroupID, bIsHebrew)
                                : Write(ref swSyllableScore, intCommonSyllableCount, intCurrentSyllableCount, intStrongsNumberCompare, intStrongsNumber, intCurrentGroupID, bIsHebrew);
                        }
                    }
                }
            }
            else
            {
                foreach (int intStrongsNumber in dWordSyllablesGreek.Keys.OrderBy(a => a))
                {
                    strCurrentSyllable = dWordSyllablesGreek[intStrongsNumber][1];

                    if (!dNonZero.ContainsKey(strCurrentSyllable))
                    {
                        dNonZero.Add(strCurrentSyllable, new List<int>());
                    }

                    dNonZero[strCurrentSyllable].Add(intStrongsNumber);
                }

                foreach (string strFirstSyllable in dNonZero.Keys.OrderBy(a => a))
                {
                    intCurrentGroupID++;

                    foreach (int intStrongsNumber in dNonZero[strFirstSyllable])
                    {
                        int intCurrentSyllableCount = dWordSyllablesGreek[intStrongsNumber].Count;

                        foreach (int intStrongsNumberCompare in dNonZero[strFirstSyllable].Where(a => a > intStrongsNumber)) //Where: Don't compare a word with itself or any word that has already compared with it; eg. 5 ^ 4 && 4 ^ 5
                        {
                            int intCompareSyllableCount = dWordSyllablesGreek[intStrongsNumberCompare].Count;
                            int intCommonSyllableCount = 0;

                            foreach (int intSyllablePosition in dWordSyllablesGreek[intStrongsNumber].Keys) //does the first word have more syllables than the second word?
                            {
                                if (dWordSyllablesGreek[intStrongsNumberCompare].ContainsKey(intSyllablePosition))
                                {

                                    if (dWordSyllablesGreek[intStrongsNumber][intSyllablePosition] == //do the two words share this syllable?
                                        dWordSyllablesGreek[intStrongsNumberCompare][intSyllablePosition])
                                    {
                                        intCommonSyllableCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            _ = intCurrentSyllableCount > intCompareSyllableCount
                                ? Write(ref swSyllableScore, intCommonSyllableCount, intCompareSyllableCount, intStrongsNumber, intStrongsNumberCompare, intCurrentGroupID, bIsHebrew)
                                : Write(ref swSyllableScore, intCommonSyllableCount, intCurrentSyllableCount, intStrongsNumberCompare, intStrongsNumber, intCurrentGroupID, bIsHebrew);
                        }
                    }
                }
            }

            swSyllableScore.Close();
        }

        public void ReadSyllableScore(ref StreamReader srSyllableScore, bool bIsHebrew)
        {
            bool bSeenHeader = false;

            if (bIsHebrew == true)
            {
                dClosenessHebrew.Clear();

                while (!srSyllableScore.EndOfStream)
                {
                    string strLine = srSyllableScore.ReadLine();

                    if (bSeenHeader == false)
                    {
                        bSeenHeader = true;
                    }
                    else
                    {
                        //0 - intCurrentGroupID
                        //1 - intStrongsNumber
                        //2 - intStrongsNumberCompare
                        //3 - dCloseness[intStrongsNumber][intStrongsNumberCompare]
                        //4 - intCommonSyllableCount
                        //5 - dWordSyllables[intStrongsNumber].Count
                        //6 - dWordSyllables[intStrongsNumberCompare].Count
                        //7 - swSyllableScore.Write(dWordSyllables[intStrongsNumber][intSyllablePosition]
                        //8 - swSyllableScore.Write(dWordSyllables[intStrongsNumberCompare][intSyllablePosition]

                        string[] strsLine = strLine.Split('^');

                        //dCloseness[1][2] = 3

                        if (!dClosenessHebrew.ContainsKey(Convert.ToInt16(strsLine[1])))
                        {
                            dClosenessHebrew.Add(Convert.ToInt16(strsLine[1]), new Dictionary<int, double>());
                        }

                        if (!dClosenessHebrew[Convert.ToInt16(strsLine[1])].ContainsKey(Convert.ToInt16(strsLine[2])))
                        {
                            dClosenessHebrew[Convert.ToInt16(strsLine[1])].Add(Convert.ToInt16(strsLine[2]),
                                Convert.ToDouble(strsLine[3]));
                        }
                    }
                }
            }
            else
            {
                dClosenessGreek.Clear();

                while (!srSyllableScore.EndOfStream)
                {
                    string strLine = srSyllableScore.ReadLine();

                    if (bSeenHeader == false)
                    {
                        bSeenHeader = true;
                    }
                    else
                    {
                        //0 - intCurrentGroupID
                        //1 - intStrongsNumber
                        //2 - intStrongsNumberCompare
                        //3 - dCloseness[intStrongsNumber][intStrongsNumberCompare]
                        //4 - intCommonSyllableCount
                        //5 - dWordSyllables[intStrongsNumber].Count
                        //6 - dWordSyllables[intStrongsNumberCompare].Count
                        //7 - swSyllableScore.Write(dWordSyllables[intStrongsNumber][intSyllablePosition]
                        //8 - swSyllableScore.Write(dWordSyllables[intStrongsNumberCompare][intSyllablePosition]

                        string[] strsLine = strLine.Split('^');

                        //dCloseness[1][2] = 3

                        if (!dClosenessGreek.ContainsKey(Convert.ToInt16(strsLine[1])))
                        {
                            dClosenessGreek.Add(Convert.ToInt16(strsLine[1]), new Dictionary<int, double>());
                        }

                        if (!dClosenessGreek[Convert.ToInt16(strsLine[1])].ContainsKey(Convert.ToInt16(strsLine[2])))
                        {
                            dClosenessGreek[Convert.ToInt16(strsLine[1])].Add(Convert.ToInt16(strsLine[2]),
                                Convert.ToDouble(strsLine[3]));
                        }
                    }
                }
            }

            srSyllableScore.Close();
        }

        public double Write(ref StreamWriter swSyllableScore, int intCommonSyllableCount, int intCompareSyllableCount,
            int intStrongsNumber, int intStrongsNumberCompare, int intCurrentGroupID, bool bIsHebrew)
        {
            double dblReturn = intCommonSyllableCount / (double)intCompareSyllableCount;

            if (bIsHebrew == true)
            {
                if (!dClosenessHebrew.ContainsKey(intStrongsNumber))
                {
                    dClosenessHebrew.Add(intStrongsNumber, new Dictionary<int, double>());
                }

                dClosenessHebrew[intStrongsNumber].Add(intStrongsNumberCompare, dblReturn);

                swSyllableScore.Write(intCurrentGroupID.ToString() + " ^ " + intStrongsNumber.ToString() + " ^ " +
                    intStrongsNumberCompare.ToString() + " ^ " + dClosenessHebrew[intStrongsNumber][intStrongsNumberCompare].ToString("0.00") +
                    " ^ " + intCommonSyllableCount.ToString() + " ^ " + dWordSyllablesHebrew[intStrongsNumber].Count.ToString() +
                    " ^ " + dWordSyllablesHebrew[intStrongsNumberCompare].Count.ToString() + " ^ ");

                for (int intSyllablePosition = 1; intSyllablePosition <= dWordSyllablesHebrew[intStrongsNumber].Count; intSyllablePosition++)
                {
                    if (intSyllablePosition < dWordSyllablesHebrew[intStrongsNumber].Count)
                    {
                        swSyllableScore.Write(dWordSyllablesHebrew[intStrongsNumber][intSyllablePosition] + "-");
                    }
                    else
                    {
                        swSyllableScore.Write(dWordSyllablesHebrew[intStrongsNumber][intSyllablePosition]);
                    }
                }

                swSyllableScore.Write(" ^ ");


                for (int intSyllablePosition = 1; intSyllablePosition <= dWordSyllablesHebrew[intStrongsNumberCompare].Count; intSyllablePosition++)
                {
                    if (intSyllablePosition < dWordSyllablesHebrew[intStrongsNumberCompare].Count)
                    {
                        swSyllableScore.Write(dWordSyllablesHebrew[intStrongsNumberCompare][intSyllablePosition] + "-");
                    }
                    else
                    {
                        swSyllableScore.Write(dWordSyllablesHebrew[intStrongsNumberCompare][intSyllablePosition]);
                    }
                }
            }
            else
            {
                if (!dClosenessGreek.ContainsKey(intStrongsNumber))
                {
                    dClosenessGreek.Add(intStrongsNumber, new Dictionary<int, double>());
                }

                dClosenessGreek[intStrongsNumber].Add(intStrongsNumberCompare, dblReturn);

                swSyllableScore.Write(intCurrentGroupID.ToString() + " ^ " + intStrongsNumber.ToString() + " ^ " +
                    intStrongsNumberCompare.ToString() + " ^ " + dClosenessGreek[intStrongsNumber][intStrongsNumberCompare].ToString("0.00") +
                    " ^ " + intCommonSyllableCount.ToString() + " ^ " + dWordSyllablesGreek[intStrongsNumber].Count.ToString() +
                    " ^ " + dWordSyllablesGreek[intStrongsNumberCompare].Count.ToString() + " ^ ");

                for (int intSyllablePosition = 1; intSyllablePosition <= dWordSyllablesGreek[intStrongsNumber].Count; intSyllablePosition++)
                {
                    if (intSyllablePosition < dWordSyllablesGreek[intStrongsNumber].Count)
                    {
                        swSyllableScore.Write(dWordSyllablesGreek[intStrongsNumber][intSyllablePosition] + "-");
                    }
                    else
                    {
                        swSyllableScore.Write(dWordSyllablesGreek[intStrongsNumber][intSyllablePosition]);
                    }
                }

                swSyllableScore.Write(" ^ ");


                for (int intSyllablePosition = 1; intSyllablePosition <= dWordSyllablesGreek[intStrongsNumberCompare].Count; intSyllablePosition++)
                {
                    if (intSyllablePosition < dWordSyllablesGreek[intStrongsNumberCompare].Count)
                    {
                        swSyllableScore.Write(dWordSyllablesGreek[intStrongsNumberCompare][intSyllablePosition] + "-");
                    }
                    else
                    {
                        swSyllableScore.Write(dWordSyllablesGreek[intStrongsNumberCompare][intSyllablePosition]);
                    }
                }
            }

            swSyllableScore.WriteLine();

            return dblReturn;
        }

    }
}

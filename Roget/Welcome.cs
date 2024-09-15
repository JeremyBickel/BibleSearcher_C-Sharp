using System.Text.RegularExpressions;

namespace Roget
{
    public class Welcome
    {
        private readonly RogetData rogetdata = new();


        //CLASS
        //DIVISION
        //SECTION.
        //1. Heading -- 
        //<Type A, //ends with . ; optionally begins with POS - N. V. Adj. Adv. Phr.
        //<Type B, //ends with ;
        //<Type C, //ends with ,
        //bN
        //bV
        //bAdj
        //bAdv
        //bPhr
        //bObsolete
        //bLatin //[Lat.]
        //pos
        //secondary pos
        //archaic pos
        //[bracketed]
        //(parenthesized)
        //&c //just delete this?
        //link number
        //word

        public Welcome(bool bCreate = true)
        {
            if (bCreate == true)
            {
                Create();
            }
            else
            {
                Read();
            }
        }

        public int RomanNumeralToInteger(string strRomanNumeral)
        {
            int intReturn = strRomanNumeral switch
            {
                "I" => 1,
                "II" => 2,
                "III" => 3,
                "IV" => 4,
                "V" => 5,
                "VI" => 6,
                "VII" => 7,
                "VIII" => 8,
                "IX" => 9,
                "X" => 10,
                "XI" => 11,
                "XII" => 12,
                "XIII" => 13,
                "XIV" => 14,
                "XV" => 15,
                "XVI" => 16,
                "XVII" => 17,
                "XVIII" => 18,
                "XIX" => 19,
                "XX" => 20,
                _ => throw new Exception("add more roman numerals: " + strRomanNumeral),
            };
            return intReturn;
        }

        public void ReadRoget()
        {
            Regex rgxClass = new(@"^CLASS (?<classid>.*)$");
            Regex rgxDivision = new(@"^DIVISION (?<divisionid>.*)$");
            Regex rgxSection = new(@"^SECTION (?<sectionid>[^\.]{1,})\.$");
            Regex rgxHeading = new(@"^(?<headingid>[0-9]{1,})\. {1,}(?<heading>[^a-z]{1,})$");
            Regex rgxIDAndEntry = new(@"^(?<id>[0-9]{1,})\. {1,}(?<entry>[^\-]{1,})\-\- {1,}(?<entryline>.{1,})$"); //ie, "101. Something something"

            Regex rgxDotEnding = new(@"^(?<beforedot>[^\.]{1,})\.(?<afterdot>.*)$");
            //_ = new(@"^(?<beforesemicolon>[^;]{1,});(?<aftersemicolon>.*)$");
            //_ = new(@"^(?<beforecomma>[^,]{1,}),(?<aftercomma>.*)$");

            Regex rgxPOSStartN = new(@"^N\. ");
            Regex rgxPOSStartV = new(@"^V\. ");
            Regex rgxPOSStartAdj = new(@"^Adj\. ");
            Regex rgxPOSStartAdv = new(@"^Adv\. ");
            Regex rgxPOSStartPhr = new(@"^Phr\. ");
            //_ = new(@"&c n\. ");
            //_ = new(@"&c v\. ");
            //_ = new(@"&c adj\. ");
            //_ = new(@"&c adv\. ");
            //_ = new(@"&c phr\. ");
            //_ = new(@"\[(?<squareword>[^\]]{1,})\]");
            //_ = new(@"\((?<parathesisword>[^\)]{1,})\)");
            //_ = new(@" (?<link>[0-9]{1,})");

            bool bClass = false;
            bool bDivision = false;
            bool bSection = false;

            StreamReader srRoget = new(@"Data\Synonyms\10681-body.txt");

            while (!srRoget.EndOfStream)
            {
                string strLine = srRoget.ReadLine().Trim();

                if (strLine != "")
                {
                    if (rgxClass.IsMatch(strLine))
                    {
                        bClass = true;
                        _ = Convert.ToInt16(RomanNumeralToInteger(rgxClass.Match(strLine).Groups["classid"].Value));
                    }
                    else if (rgxDivision.IsMatch(strLine))
                    {
                        bDivision = true;
                        _ = Convert.ToInt16(RomanNumeralToInteger(rgxDivision.Match(strLine).Groups["divisionid"].Value));
                    }
                    else if (rgxSection.IsMatch(strLine))
                    {
                        bSection = true;
                        _ = Convert.ToInt16(RomanNumeralToInteger(rgxSection.Match(strLine).Groups["sectionid"].Value));
                    }
                    else if (rgxHeading.IsMatch(strLine))
                    {
                        Match mHeading = rgxHeading.Match(strLine);
                        _ = Convert.ToInt16(mHeading.Groups["headingid"].Value);
                        _ = mHeading.Groups["heading"].Value;
                    }
                    else if (rgxIDAndEntry.IsMatch(strLine))
                    {
                        Match mEntry = rgxIDAndEntry.Match(strLine);
                        _ = Convert.ToInt16(mEntry.Groups["id"].Value);
                        _ = mEntry.Groups["entry"].Value;
                        _ = mEntry.Groups["entryline"].Value;
                    }
                    else if (bClass == true)
                    {
                        bClass = false;
                    }
                    else if (bDivision == true)
                    {
                        bDivision = false;
                    }
                    else if (bSection == true)
                    {
                        bSection = false;
                    }
                    else //inside an entry
                    {
                        string strLineCopy = strLine;

                        //POS
                        if (rgxPOSStartN.IsMatch(strLineCopy))
                        {
                            Match mN = rgxPOSStartN.Match(strLine);
                            strLineCopy = strLine[(mN.Index + 3)..];
                        }
                        else if (rgxPOSStartV.IsMatch(strLineCopy))
                        {
                            Match mV = rgxPOSStartV.Match(strLine);
                            strLineCopy = strLine[(mV.Index + 3)..];
                        }
                        else if (rgxPOSStartAdj.IsMatch(strLineCopy))
                        {
                            Match mAdj = rgxPOSStartAdj.Match(strLine);
                            strLineCopy = strLine[(mAdj.Index + 5)..];
                        }
                        else if (rgxPOSStartAdv.IsMatch(strLineCopy))
                        {
                            Match mAdv = rgxPOSStartAdv.Match(strLine);
                            strLineCopy = strLine[(mAdv.Index + 5)..];
                        }
                        else if (rgxPOSStartPhr.IsMatch(strLineCopy))
                        {
                            Match mPhr = rgxPOSStartPhr.Match(strLine);
                            strLineCopy = strLine[(mPhr.Index + 5)..];
                        }
                        else
                        {
                            strLineCopy = strLine;
                        }

                        if (rgxDotEnding.IsMatch(strLineCopy))
                        {
                            foreach (Match mDot in rgxDotEnding.Matches(strLineCopy))
                            {
                                _ = mDot.Groups["beforedot"].Value;
                                _ = mDot.Groups["afterdot"].Value;

                            }
                        }

                    }
                } //if (strLine != "")
            }
        }

        public void ListBrackets()
        {
            StreamWriter swSquareBracket = new(@"Data\Synonyms\SquareBracketPhrases.txt");
            StreamWriter swCurvedBracket = new(@"Data\Synonyms\CurvedBracketPhrases.txt");
            StreamReader srRoget = new(@"Data\Synonyms\10681-body.txt");

            List<string> lSquare = new();
            List<string> lCurved = new();

            Regex rgxSquare = new(@"\[(?<square>[^]]{1,})\]");
            Regex rgxCurved = new(@"\((?<curved>[^)]{1,})\)");

            while (!srRoget.EndOfStream)
            {
                string strLine = srRoget.ReadLine();

                foreach (Match mSquare in rgxSquare.Matches(strLine))
                {
                    string strSquareWord = mSquare.Groups["square"].Value.ToLower().Trim();

                    if (!lSquare.Contains(strSquareWord))
                    {
                        lSquare.Add(strSquareWord);
                    }
                }

                foreach (Match mCurved in rgxCurved.Matches(strLine))
                {
                    string strCurvedWord = mCurved.Groups["curved"].Value.ToLower().Trim();

                    if (!lCurved.Contains(strCurvedWord))
                    {
                        lCurved.Add(strCurvedWord);
                    }
                }
            }

            srRoget.Close();

            foreach (string strSquare in lSquare.OrderBy(a => a))
            {
                swSquareBracket.WriteLine(strSquare);
            }

            foreach (string strCurved in lCurved.OrderBy(a => a))
            {
                swCurvedBracket.WriteLine(strCurved);
            }

            swSquareBracket.Close();
            swCurvedBracket.Close();
        }


        public void Create()
        {

        }

        public void Read()
        {

        }

        public void Write()
        {

        }

    }

    public class RogetData
    {

    }
}
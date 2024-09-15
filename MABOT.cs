using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB
{
    public class Verses
    {
        public Dictionary<int, Verse> dVerses = new();

        public void Load(StreamReader srMABOT)
        {
            while (!srMABOT.EndOfStream)
            {
                string strLine = srMABOT.ReadLine();
                string[] strsLine = strLine.Split('^');
                
                for(int intCounter = 0; intCounter < strsLine.Length; intCounter++)
                {
                    string strPart = strsLine[intCounter].Trim();

                    if (intCounter == 0)
                    {
                        string[] strsVerseID;

                        dVerses.Add(dVerses.Count + 1, new Verse());

                        strsVerseID = strPart.Split('.');

                        dVerses[dVerses.Count].intBook = Convert.ToInt16(strsVerseID[0].Substring(1)); //the first character is 'v'
                        dVerses[dVerses.Count].intBook = Convert.ToInt16(strsVerseID[1]);
                        dVerses[dVerses.Count].intBook = Convert.ToInt16(strsVerseID[2]);
                    }
                    else
                    {
                        string[] strsPart = strPart.Split(':');

                        switch (strsPart[0])
                        {
                            case "c":
                                dVerses[dVerses.Count].dClauses.Add(
                                    dVerses[dVerses.Count].dClauses.Count + 1, new Clause());
                                
                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .intID = Convert.ToInt32(strsPart[1].Trim().Substring(1));
                                break;
                            case "CKind":
                                Enum.TryParse(EnumifyString(strsPart[1].Trim()), out Clause.Kind k);
                                
                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .eKind = k;
                                break;
                            case "CTyp":
                                Enum.TryParse(EnumifyString(strsPart[1].Trim()), out Clause.Type t);

                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .eType = t;
                                break;
                            case "p":
                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases.Add(
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases.Count + 1, new Phrase());

                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                        .intPID = Convert.ToInt32(strsPart[1].Trim().Substring(1));
                                break;
                            case "pd":
                                Enum.TryParse(EnumifyString(strsPart[1].Trim()), out Phrase.PD pd);

                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                        .ePD = pd;
                                break;
                            case "pt":
                                Enum.TryParse(EnumifyString(strsPart[1].Trim()), out Phrase.PT pt);

                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                        .ePT = pt;
                                break;
                            case "pu":
                                Enum.TryParse(EnumifyString(strsPart[1].Trim()), out Phrase.PU pu);

                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                        .ePU = pu;
                                break;
                            case "w":
                                dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                        .dWords.Add(
                                    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                        .dWords.Count + 1, new Word());

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
                                //dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                //    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                //        .dWords[
                                //    dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count]
                                //    .dPhrases[dVerses[dVerses.Count].dClauses[dVerses[dVerses.Count].dClauses.Count].dPhrases.Count]
                                //        .dWords.Count]
                                //            .strParse
                                break;
                        }
                    }
                }
            }
        }

        private string EnumifyString(string strInput)
        {
            string strReturn = "";
            int intSpaceIndex = 0;

            foreach (string strPart in strInput.Split('-'))
            {
                if (strPart == "x")
                {
                    strReturn += "LittleX";
                }
                else if (strPart == "X")
                {
                    strReturn += "BigX";
                }
                else
                {
                    string strBuilder = "";
                    int intCharCounter = 0;

                    foreach (char c in strInput)
                    {
                        intCharCounter++;

                        if (intCharCounter == 1)
                        {
                            strBuilder = c.ToString().ToUpper();
                        }
                        else
                        {
                            strBuilder += c;
                        }
                    }

                    strReturn += strBuilder;
                }
            }

            intSpaceIndex = strReturn.IndexOf(' ');
            strReturn.Remove(intSpaceIndex, 1);
            strReturn[intSpaceIndex].ToString().ToUpper();

            return strReturn;
        }
    }

    public class Verse
    {
        public int intBook;
        public int intChapter;
        public int intVerse;

        public Dictionary<int, Clause> dClauses;
    }

    public class Clause
    {
        public int intID;
        public Kind eKind;
        public Type eType;

        public Dictionary<int, Phrase> dPhrases;

        public enum Kind
        {
            ClausesWithoutPredication = 1,
            NominalClauses = 2,
            VerbalClauses = 3
        }

        public enum Type
        {
            AdjectiveClause, //??always inside Nominal clauses,??
            AdjectiveClauseComma, //??always inside Nominal clauses,??
            CasusPendens, //??always inside Clauses without predication,??
            CasusPendensComma, //??always inside Clauses without predication,??
            Ellipsis,
            EllipsisComma,
            Extraposition,
            ExtrapositionComma,
            InfinitiveAbsoluteClause,
            InfinitiveAbsoluteClauseComma,
            InfinitiveConstructClause,
            InfinitiveConstructClauseComma,
            MacrosyntacticSign, //??always in Interrogative pronoun phrase?? //??always means "therefore"??, connecting semantically
            NominalClause,
            NominalClauseComma,
            ParticipleClause,
            ParticipleClauseComma,
            Reopening,
            ReopeningComma,
            VocativeClause,
            VocativeClauseComma,
            WayyiqtolNullClause,
            WayyiqtolNullClauseComma,
            WayyiqtolBigXClause,
            WayyiqtolBigXClauseComma,
            WeImperativeNullClause,
            WeImperativeNullClauseComma,
            WeImperativeBigXClause,
            WeQatalNullClause,
            WeQatalNullClauseComma,
            WeQatalBigXClause,
            WeQatalBigXClauseComma,
            WeBigXImperativeClause,
            WeBigXImperativeClauseComma,
            WeLittleXImperativeNullClause,
            WeLittleXImperativeNullClauseComma,
            WeLittleXImperativeBigXClause,
            WeBigXQatalClause,
            WeBigXQatalClauseComma,
            WeLittleXQatalNullClause,
            WeLittleXQatalNullClauseComma,
            WeLittleXQatalBigXClause,
            WeLittleXQatalBigXClauseComma,
            WeBigXYiqtolClause,
            WeBigXYiqtolClauseComma,
            WeLittleXYiqtolNullClause,
            WeLittleXYiqtolNullClauseComma,
            WeLittleXYiqtolBigXClause,
            WeLittleXYiqtolBigXClauseComma,
            WeYiqtolNullClause,
            WeYiqtolNullClauseComma,
            WeYiqtolBigXClause,
            WeYiqtolBigXClauseComma,
            BigXImperativeClause,
            BigXImperativeClauseComma,
            LittleXImperativeNullClause,
            LittleXImperativeNullClauseComma,
            LittleXImperativeBigXClause,
            BigXQatalClause,
            BigXQatalClauseComma,
            LittleXQatalNullClause,
            LittleXQatalNullClauseComma,
            LittleXQatalBigXClause,
            LittleXQatalBigXClauseComma,
            BigXYiqtolClause,
            BigXYiqtolClauseComma,
            LittleXYiqtolNullClause,
            LittleXYiqtolNullClauseComma,
            LittleXYiqtolBigXClause,
            LittleXYiqtolBigXClauseComma,
            ZeroImperativeNullClause,
            ZeroImperativeNullClauseComma,
            ZeroImperativeBigXClause,
            ZeroQatalNullClause,
            ZeroQatalNullClauseComma,
            ZeroQatalBigXClause,
            ZeroQatalBigXClauseComma,
            ZeroYiqtolNullClause,
            ZeroYiqtolNullClauseComma,
            ZeroYiqtolBigXClause,
            ZeroYiqtolBigXClauseComma
        }
    }

    public class Phrase
    {
        public int intPID;
        public PD ePD;
        public PT ePT;
        public PU ePU;

        public Dictionary<int, Word> dWords;

        public enum PD
        {
            DemonstrativePronounPhrase,
            NominalPhrase,
            PersonalPronounPhrase,
            PrepositionalPhrase,
            ProperNounPhrase
        }

        public enum PT
        {
            AdjectivePhrase,
            AdverbialPhrase,
            ConjunctivePhrase,
            InterjectionalPhrase,
            InterrogativePhrase,
            NegativePhrase,
            PrepositionalPhrase,
            VerbalPhrase
        }

        public enum PU
        {
            InterrogativePronounPhrase,
            NominalPhrase,
            PrepositionalPhrase
        }
    }

    public class Word
    {
        public int intWID;
        public string strEnglish;
        public string strParse;

        public enum WP
        {

        }
    }
}

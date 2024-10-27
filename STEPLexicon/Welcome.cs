namespace STEPLexicon
{
    public class Welcome
    {
        public STEPData stepdata = new();

        public Welcome(ref StreamReader srSTEPHebrew, ref StreamWriter swSTEPHebrew)
        {
            Create(ref stepdata, ref srSTEPHebrew);
            Write(ref stepdata, ref swSTEPHebrew);
        }

        public Welcome(ref StreamReader srSTEPHebrew)
        {
            Read(ref stepdata, ref srSTEPHebrew);
        }

        public void Create(ref STEPData stepdata, ref StreamReader srSTEPHebrew)
        {
            stepdata.shebLexicon.CreateSTEPBibleHebrewLexicon(ref stepdata, ref srSTEPHebrew);
            //stepdata.sgrkLexicon.
        }

        public void Read(ref STEPData stepdata, ref StreamReader srSTEPHebrew)
        {
            stepdata.shebLexicon.Read(ref stepdata, ref srSTEPHebrew);
        }

        public void Write(ref STEPData stepdata, ref StreamWriter swSTEPHebrew)
        {
            stepdata.shebLexicon.Write(ref stepdata, ref swSTEPHebrew);
        }
    }

    public class STEPData
    {
        public Dictionary<int, Dictionary<int, Lexicon>> dHebrewLexicon = new(); //D<WordID, D<AlternateID, Lexicon>>
        public Dictionary<int, Dictionary<int, Lexicon>> dGreekLexicon = new(); //D<WordID, D<AlternateID, Lexicon>>
        public STEPBibleHebrewLexicon shebLexicon = new();
        public STEPBibleGreekLexicon sgrkLexicon = new();

        public Dictionary<string, string> dSTEPBookNameAbbreviationNormalizations = new(); //D<seen, normalized> for STEPBible
        public string strReference = ""; //Gen.46.18-12
    }
}
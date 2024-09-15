namespace BLBConcordance
{
    public class Welcome
    {
        public BLBConcData blbconcdata = new();

        public Welcome(ref StreamReader srBLBHebrewConcordanceIn, //Create constructor
            ref StreamReader srBLBGreekConcordanceIn,
            ref StreamWriter swBLBHebrewConcordanceIn,
            ref StreamWriter swBLBGreekConcordanceIn)
        {
            Create(ref srBLBHebrewConcordanceIn, ref srBLBGreekConcordanceIn);
            Write(ref swBLBHebrewConcordanceIn, ref swBLBGreekConcordanceIn);
        }

        public Welcome(ref StreamReader srBLBHebrewConcordanceIn, //Load constructor
            ref StreamReader srBLBGreekConcordanceIn)
        {
            Read(ref srBLBHebrewConcordanceIn, ref srBLBGreekConcordanceIn);
        }

        public void Create(ref StreamReader srBLBHebrewConcordanceInput, //hebconc.js grkconc.js
            ref StreamReader srBLBGreekConcordanceInput)
        {
            blbconcdata.conc.CreateBLBHebrewConcordance(ref srBLBHebrewConcordanceInput,
                ref blbconcdata.dBLBHebrewConcordance);
            blbconcdata.conc.CreateBLBGreekConcordance(ref srBLBGreekConcordanceInput,
                ref blbconcdata.dBLBGreekConcordance);
        }

        public void Write(ref StreamWriter swBLBHebrewConcordance,
            ref StreamWriter swBLBGreekConcordance)
        {
            blbconcdata.conc.WriteBLBHebrewConcordance(ref swBLBHebrewConcordance,
                ref blbconcdata.dBLBHebrewConcordance);
            blbconcdata.conc.WriteBLBGreekConcordance(ref swBLBGreekConcordance,
                ref blbconcdata.dBLBGreekConcordance);
        }

        public void Read(ref StreamReader srBLBHebrewConcordance,
            ref StreamReader srBLBGreekConcordance)
        {
            blbconcdata.conc.FillBLBHebrewConcordance(ref srBLBHebrewConcordance,
                ref blbconcdata.dBLBHebrewConcordance);
            blbconcdata.conc.FillBLBGreekConcordance(ref srBLBGreekConcordance,
                ref blbconcdata.dBLBGreekConcordance);
        }
    }

    public class BLBConcData
    {
        public Dictionary<int, Dictionary<int, string>> dBLBHebrewConcordance = new(); //D<Concordance ID, D<Reference ID, Reference>>
        public Dictionary<int, Dictionary<int, string>> dBLBGreekConcordance = new(); //D<Concordance ID, D<Reference ID, Reference>>

        public Concordance conc = new();
    }
}
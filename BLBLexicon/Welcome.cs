namespace BLBLexicon
{
    public class Welcome
    {
        public BLBLexiconData blblexicondata = new();

        public Welcome(ref StreamReader srBLBHebrewLexiconIn,
            ref StreamReader srBLBGreekLexiconIn,
            ref StreamWriter swBLBHebrewLexiconOut,
            ref StreamWriter swBLBGreekLexiconOut,
            ref StreamWriter swBLBDerivativesOut, ref StreamWriter swBLBConnectionsOut,
            ref StreamWriter swBLBHRootsOut, ref StreamWriter swBLBHNonRootsOut,
            ref StreamWriter swBLBHAramaicOut, ref StreamWriter swBLBHNonAramaicOut,
            ref StreamWriter swBLBHRootedAramaicOut, ref StreamReader srRoots,
            ref StreamReader srNonRoots)
        {
            Create(ref srBLBHebrewLexiconIn, ref srBLBGreekLexiconIn,
                ref srRoots, ref srNonRoots);
            Write(ref swBLBHebrewLexiconOut, ref swBLBGreekLexiconOut,
                ref swBLBDerivativesOut, ref swBLBConnectionsOut,
                ref swBLBHRootsOut, ref swBLBHNonRootsOut,
                ref swBLBHAramaicOut, ref swBLBHNonAramaicOut,
                ref swBLBHRootedAramaicOut);
        }

        public Welcome(ref StreamReader srBLBHebrewLexiconIn,
            ref StreamReader srBLBGreekLexiconIn,
            ref StreamReader srBLBDerivativesIn, ref StreamReader srBLBConnectionsIn,
            ref StreamReader srBLBHRootsIn, ref StreamReader srBLBHNonRootsIn,
            ref StreamReader srBLBHAramaicIn, ref StreamReader srBLBHNonAramaicIn,
            ref StreamReader srBLBHRootedAramaicIn)
        {
            Read(ref srBLBHebrewLexiconIn, ref srBLBGreekLexiconIn,
                ref srBLBDerivativesIn, ref srBLBConnectionsIn, ref srBLBHRootsIn,
                ref srBLBHNonRootsIn, ref srBLBHAramaicIn, ref srBLBHNonAramaicIn,
                ref srBLBHRootedAramaicIn);
        }

        public void Create(ref StreamReader srBLBHebrewLexiconIn, ref StreamReader srBLBGreekLexiconIn,
            ref StreamReader srRoots, ref StreamReader srNonRoots)
        {
            blblexicondata.hlex.CreateBLBHebrewLexicon(ref srBLBHebrewLexiconIn,
                ref blblexicondata.dBLBHebrewLexiconEntries, ref srRoots, ref srNonRoots);

            blblexicondata.glex.CreateBLBGreekLexicon(ref srBLBGreekLexiconIn,
                ref blblexicondata.dBLBGreekLexiconEntries);

            blblexicondata.der.CreateDerivatives(ref blblexicondata.dBLBHebrewLexiconEntries,
                ref blblexicondata.dBLBGreekLexiconEntries);
        }

        public void Read(ref StreamReader srBLBHebrewLexiconIn,
            ref StreamReader srBLBGreekLexiconIn,
            ref StreamReader srBLBDerivativesIn,
            ref StreamReader srBLBConnectionsIn,
            ref StreamReader srBLBHRootsIn,
            ref StreamReader srBLBHNonRootsIn,
            ref StreamReader srBLBHAramaicIn,
            ref StreamReader srBLBHNonAramaicIn,
            ref StreamReader srBLBHRootedAramaicIn)
        {
            blblexicondata.hlex.FillBLBHebrewLexicon(ref srBLBHebrewLexiconIn,
                ref blblexicondata.dBLBHebrewLexiconEntries);

            blblexicondata.glex.FillBLBGreekLexicon(ref srBLBGreekLexiconIn,
                ref blblexicondata.dBLBGreekLexiconEntries);

            blblexicondata.der.FillDerivations(ref srBLBDerivativesIn,
                ref srBLBConnectionsIn, ref srBLBHRootsIn, ref srBLBHNonRootsIn,
                ref srBLBHAramaicIn, ref srBLBHNonAramaicIn, ref srBLBHRootedAramaicIn,
                ref blblexicondata.dBLBHebrewLexiconEntries);
        }

        public void Write(ref StreamWriter swBLBHebrewLexiconOut,
            ref StreamWriter swBLBGreekLexiconOut,
            ref StreamWriter swBLBDerivativesDerivationsOut,
            ref StreamWriter swBLBDerivativesConnectionsOut,
            ref StreamWriter swBLBDerivativesHRootsOut,
            ref StreamWriter swBLBDerivativesHNonRootsOut,
            ref StreamWriter swBLBDerivativesHAramaicOut,
            ref StreamWriter swBLBDerivativesHNonAramaicOut,
            ref StreamWriter swBLBDerivativesHRootedAramaicOut)
        {
            blblexicondata.hlex.WriteBLBHebrewLexicon(ref swBLBHebrewLexiconOut,
                ref blblexicondata.dBLBHebrewLexiconEntries);

            blblexicondata.glex.WriteBLBGreekLexicon(ref swBLBGreekLexiconOut,
                ref blblexicondata.dBLBGreekLexiconEntries);

            blblexicondata.der.WriteDerivations(
            ref swBLBDerivativesDerivationsOut,
            ref swBLBDerivativesConnectionsOut,
            ref swBLBDerivativesHRootsOut,
            ref swBLBDerivativesHNonRootsOut,
            ref swBLBDerivativesHAramaicOut,
            ref swBLBDerivativesHNonAramaicOut,
            ref swBLBDerivativesHRootedAramaicOut);
        }

    }

    public class BLBLexiconData
    {
        public Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries = new(); //D<LexID, BLBHebewLexicon entry> ; NOTE: LexID is the Strong's Number
        public Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries = new(); //D<LexID, BLBGreekLexicon entry> ; NOTE: LexID is the Strong's Number

        public BLBHebrewLexicon hlex = new();
        public BLBGreekLexicon glex = new();
        public Derivations der = new();
    }
}
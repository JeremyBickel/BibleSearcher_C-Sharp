namespace KJVStrongs
{
    public class Verse
    {
        public int intVerseID = 0;
        public string strBookName = "";
        public int intBookNumber = 0;
        public int intChapterNumber = 0;
        public int intVerseNumber = 0;
        public string strText = "";
        public Dictionary<int, Phrase> dPhrases = new(); //D<PhraseID, Phrase>
        public Dictionary<string, int> dPhraseCounts = new(); //D<Phrase, Count>
    }
}

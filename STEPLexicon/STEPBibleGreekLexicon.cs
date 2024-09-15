namespace STEPLexicon
{
    public class STEPBibleGreekLexicon
    {
        private readonly Dictionary<string, string> dNormalize = new(); //D<seen, normalized>

        public STEPBibleGreekLexicon()
        {
            dNormalize.Add("Mat", "Matthew");
            dNormalize.Add("Mrk", "Mark");
            dNormalize.Add("Luk", "Luke");
            dNormalize.Add("Jhn", "John");
            dNormalize.Add("Act", "Acts");
            dNormalize.Add("Rom", "Romans");
            dNormalize.Add("1Co", "1 Corinthians");
            dNormalize.Add("2Co", "2 Corinthians");
            dNormalize.Add("Gal", "Galatians");
            dNormalize.Add("Eph", "Ephesians");
            dNormalize.Add("Php", "Philippians");
            dNormalize.Add("Col", "Colossians");
            dNormalize.Add("1Th", "1 Thessalonians");
            dNormalize.Add("2Th", "2 Thessalonians");
            dNormalize.Add("1Ti", "1 Timothy");
            dNormalize.Add("2Ti", "2 Timothy");
            dNormalize.Add("Tit", "Titus");
            dNormalize.Add("Phm", "Philemon");
            dNormalize.Add("Heb", "Hebrews");
            dNormalize.Add("Jas", "James");
            dNormalize.Add("1Pe", "1 Peter");
            dNormalize.Add("2Pe", "2 Peter");
            dNormalize.Add("1Jn", "1 John");
            dNormalize.Add("2Jn", "2 John");
            dNormalize.Add("3Jn", "3 John");
            dNormalize.Add("Jud", "Jude");
            dNormalize.Add("Rev", "Revelation");
        }
    }
}

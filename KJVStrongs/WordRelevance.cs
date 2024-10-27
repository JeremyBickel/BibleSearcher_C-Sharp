namespace KJVStrongs
{
    public class WordRelevance
    {
        public Dictionary<string, Dictionary<string, int>> dBookWordCounts = new(); //D<Book Name, D<Word, Count in this book>>

        public void CreateWordRelevance(ref KJVData kjvdata)
        {
            // transform all synonymous words into a single word
            // find the distribution of word through book and bible
            // -remove thoroughly evenly distributed words
            // -words with a moderate count in some books but not found in others become prime definers of those books

            //for each word, how many BOOKS is that word in? 66 - filter out; 1 - head word for the book; etc.

            foreach (int intWordPosition in kjvdata.dWordPositions.Keys.OrderBy(a => a))
            {
                string strWord = kjvdata.dWordPositions[intWordPosition].ToLower();
                int intBookNumber = kjvdata.dBookStartingPositions.Keys.Where(a => a <= intWordPosition).Max();
                string strBookName = kjvdata.dBookStartingPositions[intBookNumber];

                if (!dBookWordCounts.ContainsKey(strBookName))
                {
                    dBookWordCounts.Add(strBookName, new Dictionary<string, int>());
                }

                if (!dBookWordCounts[strBookName].ContainsKey(strWord))
                {
                    dBookWordCounts[strBookName].Add(strWord, 0);
                }

                dBookWordCounts[strBookName][strWord]++;
            }
        }

        public void WriteWordRelevance(ref StreamWriter swBookWordCounts)
        {
            swBookWordCounts.WriteLine("Book Name ^ Word ^ Count");

            foreach (string strBookName in dBookWordCounts.Keys.OrderBy(a => a))
            {
                foreach (string strWord in dBookWordCounts[strBookName].Keys.OrderBy(a => a))
                {
                    swBookWordCounts.WriteLine(strBookName + " ^ " + strWord +
                        " ^ " + dBookWordCounts[strBookName][strWord].ToString());
                }
            }

            swBookWordCounts.Close();
        }

        public void ReadWordRelevance(ref StreamReader srBookWordCounts)
        {
            bool bSeenHeader = false;

            while (!srBookWordCounts.EndOfStream)
            {
                string strLine = srBookWordCounts.ReadLine();

                if (bSeenHeader == false)
                {
                    bSeenHeader = true;
                }
                else
                {
                    string[] strsLine = strLine.Split('^');

                    if (!dBookWordCounts.ContainsKey(strsLine[0]))
                    {
                        dBookWordCounts.Add(strsLine[0], new Dictionary<string, int>());
                    }

                    if (!dBookWordCounts[strsLine[0]].ContainsKey(strsLine[1]))
                    {
                        dBookWordCounts[strsLine[0]].Add(strsLine[1], Convert.ToInt16(strsLine[2]));
                    }
                }
            }

            srBookWordCounts.Close();
        }
    }
}

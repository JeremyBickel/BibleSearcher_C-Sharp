namespace KJVStrongs
{
    public partial class BibleLinks : Form
    {
        private readonly Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<int, int>>>> dVerseCounts = new(); //D<BookNumber, D<BookName, D<ChapterCount, D<ChapterNumber, VerseCount>>>>

        public BibleLinks()
        {
            InitializeComponent();

            StreamReader srVerseCounts = new(@"Data\KJVVerseCounts.csv");
            bool bFirstLine = true;

            cboBook.SelectedIndexChanged -= cboBook_SelectedIndexChanged;

            while (!srVerseCounts.EndOfStream)
            {
                string strLine = srVerseCounts.ReadLine();
                int intFieldCounter = 0;

                int intBookNumber = 0;
                string strBookName = "";
                int intChapterCount = 0;

                if (bFirstLine == true) //Data file column names
                {
                    bFirstLine = false;
                }
                else
                {
                    foreach (string strField in strLine.Split('^'))
                    {
                        intFieldCounter++;

                        switch (intFieldCounter)
                        {
                            case 1:
                                intBookNumber = Convert.ToInt16(strField.Trim());
                                dVerseCounts.Add(intBookNumber, new Dictionary<string, Dictionary<int, Dictionary<int, int>>>());
                                break;
                            case 2:
                                strBookName = strField.Trim();
                                dVerseCounts[intBookNumber].Add(strBookName, new Dictionary<int, Dictionary<int, int>>());
                                break;
                            case 3:
                                intChapterCount = Convert.ToInt16(strField.Trim());
                                dVerseCounts[intBookNumber][strBookName].Add(intChapterCount, new Dictionary<int, int>());
                                break;
                            case 4:
                                Dictionary<int, int> dict = new();
                                int intChapterCounter = 0;

                                foreach (string strVerseCount in strField.Trim().Split('\t'))
                                {
                                    intChapterCounter++;
                                    dVerseCounts[intBookNumber][strBookName][intChapterCount].Add(intChapterCounter, Convert.ToInt16(strVerseCount.Trim()));
                                }
                                break;
                        }
                    }
                }
            }

            cboBook.SelectedIndexChanged += cboBook_SelectedIndexChanged;

            UpdateCboBook();
        }

        private void UpdateCboBook()
        {
            cboBook.SelectedIndexChanged -= cboBook_SelectedIndexChanged;

            for (int intBookNumber = 1; intBookNumber <= 66; intBookNumber++)
            {
                _ = cboBook.Items.Add(dVerseCounts[intBookNumber].Keys.First());
            }

            cboBook.SelectedIndex = 0;

            cboBook.SelectedIndexChanged += cboBook_SelectedIndexChanged;

            UpdateCboChapter();
        }

        private void UpdateCboChapter()
        {
            int intChapterCount = dVerseCounts[cboBook.SelectedIndex + 1][cboBook.SelectedItem.ToString()].Keys.First();

            cboChapter.SelectedIndexChanged -= cboChapter_SelectedIndexChanged;

            cboChapter.Items.Clear();

            for (int intChapterCounter = 1; intChapterCounter <= intChapterCount; intChapterCounter++)
            {
                _ = cboChapter.Items.Add(intChapterCounter.ToString());
            }

            cboChapter.SelectedIndex = 0;

            cboChapter.SelectedIndexChanged += cboChapter_SelectedIndexChanged;

            UpdateCboVerse();

        }

        private void UpdateCboVerse()
        {
            int intChapterCount = dVerseCounts[cboBook.SelectedIndex + 1][cboBook.SelectedItem.ToString()].Keys.First();
            int intVerseCount = dVerseCounts[cboBook.SelectedIndex + 1][cboBook.SelectedItem.ToString()][intChapterCount][Convert.ToInt16(cboChapter.SelectedItem.ToString())];

            cboVerse.SelectedIndexChanged -= cboVerse_SelectedIndexChanged;

            cboVerse.Items.Clear();

            for (int intVerse = 1; intVerse <= intVerseCount; intVerse++)
            {
                _ = cboVerse.Items.Add(intVerse.ToString());
            }

            cboVerse.SelectedIndex = 0;

            cboVerse.SelectedIndexChanged += cboVerse_SelectedIndexChanged;
        }

        private void cboBook_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboChapter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboVerse_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

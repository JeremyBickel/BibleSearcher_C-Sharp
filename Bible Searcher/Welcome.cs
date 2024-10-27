using KJVStrongs;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Bible_Searcher
{
    public partial class Welcome : Form
    {
        //Regex rgxTranslationPlusNumbers = new Regex("(?<word>[^\\+]{1,})\\+[^0-9]{0,}(?<number>[0-9]{1,})");
        public Data.Welcome data;
        private int[] intsCPCLengths;
        private int[] intsIntersectingVerseIDs;

        public void Testing()
        {
            //Marvel.Welcome w = new Marvel.Welcome();
            //w.Show();

            //Data.Transformations transformLev = new Data.Transformations();
            //transformLev.PhraseReductionPipeline();
            //transformLev.Go(@"F:\BibleSearcher\Data\Data\Original\MarvelData-JSON\Data\discourseNT.data_Bible.json");
            //DeepAI.Welcome dai = new DeepAI.Welcome();

            //testing shortcut
            //MAB.VersesNT vv = new();
            //StreamReader sr = new(@"Z:\BibleSearcher\Data\Data\Processed\MAB\MAB-NT.csv");
            //vv.Load(ref sr);

            //TransformPhrasesNames();

            DeepAIConnection.CallLLM cllm = new DeepAIConnection.CallLLM();
            cllm.Main();
        }

        public Welcome()
        {
            InitializeComponent();
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            Testing();
            data = new();
            intsCPCLengths = CheckChainedPhrasalConcordanceLengths();
            CreateIntersectingPhraseMinMax();
            intsIntersectingVerseIDs = CheckIntersectingPhrasesBoundaries();

            //
            //UI
            //
            FillLBBooks();
            FillPericopeSearchTermsBox();

            lblExistingChainedConcordanceBoundaries.Text = "Lengths " +
                intsCPCLengths[0].ToString() + " to " +
                intsCPCLengths[1].ToString() + " exist.";
            lblExistingIntersectingPhrasesBoundaries.Text = "VerseIDs " +
                intsIntersectingVerseIDs[0].ToString() + " to " +
                intsIntersectingVerseIDs[1].ToString() + " exist.";
        }

        public void FillPericopeSearchTermsBox()
        {
            //GUI
            clbPericopeSearchTerms.Items.Clear();

            foreach (string strHeading in data.dataobjects.pericope.pericopedata
                .dHeadingsPositionWordID.Keys.OrderBy(a => a))
            {
                _ = clbPericopeSearchTerms.Items.Add(strHeading);
            }
        }

        public void FillLBBooks()
        {
            _ = lbBooks.Items.Add("Genesis");
            _ = lbBooks.Items.Add("Exodus");
            _ = lbBooks.Items.Add("Leviticus");
            _ = lbBooks.Items.Add("Numbers");
            _ = lbBooks.Items.Add("Deuteronomy");
            _ = lbBooks.Items.Add("Joshua");
            _ = lbBooks.Items.Add("Judges");
            _ = lbBooks.Items.Add("Ruth");
            _ = lbBooks.Items.Add("1 Samuel");
            _ = lbBooks.Items.Add("2 Samuel");
            _ = lbBooks.Items.Add("1 Kings");
            _ = lbBooks.Items.Add("2 Kings");
            _ = lbBooks.Items.Add("1 Chronicles");
            _ = lbBooks.Items.Add("2 Chronicles");
            _ = lbBooks.Items.Add("Ezra");
            _ = lbBooks.Items.Add("Nehemiah");
            _ = lbBooks.Items.Add("Esther");
            _ = lbBooks.Items.Add("Job");
            _ = lbBooks.Items.Add("Psalms");
            _ = lbBooks.Items.Add("Proverbs");
            _ = lbBooks.Items.Add("Ecclesiastes");
            _ = lbBooks.Items.Add("Song of Solomon");
            _ = lbBooks.Items.Add("Isaiah");
            _ = lbBooks.Items.Add("Jeremiah");
            _ = lbBooks.Items.Add("Lamentations");
            _ = lbBooks.Items.Add("Ezekiel");
            _ = lbBooks.Items.Add("Daniel");
            _ = lbBooks.Items.Add("Hosea");
            _ = lbBooks.Items.Add("Joel");
            _ = lbBooks.Items.Add("Amos");
            _ = lbBooks.Items.Add("Obadiah");
            _ = lbBooks.Items.Add("Jonah");
            _ = lbBooks.Items.Add("Micah");
            _ = lbBooks.Items.Add("Nahum");
            _ = lbBooks.Items.Add("Habakkuk");
            _ = lbBooks.Items.Add("Zephaniah");
            _ = lbBooks.Items.Add("Haggai");
            _ = lbBooks.Items.Add("Zechariah");
            _ = lbBooks.Items.Add("Malachi");
            _ = lbBooks.Items.Add("Matthew");
            _ = lbBooks.Items.Add("Mark");
            _ = lbBooks.Items.Add("Luke");
            _ = lbBooks.Items.Add("John");
            _ = lbBooks.Items.Add("Acts");
            _ = lbBooks.Items.Add("Romans");
            _ = lbBooks.Items.Add("1 Corinthians");
            _ = lbBooks.Items.Add("2 Corinthians");
            _ = lbBooks.Items.Add("Galatians");
            _ = lbBooks.Items.Add("Ephesians");
            _ = lbBooks.Items.Add("Philippians");
            _ = lbBooks.Items.Add("Colossians");
            _ = lbBooks.Items.Add("1 Thessalonians");
            _ = lbBooks.Items.Add("2 Thessalonians");
            _ = lbBooks.Items.Add("1 Timothy");
            _ = lbBooks.Items.Add("2 Timothy");
            _ = lbBooks.Items.Add("Titus");
            _ = lbBooks.Items.Add("Philemon");
            _ = lbBooks.Items.Add("Hebrews");
            _ = lbBooks.Items.Add("James");
            _ = lbBooks.Items.Add("1 Peter");
            _ = lbBooks.Items.Add("2 Peter");
            _ = lbBooks.Items.Add("1 John");
            _ = lbBooks.Items.Add("2 John");
            _ = lbBooks.Items.Add("3 John");
            _ = lbBooks.Items.Add("Jude");
            _ = lbBooks.Items.Add("Revelation");
        }

        public int[] CheckChainedPhrasalConcordanceLengths()
        {
            int intMinimum = 10000;
            int intMaximum = -10000;
            Regex rgxLength = new(@"ChainedPhrasalConcordance-(?<length>[0-9]{1,})\.csv");

            int[] intsReturn = (int[])Array.CreateInstance(typeof(int), 2);

            foreach (string strFilename in Directory.EnumerateFiles(@"Data\Processed\ChainedPhrasalConcordance\"))
            {
                if (rgxLength.IsMatch(strFilename))
                {
                    int intLength = Convert.ToInt16(rgxLength.Match(strFilename).Groups["length"].Value);

                    if (intLength < intMinimum)
                    {
                        intMinimum = intLength;
                    }

                    if (intLength > intMaximum)
                    {
                        intMaximum = intLength;
                    }
                }
            }

            intsReturn[0] = intMinimum;
            intsReturn[1] = intMaximum;

            return intsReturn;
        }

        public void CreateIntersectingPhraseMinMax()
        {
            //make sure the file has been written
            if (!File.Exists(@"Data\Processed\IntersectingPhrases\MinMax.csv"))
            {
                Regex rgxNumber = new(@"IntersectingPhrases_(?<number>[0-9]{1,})\.csv");
                StreamWriter swMinMax = new(@"Data\Processed\IntersectingPhrases\MinMax.csv");
                int intMin = 10000;
                int intMax = -10000;

                //1 + (100 * intVerseIDMultiplier)
                foreach (string strFilename in Directory.EnumerateFiles(@"Data\Processed\IntersectingPhrases\", "IntersectingPhrases_*.csv", SearchOption.TopDirectoryOnly))
                {
                    if (rgxNumber.IsMatch(strFilename))
                    {
                        int intPossible = Convert.ToInt16(rgxNumber.Match(strFilename).Groups["number"].Value);

                        if (intMin > intPossible)
                        {
                            intMin = intPossible;
                        }

                        if (intMax < intPossible)
                        {
                            intMax = intPossible;
                        }
                    }
                }

                swMinMax.WriteLine(((intMin * 100) - 99).ToString() + " ^ " + (intMax * 100).ToString());
                swMinMax.Close();
            }
        }

        public int[] CheckIntersectingPhrasesBoundaries()
        {
            int[] intsReturn = (int[])Array.CreateInstance(typeof(int), 2);

            StreamReader srMinMax = new(@"Data\Processed\IntersectingPhrases\MinMax.csv");
            string[] strsMinMax = srMinMax.ReadLine().Split('^');

            intsReturn[0] = Convert.ToInt16(strsMinMax[0]);
            intsReturn[1] = Convert.ToInt16(strsMinMax[1]);

            srMinMax.Close();

            return intsReturn;
        }

        //public void CreateWord2VecModel()
        //{
        //    string trainfile = @"Data\KJV-RawText.txt";
        //    string outputFileName = @"Data\Word2Vec-Output.bin";
        //    var word2Vec = Word2VecBuilder.Create()
        //        .WithTrainFile(trainfile)// Use text data to train the model;
        //        .WithOutputFile(outputFileName)//Use to save the resulting word vectors / word clusters
        //        .WithSize(200)//Set size of word vectors; default is 100
        //        .WithSaveVocubFile(@"Data\Word2Vec-Vocab.txt")//The vocabulary will be saved to <file>
        //        .WithDebug(2)//Set the debug mode (default = 2 = more info during training)
        //        .WithBinary(1)//Save the resulting vectors in binary moded; default is 0 (off)
        //        .WithCBow(1)//Use the continuous bag of words model; default is 1 (use 0 for skip-gram model)
        //        .WithAlpha(0.05f)//Set the starting learning rate; default is 0.025 for skip-gram and 0.05 for CBOW
        //        .WithWindow(7)//Set max skip length between words; default is 5
        //        .WithSample((float)1e-3)//Set threshold for occurrence of words. Those that appear with higher frequency in the training data twill be randomly down-sampled; default is 1e-3, useful range is (0, 1e-5)
        //        .WithHs(0)//Use Hierarchical Softmax; default is 0 (not used)
        //        .WithNegative(0)//Number of negative examples; default is 5, common values are 3 - 10 (0 = not used)
        //        .WithThreads(5)//Use <int> threads (default 12)
        //        .WithIter(5)//Run more training iterations (default 5)
        //        .WithMinCount(5)//This will discard words that appear less than <int> times; default is 5
        //        .WithClasses(0)//Output word classes rather than word vectors; default number of classes is 0 (vectors are written)
        //        .Build();

        //    word2Vec.TrainModel();

        //    var distance = new Distance(outputFileName);
        //    BestWord[] bestwords = distance.Search("christ");
        //}

        private void btnSearchPericope_Click(object sender, EventArgs e)
        {
            //search example
            List<string> lSearchTerms = new();
            Dictionary<int, List<string>> dSearchResults;

            lbxPericopeSearchResults.Items.Clear();

            foreach (string strTerm in clbPericopeSearchTerms.CheckedItems)
            {
                lSearchTerms.Add(strTerm);
            }

            dSearchResults = data.dataobjects.pericope.pericopedata.psHeadings.Search(lSearchTerms); //D<1, L<any-term heading results>>, D<2, L<all-term heading results>>

            if (rbnPericopeSearchAny.Checked == true)
            {
                foreach (string strResult in dSearchResults[1])
                {
                    if (!lbxPericopeSearchResults.Items.Contains(strResult))
                    {
                        lbxPericopeSearchResults.Items.Add(strResult);
                    }
                }
            }
            else
            {
                foreach (string strResult in dSearchResults[2])
                {
                    if (!lbxPericopeSearchResults.Items.Contains(strResult))
                    {
                        lbxPericopeSearchResults.Items.Add(strResult);
                    }
                }
            }
        }

        private void btnBibleLinks_Click(object sender, EventArgs e)
        {
            BibleLinks bl = new();
            bl.ShowDialog();
        }

        private void btnSemanticGroups_Click(object sender, EventArgs e)
        {
            Categories categories = new(ref data);
            categories.ShowDialog();
        }

        private void btnCreateChainedPhrasalConcordance_Click(object sender, EventArgs e)
        {
            Regex rgxNumbers = new(@"[0-9]{1,}");

            //check user length inputs
            if (!rgxNumbers.IsMatch(tbxChainedSmallest.Text))
            {
                //if they didn't input a number, then set the smallest needed
                tbxChainedSmallest.Text = (intsCPCLengths[1] + 1).ToString();
            }

            if (!rgxNumbers.IsMatch(tbxChainedLargest.Text))
            {
                //if they didn't input a number, then set one more than the smallest
                tbxChainedLargest.Text = (Convert.ToInt16(tbxChainedSmallest.Text) + 1).ToString();
            }

            data.CreateChainedPhrasalConcordance(ref data.dataobjects.kjvs.kjvdata, Convert.ToInt16(tbxChainedSmallest.Text),
                Convert.ToInt16(tbxChainedLargest.Text));
        }

        private void btnCreateIntersectingPhrases_Click(object sender, EventArgs e)
        {
            Regex rgxNumbers = new(@"[0-9]{1,}");

            //check user verse id inputs
            if (!rgxNumbers.IsMatch(tbxIntersectingMinimum.Text))
            {
                //if they didn't input a number, then set the smallest needed
                tbxIntersectingMinimum.Text = (intsIntersectingVerseIDs[1] + 1).ToString();
            }

            if (!rgxNumbers.IsMatch(tbxIntersectingMaximum.Text))
            {
                //if they didn't input a number, then set one more than the smallest
                tbxIntersectingMaximum.Text = (Convert.ToInt16(tbxIntersectingMinimum.Text) + 99).ToString();
            }

            data.CreateIntersectingPhrases(ref data.dataobjects.kjvs.kjvdata,
                Convert.ToInt16(tbxIntersectingMinimum.Text), Convert.ToInt16(tbxIntersectingMaximum.Text));
        }

        private void btnWriteKJVMobySynonyms_Click(object sender, EventArgs e)
        {
            StreamWriter swKJVMobySynonyms = new(@"Data\Processed\Synonyms\KJVMobySynonyms.csv");

            data.WriteWordIDMobySynonyms(ref data.dataobjects, ref swKJVMobySynonyms);
        }
    }
}